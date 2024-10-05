using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ChaosOverload.Items.Projectiles
{

    internal class Lightning_orb_Projectile : ModProjectile
    {
        private bool isLaunched = false; // Track whether the projectile has been launched
        private float chargeTime = 0f; // Time the projectile has been charging
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            DrawOriginOffsetX = 0;
            DrawOriginOffsetY = 0;
            Projectile.light = 1f;

            Main.projFrames[Projectile.type] = 4;
        }

        public override void AI()
        {

            // Animate the projectile
            Projectile.frameCounter++; // Increment the frame counter every tick (1/60th of a second)

            // Change frame every 5 ticks (adjust to make animation faster/slower)
            if (Projectile.frameCounter >= 1.5)
            {
                Projectile.frameCounter = 0; // Reset the frame counter
                Projectile.frame++; // Move to the next frame

                // If the frame exceeds the total number of frames, loop back to the first frame
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }


            Player player = Main.player[Projectile.owner];

            // If the player is still channeling (holding the shoot button)
            if (player.channel && !isLaunched)
            {
                chargeTime += 1f;

                Projectile.position = player.Center + new Vector2(-18, -player.height - 35);
                Projectile.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Item75, Projectile.position);

                float damage = 100 + chargeTime * 5f;
                Projectile.damage = (int)damage;


                Projectile.scale = 1 + chargeTime * 0.01f;
            }
            else if (!player.channel && !isLaunched) // If the player releases the shoot button
            {
                isLaunched = true;

                // Calculate the direction towards the mouse
                Vector2 mousePosition = Main.MouseWorld;
                Vector2 direction = mousePosition - Projectile.Center;


                direction.Normalize();
                float speed = 10f + chargeTime * 0.1f;
                Projectile.velocity = direction * speed;
            }

            // Set the rotation to match the projectile's velocity
            if (isLaunched)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float multiplier = 1 + chargeTime * 0.005f;
            float delta = 38 * multiplier - hitbox.Size().X;
            hitbox.Inflate((int)delta, (int)delta);
        }
    }
}


