﻿using System.Text;
using Harmony;
using Verse;
using System.Text.RegularExpressions;

namespace WM.SyncGrowth.Detours.Plant
{
	[HarmonyPatch(typeof(RimWorld.Plant))]
	[HarmonyPatch("GetInspectString")]
	static class GetInspectString
	{
		static void Postfix(ref string __result, RimWorld.Plant __instance)
		{
			if (!GroupsUtils.HasGroup(__instance))
				return;

			var stringBuilder = new StringBuilder(__result);
			var regex = new Regex(("GrowthRate".Translate()) + ": [0-9]+%");
			var delta = (GroupsUtils.GetGrowthMultiplierFor(__instance) - 1f) * 100f;

			if (delta >= 1 || delta <= -1)
				if (regex.IsMatch(__result))
				{
					var replace = "$0 (" + delta.ToString("+#;-#") + "%)";
					__result = regex.Replace(__result, replace);
				}
#if DEBUG
				else
				{
					__result += "\n(regex error)";
				}
			__result += "\nCanHaveGroup() = " + GroupMaker.CanHaveGroup(__instance, true);
#endif
		}
	}
}
