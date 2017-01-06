using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
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
