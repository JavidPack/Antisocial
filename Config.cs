using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Antisocial
{
	[Label("Antisocial")]
	class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(-1)]
		[Range(-1, 7)]
		[Label("[i:156] Antisocial Accessories")]
		[Tooltip("Customize how many slots are anti-social slots. -1 means all.")]
		public int SocialAccessories { get; set; }

		[DefaultValue(false)]
		[Label("[i:82] Antisocial Armor")] 
		[Tooltip("If true, social armor will also have effect. Be aware that it is buggy with some armor set bonuses.")]
		public bool SocialArmor { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) {
			ModLoader.TryGetMod("HEROsMod", out Mod HEROsMod);
			if (HEROsMod != null && HEROsMod.Version >= new Version(0, 2, 2)) {
				if (HEROsMod.Call("HasPermission", whoAmI, Antisocial.ModifyAntiSocialConfig_Permission) is bool result && result)
					return true;
				message = $"You lack the \"{Antisocial.ModifyAntiSocialConfig_Display}\" permission.";
				return false;
			}

			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}
	}
}
