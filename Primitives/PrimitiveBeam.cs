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

        private readonly int length = 2000;
        private readonly int startHeigh = 2;
        private readonly int endHeightExtra = 13;

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
            float lengthMultiplier = 1 - projectile.timeLeft / (float)Beam.lifetime;

            Vector2 v0 = new(0, -startHeigh);
            Vector2 v1 = new(0, startHeigh);
            Vector2 v2 = new(length * lengthMultiplier, -startHeigh - endHeightExtra * lengthMultiplier);
            Vector2 v3 = new(length * lengthMultiplier, startHeigh + endHeightExtra * lengthMultiplier);

            VertexPositionColorTexture[] vertices = [
                new(new Vector3(center + v0.RotatedBy(rotation), 0f), Color.White, Vector2.Zero),
                new(new Vector3(center + v1.RotatedBy(rotation), 0f), Color.White, Vector2.Zero),
                new(new Vector3(center + v2.RotatedBy(rotation), 0f), Color.White, Vector2.Zero),
                new(new Vector3(center + v3.RotatedBy(rotation), 0f), Color.White, Vector2.Zero),
            ];

            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
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