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

    internal class Lightning_orb_Projectile : ModProjectile
    {
        private bool isLaunched = false; 
        private float chargeTime = 0f;


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
            Projectile.penetrate = -1;
            Main.projFrames[Projectile.type] = 4; 
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Check the charge time to determine which texture to draw
            Texture2D texture;

            if (chargeTime < 300)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/Lightning_orb_Projectile").Value; // Default texture
            }

            else if (chargeTime < 304)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/stars").Value; // Custom charged texture
            }

            else
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/Lightning_orb_projectile2").Value; // Custom charged texture
            }

            // Assume your texture has multiple frames stacked vertically
            int frameHeight = texture.Height / Main.projFrames[Projectile.type]; // Height of each frame
            int frameY = Projectile.frame * frameHeight; // Y position of the current frame

            // Define the source rectangle to draw the current frame
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2); // Center the origin based on frame size

            // Draw the current frame of the texture
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition, // Position on screen
                sourceRectangle, // Only draw the current frame
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false; // Return false to prevent the default drawing behavior
        }

        public override void AI()
        {

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

                Projectile.position = player.Center + new Vector2(-18, -player.height - 45 - (chargeTime * 0.2f));

                Projectile.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Item75, Projectile.position);

                float damage = chargeTime * 8f;                                                              //DMG
                Projectile.damage = (int)damage;


                Projectile.scale = 1 + chargeTime * 0.01f;                                                   //SCALE
            }
            else if (!player.channel && !isLaunched) // If the player releases the shoot button
            {
                isLaunched = true;

                // Calculate the direction towards the mouse
                Vector2 mousePosition = Main.MouseWorld;
                Vector2 direction = mousePosition - Projectile.Center;


                direction.Normalize();


                float speed = 15f - chargeTime * 0.01f;                                                     //speed
                speed = Math.Max(speed, 1f);
                Projectile.velocity = direction * speed;

            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)  //WEIRD SHIT THAT I WONT TOUCH
        {
            float multiplier = 1 + chargeTime * 0.005f;
            float delta = 38 * multiplier - hitbox.Size().X;
            hitbox.Inflate((int)delta, (int)delta);
        }

        //public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        //{
        //    totalDamageDealt += damageDone;

        //    Main.NewText($"Projectile has dealt a total of {totalDamageDealt} and {chargeTime}.");


        //    if (totalDamageDealt >= 1500)
        //    {
        //        Projectile.Kill();
        //    }
        //}
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)  // Check if the NPC will be killed by the hit
            {   
                return;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            Explode();
            PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (chargeTime * 0.075f), 6f, 20, 1000f, FullName);
            Main.instance.CameraModifiers.Add(modifier); //SCREEN SHAKEEE WOO
        }

        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (int i = 0; i < 100; i++)
            {

                float angle = MathHelper.TwoPi * i / 100;
                float xOffset = 100 * (float)Math.Cos(angle);
  


                int dustIndex = Dust.NewDust(Projectile.position, 0, 0, 135, 0f, 0f, 100, default, 4f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                

                // Optionally, give the dust some velocity for a more dynamic effect
                dust.velocity = new Vector2(xOffset); // Adjust velocity multiplier as needed
            }

            float radius = 1000 * (1 + chargeTime * 0.005f);

            for (int i = 0; i < 100; i++)
            {

                float angle = MathHelper.TwoPi * i / 100;

                float xOffset = radius * (float)Math.Cos(angle);
                float yOffset = radius * (float)Math.Sin(angle);


                int dustIndex = Dust.NewDust(Projectile.position, 0, 0, 226, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;

                // Optionally, give the dust some velocity for a more dynamic effect
                dust.velocity = new Vector2(xOffset * 0.1f, yOffset * 0.1f); // Adjust velocity multiplier as needed
            }
        }

    }
}
    
