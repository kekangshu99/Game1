using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab02
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;  // Shader file
        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0,1,0),new Vector2(0.5f,0f)),
            new VertexPositionTexture(new Vector3(1,0,0), new Vector2(1,1)),
            new VertexPositionTexture(new Vector3(-1,0,0), new Vector2(0,1))
        };
        Texture2D texture;

        // lab2 Variables
        Matrix world;
        Matrix view;
        Matrix projection;

        float angle = 0;
        float distance = 2;


        public Lab02()
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
            effect = Content.Load<Effect>("SimpleTexture");
            texture = Content.Load<Texture2D>("Logo_mg");

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                   new Vector3(0, 0, 2),
                   new Vector3(0,0,0),
                   new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(90),
                        GraphicsDevice.Viewport.AspectRatio,
                        0.1f, 100f);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
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
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                distance -= 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                distance += 0.02f;
            }


            Vector3 cameraPosition = distance * 
                                                new Vector3((float)System.Math.Sin(angle), 
                                                0, 
                                                (float)System.Math.Cos(angle));

            view = Matrix.CreateLookAt(
                   cameraPosition,
                   new Vector3(0, 0, 0),
                   new Vector3(0, 1, 0));
            effect.Parameters["View"].SetValue(view);




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

            effect.Parameters["MyTexture"].SetValue(texture);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply(); // send the pass to the GPU
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}
