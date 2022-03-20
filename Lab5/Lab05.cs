using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;
        Model model;
        Texture2D texture;

        Matrix world;
        Matrix view;
        Matrix projection;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle = 0;
        float angle2 = 0;
        float distance = 10;
        MouseState previousMouseState;

        // Lab5
        Skybox skybox;

        public Lab05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            string[] skyboxTextures =
            {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5"
            };
            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            world = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                  Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));

            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.Up,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle))
                );

            previousMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            base.Draw(gameTime);
        }
    }
}
