using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using ChaosOverload.Items.Projectiles;

namespace ChaosOverload.Items.Weapons.Ranged
{
    public class BeamGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 10;
            Item.knockBack = 1f;
            Item.crit = 10;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<Beam>();
            Item.useAmmo = AmmoID.None;

            Item.shoot = ModContent.ProjectileType<Beam>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<Beam>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
