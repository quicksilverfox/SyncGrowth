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

		protected List<Plant> plants = new List<Plant>();

		protected List<Plant> sortedPlants = new List<Plant>();

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
				return plants[0].def;
			}
		}

		public Group(List<Plant> argSortedPlants=null)
		{
			if (argSortedPlants != null)
				sortedPlants = argSortedPlants;

			GroupsUtils.allGroups.Add(this);

			maxGrowth = sortedPlants.First().Growth;
			minGrowth = sortedPlants.Last().Growth;
		}

		public bool TryAdd(Plant plant)
		{
			if (Count > 0 && def != plant.def)
				return false;

			//if ( Math.Abs(minGrowth - plant.Growth) > maxGap || Math.Abs(maxGrowth - plant.Growth) > maxGap  )
			//	return false;

			if (plants.Contains(plant))
				return false;

			plants.Add(plant);

			maxGrowth = Math.Max(maxGrowth, plant.Growth);
			minGrowth = Math.Min(minGrowth, plant.Growth);

			return true;
		}

		private float GrowthCorrectionFor(int plant)
		{
			return 0f;
		}
		internal void SplitGroup()
		{
			sortedPlants = plants.OrderBy((arg) =>arg.Growth).ToList();

			if (maxGrowth - minGrowth > maxGap)
			{
				List<Plant> bottomList = sortedPlants;

				while (bottomList.Count > 0)
				{
					bottomList = sortedPlants.Where((Plant obj) => obj.Growth < maxGrowth - maxGap).ToList();
					sortedPlants.RemoveRange(sortedPlants.Count - bottomList.Count, bottomList.Count);

					GroupsUtils.allGroups.Add(new Group(bottomList));
				}

			}
		}

		public void TickLong()
		{
			if (lastTicked >= Find.TickManager.TicksGame)
				return;

			lastTicked = Find.TickManager.TicksGame;
#if DEBUG
			Log.Message("ticking group of " + this.Count + " " + this.def + ".");
#endif
			foreach (Plant crop in plants)
			{
			}
		}
	}
	
}
