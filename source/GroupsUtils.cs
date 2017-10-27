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
		public static Dictionary<Plant, Group> AllPlantsInGroups = new Dictionary<Plant, Group>();

		private static void Reset()
		{
			if (lastTicked + 2000 > (Find.TickManager.TicksGame))
				return;
#if DEBUG
			Log.Message("RESET ! had " + allGroups.Count + " groups of crops.");
#endif
			lastTicked = Find.TickManager.TicksGame;
			allListedPlants.Clear();
			AllPlantsInGroups.Clear();
			allGroups.Clear();
		}

		static List<ThingDef> blacklist = new List<ThingDef> 
		{
			ThingDefOf.PlantGrass, 
			ThingDef.Named("PlantRaspberry") 
		};

		public static void TryCreateCropsGroup(Plant crop)
		{
			Reset();

			var zone = crop.Map.zoneManager.ZoneAt(crop.Position);
			var ingestible = crop.def.ingestible;
			var plant = crop.def.plant;

			if ((ingestible != null && ingestible.foodType == FoodTypeFlags.Tree) || plant == null || plant.reproduces || blacklist.Contains(crop.def) ||
			    zone != null && !(zone is Zone_Growing))
				return;

			//if (crop.Position.GetZone(crop.Map) == null && (crop.Position.GetFirstBuilding(crop.Map) == null || crop.Position.GetFirstBuilding(crop.Map).def != ThingDef.Named("HydroponicsBasin") ) )
			//	return;

			if (AllPlantsInGroups.ContainsKey(crop))
				return;

			var list = new List<Plant>();

			try
			{
				_Iterate(crop, list);
			}
			catch (Exception ex)
			{
				Log.Warning("failed to create group from crop " + crop + " at " + crop.Position + " : " + ex.Message + ". " + ex.StackTrace);
				return;
			}

			Group newGroup;
			if (list.Any())
				newGroup = new Group(list);
		}

		static void _Iterate(Plant crop, List<Plant> list)
		{
			if (list.Any() && list.First().def != crop.def)
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

				if (!cell.InBounds(crop.Map))
					continue;
				
				thingAtCell = cell.GetPlant(crop.Map);

				if (thingAtCell != null &&
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
					crop.GrowthRateFactor_Light == thingAtCell.GrowthRateFactor_Light &&
					crop.GrowthRateFactor_Fertility == thingAtCell.GrowthRateFactor_Fertility)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
				{
					_Iterate(thingAtCell, list);
				}
			}
		}

		public static float GrowthCorrectionMultiplier(Plant plant)
		{
			Group group = plant.GroupOf();

			if (group == null)
				return 1f;

			return group.GrowthCorrectionMultiplierFor(plant);
		}

		public static Group GroupOf(this Plant plant)
		{
			var result = AllPlantsInGroups.FirstOrDefault((KeyValuePair<Plant, Group> arg) => arg.Value.Plants.Contains(plant));

			return (result.Value);
		}

		public static float TicksUntilFullyGrown(this Plant plant)
		{
			return (int)typeof(Plant).GetProperty("TicksUntilFullyGrown", Harmony.AccessTools.all).GetValue(plant, null);
		}

		public static float GrowthPerTick(this Plant plant)
		{
			return (int)typeof(Plant).GetProperty("GrowthPerTick", Harmony.AccessTools.all).GetValue(plant, null);
		}
	}
}
