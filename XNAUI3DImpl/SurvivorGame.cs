using CLRSurvivorLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SysForm = System.Windows.Forms;

namespace XNAUI3DImpl
{
    public partial class SurvivorGame : Game
    {
        string ContentRootPath, RootPath;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int MaxUDD = 10000;

        bool DebugMode = true;
        struct DbgInfo
        {
            public int uddc;
        }
        DbgInfo _dbgi = new DbgInfo();

        bool worldInitialized = false;
        object UIDataLock = new object();
        Queue<CSUpdateData> UpdateData = new Queue<CSUpdateData>();
        Queue<CSUpdateData> ApplyingUpdateData = new Queue<CSUpdateData>();

        Matrix worldMatrix;
        Camera camera;

        SKeyboard keyboard = new SKeyboard();
        const float MouseWheelC = 120.0f;
        const float MouseWheelAdjust = 4f;
        SMouse mouse;

        SpriteFont DefaultFont;

        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue BackgroundMusic;

        Model Box;
        Model Tree;
        Model Hero;

        ulong WorldWidth, WorldHeight;

        GridStorage<CSStillObject> SpaceSortedStillObjects;
        GridStorage<CSAnimal> SpaceSortedAnimals;
        GridStorage<CSHero> SpaceSortedHeroes;
        Dictionary<ulong, CSObject> EnvironmentObjects = new Dictionary<ulong, CSObject>();

        public SurvivorGame()
        {
            RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ContentRootPath = Path.Combine(RootPath, "XNASurvivorContent");

            SysForm.Form winForm = SysForm.Control.FromHandle(Window.Handle) as SysForm.Form;
            winForm.Icon = WinResource.SurvivorIcon;
            winForm.Text = "Survivor";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            int w = SysForm.Screen.PrimaryScreen.Bounds.Width;
            int h = SysForm.Screen.PrimaryScreen.Bounds.Height;
            /*float ratio = (float)w / h;
            foreach (var mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) ;*/
            graphics.PreferredBackBufferWidth = w / 2;
            graphics.PreferredBackBufferHeight = h / 2;

            Content.RootDirectory = ContentRootPath;

            if (Settings.Default.FullScreen)
                graphics.ToggleFullScreen();
        }

        public int InitializeSurvivor(CSInitializeData initializeData)
        {
            WorldWidth = initializeData.Map.Width;
            WorldHeight = initializeData.Map.Height;

            SpaceSortedStillObjects = new GridStorage<CSStillObject>(256, Math.Max(WorldWidth, WorldHeight), 0, 0);
            SpaceSortedAnimals = new GridStorage<CSAnimal>(256, Math.Max(WorldWidth, WorldHeight), 0, 0);
            SpaceSortedHeroes = new GridStorage<CSHero>(256, Math.Max(WorldWidth, WorldHeight), 0, 0);

            foreach (var o in initializeData.Map.StillObjects)
            {
                EnvironmentObjects.Add(o.ID, o);
                SpaceSortedStillObjects.Add(o, o.Position.x, o.Position.y);
            }
            foreach (var o in initializeData.Map.Animals)
            {
                EnvironmentObjects.Add(o.ID, o);
                SpaceSortedAnimals.Add(o, o.Position.x, o.Position.y);
            }
            foreach (var o in initializeData.Heroes)
            {
                EnvironmentObjects.Add(o.ID, o);
                SpaceSortedHeroes.Add(o, o.Position.x, o.Position.y);
            }

            worldInitialized = true;
            return (int)CSGState.OK;
        }

        public int UpdateSurvivor(CSUpdateData updd)
        {
            lock (UIDataLock)
            {
                UpdateData.Enqueue(updd);
            }
            return (int)(UpdateData.Count <= MaxUDD ? CSGState.OK : CSGState.UITooManyUpdateData);
        }

