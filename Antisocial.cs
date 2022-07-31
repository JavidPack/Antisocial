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
				// GetAmountOfExtraAccessorySlotsToShow only cared about what can be shown, not what is available, IsAValidEquipmentSlotForIteration is for "can this be used".
				int maxSlotToAffect = socialAccessories == -1 ? 20 : 13 + socialAccessories;
				bool skip = slot == 18 && !Main.LocalPlayer.IsAValidEquipmentSlotForIteration(18) || slot == 19 && !Main.LocalPlayer.IsAValidEquipmentSlotForIteration(19);
				if (!skip && slot < maxSlotToAffect) {
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
			ModLoader.TryGetMod("HEROsMod", out Mod HEROsMod);
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
        public override void UpdateEquips()
        {
			int start = ModContent.GetInstance<ServerConfig>().SocialArmor ? 10 : 13;
			int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
			int end = socialAccessories == -1 ? 20 : 13 + socialAccessories;
			end = Utils.Clamp(end, 13, 20);

			for (int k = start; k < end; k++) {
				if (Player.IsAValidEquipmentSlotForIteration(k))
					Player.VanillaUpdateEquip(Player.armor[k]);
			}
			for (int l = start; l < end; l++)
			{
				if (Player.IsAValidEquipmentSlotForIteration(l))
					Player.ApplyEquipFunctional(Player.armor[l], Player.hideVisibleAccessory[l - 10]);
			}
		}

		// some problems, such as Chlorophyte rapid fire.
		public override void PostUpdateEquips() {
			if (ModContent.GetInstance<ServerConfig>().SocialArmor) {
				Utils.Swap<Item>(ref Player.armor[0], ref Player.armor[10]);
				Utils.Swap<Item>(ref Player.armor[1], ref Player.armor[11]);
				Utils.Swap<Item>(ref Player.armor[2], ref Player.armor[12]);
				Player.head = Player.armor[0].headSlot;
				Player.body = Player.armor[1].bodySlot;
				Player.legs = Player.armor[2].legSlot;
				Player.UpdateArmorSets(Player.whoAmI);
				Utils.Swap<Item>(ref Player.armor[0], ref Player.armor[10]);
				Utils.Swap<Item>(ref Player.armor[1], ref Player.armor[11]);
				Utils.Swap<Item>(ref Player.armor[2], ref Player.armor[12]);
				Player.head = Player.armor[0].headSlot;
				Player.body = Player.armor[1].bodySlot;
				Player.legs = Player.armor[2].legSlot;
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
				tooltips.Add(new TooltipLine(Mod, "SocialCheat", "Antisocial: Stats WILL be gained"));
			}
		}
	}
}

