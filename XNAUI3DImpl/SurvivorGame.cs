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
    public class SurvivorGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool worldInitialized = false;
        object UIDataLock = new object();
        IntPtr pUIData = IntPtr.Zero;
        CSUIDisplayData UIData;

        KeyboardState prvKeyboardState;

        Matrix worldMatrix;
        Camera camera;

        const float MouseWheelC = 120.0f;
        const float MouseWheelAdjust = 4f;
        Cursor cursor;

        SpriteFont DefaultFont;

        Model Box;
        Model Tree;

        public SurvivorGame()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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

            Content.RootDirectory = Path.Combine(path, "XNASurvivorContent");

            if (Settings.Default.FullScreen)
                graphics.ToggleFullScreen();
        }

        public int Display(IntPtr pdata)
        {
            lock (UIDataLock)
            {
                pUIData = pdata;
            }
            return 1;
        }

        protected override void Initialize()
        {
            cursor = new Cursor(GraphicsDevice);
            cursor.MouseLeftClick += cursor_MouseLeftClick;
            cursor.MouseWheelChanged += cursor_MouseWheelChanged;

            camera = new Camera(GraphicsDevice);

            worldMatrix = Matrix.Identity;

            base.Initialize();
        }

        void cursor_MouseWheelChanged(EventArgs e)
        {
            camera.Distance += cursor.WheelDelta / MouseWheelC * MouseWheelAdjust;
        }

        void cursor_MouseLeftClick(EventArgs e)
        {
            camera.Target = cursor.SelectFloor(camera.ProjectionMatrix, camera.ViewMatrix);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursor.LoadContent(Content);
            DefaultFont = Content.Load<SpriteFont>("Font/DefaultFont");
            Box = Content.Load<Model>("Model/Box");
            Tree = Content.Load<Model>("Model/Tree");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbs= Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Escape))
                this.Exit();
            if (kbs.IsKeyDown(Keys.F))
                graphics.ToggleFullScreen();
            prvKeyboardState = kbs;

            cursor.Update(gameTime);
            camera.Update(gameTime);

            lock(UIDataLock)
            {
                UIData = new CSUIDisplayData(pUIData);
            }
            if (!worldInitialized)
                InitializeWorld();

            base.Update(gameTime);
        }

        private void InitializeWorld()
        {
            worldInitialized = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            graphics.GraphicsDevice.RasterizerState = rs;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawUIData();

            double fps = 1000 / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            string sfps = double.IsInfinity(fps) ? "inf" : fps.ToString() + "fps";
            spriteBatch.Begin();
            spriteBatch.DrawString(DefaultFont, sfps, Vector2.Zero, Color.White);
            cursor.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawUIData()
        {
            foreach (var res in UIData.Map.StillObjects)
            {
                /*switch (res.type)
                {
                    case SCMapResourceType.Hero:
                        break;
                    case SCMapResourceType.WildAnimal:
                        break;
                    case SCMapResourceType.Blood:
                        break;
                    case SCMapResourceType.Excrement:
                        break;
                    case SCMapResourceType.Body:
                        break;
                    case SCMapResourceType.Tree: DrawTree(res.region); break;
                    case SCMapResourceType.Charcoal:
                        break;
                    case SCMapResourceType.Rock:
                        break;
                    case SCMapResourceType.Hill:
                        break;
                    case SCMapResourceType.Water:
                        break;
                    case SCMapResourceType.Stream:
                        break;
                    case SCMapResourceType.Lake:
                        break;
                    case SCMapResourceType.Hole:
                        break;
                    case SCMapResourceType.Fire:
                        break;
                    case SCMapResourceType.Smoke:
                        break;
                    case SCMapResourceType.Fog:
                        break;
                    case SCMapResourceType.Rain:
                        break;
                    case SCMapResourceType.Thunder:
                        break;
                    case SCMapResourceType.TypeCount:
                        break;
                    default:
                        break;
                }*/
            }
        }

        /*private void DrawTree(_SCRegion region)
        {
            _SCPoint pos = (_SCPoint)System.Runtime.InteropServices.Marshal.PtrToStructure(region.verties, typeof(_SCPoint));

            Matrix[] transforms = new Matrix[Tree.Bones.Count];
            Tree.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in Tree.Meshes)
            {
                Vector3[] color = new Vector3[] { Color.DarkGreen.ToVector3(), Color.SandyBrown.ToVector3() };
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.FogEnabled = true;
                    effect.FogStart = 100;
                    effect.FogEnd = 500;
                    effect.FogColor = Color.CornflowerBlue.ToVector3();

                    effect.DiffuseColor = color[mesh.Name == "Leaf" ? 0 : 1];
                    // TODO Optimize it
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3((float)pos.x, 0, (float)pos.y));
                }
                mesh.Draw();
            }
        }*/
    }
}
