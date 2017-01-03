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

		protected List<RimWorld.Plant> sortedPlants = new List<RimWorld.Plant>();

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

		public Group(List<RimWorld.Plant> argSortedPlants=null)
		{
			GroupsUtils.allGroups.Add(this);


			if (argSortedPlants == null)
			{
				//Log.Message("created empty group of crops.");
				return;
			}

			sortedPlants = argSortedPlants;

			maxGrowth = sortedPlants.First().Growth;
			minGrowth = sortedPlants.Last().Growth;

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

			if (plants.Contains(plant) || GroupsUtils.allThingsInAGroup.Contains(plant))
				return false;

			plants.Add(plant);
			GroupsUtils.allThingsInAGroup.Add(plant);

			maxGrowth = Math.Max(maxGrowth, plant.Growth);
			minGrowth = Math.Min(minGrowth, plant.Growth);

			return true;
		}

		private float GrowthCorrectionFor(Plant plant)
		{
			#if DEBUG
			Log.Message("Calculating growth correction for " + plant.def + " at " + plant.Position + " in group #" + GroupsUtils.allGroups.IndexOf(this));
#endif
			return 0f;
		}
		internal void SplitGroup()
		{
			if (postProcessDone)
				throw new InvalidOperationException("trying to post process a group that was already post processed");

			postProcessDone = true;
			
			plants = plants.OrderBy((arg) =>arg.Growth).ToList();

			if (maxGrowth - minGrowth > maxGap)
			{
				List<Plant> bottomList = plants.ListFullCopy();
				Group newGroup = this;

				while (bottomList.Count > 0)
				{
					bottomList = bottomList.Where((Plant obj) => obj.Growth < newGroup.maxGrowth - maxGap).ToList();

					if (bottomList.Count > 0)
					{
						newGroup = new Group(bottomList);
						Log.Message("Splitting group of " + def);
					}

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
