using ChaosOverload.Items.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace ChaosOverload.Items.Weapons.Ranged
{
    internal class Lightning_orb : ModItem
    {
        private int channelTime = 0;
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("Zap me baby");
        }

        public override void SetDefaults()
        {
            Item.damage = '?';
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
            Item.shoot = ModContent.ProjectileType<Lightning_orb_Projectile>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.None; // No ammo for magic weapons
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Load your custom texture (or create a reference to the desired texture)
            Texture2D texture = ModContent.Request<Texture2D>("ChaosOverload/Items/Weapons/Ranged/Lightning_orb_inv").Value;
            
            Vector2 offset = new Vector2(-14, -14);
            position += offset;

            spriteBatch.Draw(
                texture,
                position, // Position in the inventory slot
                null, // Remove frame, use entire texture
                drawColor, // The draw color
                0f, // No rotation
                origin, // Origin point
                scale, // Scale of the texture
                SpriteEffects.None, // No special sprite effects
                0f // Layer depth
             );
            return false;// Return false to prevent the default item drawing
        }

        //public override void HoldItem(Player player)
        //{
        //    if (player.channel)
        //    {
        //        channelTime++; // Increment the time spent channeling

        //        // Scale the mana cost based on channel time (e.g., increase cost faster the longer you hold)
        //        int manaCost = 1 + channelTime / 120; // Base 1 mana cost, increases by 1 every second (60 frames)

        //        if (player.statMana >= manaCost) // Ensure the player has enough mana
        //        {
        //            player.statMana -= manaCost; // Deduct scaled mana cost

        //            // Prevent negative mana
        //            if (player.statMana < 0)
        //            {
        //                player.statMana = 0;
        //            }

        //            // Reset mana regeneration delay
        //            player.manaRegenDelay = 60; // Prevent mana regeneration during channeling
        //        }
        //        else
        //        {
        //            // If the player runs out of mana, stop the channeling
        //            player.channel = false;
        //        }
        //    }
        //    else
        //    {
        //        // Reset the channelTime when channeling ends
        //        channelTime = 0;
        //    }
        //}
    }
}