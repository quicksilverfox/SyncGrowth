using System;
using RimWorld;
using Harmony;
using Verse;

namespace WM.SyncGrowth.Detours.Plant
{
	[HarmonyPatch(typeof(RimWorld.Plant))]
	[HarmonyPatch("GrowthRate", PropertyMethod.Getter)]
	static class GrowthRate
	{
		static void Postfix(ref float __result, RimWorld.Plant __instance)
		{
			var v = GroupsUtils.GrowthCorrectionMultiplier(__instance);
			__result *= v;

			//#if DEBUG
			//			Log.Message(__instance + " x" + v);
			//#endif
		}
	}
}
