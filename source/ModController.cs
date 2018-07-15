using System;
using RimWorld;
using Verse;

namespace WM.SyncGrowth
{
	public class ModController : HugsLib.ModBase
	{
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
				var t = Find.Selector.SingleSelectedThing;

				if (t == null)
				{
					return;
				}
				if (t is Plant)
				{
#if DEBUG
					if (KeyBindingDefOf.Misc1.JustPressed)
					{
						GroupMaker.TryCreateGroup(Find.CurrentMap.GetComponent<MapCompGrowthSync>(), t as Plant, true);
					}
#endif
					var group = GroupsUtils.GroupOf(t as Plant);

					if (group != null)
					{
						group.Draw(0);
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
