using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace ChaosOverload.Items.Projectiles
{
    internal class Shockwave_Projectile : ModProjectile
    {
        private float shockcharge = 0f;

        private int rippleCount = 3;
        private int rippleSize = 10;
        private int rippleSpeed = 15;
        private float distortStrength;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 45;
        }

        public override void AI()
        {
            distortStrength = Projectile.ai[0] * 2;
            ActivateShockwave();
            shockcharge += 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            Filters.Scene.Deactivate("Shockwave");
        }

        private void ActivateShockwave()
        {
            if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene.Activate("Shockwave", Projectile.Center)
                .GetShader()
                .UseColor(rippleCount, rippleSize, rippleSpeed)
                .UseTargetPosition(Projectile.Center);
            }

            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                float progress = shockcharge;        //(180f - Projectile.timeLeft) / 60f;
                Filters.Scene["Shockwave"].GetShader()
                    .UseProgress(progress)
                    .UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }

    }
}