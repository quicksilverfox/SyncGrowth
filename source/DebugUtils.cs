using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

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

		public static void DebugDrawGroup(this Plant plant)
		{
			Group group = plant.Group();
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
			if (colors == null)
				
				MakeColorsList(50);
			
			return colors[id];
		}

		static List<Color> colors;

		private static void MakeColorsList(int count)
		{
			colors = new List<Color>();

			for (int i = 0; i < count; i++)
			{
				colors.Add(new Color(Rand.Value, Rand.Value , Rand.Value, 0.3f ));
			}
		}
	}
}
