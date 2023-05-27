using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Antisocial
{
	class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(-1)]
		[Range(-1, 7)]
		public int SocialAccessories { get; set; }

		[DefaultValue(false)]
		public bool SocialArmor { get; set; }

		[DefaultValue(true)]
		public bool ModdedAccessorySlots { get; set; }

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) {
			ModLoader.TryGetMod("HEROsMod", out Mod HEROsMod);
			if (HEROsMod != null && HEROsMod.Version >= new Version(0, 2, 2)) {
				if (HEROsMod.Call("HasPermission", whoAmI, Antisocial.ModifyAntiSocialConfig_Permission) is bool result && result)
					return true;
				message = this.GetLocalization("YouLackTheXPermission").Format(Antisocial.ModifyAntiSocialConfig_Display);
				return false;
			}

			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}
	}
}
