using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Antisocial
{
	public class Antisocial : Mod
	{
		internal Item hoveredItem;

		internal const string ModifyAntiSocialConfig_Permission = "ModifyAntisocialConfig";
		internal const string ModifyAntiSocialConfig_Display = "Modify Antisocial Config";

		public override void Load() {
			On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += ItemSlot_MouseHover_ItemArray_int_int;
		}

		private void ItemSlot_MouseHover_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
			orig(inv, context, slot);
			// EquipArmorVanity = 9;
			// EquipAccessoryVanity = 11;
			hoveredItem = null;
			if (context == 11) {
				int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
				if (slot < (socialAccessories == -1 ? 18 + Main.LocalPlayer.extraAccessorySlots : 13 + socialAccessories)) {
					hoveredItem = Main.HoverItem;
					Main.HoverItem.social = false;
				}
			}
			if (context == 9 && ModContent.GetInstance<ServerConfig>().SocialArmor) {
				hoveredItem = Main.HoverItem;
				Main.HoverItem.social = false;
			}
		}

		public override void PostSetupContent() {
			Mod HEROsMod = ModLoader.GetMod("HEROsMod");
			if (HEROsMod != null) {
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
		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
			int start = ModContent.GetInstance<ServerConfig>().SocialArmor ? 10 : 13;
			int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
			int end = socialAccessories == -1 ? 18 + player.extraAccessorySlots : 13 + socialAccessories;
			end = Utils.Clamp(end, 13, 18 + player.extraAccessorySlots);

			bool olddd2Accessory = player.dd2Accessory;
			for (int k = start; k < end; k++) {
				player.VanillaUpdateEquip(player.armor[k]);
			}
			for (int l = start; l < end; l++) {
				player.VanillaUpdateAccessory(player.whoAmI, player.armor[l], false /*player.hideVisual[l]*/, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
			}

			//PlayerHooks.UpdateEquips is after this in vanilla, so we need to fix manually
			if (!olddd2Accessory && player.dd2Accessory) {
				player.minionDamage += 0.1f;
				player.maxTurrets++;
			}
		}

		// some problems, such as Chlorophyte rapid fire.
		public override void PostUpdateEquips() {
			if (ModContent.GetInstance<ServerConfig>().SocialArmor) {
				Utils.Swap<Item>(ref player.armor[0], ref player.armor[10]);
				Utils.Swap<Item>(ref player.armor[1], ref player.armor[11]);
				Utils.Swap<Item>(ref player.armor[2], ref player.armor[12]);
				player.head = player.armor[0].headSlot;
				player.body = player.armor[1].bodySlot;
				player.legs = player.armor[2].legSlot;
				player.UpdateArmorSets(player.whoAmI);
				Utils.Swap<Item>(ref player.armor[0], ref player.armor[10]);
				Utils.Swap<Item>(ref player.armor[1], ref player.armor[11]);
				Utils.Swap<Item>(ref player.armor[2], ref player.armor[12]);
				player.head = player.armor[0].headSlot;
				player.body = player.armor[1].bodySlot;
				player.legs = player.armor[2].legSlot;
			}
		}
	}

	public class AntisocialGlobalItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			// leaving social this late means .defense tooltips not added.
			//bool addTooltip = false;
			//if (item.social && item.accessory) {
			//	addTooltip = true;
			//}
			//if (item.social && (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0) && ModContent.GetInstance<ServerConfig>().SocialArmor) {
			//	addTooltip = true;
			//}
			if (item == ModContent.GetInstance<Antisocial>().hoveredItem) {
				//tooltips.RemoveAll(x => x.Name == "Social" || x.Name == "SocialDesc");
				tooltips.Add(new TooltipLine(mod, "SocialCheat", "Antisocial: Stats WILL be gained"));
			}
		}
	}
}

