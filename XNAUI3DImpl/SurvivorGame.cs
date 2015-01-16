using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CSSurvivorLibrary;

using SysForm = System.Windows.Forms.Form;
using SysCtrl = System.Windows.Forms.Control;

namespace XNAUI3DImpl
{
    public class SurvivorGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        object UIDataLock = new object();
        IntPtr pUIData = IntPtr.Zero;
        UIDisplayData UIData;

        MouseState prvMouseState;
        KeyboardState prvKeyboardState;

        Matrix worldMatrix;
        Camera camera = new Camera();
        Matrix projectionMatrix;

        Model box;

        public SurvivorGame()
        {
            SysForm winForm = SysCtrl.FromHandle(Window.Handle) as SysForm;
            winForm.Icon = WinResource.SurvivorIcon;
            winForm.Text = "Survivor";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = @"D:\Develop\Survivor\x86\Debug\XNASurvivorContent";

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            worldMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                (float)GraphicsDevice.Viewport.Width /
                (float)GraphicsDevice.Viewport.Height,
                0.1f, 10000.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            box = Content.Load<Model>("Box");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbs= Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Escape))
                this.Exit();
            prvKeyboardState = kbs;

            MouseState ms = Mouse.GetState();
            int delta = ms.ScrollWheelValue - prvMouseState.ScrollWheelValue;
            if (delta != 0)
                camera.Distance += delta / 120.0f;
            prvMouseState = ms;

            lock(UIDataLock)
            {
                UIData = new UIDisplayData(pUIData);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rs;

            foreach (ModelMesh mesh in box.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    //effect.View = viewMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.Projection = projectionMatrix;
                    effect.World = worldMatrix;
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
