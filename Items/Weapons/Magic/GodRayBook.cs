using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using ChaosOverload.Items.Projectiles;
using System;

namespace ChaosOverload.Items.Weapons.Magic
{
    public class GodRayBook : ModItem
    {
        private int? projectileId;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.useTime = -1;
            Item.useAnimation = -1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.channel = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 100;
            Item.knockBack = 1f;
            Item.crit = 0;
            Item.noMelee = true;
            Item.mana = 0;

            Item.shoot = ModContent.ProjectileType<GodRay>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (projectileId == null)
            {
                projectileId = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<GodRay>(), damage, knockback, player.whoAmI);
                UpdateProjectile(player);
            }

            return false;
        }

        public override void HoldItem(Player player)
        {
            if (player.channel)
            {
                if (projectileId == null)
                {
                    return;
                }

                UpdateProjectile(player);
                return;
            }

            if (projectileId != null)
            {
                projectileId = null;
            }
        }

        private void UpdateProjectile(Player player)
        {
            GodRay projectile = (GodRay)Main.projectile[(int)projectileId].ModProjectile;

            if (projectile.GetTimeLeft() <= GodRay.lifetime / 2)
            {
                projectile.Projectile.timeLeft += 1;
            }

            projectile.rotation = player.Center.AngleTo(Main.MouseWorld);
            projectile.origin = player.Center + Vector2.UnitX.RotatedBy(projectile.rotation) * 30f;
        }
    }
}
