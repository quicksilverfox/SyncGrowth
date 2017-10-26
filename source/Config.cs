using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class Config : HugsLib.ModBase
	{
		internal static float controlerRange = 20;
		internal static float LandingDesignatorRange = 20;

		private string modName = "SyncGrowth";

		public override string ModIdentifier
		{
			get
			{
				return modName;
			}
		}

		private static SettingHandle<bool> drawGroups;

		public override void Update()
		{
			try
			{
				if (Find.Selector == null)
					return;

				var t = Find.Selector.SingleSelectedThing;

				if (t == null)
					return;

				if (t is Plant)
				{
					var group = ((RimWorld.Plant)t).GroupOf();

					if (group != null)
					{
						GroupsUtils.TryCreateCropsGroup(t as Plant);
						GenDraw.DrawFieldEdges(group.plants.Select(arg => arg.Position).ToList(), SimpleColor.Red.ToUnityColor());
					}
				}
			}
			catch (Exception ex)
			{
				//screw that bug, I can't fix it and it seems harmless.
				if (!(ex is InvalidCastException))
					throw ex;
			}
		}

		// not working :( . Could avoid detouring
		//public override void DefsLoaded()
		//{
		//	//DefDatabase<ThingDef>.GetNamed("PlantBase").thingClass = typeof(WM.SyncGrowth.Detour.Plant);

		//	List<ThingDef> list = DefDatabase<ThingDef>.AllDefs.ToList();

		//	//List<ThingDef> list = ThingCategoryDef.Named("Plant").
		//	int n = 0;
		//	if(list != null)
		//	foreach (ThingDef current in list)
		//	{
		//		if (current.thingClass == typeof(RimWorld.Plant))
		//		{
		//			//current.thingClass = typeof(Detour.Plant);

		//			//typeof(ThingDef).GetField("thingClass").SetValue(current, typeof(Detour.Plant));

		//			n++;

		//			//Logger.Message("Injected to def : "+current);
		//		}
		//	}

		//	Logger.Message(n + " defs of plants injected");
		//}
	}
}
