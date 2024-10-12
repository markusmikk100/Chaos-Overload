using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosOverload.Items.Projectiles
{
    public class Lightning_Projectile : ModProjectile
    {
        private bool isLaunched = false; // Track whether the projectile has been launched
        private float chargeTime = 0f; // Time the projectile has been charging
        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            DrawOriginOffsetX = 0;
            DrawOriginOffsetY = 0;
            Projectile.light = 0.5f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // If the player is still channeling (holding the shoot button)
            if (player.channel && !isLaunched)
            {
                chargeTime += 1f;

                Projectile.position = player.Center + new Vector2(-67, -player.height - 30);
                Projectile.velocity = Vector2.Zero;

                EmitParticles();

                SoundEngine.PlaySound(SoundID.Item75, Projectile.position);

                float damage = 750 + chargeTime * 4f;
                Projectile.damage = (int)damage;  // Cast to int

            }
            else if (!player.channel && !isLaunched) // If the player releases the shoot button
            {
                isLaunched = true; // Mark the projectile as launched

                // Calculate the direction towards the mouse
                Vector2 mousePosition = Main.MouseWorld;
                Vector2 direction = mousePosition - Projectile.Center;

                // Normalize the direction and adjust speed based on charge time
                direction.Normalize();
                float speed = 10f + chargeTime * 0.1f; // Speed increases with charge time
                Projectile.velocity = direction * speed;
                // Reset charge time
                chargeTime = 0f;
            }

            // Set the rotation to match the projectile's velocity
            if (isLaunched)
            {
                EmitParticlesChase();
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }
        private void EmitParticles()
        {
            Vector2 offset = new Vector2(-30, -6);  // Adjust the X offset (negative = left)
            Vector2 particlePosition = Projectile.position + offset;

            // Emit particles based on the Projectile's position
            if (Main.rand.NextBool(1))  // Emit particles every frame (1 in 3 chance)
            {
                // Create a dust particle
                int dustIndex = Dust.NewDust(particlePosition, 190, 34, 226, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                // Optionally, make the dust move towards the center of the projectile
                Vector2 directionToCenter = Projectile.Center - dust.position;
                directionToCenter.Normalize();  // Normalize to get direction only
                dust.velocity += directionToCenter * 5f;  // Add a small pull towards the center

                // Set the fade time for the dust
                dust.fadeIn = 0.2f;
            }
        }
        private void EmitParticlesChase()
        {
            Vector2 offset = new Vector2(-30, -6);  // Adjust the X offset (negative = left)
            Vector2 particlePosition = Projectile.position + offset;

            // Emit particles based on the Projectile's position
            if (Main.rand.NextBool(1))  // Emit particles every frame (1 in 1 chance)
            {
                // Create a dust particle
                int dustIndex = Dust.NewDust(particlePosition, 190, 34, 226, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                // Optionally, make the dust move towards the center of the projectile
                Vector2 directionToCenter = Projectile.Center - dust.position;
                directionToCenter.Normalize();  // Normalize to get direction only
                dust.velocity += directionToCenter * 5f + Projectile.velocity * 0.5f;  // Add a small pull towards the center

                // Set the fade time for the dust
                dust.fadeIn = 0.2f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Explode();
        }


        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            Vector2 offset = new Vector2(-0f, -0f);
            Vector2 explosionPosition = Projectile.position + offset;

            for (int i = 0; i < 40; i++)
            {
                int dustIndex = Dust.NewDust(explosionPosition, 130, 22, 226, 0f, 0f, 100, default, 0.5f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true; 
                dust.fadeIn = 0.2f; 
                dust.velocity = Projectile.velocity * 2f;
            }
        }


        public override void PostDraw(Color lightColor)
        {
            // Get the glow mask texture
            Texture2D glowMask = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/Lightning_ProjectileGLOW").Value;

            // Position and origin for centering
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(glowMask.Width / 2, glowMask.Height / 2);

            // Draw the glow mask with additive blending for a glowing effect
            Main.spriteBatch.Draw(
                glowMask,
                drawPosition,
                null,
                Color.Aqua,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f);
        }

    }
}