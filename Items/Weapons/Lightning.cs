using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ChaosOverload.Items.Projectiles;

namespace ChaosOverload.Items.Weapons
{
    public class Lightning : ModItem
    {
        private int channelTime = 0;
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Zap me baby");
        }

        public override void SetDefaults()
        {
            Item.damage = 750;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 0;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.channel = true; // You can hold the weapon
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item75;
            Item.shoot = ModContent.ProjectileType<Lightning_Projectile>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.None; // No ammo for magic weapons
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }


        public override void HoldItem(Player player)
        {
            if (player.channel)
            {
                channelTime++; // Increment the time spent channeling

                // Scale the mana cost based on channel time (e.g., increase cost faster the longer you hold)
                int manaCost = 1 + (channelTime / 120); // Base 1 mana cost, increases by 1 every second (60 frames)

                if (player.statMana >= manaCost) // Ensure the player has enough mana
                {
                    player.statMana -= manaCost; // Deduct scaled mana cost

                    // Prevent negative mana
                    if (player.statMana < 0)
                    {
                        player.statMana = 0;
                    }

                    // Reset mana regeneration delay
                    player.manaRegenDelay = 60; // Prevent mana regeneration during channeling
                }
                else
                {
                    // If the player runs out of mana, stop the channeling
                    player.channel = false;
                }
            }
            else
            {
                // Reset the channelTime when channeling ends
                channelTime = 0;
            }
        }
    }
}