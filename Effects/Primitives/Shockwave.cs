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
                Asset<Effect> screenEffect = ModContent.Request<Effect>("ChaosOverload/Effects/Shaders/ShockwaveEffect");

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenEffect, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();
            }
        }
    }
}