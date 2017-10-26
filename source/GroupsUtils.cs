using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{

	public static class GroupsUtils
	{
		private static int lastTicked = 0;

		public static List<Plant> allListedPlants = new List<Plant>();
		public static List<Group> allGroups = new List<Group>();
		public static Dictionary<Plant, Group> allThingsInAGroup = new Dictionary<Plant, Group>();

		private static void Reset()
		{
			if (lastTicked + 2000 > (Find.TickManager.TicksGame))
				return;

#if DEBUG
			Log.Message("RESET ! had " + allGroups.Count + " groups of crops.");
#endif

			lastTicked = Find.TickManager.TicksGame;

			allListedPlants.Clear();
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

			var zone = crop.Map.zoneManager.ZoneAt(crop.Position);
			var ingestible = crop.def.ingestible;
			var plant = crop.def.plant;

			if ((ingestible != null && ingestible.foodType == FoodTypeFlags.Tree) || (plant == null || plant.reproduces) || blacklist.Contains(crop.def) ||
			    zone != null && !(zone is Zone_Growing)
			   )
				return;

			//if (crop.Position.GetZone(crop.Map) == null && (crop.Position.GetFirstBuilding(crop.Map) == null || crop.Position.GetFirstBuilding(crop.Map).def != ThingDef.Named("HydroponicsBasin") ) )
			//	return;

			if (allThingsInAGroup.ContainsKey(crop))
				return;

			List<Plant> list = new List<Plant>();

			try
			{
				_Iterate(crop, list);

				//newGroup.TickLong();
			}
			catch (Exception ex)
			{
				Log.Warning("failed to create group from crop " + crop + " at " + crop.Position + " : " + ex.Message + ". " + ex.StackTrace);
				return;
			}

			Group newGroup;
			if (list.Count > 0)
				newGroup = new Group(list);

			//if (newGroup.Count > 1)
			//{
			//	newGroup.PostProcess();
			//}


		}

		static void _Iterate(Plant crop, List<Plant> list)
		{
			if (list.Count > 0 && list[0].def != crop.def)
				return;

			//if (list.Contains(crop))
			//	return;

			if (GroupsUtils.allListedPlants.Contains(crop))
				return;

			GroupsUtils.allListedPlants.Add(crop);
			list.Add(crop);

			foreach (IntVec3 cell in crop.CellsAdjacent8WayAndInside())
			{
				Plant thingAtCell;

				if (cell.InBounds(crop.Map))
				{
					thingAtCell = cell.GetPlant(crop.Map);

					if (thingAtCell != null &&
						crop.GrowthRateFactor_Light == thingAtCell.GrowthRateFactor_Light &&
						crop.GrowthRateFactor_Fertility == thingAtCell.GrowthRateFactor_Fertility)

						_Iterate(thingAtCell, list);
				}
			}
		}

		//public static float GrowthCorrectionFor(this Plant plant)
		//{
		//	if (!allThingsInAGroup.ContainsKey(plant))
		//		return 0f;

		//	Group group = allThingsInAGroup[plant];

		//	return group.GrowthCorrectionFor(plant);
		//}

		public static float GrowthCorrectionMultiplier(this Plant plant)
		{
			Group group = plant.GroupOf();

			if (group == null)
				return 1f;

			return group.GrowthCorrectionMultiplierFor(plant);
			//return 0f;
		}

		public static int GroupIndex(this Plant plant)
		{
			return allGroups.IndexOf(allThingsInAGroup[plant]);
			//return 0f;
		}

		public static Group GroupOf(this Plant plant)
		{
			try
			{
				return allGroups[plant.GroupIndex()];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public static float TicksUntilFullyGrown(this Plant plant)
		{
			return (int)typeof(RimWorld.Plant).GetProperty("TicksUntilFullyGrown", Harmony.AccessTools.all).GetValue(plant, null);
		}

		public static float GrowthPerTick(this Plant plant)
		{
			return (int)typeof(RimWorld.Plant).GetProperty("GrowthPerTick", Harmony.AccessTools.all).GetValue(plant, null);
		}
	}
}
