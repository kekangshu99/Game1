using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // lab3 Variables
        Matrix world;
        Matrix view;
        Matrix projection;

        float angle = 0;
        float angle2 = 0;
        float distance = 10;

        // Lab3 Variables
        Model model;
        Effect effect;

        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float diffuseIntensity = 1.0f;

        Vector3 camPos = new Vector3(0, 0, 10);
        Vector3 camTarget = new Vector3(0, 0, 0);

        // Mouse State
        MouseState previousMouseState;

        public Lab03()
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
            effect = Content.Load<Effect>("Diffuse");

            // TODO: use this.Content to load your game content here

            model = Content.Load<Model>("bunny");

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

            MouseState currentMouseState = Mouse.GetState();
            // TODO: Add your update logic here

            //if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            if(currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            if(currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
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
            Vector3 cameraPosition = distance *
                                                new Vector3((float)System.Math.Sin(angle),
                                                0,
                                                (float)System.Math.Cos(angle));
            */

            //This is working, but have bug when camare look directly down
            /*
            Vector3 cameraPosition = Vector3.Transform(camPos,
                                                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));

            
            
            
            view = Matrix.CreateLookAt(
                   cameraPosition,
                   camTarget,
                   new Vector3(0, 1, 0));
            */

            // Solving the above problem
            Vector3 cameraPosition = new Vector3(0, 0, distance);

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

            // TODO: Add your drawing code here


            //*** Lab3 Step1:
            /*
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            model.Draw(world, view, projection);

            */
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            effect.CurrentTechnique = effect.Techniques[0];// first technique

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
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
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseLightDirection"].SetValue(diffuseLightDirection);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

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
