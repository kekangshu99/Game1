using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Matrix world;
        Matrix view;
        Matrix projection;

        float angle = 0;
        float angle2 = 0;
        float distance = 10;

        Model model;
        Effect effect;
        SpriteFont font;

        Vector4 ambientColor = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 LightPosition = new Vector3(1, 1, 1);
        float diffuseIntensity = 1.0f;

        Vector3 camPos = new Vector3(0, 0, 10);
        Vector3 camTarget = new Vector3(0, 0, 0);

        // Lab4 variable
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;
        Vector3 cameraPosition;
        float shininess = 20f;


        // Mouse State
        MouseState previousMouseState;


        public Lab04()
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


            effect = Content.Load<Effect>("SimpleShading");

            model = Content.Load<Model>("Torus");

            font = Content.Load<SpriteFont>("Font");

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                   new Vector3(0, 0, distance),
                   new Vector3(0, 0, 0),
                   new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(90),
                        GraphicsDevice.Viewport.AspectRatio,
                        0.1f, 100f);
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

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                camPos.Z += previousMouseState.X - currentMouseState.X;
            }


            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                camPos.X += previousMouseState.X - currentMouseState.X;
                camPos.Y -= previousMouseState.Y - currentMouseState.Y;

                camTarget.X += previousMouseState.X - currentMouseState.X;
                camTarget.Y -= previousMouseState.Y - currentMouseState.Y;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Left))
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

            /*
            cameraPosition = Vector3.Transform(camPos,
                                                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            */
            cameraPosition = camPos;

            view = Matrix.CreateRotationX(angle2) *
                   Matrix.CreateRotationY(angle) *
                   Matrix.CreateTranslation(-cameraPosition);

            // Update mouse state
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

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            effect.CurrentTechnique = effect.Techniques[1];// first technique

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
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                     
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["DiffuseLightDirection"].SetValue(LightPosition);

                        effect.Parameters["LightPosition"].SetValue(LightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);

                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);

                        pass.Apply(); // Send the data to GPU
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

            base.Draw(gameTime);
        }
    }
}
