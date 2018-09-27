using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;
using Terraria.ModLoader;

namespace Antisocial
{
	public class Antisocial : Mod
	{
		const string HarmonyID = "mod.Antisocial";
		HarmonyInstance harmonyInstance;

		public override void Load()
		{
			harmonyInstance = HarmonyInstance.Create(HarmonyID);
			if (!harmonyInstance.HasAnyPatches(HarmonyID)) // In case Unload failed, don't double up.
			{
				harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			}
		}

		public override void Unload()
		{
			if (harmonyInstance != null)
			{
				harmonyInstance.UnpatchAll(HarmonyID);
			}
		}
	}

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
				//tooltips.RemoveAll(x => x.Name == "Social" || x.Name == "SocialDesc");
				tooltips.Add(new TooltipLine(mod, "SocialCheat", "Antisocial: Stats WILL be gained"));
			}
		}
	}

	[HarmonyPatch(typeof(Terraria.UI.ItemSlot))]
	[HarmonyPatch("MouseHover")]
	[HarmonyPatch(new Type[] { typeof(Item[]), typeof(int), typeof(int) })]
	public class MouseText_DrawItemTooltip_Patcher
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			//	Modifies this code:
			//	if (context == 11 || context == 9)
			//	{
			//		Main.HoverItem.social = true;
			//		if (context == 11) // Only works on accessories for now.
			//+			Main.HoverItem.social = false; // it is simpler to write this in IL.
			//	}

			bool success = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand) == AccessTools.Field(typeof(Item), nameof
					(Item.social)))
				{
					yield return instruction;
					Label afterIf11 = il.DefineLabel();

					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Ldc_I4_S, (byte)11);
					yield return new CodeInstruction(OpCodes.Bne_Un_S, afterIf11);

					yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Main), nameof(Main.HoverItem)));
					yield return new CodeInstruction(OpCodes.Ldc_I4_0);
					yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Item), nameof
					(Item.social)));
					yield return new CodeInstruction(OpCodes.Nop) { labels = new List<Label>() { afterIf11 } };
					success = true;
				}
				else
					yield return instruction;
			}
			if(!success)
				ErrorLogger.Log($"Antisocial patch failure. Please report to website. {ModLoader.versionedName}, {ModLoader.compressedPlatformRepresentation}, Antisocial {ModLoader.GetMod("Antisocial")?.Version}");
		}
	}
}

