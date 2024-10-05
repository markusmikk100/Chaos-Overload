using ChaosOverload.Items.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ChaosOverload.Primitives
{
    class PrimitiveBeam : ModSystem
    {
        private static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;

        private BasicEffect basicEffect;
        private Effect beamEffect;

        public override void Load()
        {
            Main.RunOnMainThread(() =>
            {
                basicEffect = new BasicEffect(GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    TextureEnabled = true,

                    View = Main.GameViewMatrix.TransformationMatrix
                };
            }).Wait();

            beamEffect = ModContent.Request<Effect>("ChaosOverload/Shaders/BeamShader", AssetRequestMode.ImmediateLoad).Value;
        }

        private void DrawTriangle(Projectile projectile)
        {
            Vector2 center = projectile.Center;
            float rotation = projectile.rotation;

            Vector2 v0 = new(-200, -600);
            Vector2 v1 = new(-200, 600);
            Vector2 v2 = new(2500, -600);
            Vector2 v3 = new(2500, 600);

            VertexPositionColorTexture[] vertices = [
                new(new Vector3(center + v0.RotatedBy(rotation), 0f), Color.White, new(0, 0)),
                new(new Vector3(center + v1.RotatedBy(rotation), 0f), Color.White, new(0, 1)),
                new(new Vector3(center + v2.RotatedBy(rotation), 0f), Color.White, new(1, 0)),
                new(new Vector3(center + v3.RotatedBy(rotation), 0f), Color.White, new(1, 1)),
            ];

            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
        }

        public override void PostDrawTiles()
        {
            Matrix world = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            var viewport = GraphicsDevice.Viewport;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);

            beamEffect.Parameters["WorldViewProjection"].SetValue(world * view * projection);
            beamEffect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;

            int projectileType = ModContent.ProjectileType<Beam>();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (!Main.projectile[i].active) continue;
                if (Main.projectile[i].type != projectileType) continue;

                DrawTriangle(Main.projectile[i]);
            }
        }

        public override void Unload()
        {
            Main.RunOnMainThread(() =>
            {
                basicEffect?.Dispose();
                basicEffect = null;

                beamEffect?.Dispose();
                beamEffect = null;
            });
        }
    }
}