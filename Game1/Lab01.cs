using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab01
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab01 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Effect effect;  // Shader file

        float angle; // a global variable
        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0,1,0),new Vector2(0.5f,-1f)),
            new VertexPositionTexture(new Vector3(1,0,0), new Vector2(1,0)),
            new VertexPositionTexture(new Vector3(-1,0,0), new Vector2(0,0)) 
        };

        Texture2D texture;

        public Lab01()
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

            effect = Content.Load<Effect>("SimpleHLSL");
            texture = Content.Load<Texture2D>("Logo_mg");
            effect.Parameters["MyTexture"].SetValue(Content.Load<Texture2D>("logo_mg"));

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
                Vector3 offset = new Vector3(
                                 (float)System.Math.Cos(angle),
                                 (float)System.Math.Sin(angle),
                                 0);
                effect.Parameters["offset"].SetValue(offset);
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
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // TODO: Add your drawing code here
            foreach(var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply(); // send the pass to the GPU
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}
