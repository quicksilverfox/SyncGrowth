using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Source.Detour;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	
	public static class GroupsUtils
	{
		private static int lastTicked = 0;

		public static List<Group> allGroups = new List<Group>();
		public static Dictionary<Plant, Group> allThingsInAGroup = new Dictionary<Plant, Group>();

		private static void Reset()
		{
			if (lastTicked + 2000 > (Find.TickManager.TicksGame))
				return ;

			#if DEBUG
			Log.Message("RESET ! had "+allGroups.Count+" groups of crops.");
#endif

			lastTicked = Find.TickManager.TicksGame;

			allThingsInAGroup.Clear();
			allGroups.Clear();

			//foreach (List<Thing> list in thingsAndGroups.Values)
			//{
			//	list.Clear();
			//}
		}
		static List<ThingDef> blacklist = new List<ThingDef> { ThingDefOf.PlantGrass, ThingDef.Named("PlantRaspberry") };

		public static void TryCreateCropsGroup(Plant crop)
		{
			Reset();

			if (crop.def.ingestible.foodType == FoodTypeFlags.Tree  || crop.def.plant.reproduces || blacklist.Contains(crop.def))
				return;

			//if (crop.Position.GetZone(crop.Map) == null && (crop.Position.GetFirstBuilding(crop.Map) == null || crop.Position.GetFirstBuilding(crop.Map).def != ThingDef.Named("HydroponicsBasin") ) )
			//	return;
			
			if (allThingsInAGroup.ContainsKey(crop))
				return ;

			Group newGroup = new Group();

			try
			{
				Iterate(crop, newGroup);

				//newGroup.TickLong();
			}
			catch (StackOverflowException ex)
			{
				Log.Warning("failed to create group from crop " + crop + " : " + ex.Message + ".");
			}

			if (newGroup.Count > 1)
			{
				newGroup.PostProcess();
			}


		}

		static void Iterate(Plant crop,Group group)
		{
			if (!group.TryAdd(crop))
				return;

			foreach (IntVec3 cell in crop.CellsAdjacent8WayAndInside())
			{
				Plant thingAtCell = cell.GetPlant(crop.Map);

				if (thingAtCell != null && crop.GrowthRateFactor_Light == thingAtCell.GrowthRateFactor_Light && crop.GrowthRateFactor_Fertility == thingAtCell.GrowthRateFactor_Fertility)
					Iterate(thingAtCell, group);
			}
		}

		//public static float GrowthCorrectionFor(this Plant plant)
		//{
		//	if (!allThingsInAGroup.ContainsKey(plant))
		//		return 0f;

		//	Group group = allThingsInAGroup[plant];

		//	return group.GrowthCorrectionFor(plant);
		//}

		public static float GrowthRateCorrection(this Plant plant)
		{
			if (!allThingsInAGroup.ContainsKey(plant))
				return 1f;

			Group group = allThingsInAGroup[plant];

			return group.GrowthRateCorrectionFor(plant);
			//return 0f;
		}

		public static float TicksUntilFullyGrown(this Plant plant)
		{
			return (int)typeof(RimWorld.Plant).GetProperty("TicksUntilFullyGrown", Helpers.AllBindingFlags).GetValue(plant, null);
		}

		public static float GrowthPerTick(this Plant plant)
		{
			return (int)typeof(RimWorld.Plant).GetProperty("GrowthPerTick", Helpers.AllBindingFlags).GetValue(plant, null);
		}
	}
}
