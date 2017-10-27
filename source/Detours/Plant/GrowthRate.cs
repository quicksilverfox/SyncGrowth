using Harmony;

namespace WM.SyncGrowth.Detours.Plant
{
	[HarmonyPatch(typeof(RimWorld.Plant))]
	[HarmonyPatch("GrowthRate", PropertyMethod.Getter)]
	static class GrowthRate
	{
		static void Postfix(ref float __result, RimWorld.Plant __instance)
		{
			var v = GroupsUtils.GetGrowthMultiplierFor(__instance);
			__result *= v;
		}
	}
}
