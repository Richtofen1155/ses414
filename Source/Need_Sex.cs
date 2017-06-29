
using System;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw {
	public class Need_Sex : Need {
		
		private static readonly SimpleCurve sex_need_factor_from_age = new SimpleCurve
		{
			new CurvePoint(0f,  0.00f),
			new CurvePoint(5f,  0.25f),
			new CurvePoint(16f, 1.00f),
			new CurvePoint(22f, 1.00f),
			new CurvePoint(30f, 0.90f),
			new CurvePoint(40f, 0.75f),
			new CurvePoint(60f, 0.50f),
			new CurvePoint(80f, 0.25f)
		};

		private static readonly SimpleCurve animal_sex_need_factor_from_age = new SimpleCurve
		{
			new CurvePoint(0f,  0.50f),
			new CurvePoint(1f,  1.00f),
			new CurvePoint(5f,  2.00f),
			new CurvePoint(10f, 1.00f),
			new CurvePoint(20f, 1.00f),
			new CurvePoint(30f, 0.90f),
			new CurvePoint(40f, 0.75f),
			new CurvePoint(60f, 0.50f),
		};

		public float thresh_frustrated () { return 0.10f; }

		public float thresh_horny () { return (pawn.gender == Gender.Male) ? 0.50f : 0.25f; }

		public float thresh_satisfied () { return 0.75f; }

		public float thresh_ahegao () { return 0.95f; }		
		
		public Need_Sex (Pawn pawn) : base(pawn)
		{
			this.threshPercents = new List<float> ();

			this.threshPercents.Add (thresh_frustrated ());
			this.threshPercents.Add (thresh_horny ());
			this.threshPercents.Add (thresh_satisfied ());
			this.threshPercents.Add (thresh_ahegao ());			
		}
		
		public static float balance_factor (float lev)
		{
			const float one_on_point_three = 1.0f / 0.30f;
			if (lev >= 0.70f)
				return 1.0f + one_on_point_three * (lev - 0.70f) * one_on_point_three;
			else if (lev >= 0.30f)
				return 1.0f;
			else
				return 1.0f - 0.5f * one_on_point_three * (0.30f - lev);
		}
		
		public override void NeedInterval ()
		{
			float age = pawn.ageTracker.AgeBiologicalYearsFloat;
			float age_factor = 0f;

			if (xxx.is_animal (pawn)) {
				age_factor = animal_sex_need_factor_from_age.Evaluate (age);
			} else {
				age_factor = sex_need_factor_from_age.Evaluate (age);
			}

			var fall_per_tick = (xxx.is_nympho (pawn) ? 3.0f : 1.0f) * age_factor * balance_factor (CurLevel) * def.fallPerDay / 60000.0f;
			CurLevel -= fall_per_tick * 150.0f; // 150 ticks between each call

			// I just put this here so that it gets called on every pawn on a regular basis. There's probably a
			// better way to do this sort of thing, but whatever. This works.
			if (!xxx.is_animal (pawn)) {
				std.update (pawn);
			}
			
			// Similarly, there must be a better place to call this.
			if (pawn.Map != null) {
				xxx.bootstrap (pawn, pawn.Map);
			}
		}
	}
}