        protected override void Initialize()
        {
            keyboard.KeyClick += keyboard_KeyClick;

            mouse = new SMouse(GraphicsDevice);
            mouse.MouseLeftClick += cursor_MouseLeftClick;
            mouse.MouseWheelChanged += cursor_MouseWheelChanged;

            camera = new Camera(GraphicsDevice);

            worldMatrix = Matrix.Identity;

            base.Initialize();
        }

        void keyboard_KeyClick(SKeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Escape: Exit(); break;
                case Keys.D: DebugMode = !DebugMode; break;
                case Keys.F: graphics.ToggleFullScreen(); break;
            }
        }

        void cursor_MouseWheelChanged(EventArgs e)
        {
            camera.Distance += mouse.WheelDelta / MouseWheelC * MouseWheelAdjust;
        }

        void cursor_MouseLeftClick(EventArgs e)
        {
            camera.Target = mouse.SelectFloor(camera.ProjectionMatrix, camera.ViewMatrix);
        }

        protected override void LoadContent()
        {
            audioEngine = new AudioEngine(Path.Combine(ContentRootPath, "Audio/SurvivorAudio.xgs"));
            soundBank = new SoundBank(audioEngine, Path.Combine(ContentRootPath, "Audio/SurvivorAudio.xsb"));
            waveBank = new WaveBank(audioEngine, Path.Combine(ContentRootPath, "Audio/SurvivorAudio.xwb"));
            BackgroundMusic = soundBank.GetCue("YouWin");
            BackgroundMusic.Play();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouse.LoadContent(Content);
            DefaultFont = Content.Load<SpriteFont>("Font/DefaultFont");
            Box = Content.Load<Model>("Model/Box");
            Tree = Content.Load<Model>("Model/Tree");
            Hero = Content.Load<Model>("Model/Hero000");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            audioEngine.Update();
            keyboard.Update(gameTime);
            mouse.Update(gameTime);
            camera.Update(gameTime);

            lock (UIDataLock)
            {
                Queue<CSUpdateData> swap = ApplyingUpdateData;
                ApplyingUpdateData = UpdateData;
                UpdateData = swap;
            }
            if (DebugMode)
                _dbgi.uddc = ApplyingUpdateData.Count;
            ApplyUpdate();

            base.Update(gameTime);
        }

        private void ApplyUpdate()
        {
            while (ApplyingUpdateData.Count > 0)
            {
                CSUpdateData data = ApplyingUpdateData.Dequeue();
                foreach (var a in data.Animals)
                    ApplyUpdateTarget(SpaceSortedAnimals, a);
                foreach (var h in data.Heroes)
                    ApplyUpdateTarget(SpaceSortedHeroes, h);
                foreach (var s in data.StillObjects)
                    ApplyUpdateTarget(SpaceSortedStillObjects, s);
            }
        }

