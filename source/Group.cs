using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class Group
	{
		public const float maxGap = 0.08f;

		private int lastTicked = 0;

		float maxGrowth;
		float minGrowth;

		float avgGrowth;

		protected List<RimWorld.Plant> plants = new List<RimWorld.Plant>();
		protected List<float> correctionRates = new List<float>();

		//protected List<RimWorld.Plant> sortedPlants = new List<RimWorld.Plant>();

		bool postProcessDone = false;

		public int Count
		{
			get
			{
				return plants.Count;
			}
		}

		public ThingDef def
		{
			get
			{
				if (plants.Count == 0)
					return null;
				return plants[0].def;
			}
		}

		public Group(List<RimWorld.Plant> argSortedPlants = null)
		{
			GroupsUtils.allGroups.Add(this);


			if (argSortedPlants == null)
			{
				//Log.Message("created empty group of crops.");
				return;
			}

			plants = argSortedPlants;

			maxGrowth = plants.First().Growth;
			minGrowth = plants.Last().Growth;

			//Log.Message("created group of " + def+ " with " + argSortedPlants.Count + " crops.");
		}

		public bool TryAdd(Plant plant)
		{
			if (postProcessDone)

				throw new InvalidOperationException("Tried to add a crop to a group that is locked because it has already been post processed");

			if (Count > 0 && def != plant.def)
				return false;

			//if ( Math.Abs(minGrowth - plant.Growth) > maxGap || Math.Abs(maxGrowth - plant.Growth) > maxGap  )
			//	return false;

			if (plants.Contains(plant) || GroupsUtils.allThingsInAGroup.ContainsKey(plant))
				return false;

			plants.Add(plant);
			GroupsUtils.allThingsInAGroup.Add(plant, this);

			maxGrowth = Math.Max(maxGrowth, plant.Growth);
			minGrowth = Math.Min(minGrowth, plant.Growth);

			return true;
		}

		internal float GrowthCorrectionFor(Plant plant)
		{
			float delta;
			int longTicksNeeded = (int)((plant.TicksUntilFullyGrown() / 2000f) - 10);

			if (plant.GrowthRate > 0 && longTicksNeeded > 0)
			{
				delta = (plant.Growth - avgGrowth) / longTicksNeeded;
#if DEBUG
				Log.Message("Calculating growth correction for " + plant.def + " (value = " + delta + ") at " + plant.Position + " in group #" + GroupsUtils.allGroups.IndexOf(this));
#endif

#if DEBUG
				Log.Message("longTicksNeeded : " + longTicksNeeded + " (" + (Math.Round(plant.TicksUntilFullyGrown() / 2000f) - 1) + ")");
#endif
			}
			else
				delta = 0;

			//plant.GrowthPerTick()

			return delta;
		}

		internal float GrowthRateCorrectionFor(Plant plant)
		{
			//			float delta;
			//			int longTicksNeeded = (int)((plant.TicksUntilFullyGrown() / 2000f) - 10);

			//			if (plant.GrowthRate > 0 && longTicksNeeded > 0)
			//			{
			//				delta = (plant.Growth - avgGrowth) / longTicksNeeded;
			//#if DEBUG
			//				Log.Message("Calculating growth correction for " + plant.def + " (value = " + delta + ") at " + plant.Position + " in group #" + GroupsUtils.allGroups.IndexOf(this));
			//#endif

			//#if DEBUG
			//				Log.Message("longTicksNeeded : " + longTicksNeeded + " (" + (Math.Round(plant.TicksUntilFullyGrown() / 2000f) - 1) + ")");
			//#endif
			//			}
			//			else
			//				delta = 0;

			//			//plant.GrowthPerTick()

			//			return delta * 2000;

			if(!postProcessDone)
				return 1f;
			
			if (!plants.Contains(plant))
				return 1f;

			return 1 + correctionRates[plants.IndexOf(plant)];
		}

		internal void PostProcess()
		{
			if (postProcessDone)

				throw new InvalidOperationException("trying to post process a group that was already post processed");

			plants = plants.OrderBy((arg) => arg.Growth).ToList();

			if (maxGrowth - minGrowth > maxGap)
			{
				List<Plant> bottomList = plants;
				List<Plant> newList;
				Group newGroup = this;

				while (bottomList.Count > 0)
				{
					newList = bottomList.Where((Plant obj) => obj.Growth < newGroup.maxGrowth - maxGap).ToList();

					if (newList.Count > 0)
					{
						newGroup = new Group(newList);

						//bottomList.RemoveAll((Plant obj) => obj.Growth < newGroup.maxGrowth - maxGap);
						bottomList.RemoveRange(bottomList.Count - newList.Count,newList.Count);
#if DEBUG
						//Log.Message("Splitting group of " + def + " for " + newList.Count + " and " + bottomList.Count + " crops");
#endif
					}

					bottomList = newList;
				}

			}

			foreach (Plant current in plants)
			{
				avgGrowth += current.Growth;
			}

			avgGrowth /= plants.Count;

			for (int i = 0; plants.Count > i; i++)
			{
				correctionRates.Add((avgGrowth - plants[i].Growth) * 8);
			}

			postProcessDone = true;
		}

//		public void TickLong()
//		{
//			if (lastTicked >= Find.TickManager.TicksGame)
//				return;

//			lastTicked = Find.TickManager.TicksGame;
//#if DEBUG
//			Log.Message("ticking group of " + this.Count + " " + this.def + ".");
//#endif
//			foreach (Plant crop in plants)
//			{
//			}
//		}
	}

}
