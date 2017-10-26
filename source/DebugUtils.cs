using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Linq;

namespace WM.SyncGrowth
{
	public static class DebugUtils
	{
		static Thing dummy;

		//internal static void CreateDummyThing()
		//{
		//	ThingDef def = new ThingDef();
		//	def.thingClass = typeof(dummyThingClass);
		//	dummy = ThingMaker.MakeThing(def);
		//}

		public static void DebugDrawGroups(this Plant plant)
		{
			
			Group group = plant.GroupOf();
			if (group != null)
			{
				Color groupColor = GroupColor(group.Index);
				foreach (Plant current in group.plants)
				{
					CellRenderer.RenderCell(current.Position, SolidColorMaterials.SimpleSolidColorMaterial(groupColor) );
				}
			}
		}

		private static Color GroupColor(int id)
		{
			int n;
			n = colors.Length - (colors.Length % id);

			return colors[n];
		}

		static readonly Color[] colors = { new Color(80, 0, 0),new Color(0, 80, 0),new Color(0, 0, 80) };

	}
}
