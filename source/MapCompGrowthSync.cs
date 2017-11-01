using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class MapCompGrowthSync : MapComponent
	{
		List<Group> groups = new List<Group>();

		public int Count
		{
			get
			{
				return (groups.Count);
			}
		}

		public MapCompGrowthSync(Map map) : base(map)
		{
		}

		public float GetGrowthMultiplierFor(Plant plant)
		{
			var group = GroupOf(plant);

			if (group == null)
				return (1f);

			return group.GetGrowthMultiplierFor(plant);
		}

		public Group GroupOf(Plant plant)
		{
			var result = (this.groups.FirstOrDefault((obj) => obj.Plants.Contains(plant)));
			return (result);
		}

		public override void MapComponentTick()
		{
			if (Find.TickManager.TicksGame % 2000 != 0)
				return;

			groups.Clear();

			var plants = this.map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);

			foreach (Plant item in plants)
			{
#if DEBUG
				var timer = Stopwatch.StartNew();
#endif
				var group = GroupMaker.TryCreateGroup(item);

				if (group != null)
				{
					groups.Add(group);
#if DEBUG
					timer.Stop();
					Log.Message("Created group of " + group.Count + " " + group.PlantDef + " (" + timer.Elapsed.TotalMilliseconds.ToString("0.000 ms") + ")");
#endif
				}
			}
		}
	}
}
