using System;
using System.Collections.Generic;
using System.Reflection;

using Verse;
using RimWorld;

using Harmony;


namespace rjw
{
	static class Genital_Helper
	{
		public static HediffDef vagina = HediffDef.Named ("Vagina");
		public static HediffDef penis = HediffDef.Named ("Penis");

		public static HediffDef micropenis = HediffDef.Named ("Micropenis");
		public static HediffDef small_penis = HediffDef.Named ("SmallPenis");
		public static HediffDef big_penis = HediffDef.Named ("BigPenis");
		public static HediffDef huge_penis = HediffDef.Named ("HugePenis");

		public static HediffDef bionic_penis = HediffDef.Named ("BionicPenis");
		public static HediffDef bionic_vagina = HediffDef.Named ("BionicVagina");

		public static HediffDef dummy_privates_initializer = DefDatabase<HediffDef>.GetNamed ("DummyPrivates");


		public static BodyPartRecord get_genitals (Pawn pawn)
		{
			BodyPartRecord genitalPart = pawn.RaceProps.body.AllParts.Find ((BodyPartRecord bpr) => String.Equals (bpr.def.defName, "Genitals"));

			if (genitalPart == null) {
				return null;
				//First.inject_genitals (pawn.RaceProps.body);
				//genitalPart = pawn.RaceProps.body.AllParts.Find ((BodyPartRecord bpr) => String.Equals (bpr.def.defName, "Genitals"));
			}

			return genitalPart;
		}

		public static bool genitals_blocked (Pawn pawn)
		{
			if (pawn.apparel != null)
				foreach (var app in pawn.apparel.WornApparel) {
					var gear_def = app.def as bondage_gear_def;
					if ((gear_def != null) && (gear_def.blocks_genitals))
						return true;
				}
			return false;
		}

		public static bool is_sexualized (Pawn pawn)
		{
			BodyPartRecord genitalPart = get_genitals(pawn);

			return pawn.health.hediffSet.hediffs.Any ((Hediff hed) =>
			                                       (hed.Part == genitalPart) &&
			                                       (((hed as Hediff_Implant) != null) || ((hed as Hediff_AddedPart) != null)) &&
			                                       (hed.def != dummy_privates_initializer));
		}

		public static void sexualize_everyone () {
			foreach (var p in Find.WorldPawns.AllPawnsAliveOrDead)
				if ((! p.Dead) || p.Spawned)
					xxx.sexualize (p);

			foreach (var m in Find.Maps)
				foreach (var p in m.mapPawns.AllPawns)
					xxx.sexualize (p);
		}

		public static void sexualize (Pawn pawn)
		{
			if (xxx.is_animal (pawn)) {
				if (pawn.RaceProps.hasGenders && !is_sexualized (pawn)) {
					sexualize_animal (pawn);
				}
			} else if (!is_sexualized (pawn)) {
				sexualize_human (pawn);
			}
		}

		public static void sexualize_human (Pawn pawn)
		{
			HediffDef privates;

			BodyPartRecord genitalPart = get_genitals (pawn);

			if (genitalPart == null) {
				return;
			}

			if (pawn.gender == Gender.Male) {
				if (Rand.Value < 0.75) {
					privates = penis;
				}
				else if (Rand.Value < 0.75) {
					privates = (Rand.Bool) ? small_penis : big_penis;
				}
				else {
					privates = (Rand.Bool) ? micropenis : huge_penis;
				}
			} else {
				privates = vagina;
			}

			pawn.health.AddHediff (privates, genitalPart);
		}

		public static void sexualize_animal (Pawn pawn) 
		{
			HediffDef privates;
			BodyPartRecord genitalPart = get_genitals (pawn);

			if (genitalPart == null) {
				return;
				//First.inject_genitals (pawn.RaceProps.body);
			}

			if (pawn.RaceProps.IsMechanoid) {
				if (pawn.gender == Gender.Male) {
					privates = bionic_penis;
				} else {
					privates = bionic_vagina;
				}
			}
			else {
				if (pawn.gender == Gender.Male) {
					privates = penis;
				} else {
					privates = vagina;
				}
			}

			pawn.health.AddHediff (privates, genitalPart);
		}

		public static void inject_genitals (BodyDef target)
		{
			BodyPartRecord tor_rec = target.corePart;

			if (tor_rec != null) {

				var gen_rec = new BodyPartRecord ();
				gen_rec.def = DefDatabase<BodyPartDef>.GetNamed ("Genitals");
				gen_rec.height = BodyPartHeight.Bottom;
				gen_rec.depth = BodyPartDepth.Outside;
				gen_rec.coverage = 0.02f;
				gen_rec.groups.Add (BodyPartGroupDefOf.Torso);
				gen_rec.parent = tor_rec;

				// TODO lots of broken/missing stuff here

				//gen_rec.fleshCoverage = 1.0f;
				//gen_rec.absoluteCoverage = gen_rec.parent.absoluteCoverage * gen_rec.coverage;
				//gen_rec.absoluteFleshCoverage = gen_rec.absoluteCoverage * gen_rec.fleshCoverage;

				// coverage is set by XML (or hardcoded like above)
				// absoluteCoverage is derived from coverage and the parent's absoluteCoverage
				// fleshCoverage is derived from the parts' coverages
				// absoluteFleshCoverage is derived from absoluteCoverage and fleshCoverage

				// so inserting the genitals affects the Torso's fleshCoverage which affects its absoluteFleshCoverage
				tor_rec.parts.Add (gen_rec);
				//if (gen_rec.coverage <= tor_rec.fleshCoverage)
					//tor_rec.fleshCoverage -= gen_rec.coverage;
				//else {
					//tor_rec.fleshCoverage = 0.0f;
					//Log.Warning ("[RJW] Torso BPR fleshCoverage pushed below zero during genitals injection");
				//}
				//tor_rec.absoluteFleshCoverage = tor_rec.absoluteCoverage * tor_rec.fleshCoverage;

				target.AllParts.Add (gen_rec);

			} else
				Log.Error ("[RJW] Failed to find the \"Torso\" BodyPartRecord");
		}
	}
}
