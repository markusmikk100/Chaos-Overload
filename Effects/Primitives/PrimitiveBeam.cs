using ChaosOverload.Items.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace ChaosOverload.Effects.Primitives
{
    class BeamRT : ARenderTargetContentByRequest
    {
        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_AndListenToEvents(ref _target, device, 2560, 1600, RenderTargetUsage.PlatformContents);

            RenderTargetBinding[] oldTargets = device.GetRenderTargets();
            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Matrix world = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            var viewport = device.Viewport;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);

            PrimitiveBeam.beamEffect.Parameters["WorldViewProjection"].SetValue(world * view * projection);
            PrimitiveBeam.beamEffect.CurrentTechnique.Passes[0].Apply();

            device.RasterizerState = RasterizerState.CullNone;
            device.Textures[0] = TextureAssets.MagicPixel.Value;

            int projectileType = ModContent.ProjectileType<Beam>();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (!Main.projectile[i].active) continue;
                if (Main.projectile[i].type != projectileType) continue;

                Vector2 center = Main.projectile[i].Center;
                float rotation = Main.projectile[i].rotation;

                Vector2 v0 = new(0, -10);
                Vector2 v1 = new(0, 10);
                Vector2 v2 = new(2500, -10);
                Vector2 v3 = new(2500, 10);

                VertexPositionColorTexture[] vertices = [
                    new(new Vector3(center + v0.RotatedBy(rotation), 0f), Color.White, new(0, 0)),
                    new(new Vector3(center + v1.RotatedBy(rotation), 0f), Color.White, new(0, 1)),
                    new(new Vector3(center + v2.RotatedBy(rotation), 0f), Color.White, new(1, 0)),
                    new(new Vector3(center + v3.RotatedBy(rotation), 0f), Color.White, new(1, 1)),
                ];

                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
            }

            spriteBatch.End();
            device.SetRenderTargets(oldTargets);
            _wasPrepared = true;
        }
    }

    class PrimitiveBeam : ModSystem
    {
        public static Effect beamEffect;
        private static BeamRT beamRT;

        public override void Load()
        {
            Main.ContentThatNeedsRenderTargets.Add(beamRT = new BeamRT());
            beamEffect = ModContent.Request<Effect>("ChaosOverload/Effects/Shaders/BeamShader", AssetRequestMode.ImmediateLoad).Value;

            Asset<Effect> bloom = ChaosOverload.Instance.Assets.Request<Effect>("Effects/Shaders/BeamScreenShader", AssetRequestMode.ImmediateLoad);
            Filters.Scene["Bloom"] = new Filter(new ScreenShaderData(bloom, "P0"), EffectPriority.High);
            Filters.Scene["Bloom"].Load();
        }

        public override void PostDrawTiles()
        {
            beamRT.Request();

            if (beamRT.IsReady)
            {
                Main.spriteBatch.Begin();

             //   Filters.Scene.Activate("Bloom");

                Main.spriteBatch.Draw(beamRT.GetTarget(), Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }
        }

        public override void Unload()
        {
            Main.RunOnMainThread(() =>
            {
                beamEffect?.Dispose();
                beamEffect = null;
            });

            Main.ContentThatNeedsRenderTargets.Remove(beamRT);
        }
    }
}