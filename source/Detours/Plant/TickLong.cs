using System;
using RimWorld;
using Harmony;
using Verse;

namespace WM.SyncGrowth.Detours.Plant
{
	[HarmonyPatch(typeof(RimWorld.Plant))]
	[HarmonyPatch("TickLong")]
	static class TickLong
	{
		static void Prefix(RimWorld.Plant __instance)
		{
			GroupsUtils.TryCreateCropsGroup(__instance);
		}
	}
}
