using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class Group
	{
		// should be constant ?
		private static InvalidOperationException notPostProcessedYet = new InvalidOperationException("Crops group hasn't been post processed yet.");
		//private static InvalidOperationException alreadyPostProcessed = new InvalidOperationException("Crops group has already been post processed.");

		public const float maxGap = 0.08f;

		float maxGrowth = 0;
		float minGrowth = 1;

		float avgGrowth;

		internal List<RimWorld.Plant> plants = new List<RimWorld.Plant>();
		internal List<float> correctionRates = new List<float>();

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

		public float MaxGrowth
		{
			get
			{
				if (!postProcessDone)
					throw notPostProcessedYet;
				return maxGrowth;
			}
		}

		public float MinGrowth
		{
			get
			{
				if (!postProcessDone)
					throw notPostProcessedYet;
				return minGrowth;
			}
		}

		public int Index
		{
			get
			{
				return GroupsUtils.allGroups.IndexOf(this);
			}
		}

		public Group(List<RimWorld.Plant> argPlants) // must be a sorted list
		{
			GroupsUtils.allGroups.Add(this);

			plants = argPlants.ToList();
				
			this.PostProcess();
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

		internal float GrowthCorrectionMultiplierFor(Plant plant)
		{
			if(!postProcessDone)
				
				return 1f;
			
			if (!plants.Contains(plant))
				
				return 1f;

			return 1 + correctionRates[plants.IndexOf(plant)];
		}

		internal void PostProcess()
		{
			plants = plants.OrderBy((Plant arg) => arg.Growth).ToList();

			// should not be needed
			//plants.RemoveDuplicates();

			maxGrowth = plants.Max((Plant arg) => arg.Growth);
			minGrowth = plants.Min((Plant arg) => arg.Growth);

			if (maxGrowth - minGrowth > maxGap)
			{
				List<Plant> newList = plants.Where((Plant obj) => obj.Growth < this.maxGrowth - maxGap).ToList();

				if (newList.Count > 0)
				{
					Group newGroup = new Group(newList);

					// ducktapestan 

					//bottomList.RemoveAll((Plant obj) => obj.Growth < newGroup.maxGrowth - maxGap);
					//plants.RemoveRange(plants.Count - newList.Count,newList.Count);
					plants.RemoveAll((Plant obj) => newList.Contains(obj));
#if DEBUG
					Log.Message("Splitting group #"+newGroup.Index+" of " + def + " for " + newList.Count + " and " + plants.Count + " crops");
#endif

				}

				maxGrowth = plants.Max((Plant arg) => arg.Growth);
				minGrowth = plants.Min((Plant arg) => arg.Growth);

			}

			int duplicates = 0;

			foreach (Plant current in plants)
			{
				if (!GroupsUtils.allThingsInAGroup.ContainsKey(current))
					GroupsUtils.allThingsInAGroup.Add(current, this);
				else
					duplicates++;

				avgGrowth += current.Growth;
			}

			if (duplicates > 0)
			{
				Log.Error("Crops group #" + this.Index + " has " + duplicates + " crops already listed in another group. Please contact the mod author.");
			}

			avgGrowth /= plants.Count;

			for (int i = 0; plants.Count > i; i++)
			{
				correctionRates.Add((avgGrowth - plants[i].Growth) * 8);
			}

			postProcessDone = true;
		}
	}
}
