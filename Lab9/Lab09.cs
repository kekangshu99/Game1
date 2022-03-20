using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab9
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab09 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
            new Vector3(0, 0, 20),
            new Vector3(0, 0, 0),
            Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            800f / 600f,
            0.1f,
            100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model[] models;
        Texture2D texture;

        // End of Template

        Matrix lightView;
        Matrix lightProjection;


        // Lab9
        RenderTarget2D renderTarget;
        Texture2D shadowMap;


        public Lab09()
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

            font = Content.Load<SpriteFont>("Font");
            models = new Model[2];
            models[0] = Content.Load<Model>("Plane");
            models[1] = Content.Load<Model>("torus");
            effect = Content.Load<Effect>("ShadowShader");

            // Lab9 Step1
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(
                            GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single,
                            DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
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

            // ************ TEMPLATE ************ //
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero; }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            lightView = Matrix.CreateLookAt(
                lightPosition,
                Vector3.Zero,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));

            lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);


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

            // Lab9 Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            // Lab9 Step3 Render shadow map
            DrawShadowMap();

            // Lab9 Step4 Clear the render target
            GraphicsDevice.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;

            // Lab9 Step5 Clear the render target
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.DarkSlateBlue, 1.0f, 0);
            // Lab9 Step6 Draw a Scene
            DrawShadowedScene();

            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
            //                    Color.DarkSlateBlue, 1.0f, 0);

            
            //Draw Depth Map on top of 3D scene
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                sprite.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0,
                        new Vector2(0, 0), 0.1f, SpriteEffects.None, 1);
                sprite.End();
            }
            

            shadowMap = null;
            base.Draw(gameTime);
        }

        // Private Method
        private void DrawShadowMap()
        {
            effect.CurrentTechnique = effect.Techniques[0];

            foreach (Model model in models)
            {
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


                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                            effect.Parameters["LightPosition"].SetValue(lightPosition);
                            //effect.Parameters["ProjectiveTexture"].SetValue(texture);

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

        private void DrawShadowedScene()
        {
            effect.CurrentTechnique = effect.Techniques["ShadowedScene"];
            foreach (Model model in models)
            {
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


                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                            effect.Parameters["LightPosition"].SetValue(lightPosition);

                            // Lab9
                            effect.Parameters["ShadowMap"].SetValue(shadowMap);

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
}
