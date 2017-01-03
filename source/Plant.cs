using System;
using HugsLib.Source.Detour;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.SyncGrowth.Detour
{
	public class Plant : RimWorld.Plant
	{
		private string cachedLabelMouseover
		{
			get
			{
				return (string)typeof(RimWorld.Plant).GetField("cachedLabelMouseover", Helpers.AllBindingFlags).GetValue(this);
			}
			set
			{
				typeof(RimWorld.Plant).GetField("cachedLabelMouseover", Helpers.AllBindingFlags).SetValue(this, value);
			}
		}

		//public new int TicksUntilFullyGrown
		//{
		//	get
		//	{
		//		return (int)typeof(RimWorld.Plant).GetProperty("TicksUntilFullyGrown", Helpers.AllBindingFlags).GetValue(this,null);
		//	}
		//}

		[DetourProperty(typeof(RimWorld.Plant), "GrowthRate", DetourProperty.Getter)]
		public virtual float _GrowthRate
		{
			get
			{
				return (this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light) * this.GrowthRateCorrection();
			}
		}

		// RimWorld.Plant
		[DetourMethod(typeof(RimWorld.Plant), "TickLong")]
		public override void TickLong()
		{
			//float growthBefore = this.Growth;

			GroupsUtils.TryCreateCropsGroup(this);

			//this.Growth += GroupsUtils.GrowthCorrectionFor(this) * this.GrowthRate;

			TickLong_base();
		}

		// RimWorld.Plant
		public void TickLong_base()
		{
			this.CheckTemperatureMakeLeafless();
			if (base.Destroyed)
			{
				return;
			}
			if (GenPlant.GrowthSeasonNow(base.Position, base.Map))
			{
				if (!this.HasEnoughLightToGrow)
				{
					this.unlitTicks += 2000;
				}
				else
				{
					this.unlitTicks = 0;
				}
				float num = this.growthInt;
				bool flag = this.LifeStage == PlantLifeStage.Mature;
				this.growthInt += this.GrowthPerTick * 2000f;
				if (this.growthInt > 1f)
				{
					this.growthInt = 1f;
				}
				if (((!flag && this.LifeStage == PlantLifeStage.Mature) || (int)(num * 10f) != (int)(this.growthInt * 10f)) && this.CurrentlyCultivated())
				{
					base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
				}
				if (this.def.plant.LimitedLifespan)
				{
					this.ageInt += 2000;
					if (this.Dying)
					{
						Map map = base.Map;
						bool isCrop = this.IsCrop;
						int amount = Mathf.CeilToInt(10f);
						base.TakeDamage(new DamageInfo(DamageDefOf.Rotting, amount, -1f, null, null, null));
						if (base.Destroyed)
						{
							if (isCrop && this.def.plant.Harvestable && MessagesRepeatAvoider.MessageShowAllowed("MessagePlantDiedOfRot-" + this.def.defName, 240f))
							{
								Messages.Message("MessagePlantDiedOfRot".Translate(new object[]
								{
							this.Label
								}).CapitalizeFirst(), new TargetInfo(base.Position, map, false), MessageSound.Negative);
							}
							return;
						}
					}
				}
				if (this.def.plant.reproduces && this.growthInt >= 0.6f && Rand.MTBEventOccurs(this.def.plant.reproduceMtbDays, 60000f, 2000f))
				{
					if (!GenPlant.SnowAllowsPlanting(base.Position, base.Map))
					{
						return;
					}
					GenPlantReproduction.TryReproduceFrom(base.Position, this.def, SeedTargFindMode.Reproduce, base.Map);
				}
			}
			this.cachedLabelMouseover = null;
		}

	}
}