        private void ApplyUpdateTarget<T>(GridStorage<T> gs, T t) where T : CSObject
        {
            if (EnvironmentObjects.ContainsKey(t.ID))
            {
                T ola = EnvironmentObjects[t.ID] as T;
                gs.Replace(ola, ola.Position.x, ola.Position.y, t, t.Position.x, t.Position.y);
                EnvironmentObjects[t.ID] = t;
            }
            else
            {
                gs.Add(t, t.Position.x, t.Position.y);
                EnvironmentObjects.Add(t.ID, t);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            graphics.GraphicsDevice.RasterizerState = rs;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (worldInitialized)
            {
                DrawEnvironment();

                spriteBatch.Begin();
                if (DebugMode)
                    PrintDebugInfo(gameTime);
                mouse.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {

            }

            base.Draw(gameTime);
        }
        
        private void PrintDebugInfo(GameTime gameTime)
        {
            const float fontsize = 11;
            double fps = 1000 / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            string sfps = double.IsInfinity(fps) ? "inf" : fps.ToString("f1") + "fps";
            float cx = camera.CurrentTarget.X;
            float cz = camera.CurrentTarget.Z;
            float ypx = 0;
            spriteBatch.DrawString(DefaultFont, sfps, new Vector2(0, ypx), Color.White);
            ypx += fontsize;
            spriteBatch.DrawString(DefaultFont, "camera pos : " + cx.ToString("f1") + "," + cz.ToString("f1"),
                new Vector2(0, ypx), Color.White);
            ypx += fontsize;
            spriteBatch.DrawString(DefaultFont, "data pack : " + _dbgi.uddc, new Vector2(0, ypx), Color.White);
        }

        private void DrawEnvironment()
        {
            const int Wide = 1;
            int x = SpaceSortedStillObjects.CellPositionX(camera.CurrentTarget.X);
            int y = SpaceSortedStillObjects.CellPositionY(camera.CurrentTarget.Z);
            int minx = Math.Max(x - Wide, 0);
            int miny = Math.Max(y - Wide, 0);
            int maxx = Math.Min(x + Wide, SpaceSortedStillObjects.GridCount - 1);
            int maxy = Math.Min(y + Wide, SpaceSortedStillObjects.GridCount - 1);
            for (int i = minx; i <= maxx; i++)
                for (int j = miny; j <= maxy; j++)
                {
                    foreach (var obj in SpaceSortedStillObjects[i, j])
                        DrawStillObject(obj);
                    foreach (var obj in SpaceSortedAnimals[i, j])
                        DrawAnimal(obj);
                    foreach (var obj in SpaceSortedHeroes[i, j])
                        DrawHero(obj);
                }
        }

        private void DrawHero(CSHero obj)
        {
            Matrix[] transforms = new Matrix[Hero.Bones.Count];
            Hero.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in Hero.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.FogEnabled = true;
                    effect.FogStart = camera.Distance;
                    effect.FogEnd = camera.Distance * 2;
                    effect.FogColor = Color.CornflowerBlue.ToVector3();
                    effect.DiffuseColor = Color.OrangeRed.ToVector3();
                    // TODO Optimize it
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY((float)Math.Acos(obj.Direction.x)) * // It is right ?
                        Matrix.CreateTranslation((float)obj.Position.x, 0, (float)obj.Position.y);
                }
                mesh.Draw();
            }
        }

        private void DrawAnimal(CSAnimal obj)
        {
        }

        private void DrawStillObject(CSStillObject obj)
        {
            switch (obj.Type)
            {
                case CSStillObjectType.Blood:
                    break;
                case CSStillObjectType.Body:
                    break;
                case CSStillObjectType.Charcoal:
                    break;
                case CSStillObjectType.Excrement:
                    break;
                case CSStillObjectType.Fire:
                    break;
                case CSStillObjectType.Fog:
                    break;
                case CSStillObjectType.Hero:
                    break;
                case CSStillObjectType.Hill:
                    break;
                case CSStillObjectType.Hole:
                    break;
                case CSStillObjectType.Lake:
                    break;
                case CSStillObjectType.Rain:
                    break;
                case CSStillObjectType.Rock:
                    break;
                case CSStillObjectType.Smoke:
                    break;
                case CSStillObjectType.Stream:
                    break;
                case CSStillObjectType.Thunder:
                    break;
                case CSStillObjectType.Tree: DrawTree(obj.Position); break;
                case CSStillObjectType.TypeCount:
                    break;
                case CSStillObjectType.Water:
                    break;
                case CSStillObjectType.WildAnimal:
                    break;
            }
        }

        private void DrawTree(CSVector2 p)
        {
            Matrix[] transforms = new Matrix[Tree.Bones.Count];
            Tree.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in Tree.Meshes)
            {
                Vector3[] color = new Vector3[] { Color.DarkGreen.ToVector3(), Color.SandyBrown.ToVector3() };
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.FogEnabled = true;
                    effect.FogStart = camera.Distance;
                    effect.FogEnd = camera.Distance * 2;
                    effect.FogColor = Color.CornflowerBlue.ToVector3();

                    effect.DiffuseColor = color[mesh.Name == "Leaf" ? 0 : 1];
                    // TODO Optimize it
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation((float)p.x, 0, (float)p.y);
                }
                mesh.Draw();
            }
        }
    }
}
