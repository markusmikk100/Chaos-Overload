using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosOverload.Items.Projectiles
{
    public class Beam : ModProjectile
    {
        public static int lifetime = 25;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = lifetime;
            Projectile.penetrate = 999;
            
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            
            Projectile.aiStyle = -1;
            AIType = ProjectileID.None;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.Center.AngleTo(Main.MouseWorld);
            Projectile.position += Vector2.UnitX.RotatedBy(Projectile.rotation) * 30f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Collision.CheckAABBvLineCollision(
                targetHitbox.Location.ToVector2(), 
                targetHitbox.Size(), 
                Projectile.Center, 
                Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 2000
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}