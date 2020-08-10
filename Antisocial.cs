using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace Antisocial
{
    public class Antisocial : Mod
    {
        internal Item hoveredItem;

        internal const string ModifyAntiSocialConfig_Permission = "ModifyAntisocialConfig";
        internal const string ModifyAntiSocialConfig_Display = "Modify Antisocial Config";

        public override void Load()
        {
            On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += ItemSlot_MouseHover_ItemArray_int_int;
        }

        private void ItemSlot_MouseHover_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            orig(inv, context, slot);
            // EquipArmorVanity = 9;
            // EquipAccessoryVanity = 11;
            hoveredItem = null;
            if (context == 11)
            {
                int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
                if (slot < (socialAccessories == -1 ? 18 + Main.LocalPlayer.extraAccessorySlots : 13 + socialAccessories))
                {
                    hoveredItem = Main.HoverItem;
                    Main.HoverItem.social = false;
                }
            }
            if (context == 9 && ModContent.GetInstance<ServerConfig>().SocialArmor)
            {
                hoveredItem = Main.HoverItem;
                Main.HoverItem.social = false;
            }
        }

        public override void PostSetupContent()
        {
            Mod HEROsMod = ModLoader.GetMod("HEROsMod");
            if (HEROsMod != null)
            {
                HEROsMod.Call(
                    "AddPermission",
                    ModifyAntiSocialConfig_Permission,
                    ModifyAntiSocialConfig_Display
                );
            }
        }
    }

    public class AntisocialPlayer : ModPlayer
    {
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            int start = ModContent.GetInstance<ServerConfig>().SocialArmor ? 10 : 13;
            int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
            int end = socialAccessories == -1 ? 18 + player.extraAccessorySlots : 13 + socialAccessories;
            end = Utils.Clamp(end, 13, 18 + player.extraAccessorySlots);

            bool olddd2Accessory = player.dd2Accessory;
            for (int k = start; k < end; k++)
            {
                player.VanillaUpdateEquip(player.armor[k]);
            }
            for (int l = start; l < end; l++)
            {
                player.VanillaUpdateAccessory(player.whoAmI, player.armor[l], false /*player.hideVisual[l]*/, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
            }

            //PlayerHooks.UpdateEquips is after this in vanilla, so we need to fix manually
            if (!olddd2Accessory && player.dd2Accessory)
            {
                player.minionDamage += 0.1f;
                player.maxTurrets++;
            }
        }

        // some problems, such as Chlorophyte rapid fire.
        public override void PostUpdateEquips()
        {
            if (ModContent.GetInstance<ServerConfig>().SocialArmor)
            {
                string setbonus = player.setBonus;
                Item head = player.armor[10];
                Item body = player.armor[11];
                Item legs = player.armor[12];

                AddSocialArmorSetBonus(player, head, body, legs);

                string setBonus = player.setBonus;

                player.setBonus = setbonus;
            }
        }

        /// <summary>
        /// Add SocialArmor Set Bonus,Copy from Terraria.Player.UpdateArmorSet
        /// </summary>
        /// <param name="player"></param>
        /// <param name="head"></param>
        /// <param name="body"></param>
        /// <param name="legs"></param>
        /// <returns></returns>
        private string AddSocialArmorSetBonus(Player player, Item head, Item body, Item legs)
        {
            string setBonus = "";
            int headSlot = head.headSlot;
            int bodySlot = body.bodySlot;
            int legSlot = legs.legSlot;

            if (bodySlot == 67 && legSlot == 56 && headSlot >= 103 && headSlot <= 105)
            {
                //player.setBonus = Language.GetTextValue("ArmorSetBonus.Shroomite");
                player.shroomiteStealth = true;
            }
            if ((headSlot == 52 && bodySlot == 32 && legSlot == 31) || (headSlot == 53 && bodySlot == 33 && legSlot == 32) || (headSlot == 54 && bodySlot == 34 && legSlot == 33) || (headSlot == 55 && bodySlot == 35 && legSlot == 34) || (headSlot == 70 && bodySlot == 46 && legSlot == 42) || (headSlot == 71 && bodySlot == 47 && legSlot == 43) || (headSlot == 166 && bodySlot == 173 && legSlot == 108) || (headSlot == 167 && bodySlot == 174 && legSlot == 109))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Wood");
                player.statDefense++;
            }
            if ((headSlot == 1 && bodySlot == 1 && legSlot == 1) || ((headSlot == 72 || headSlot == 2) && bodySlot == 2 && legSlot == 2) || (headSlot == 47 && bodySlot == 28 && legSlot == 27))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier1");
                player.statDefense += 2;
            }
            if ((headSlot == 3 && bodySlot == 3 && legSlot == 3) || ((headSlot == 73 || headSlot == 4) && bodySlot == 4 && legSlot == 4) || (headSlot == 48 && bodySlot == 29 && legSlot == 28) || (headSlot == 49 && bodySlot == 30 && legSlot == 29))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.MetalTier2");
                player.statDefense += 3;
            }
            if (headSlot == 188 && bodySlot == 189 && legSlot == 129)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Fossil");
                player.thrownCost50 = true;
            }
            if (headSlot == 50 && bodySlot == 31 && legSlot == 30)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Platinum");
                player.statDefense += 4;
            }
            if (headSlot == 112 && bodySlot == 75 && legSlot == 64)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Pumpkin");
                player.allDamage += 0.1f;
            }
            if (headSlot == 22 && bodySlot == 14 && legSlot == 14)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Ninja");
                player.thrownCost33 = true;

            }
            if (headSlot == 157 && bodySlot == 105 && legSlot == 98)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDamage");
                int num = 0;

                player.beetleOffense = true;
                player.beetleCounter -= 3f;
                player.beetleCounter -= player.beetleCountdown / 10;
                player.beetleCountdown++;
                if (player.beetleCounter < 0f)
                {
                    player.beetleCounter = 0f;
                }
                int num6 = 400;
                int num7 = 1200;
                int num8 = 4600;
                if (player.beetleCounter > (float)(num6 + num7 + num8 + num7))
                {
                    player.beetleCounter = num6 + num7 + num8 + num7;
                }
                if (player.beetleCounter > (float)(num6 + num7 + num8))
                {
                    player.AddBuff(100, 5, quiet: false);
                    num = 3;
                }
                else if (player.beetleCounter > (float)(num6 + num7))
                {
                    player.AddBuff(99, 5, quiet: false);
                    num = 2;
                }
                else if (player.beetleCounter > (float)num6)
                {
                    player.AddBuff(98, 5, quiet: false);
                    num = 1;
                }
                if (num < player.beetleOrbs)
                {
                    player.beetleCountdown = 0;
                }
                else if (num > player.beetleOrbs)
                {
                    player.beetleCounter += 200f;
                }
                if (num != player.beetleOrbs && player.beetleOrbs > 0)
                {
                    for (int j = 0; j < Player.MaxBuffs; j++)
                    {
                        if (player.buffType[j] >= 98 && player.buffType[j] <= 100 && player.buffType[j] != 97 + num)
                        {
                            player.DelBuff(j);
                        }
                    }
                }
            }
            else if (headSlot == 157 && bodySlot == 106 && legSlot == 98)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.BeetleDefense");
                player.beetleDefense = true;
                player.beetleCounter += 1f;
                int num9 = 180;
                if (player.beetleCounter >= (float)num9)
                {
                    if (player.beetleOrbs > 0 && player.beetleOrbs < 3)
                    {
                        for (int k = 0; k < Player.MaxBuffs; k++)
                        {
                            if (player.buffType[k] >= 95 && player.buffType[k] <= 96)
                            {
                                player.DelBuff(k);
                            }
                        }
                    }
                    if (player.beetleOrbs < 3)
                    {
                        player.AddBuff(95 + player.beetleOrbs, 5, quiet: false);
                        player.beetleCounter = 0f;
                    }
                    else
                    {
                        player.beetleCounter = num9;
                    }
                }
            }
            if (!player.beetleDefense && !player.beetleOffense)
            {
                player.beetleCounter = 0f;
            }
            else
            {
                player.beetleFrameCounter++;
                if (player.beetleFrameCounter >= 1)
                {
                    player.beetleFrameCounter = 0;
                    player.beetleFrame++;
                    if (player.beetleFrame > 2)
                    {
                        player.beetleFrame = 0;
                    }
                }
                for (int l = player.beetleOrbs; l < 3; l++)
                {
                    player.beetlePos[l].X = 0f;
                    player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < player.beetleOrbs; m++)
                {
                    player.beetlePos[m] += player.beetleVel[m];
                    player.beetleVel[m].X += (float)Main.rand.Next(-100, 101) * 0.005f;
                    player.beetleVel[m].Y += (float)Main.rand.Next(-100, 101) * 0.005f;
                    float x = player.beetlePos[m].X;
                    float y = player.beetlePos[m].Y;
                    float num10 = (float)Math.Sqrt(x * x + y * y);
                    if (num10 > 100f)
                    {
                        num10 = 20f / num10;
                        x *= 0f - num10;
                        y *= 0f - num10;
                        int num11 = 10;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (float)(num11 - 1) + x) / (float)num11;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (float)(num11 - 1) + y) / (float)num11;
                    }
                    else if (num10 > 30f)
                    {
                        num10 = 10f / num10;
                        x *= 0f - num10;
                        y *= 0f - num10;
                        int num12 = 20;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (float)(num12 - 1) + x) / (float)num12;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (float)(num12 - 1) + y) / (float)num12;
                    }
                    x = player.beetleVel[m].X;
                    y = player.beetleVel[m].Y;
                    num10 = (float)Math.Sqrt(x * x + y * y);
                    if (num10 > 2f)
                    {
                        player.beetleVel[m] *= 0.9f;
                    }
                    player.beetlePos[m] -= player.velocity * 0.25f;
                }
            }
            if (headSlot == 14 && ((bodySlot >= 58 && bodySlot <= 63) || bodySlot == 167))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Wizard");
                player.magicCrit += 10;
            }
            if (headSlot == 159 && ((bodySlot >= 58 && bodySlot <= 63) || bodySlot == 167))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.MagicHat");
                player.statManaMax2 += 60;
            }
            if ((headSlot == 5 || headSlot == 74) && (bodySlot == 5 || bodySlot == 48) && (legSlot == 5 || legSlot == 44))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.ShadowScale");
                player.moveSpeed += 0.15f;
            }
            if (headSlot == 57 && bodySlot == 37 && legSlot == 35)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Crimson");
                player.crimsonRegen = true;
            }
            if (headSlot == 101 && bodySlot == 66 && legSlot == 55)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.SpectreHealing");
                player.ghostHeal = true;
            }
            if (headSlot == 156 && bodySlot == 66 && legSlot == 55)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.SpectreDamage");
                player.ghostHurt = true;
            }
            if (headSlot == 6 && bodySlot == 6 && legSlot == 6)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Meteor");
                player.spaceGun = true;
            }
            if (headSlot == 46 && bodySlot == 27 && legSlot == 26)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Frost");
                player.frostArmor = true;

                player.frostBurn = true;
            }
            if ((headSlot == 75 || headSlot == 7) && bodySlot == 7 && legSlot == 7)
            {

                //setBonus = Language.GetTextValue("ArmorSetBonus.Bone");
                player.boneArmor = true;
                player.ammoCost80 = true;
            }
            if ((headSlot == 76 || headSlot == 8) && (bodySlot == 49 || bodySlot == 8) && (legSlot == 45 || legSlot == 8))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Jungle");
                player.manaCost -= 0.16f;
            }
            if (headSlot == 9 && bodySlot == 9 && legSlot == 9)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Molten");
                player.meleeDamage += 0.17f;
            }
            if (headSlot == 11 && bodySlot == 20 && legSlot == 19)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Mining");
                player.pickSpeed -= 0.3f;
            }
            if ((headSlot == 78 || headSlot == 79 || headSlot == 80) && bodySlot == 51 && legSlot == 47)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Chlorophyte");
                player.AddBuff(60, 18000);
            }
            else if (player.crystalLeaf)
            {
                for (int n = 0; n < Player.MaxBuffs; n++)
                {
                    if (player.buffType[n] == 60)
                    {
                        player.DelBuff(n);
                    }
                }
            }
            if (headSlot == 99 && bodySlot == 65 && legSlot == 54)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Turtle");
                player.thorns = 1f;
                player.turtleThorns = true;
            }
            if (bodySlot == 17 && legSlot == 16)
            {
                if (headSlot == 29)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.CobaltCaster");
                    player.manaCost -= 0.14f;
                }
                else if (headSlot == 30)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.CobaltMelee");
                    player.meleeSpeed += 0.15f;
                }
                else if (headSlot == 31)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.CobaltRanged");
                    player.ammoCost80 = true;
                }
            }
            if (bodySlot == 18 && legSlot == 17)
            {
                if (headSlot == 32)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.MythrilCaster");
                    player.manaCost -= 0.17f;
                }
                else if (headSlot == 33)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.MythrilMelee");
                    player.meleeCrit += 5;
                }
                else if (headSlot == 34)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.MythrilRanged");
                    player.ammoCost80 = true;
                }
            }
            if (bodySlot == 19 && legSlot == 18)
            {
                if (headSlot == 35)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteCaster");
                    player.manaCost -= 0.19f;
                }
                else if (headSlot == 36)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteMelee");
                    player.meleeSpeed += 0.18f;
                    player.moveSpeed += 0.18f;
                }
                else if (headSlot == 37)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.AdamantiteRanged");
                    player.ammoCost75 = true;
                }
            }
            if (bodySlot == 54 && legSlot == 49 && (headSlot == 83 || headSlot == 84 || headSlot == 85))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Palladium");
                player.onHitRegen = true;
            }
            if (bodySlot == 55 && legSlot == 50 && (headSlot == 86 || headSlot == 87 || headSlot == 88))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Orichalcum");
                player.onHitPetal = true;
            }
            if (bodySlot == 56 && legSlot == 51 && (headSlot == 89 || headSlot == 90 || headSlot == 91))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Titanium");
                player.onHitDodge = true;
            }
            if (bodySlot == 24 && legSlot == 23)
            {
                if (headSlot == 42)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.HallowCaster");
                    player.manaCost -= 0.2f;
                }
                else if (headSlot == 43)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.HallowMelee");
                    player.meleeSpeed += 0.19f;
                    player.moveSpeed += 0.19f;
                }
                else if (headSlot == 41)
                {
                    //setBonus = Language.GetTextValue("ArmorSetBonus.HallowRanged");
                    player.ammoCost75 = true;
                }
            }
            if (headSlot == 82 && bodySlot == 53 && legSlot == 48)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Tiki");
                player.maxMinions++;
            }
            if (headSlot == 134 && bodySlot == 95 && legSlot == 79)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Spooky");
                player.minionDamage += 0.25f;
            }
            if (headSlot == 160 && bodySlot == 168 && legSlot == 103)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Bee");
                player.minionDamage += 0.1f;
                if (player.itemAnimation > 0 && player.inventory[player.selectedItem].type == 1121)
                {
                    AchievementsHelper.HandleSpecialEvent(player, 3);
                }
            }
            if (headSlot == 162 && bodySlot == 170 && legSlot == 105)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Spider");
                player.minionDamage += 0.12f;
            }
            if (headSlot == 171 && bodySlot == 177 && legSlot == 112)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Solar");
                player.setSolar = true;

                player.solarCounter++;
                int num13 = 240;
                if (player.solarCounter >= num13)
                {
                    if (player.solarShields > 0 && player.solarShields < 3)
                    {
                        for (int num2 = 0; num2 < Player.MaxBuffs; num2++)
                        {
                            if (player.buffType[num2] >= 170 && player.buffType[num2] <= 171)
                            {
                                player.DelBuff(num2);
                            }
                        }
                    }
                    if (player.solarShields < 3)
                    {
                        player.AddBuff(170 + player.solarShields, 5, quiet: false);
                        for (int num3 = 0; num3 < 16; num3++)
                        {
                            Dust obj = Main.dust[Dust.NewDust(player.position, player.width, player.height, 6, 0f, 0f, 100)];
                            obj.noGravity = true;
                            obj.scale = 1.7f;
                            obj.fadeIn = 0.5f;
                            obj.velocity *= 5f;
                            obj.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                        }
                        player.solarCounter = 0;
                    }
                    else
                    {
                        player.solarCounter = num13;
                    }
                }
                for (int num4 = player.solarShields; num4 < 3; num4++)
                {
                    player.solarShieldPos[num4] = Vector2.Zero;
                }
                for (int num5 = 0; num5 < player.solarShields; num5++)
                {
                    player.solarShieldPos[num5] += player.solarShieldVel[num5];
                    Vector2 value = ((float)player.miscCounter / 100f * ((float)Math.PI * 2f) + (float)num5 * ((float)Math.PI * 2f / (float)player.solarShields)).ToRotationVector2() * 6f;
                    value.X = player.direction * 20;
                    player.solarShieldVel[num5] = (value - player.solarShieldPos[num5]) * 0.2f;
                }
                if (player.dashDelay >= 0)
                {
                    player.solarDashing = false;
                    player.solarDashConsumedFlare = false;
                }
                bool flag = player.solarDashing && player.dashDelay < 0;
                if (player.solarShields > 0 || flag)
                {
                    player.dash = 3;
                }
            }
            else
            {
                player.solarCounter = 0;
            }
            if (headSlot == 169 && bodySlot == 175 && legSlot == 110)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Vortex", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
                player.setVortex = true;

            }
            else
            {
                //vortexStealthActive = false;
            }
            if (headSlot == 170 && bodySlot == 176 && legSlot == 111)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Nebula");
                if (player.nebulaCD > 0)
                {
                    player.nebulaCD--;
                }
                player.setNebula = true;

            }
            if (headSlot == 189 && bodySlot == 190 && legSlot == 130)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Stardust", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
                player.setStardust = true;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(187) == -1)
                    {
                        player.AddBuff(187, 3600);
                    }
                    if (player.ownedProjectileCounts[623] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, 623, 0, 0f, Main.myPlayer);
                    }
                }
            }
            else if (player.FindBuffIndex(187) != -1)
            {
                player.DelBuff(player.FindBuffIndex(187));
            }
            if (headSlot == 200 && bodySlot == 198 && legSlot == 142)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.Forbidden", Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN"));
                player.setForbidden = true;
                player.UpdateForbiddenSetLock();
                Lighting.AddLight(player.Center, 0.8f, 0.7f, 0.2f);
            }
            if (headSlot == 204 && bodySlot == 201 && legSlot == 145)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.SquireTier2");
                player.setSquireT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 203 && bodySlot == 200 && legSlot == 144)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.ApprenticeTier2");
                player.setApprenticeT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 205 && bodySlot == 202 && (legSlot == 147 || legSlot == 146))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.HuntressTier2");
                player.setHuntressT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 206 && bodySlot == 203 && legSlot == 148)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.MonkTier2");
                player.setMonkT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 210 && bodySlot == 204 && legSlot == 152)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.SquireTier3");
                player.setSquireT3 = true;
                player.setSquireT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 211 && bodySlot == 205 && legSlot == 153)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.ApprenticeTier3");
                player.setApprenticeT3 = true;
                player.setApprenticeT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 212 && bodySlot == 206 && (legSlot == 154 || legSlot == 155))
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.HuntressTier3");
                player.setHuntressT3 = true;
                player.setHuntressT2 = true;
                player.maxTurrets++;
            }
            if (headSlot == 213 && bodySlot == 207 && legSlot == 156)
            {
                //setBonus = Language.GetTextValue("ArmorSetBonus.MonkTier3");
                player.setMonkT3 = true;
                player.setMonkT2 = true;
                player.maxTurrets++;
            }
            ItemLoader.UpdateArmorSet(player, head, body, legs);
            return setBonus;
        }
    }

    public class AntisocialGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // leaving social this late means .defense tooltips not added.
            //bool addTooltip = false;
            //if (item.social && item.accessory) {
            //	addTooltip = true;
            //}
            //if (item.social && (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0) && ModContent.GetInstance<ServerConfig>().SocialArmor) {
            //	addTooltip = true;
            //}
            if (item == ModContent.GetInstance<Antisocial>().hoveredItem)
            {
                //tooltips.RemoveAll(x => x.Name == "Social" || x.Name == "SocialDesc");
                tooltips.Add(new TooltipLine(mod, "SocialCheat", "Antisocial: Stats WILL be gained"));
            }
        }
    }
}

