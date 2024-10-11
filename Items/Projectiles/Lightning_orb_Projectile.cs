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


        private int rippleCount = 3;
        private int rippleSize = 15;
        private int rippleSpeed = 15;
        private float distortStrength = 100f;




        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            DrawOriginOffsetX = 0;
            DrawOriginOffsetY = 0;
            Projectile.light = 1f;
            Projectile.penetrate = -1;
            Main.projFrames[Projectile.type] = 4;
            Projectile.timeLeft = 36000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Check the charge time to determine which texture to draw
            Texture2D texture;

            if (chargeTime < 60)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/S1").Value; // Default texture

                PullNearbyEnemies1();
            }

            else if (chargeTime < 260)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/Lightning_orb_Projectile").Value; // Custom charged texture

                PullNearbyEnemies1();
            }

            else if (chargeTime < 320)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/S2").Value; // Custom charged texture

                PullNearbyEnemies1();
            }

            else if (chargeTime < 520)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/LP2").Value; // Custom charged texture

                PullNearbyEnemies1();
            }

            else if (chargeTime < 580)
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/S3").Value; // Custom charged texture

                PullNearbyEnemies1();
            }

            else
            {
                texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Projectiles/LP3").Value; // Custom charged texture

                PullNearbyEnemies1();
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
                if (chargeTime < 60)
                {
                    charge();
                }

                else if (chargeTime < 260)
                {
                    charge();
                }

                else if (chargeTime < 320)
                {
                    charge2();
                }

                else if (chargeTime < 520)
                {
                    charge2();
                }

                else if (chargeTime < 580)
                {
                    charge3();
                }

                else
                {
                    charge3();
                }

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

                Projectile.tileCollide = true;

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
            ActivateShockwave();
            Explode();
            PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (chargeTime * 0.075f), 6f, 20, -1, FullName);
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

            for (int i = 0; i < 100; i++)
            {

                float angle = MathHelper.TwoPi * i / 100;
                float xOffset = 100 * (float)Math.Cos(angle);



                int dustIndex = Dust.NewDust(Projectile.position, 0, 0, 135, 0f, 0f, 100, default, 4f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;


                // Optionally, give the dust some velocity for a more dynamic effect
                dust.velocity = new Vector2(-xOffset); // Adjust velocity multiplier as needed
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

        private void charge()
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            Player player = Main.player[Projectile.owner];

            Vector2 offset = new Vector2(1000, 800);
            Vector2 pos = player.Center - offset;

            for (int i = 0; i < 10; i++)
            {

                int dustIndex = Dust.NewDust(pos, 2000, 1600, 135, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.noLight = true;

                Vector2 directionToCenter = Projectile.Center - dust.position;
                directionToCenter.Normalize();  // Normalize to get direction only
                dust.velocity += directionToCenter * 5f + Projectile.velocity * 15f;
            }
        }
        private void charge2()
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            Player player = Main.player[Projectile.owner];

            Vector2 offset = new Vector2(1000, 800);
            Vector2 pos = player.Center - offset;

            for (int i = 0; i < 10; i++)
            {

                int dustIndex = Dust.NewDust(pos, 2000, 1600, 127, 0f, 0f, 100, default, 3f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.noLight = true;

                Vector2 directionToCenter = Projectile.Center - dust.position;
                directionToCenter.Normalize();  // Normalize to get direction only
                dust.velocity += directionToCenter * 5f + Projectile.velocity * 15f;
            }
        }
        private void charge3()
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            Player player = Main.player[Projectile.owner];

            Vector2 offset = new Vector2(1000, 800);
            Vector2 pos = player.Center - offset;

            for (int i = 0; i < 10; i++)
            {

                int dustIndex = Dust.NewDust(pos, 2000, 1600, 181, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.noLight = true;

                Vector2 directionToCenter = Projectile.Center - dust.position;
                directionToCenter.Normalize();  // Normalize to get direction only
                dust.velocity += directionToCenter * 5f + Projectile.velocity * 15f;
            }
        }

        private void PullNearbyEnemies1()  //GLITCH DUMMYS
        {
            float pullRadius = 100f + chargeTime*0.4f; // Radius around the projectile to pull enemies
            float pullStrength = 2f; // Adjust the strength of the pulling force

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.dontTakeDamage) // Only pull enemies
                {
                    float distanceToProjectile = Vector2.Distance(npc.Center, Projectile.Center);

                    if (distanceToProjectile < pullRadius)
                    {
                        Vector2 directionToProjectile = Projectile.Center - npc.Center;
                        directionToProjectile.Normalize();

                        // Apply pulling force
                        float pullForce = (pullRadius - distanceToProjectile) / pullRadius * pullStrength;
                        npc.velocity += directionToProjectile * pullForce;

                        // Optionally, reduce the NPC's speed if it's being pulled too fast
                        if (npc.velocity.Length() > 10f)
                        {
                            npc.velocity *= 0.9f; // Slow down the NPC if it's too fast
                        }
                    }
                }
            }
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
                float progress = (180f - Projectile.timeLeft) / 60f; // Progress based on time left
                Filters.Scene["Shockwave"].GetShader()
                    .UseProgress(progress)
                    .UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }

    }
}
    
