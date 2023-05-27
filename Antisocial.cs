using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Antisocial
{
	public class Antisocial : Mod
	{
		internal Item hoveredItem;
		internal bool hoveredItemIsSocialArmor;
		internal string socialArmorSetBonus;

		internal const string ModifyAntiSocialConfig_Permission = "ModifyAntisocialConfig";
		internal const string ModifyAntiSocialConfig_Display = "Modify Antisocial Config";

		public override void Load() {
			Terraria.UI.On_ItemSlot.MouseHover_ItemArray_int_int += ItemSlot_MouseHover_ItemArray_int_int;
		}

		private void ItemSlot_MouseHover_ItemArray_int_int(Terraria.UI.On_ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
			orig(inv, context, slot);
			// EquipArmorVanity = 9;
			// EquipAccessoryVanity = 11;
			hoveredItem = null;
			hoveredItemIsSocialArmor = false;
			if (context == ItemSlot.Context.EquipAccessoryVanity) {
				if (inv == Main.LocalPlayer.armor) {
					int socialAccessories = ModContent.GetInstance<ServerConfig>().SocialAccessories;
					// GetAmountOfExtraAccessorySlotsToShow only cared about what can be shown, not what is available, IsAValidEquipmentSlotForIteration is for "can this be used".
					int maxSlotToAffect = socialAccessories == -1 ? 20 : 13 + socialAccessories;
					bool skip = slot == 18 && !Main.LocalPlayer.IsItemSlotUnlockedAndUsable(18) || slot == 19 && !Main.LocalPlayer.IsItemSlotUnlockedAndUsable(19);
					if (!skip && slot < maxSlotToAffect) {
						hoveredItem = Main.HoverItem;
						Main.HoverItem.social = false;
					}
				}
				else {
					// Is a modded slot.
					if (ModContent.GetInstance<ServerConfig>().ModdedAccessorySlots) {
						hoveredItem = Main.HoverItem;
						Main.HoverItem.social = false;
					}
				}
			}
			if (context == ItemSlot.Context.EquipArmorVanity && ModContent.GetInstance<ServerConfig>().SocialArmor) {
				hoveredItem = Main.HoverItem;
				hoveredItemIsSocialArmor = true;
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
		public override void UpdateEquips() {
			ServerConfig serverConfig = ModContent.GetInstance<ServerConfig>();
			int start = serverConfig.SocialArmor ? 10 : 13;
			int socialAccessories = serverConfig.SocialAccessories;
			int end = socialAccessories == -1 ? 20 : 13 + socialAccessories;
			end = Utils.Clamp(end, 13, 20);

			for (int k = start; k < end; k++) {
				Item item = Player.armor[k];
				if (!item.IsAir && Player.IsItemSlotUnlockedAndUsable(k) && (!item.expertOnly || Main.expertMode)) {
					if (item.accessory)
						Player.GrantPrefixBenefits(item);

					Player.GrantArmorBenefits(item);
				}
			}
			for (int l = 13; l < end; l++) {
				if (Player.IsItemSlotUnlockedAndUsable(l))
					Player.ApplyEquipFunctional(Player.armor[l], Player.hideVisibleAccessory[l - 10]);
			}

			// TODO: test this with a mod that actually uses this.
			if (serverConfig.ModdedAccessorySlots) {
				var loader = LoaderManager.Get<AccessorySlotLoader>();
				var bonusSlotPlayer = Player.GetModPlayer<Terraria.ModLoader.Default.ModAccessorySlotPlayer>();
				for (int k = 0; k < bonusSlotPlayer.SlotCount; k++) {
					if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Player)) {
						var slot = loader.Get(k, Player);
						Item item = slot.VanityItem;

						if (item.accessory)
							Player.GrantPrefixBenefits(item);

						Player.GrantArmorBenefits(item);
						Player.ApplyEquipFunctional(item, slot.HideVisuals);
					}
				}
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
				string originalSetBonus = Player.setBonus;
				/*
				for (int i = 0; i < 10; i++) {
					// TODO: armor set stat multiplier! Needs tooltip hint, equip and sets, and original set.
				}
				*/
				Player.UpdateArmorSets(Player.whoAmI);
				Utils.Swap<Item>(ref Player.armor[0], ref Player.armor[10]);
				Utils.Swap<Item>(ref Player.armor[1], ref Player.armor[11]);
				Utils.Swap<Item>(ref Player.armor[2], ref Player.armor[12]);
				Player.head = Player.armor[0].headSlot;
				Player.body = Player.armor[1].bodySlot;
				Player.legs = Player.armor[2].legSlot;
				if (Player.whoAmI == Main.myPlayer)
					ModContent.GetInstance<Antisocial>().socialArmorSetBonus = Player.setBonus; // Move to ModPlayer?
				Player.setBonus = originalSetBonus;
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
			// TODO: Calamity compat: Platinum armor set change not taking place due to calamity code placement:
			// Main.NewText(Main.LocalPlayer.pickSpeed);
			Antisocial antisocial = ModContent.GetInstance<Antisocial>();
			if (item == antisocial.hoveredItem) {
				if (antisocial.hoveredItemIsSocialArmor && antisocial.socialArmorSetBonus != "") {
					tooltips.Add(new TooltipLine(Mod, "SocialArmorSetCheat", Lang.tip[48].Value + " " + antisocial.socialArmorSetBonus));
				}

				//tooltips.RemoveAll(x => x.Name == "Social" || x.Name == "SocialDesc");
				tooltips.Add(new TooltipLine(Mod, "SocialCheat", "Antisocial: Stats WILL be gained"));
			}
		}
	}
}