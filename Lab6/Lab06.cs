using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab6
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab06 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;
        Model model;
        Texture2D texture;
        float reflectivity = 0.5f;

        Matrix world;
        Matrix view;
        Matrix projection;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle = 0;
        float angle2 = 0;
        float distance = 3;
        MouseState previousMouseState;

        Skybox skybox;

        public Lab06()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;
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
            model = Content.Load<Model>("Helicopter");
            texture = Content.Load<Texture2D>("HelicopterTexture");

            string[] skyboxTextures =
            {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5"
            };
            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);

            effect = Content.Load<Effect>("Reflection");
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
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            // Draw skyboxfirst
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(view, projection, cameraPosition);

            //Draw 3D model
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            DrawModelWithEffect();

            base.Draw(gameTime);
        }

        private void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[0];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["decalMap"].SetValue(texture);

                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["reflectivity"].SetValue(reflectivity);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyboxTexture);
                        
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }
        }
    }
}
