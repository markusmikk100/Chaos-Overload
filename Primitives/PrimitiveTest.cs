using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ChaosOverload.Primitives
{
    class PrimitiveTest : ModSystem
    {
        static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;

        BasicEffect basicEffect;

        public override void Load()
        {
            Main.RunOnMainThread(() =>
            {
                basicEffect = new BasicEffect(GraphicsDevice);

                basicEffect.VertexColorEnabled = true;
                basicEffect.TextureEnabled = true;
            }).Wait();
        }

        public override void PostDrawTiles()
        {
            Vector2 playerCenter = Main.LocalPlayer.Center;

            VertexPositionColorTexture[] vertices = [
                new(new Vector3(playerCenter + new Vector2(100, 70), 0f), Color.Red, Vector2.Zero),
                    new(new Vector3(playerCenter + new Vector2(-100, 70), 0f), Color.Blue, Vector2.Zero),
                    new(new Vector3(playerCenter + new Vector2(0, -100), 0f), Color.Green, Vector2.Zero),
                ];

            basicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
            basicEffect.View = Main.GameViewMatrix.TransformationMatrix;

            var viewport = GraphicsDevice.Viewport;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: -1, zFarPlane: 10);

            Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;

            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1);
        }

        public override void Unload()
        {
            Main.RunOnMainThread(() =>
            {
                basicEffect?.Dispose();
                basicEffect = null;
            });
        }
    }
}