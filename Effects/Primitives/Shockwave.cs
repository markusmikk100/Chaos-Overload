using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosOverload.Effects.Primitives
{
    class Shockwave : ModSystem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> Shockwave = ChaosOverload.Instance.Assets.Request<Effect>("Effects/Shaders/ShockwaveEffect", AssetRequestMode.ImmediateLoad);
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(Shockwave, "Shockwave"), EffectPriority.High);
                Filters.Scene["Shockwave"].Load();
            }
        }
    }
}