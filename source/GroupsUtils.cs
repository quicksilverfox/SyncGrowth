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

		public static List<Group> allGroups = new List<Group>();
		private static List<Plant> allThingsInAGroup = new List<Plant>();

		private static void Reset()
		{
			if (lastTicked >= Find.TickManager.TicksGame)
				return;

			lastTicked = Find.TickManager.TicksGame;

			allThingsInAGroup.Clear();
			allGroups.Clear();

			//foreach (List<Thing> list in thingsAndGroups.Values)
			//{
			//	list.Clear();
			//}
		}

		public static void TryCreateCropsGroup(Plant crop)
		{
			Reset();

			if (crop.def.ingestible.foodType == FoodTypeFlags.Tree)
				return;
			
			if (allThingsInAGroup.Contains(crop))
				return ;

			Group newGroup = new Group();
			allGroups.Add(newGroup);

			try
			{
				Iterate(crop, newGroup);

				//newGroup.TickLong();
			}
			catch (StackOverflowException ex)
			{
				Log.Warning("failed to create group from crop " + crop + " : " + ex.Message + ".");
			}

			newGroup.SplitGroup();

		}

		static void Iterate(Plant crop,Group group)
		{
			allThingsInAGroup.Add(crop);

			foreach (IntVec3 cell in crop.CellsAdjacent8WayAndInside())
			{
				Plant thingAtCell = (WM.SyncGrowth.Plant)cell.GetPlant(crop.Map);

				if(group.TryAdd(thingAtCell))
					
					Iterate(thingAtCell, group);
			}
		}
	}
}
