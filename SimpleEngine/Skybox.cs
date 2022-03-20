﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI411.SimpleEngine
{
    public class Skybox
    {
        private Model skybox;
        public TextureCube skyboxTexture;

        private Effect skyboxEffect;
        private float size = 50f;

        public Skybox(string[] skyboxTextures, ContentManager Content, GraphicsDevice g)
        {
            skybox = Content.Load<Model>("skybox/cube");
            skyboxEffect = Content.Load<Effect>("skybox/Skybox");

            skyboxTexture = new TextureCube(g, 512, false, SurfaceFormat.Color);
            byte[] data = new byte[512 * 512 * 4];
            Texture2D tempTexture = Content.Load<Texture2D>(skyboxTextures[0]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.NegativeX, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[1]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.PositiveX, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[2]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.NegativeY, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[3]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.PositiveY, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[4]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.NegativeZ, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[5]);
            tempTexture.GetData<byte>(data);
            skyboxTexture.SetData<byte>(CubeMapFace.PositiveZ, data);

        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            foreach(EffectPass pass in skyboxEffect.CurrentTechnique.Passes)
            {
                foreach(ModelMesh mesh in skybox.Meshes)
                {
                    foreach(ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyboxEffect;
                        part.Effect.Parameters["World"].SetValue(
                                Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyboxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
