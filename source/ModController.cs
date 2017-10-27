using System;
using System.Linq;
using HugsLib.Settings;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class ModController : HugsLib.ModBase
	{
		private static SettingHandle<bool> drawGroups;
		private string modName = "WMSyncGrowth";

		public override string ModIdentifier
		{
			get
			{
				return modName;
			}
		}

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
					var group = ((Plant)t).GroupOf();

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
	}
}
