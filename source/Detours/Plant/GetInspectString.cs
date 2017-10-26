using System;
using RimWorld;
using Harmony;
using Verse;
using System.Text;
using System.Text.RegularExpressions;

namespace WM.SyncGrowth.Detours.Plant
{
	[HarmonyPatch(typeof(RimWorld.Plant))]
	[HarmonyPatch("GetInspectString")]
	static class GetInspectString
	{
		//		static void Postfix(ref string __result, RimWorld.Plant __instance)
		//		{
		//			StringBuilder stringBuilder = new StringBuilder(__result);

		//			Regex.Replace(__result,""

		//			stringBuilder.Append(GetInspectString_base());

		//			// --------------- mod -------------------
		//			Group group = this.GroupOf();
		//			if (group == null)
		//			{
		//				GroupsUtils.TryCreateCropsGroup(this);
		//				group = this.GroupOf();
		//			}

		//			if (group != null)
		//			{
		//				stringBuilder.AppendLine("SyncGrowth: Group #" + GroupsUtils.allGroups.IndexOf(group) + " (" + group.plants.Count + " crops) Growth range: (" + group.MinGrowth.ToStringPercent() + " - " + group.MaxGrowth.ToStringPercent() + ")");

		//#if DEBUG
		//				stringBuilder.AppendLine("SyncGrowth debug: Growth range: (" + group.MinGrowth.ToStringPercent() + " - " + group.MaxGrowth.ToStringPercent() + ")");

		//				// TODO: works very pooly, should be upgraded
		//				this.DebugDrawGroup();
		//#endif
		//			}

		//			// --------------- mod end -------------------

		//			return stringBuilder.ToString();
		//		}
	}
}
