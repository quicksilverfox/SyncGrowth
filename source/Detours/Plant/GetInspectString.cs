using System.Text;
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
			StringBuilder stringBuilder = new StringBuilder(__result);
			Group group = __instance.GroupOf();

			if (group == null)
			{
				return;
			}

			var regex = new Regex(("GrowthRate".Translate()) + ": [0-9]+%");
			var delta = (GroupsUtils.GrowthCorrectionMultiplier(__instance) - 1f) * 100f;

			if (delta >= 1 || delta <= -1)
				if (regex.IsMatch(__result))
				{
					var replace = "$0 (" + delta.ToString("+#;-#") + "%)";
					__result = regex.Replace(__result, replace);
				}
#if DEBUG
			else
			{
				__result = "(regex error)";
			}
#endif
		}
	}
}
