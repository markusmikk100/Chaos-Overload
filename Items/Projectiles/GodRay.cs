using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosOverload.Items.Projectiles
{
    class GodRayDisplay : ModSystem
    {
        public override void Load()
        {
            Asset<Effect> shader = ChaosOverload.Instance.Assets.Request<Effect>("Effects/Shaders/GodRay", AssetRequestMode.ImmediateLoad);
            Filters.Scene["GodRay"] = new Filter(new ScreenShaderData(shader, "P0"), EffectPriority.High);
            Filters.Scene["GodRay"].Load();
        }

        public override void PostUpdateProjectiles()
        {
            int projectileType = ModContent.ProjectileType<GodRay>();

            List<Vector2> origins = [];
            List<Vector2> targets = [];

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];

                if (!proj.active) continue;
                if (proj.type != projectileType) continue;
                if (origins.Count == 10) break;

                GodRay modProj = (GodRay)proj.ModProjectile;

                if (!modProj.IsAlive())
                {
                    continue;
                }

                float startPosMultiplier = 0;
                float endPosMultiplier = 1;

                origins.Add(GetScreenSpacePosition(proj.Center + Vector2.UnitX.RotatedBy(proj.rotation) * GodRay.rayLength * startPosMultiplier));
                targets.Add(GetScreenSpacePosition(proj.Center + Vector2.UnitX.RotatedBy(proj.rotation) * GodRay.rayLength * endPosMultiplier));
            }

            if (origins.Count == 0)
            {
                if (Filters.Scene["GodRay"].IsActive())
                {
                    Filters.Scene["GodRay"].GetShader().Shader.Parameters["count"].SetValue(0);
                    Filters.Scene.Deactivate("GodRay");
                }

                return;
            }

            if (!Filters.Scene["GodRay"].IsActive())
            {
                Filters.Scene.Activate("GodRay");
            }

            Filters.Scene["GodRay"].GetShader().Shader.Parameters["count"].SetValue(origins.Count);
            Filters.Scene["GodRay"].GetShader().Shader.Parameters["origins"].SetValue(origins.ToArray());
            Filters.Scene["GodRay"].GetShader().Shader.Parameters["targets"].SetValue(targets.ToArray());
        }

        private Vector2 GetScreenSpacePosition(Vector2 origin)
        {
            Vector2 res = Main.ViewSize;

            Vector2 normalized = (origin - Main.ViewPosition) / res;
            Vector2 yFlipped = new(normalized.X, 1 - normalized.Y);
            Vector2 centered = yFlipped * 2 - Vector2.One;
            Vector2 adjustedRes = new(centered.X * (res.X / res.Y), centered.Y);

            return adjustedRes;
        }
    }

    class GodRay : ModProjectile
    {
        public static readonly int rayLength = 2500;
        public static readonly int lifetime = 60;
        public static readonly int despawnOffset = 30;

        public Vector2 origin = Vector2.Zero;
        public float rotation = 0;

        public int GetTimeLeft()
        {
            return Projectile.timeLeft - despawnOffset;
        }

        public bool IsAlive()
        {
            return GetTimeLeft() > 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = lifetime;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.alpha = 255;

            Projectile.aiStyle = -1;
            AIType = ProjectileID.None;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.Center.AngleTo(Main.MouseWorld);
            Projectile.position += Vector2.UnitX.RotatedBy(Projectile.rotation) * 30f;
        }

        public override void AI()
        {
            Projectile.position = Vector2.Lerp(Projectile.position, origin, 0.15f);

            float deltaAngle = MathHelper.WrapAngle(rotation - Projectile.rotation);
            float newRotation = Projectile.rotation + Math.Clamp(deltaAngle, -0.5f, 0.5f);
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, newRotation, 0.15f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(
                targetHitbox.Location.ToVector2(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * rayLength
            );
        }
    }
}