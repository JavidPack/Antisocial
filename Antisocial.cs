using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Antisocial
{
	public class Antisocial : Mod { }

	public class AntisocialPlayer : ModPlayer
	{
		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			// TODO, change to 13 or have a ModConfig
			for (int k = 13; k < 18 + player.extraAccessorySlots; k++)
			{
				player.VanillaUpdateEquip(player.armor[k]);
			}
			for (int l = 13; l < 18 + player.extraAccessorySlots; l++)
			{
				player.VanillaUpdateAccessory(player.whoAmI, player.armor[l], false /*player.hideVisual[l]*/, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
			}
		}

		// some problems, such as Chlorophyte rapid fire.
		//public override void PostUpdateEquips()
		//{
		//	Utils.Swap<Item>(ref player.armor[0], ref player.armor[10]);
		//	Utils.Swap<Item>(ref player.armor[1], ref player.armor[11]);
		//	Utils.Swap<Item>(ref player.armor[2], ref player.armor[12]);
		//	player.head = player.armor[0].headSlot;
		//	player.body = player.armor[1].bodySlot;
		//	player.legs = player.armor[2].legSlot;
		//	player.UpdateArmorSets(player.whoAmI);
		//	Utils.Swap<Item>(ref player.armor[0], ref player.armor[10]);
		//	Utils.Swap<Item>(ref player.armor[1], ref player.armor[11]);
		//	Utils.Swap<Item>(ref player.armor[2], ref player.armor[12]);
		//	player.head = player.armor[0].headSlot;
		//	player.body = player.armor[1].bodySlot;
		//	player.legs = player.armor[2].legSlot;
		//}
	}

	public class AntisocialGlobalItem : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.social && item.accessory)
			{
				tooltips.RemoveAll(x => x.Name == "Social" || x.Name == "SocialDesc");
				tooltips.Add(new TooltipLine(mod, "SocialCheat", "Antisocial: Stats WILL be gained, but are not shown"));
			}
		}
	}
}

