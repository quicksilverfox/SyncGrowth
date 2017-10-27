using RimWorld;
using UnityEngine;
using Verse;

namespace WM.SyncGrowth
{
	public static class DebugUtils
	{
		public static void DebugDrawGroups(this Plant plant)
		{
			Group group = plant.GroupOf();

			if (group != null)
			{
				Color groupColor = GroupColor(group.Index);
				foreach (Plant current in group.plants)
				{
					CellRenderer.RenderCell(current.Position, SolidColorMaterials.SimpleSolidColorMaterial(groupColor));
				}
			}
		}

		private static Color GroupColor(int id)
		{
			int n;
			n = colors.Length - (colors.Length % id);

			return colors[n];
		}

		static readonly Color[] colors = { new Color(80, 0, 0), new Color(0, 80, 0), new Color(0, 0, 80) };
	}
}
