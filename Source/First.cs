
using System;
using System.Collections.Generic;
using System.Reflection;

using Verse;
using RimWorld;

using Harmony;

namespace rjw {

	[StaticConstructorOnStartup]
	static class First {

		// Generate a HediffGiver for the dummy hediff, then inject it into the OrganicStandard HediffGiverSet
		static void inject_sexualizer ()
		{
			var hgs = DefDatabase<HediffGiverSetDef>.GetNamed ("OrganicStandard");
			if (hgs != null) {
				var giv = new HediffGiver_Birthday ();
				giv.hediff = HediffDef.Named ("DummyPrivates");
				giv.partsToAffect = new List<BodyPartDef> ();
				giv.partsToAffect.Add (DefDatabase<BodyPartDef>.GetNamed ("Genitals"));
				giv.canAffectAnyLivePart = false;
				giv.ageFractionChanceCurve = new SimpleCurve ();
				giv.ageFractionChanceCurve.Add (0.00f, 1.0f);
				giv.ageFractionChanceCurve.Add (0.05f, 1.0f);
				giv.ageFractionChanceCurve.Add (0.06f, 0.0f); // Stop triggering after 5% age so as not to spam the user with messages
				giv.ageFractionChanceCurve.Add (1.00f, 0.0f); // about colonists getting dummy parts on their birthdays.
				giv.averageSeverityPerDayBeforeGeneration = 0.0f;
				hgs.hediffGivers.Add (giv);
			}
		}

		static void show_bpr (String body_part_record_def_name)
		{
			var bpr = BodyDefOf.Human.AllParts.Find ((BodyPartRecord can) => String.Equals (can.def.defName, body_part_record_def_name));
			Log.Message (body_part_record_def_name + " BPR internals:");
			Log.Message ("  def: " + bpr.def.ToString ());
			Log.Message ("  parts: " + bpr.parts.ToString ());
			Log.Message ("  parts.count: " + bpr.parts.Count.ToString ());
			Log.Message ("  height: " + bpr.height.ToString ());
			Log.Message ("  depth: " + bpr.depth.ToString ());
			Log.Message ("  coverage: " + bpr.coverage.ToString ());
			Log.Message ("  groups: " + bpr.groups.ToString ());
			Log.Message ("  groups.count: " + bpr.groups.Count.ToString ());
			Log.Message ("  parent: " + bpr.parent.ToString ());
			//Log.Message ("  fleshCoverage: " + bpr.fleshCoverage.ToString ());
			//Log.Message ("  absoluteCoverage: " + bpr.absoluteCoverage.ToString ());
			//Log.Message ("  absoluteFleshCoverage: " + bpr.absoluteFleshCoverage.ToString ());
		}

		// Generate a BodyPartRecord for the genitals part and inject it into the Human BodyDef. By adding the
		// genitals at the end of the list of body parts we can hopefully avoid breaking existing saves with
		// mods that also modify the human BodyDef.
		public static void inject_genitals (BodyDef target = null)
		{
			if (target == null) {
				target = BodyDefOf.Human;
			}

			Genital_Helper.inject_genitals (target);
		}

		static void inject_recipes ()
		{
			var cra_spo = DefDatabase<ThingDef>.GetNamed ("CraftingSpot");
			var mac_ben = DefDatabase<ThingDef>.GetNamed ("TableMachining");
			var tai_ben = DefDatabase<ThingDef>.GetNamed ("ElectricTailoringBench");

			// Inject the recipes to create the artificial privates into the crafting spot or machining bench.
			// BUT, also dynamically detect if EPOE is loaded and, if it is, inject the recipes into EPOE's
			// crafting benches instead.
			var bas_ben = DefDatabase<ThingDef>.GetNamed ("TableBasicProsthetic", false);
			(bas_ben ?? cra_spo).AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakePegDick"));

			var sim_ben = DefDatabase<ThingDef>.GetNamed ("TableSimpleProsthetic", false);
			(sim_ben ?? mac_ben).AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeHydraulicVagina"));

			var bio_ben = DefDatabase<ThingDef>.GetNamed ("TableBionics", false);
			(bio_ben ?? mac_ben).AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeBionicVagina"));
			(bio_ben ?? mac_ben).AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeBionicPenis"));


			// Inject the bondage gear recipes into their appropriate benches
			if (xxx.config.bondage_gear_enabled) {
				mac_ben.AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeHololock"));
				tai_ben.AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeArmbinder"));
				tai_ben.AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeGag"));
				tai_ben.AllRecipes.Add (DefDatabase<RecipeDef>.GetNamed ("MakeChastityBelt"));
			}
		}

		static void show_bs (Backstory bs)
		{
			Log.Message ("Backstory \"" + bs.Title + "\" internals:");
			Log.Message ("  identifier: " + bs.identifier);
			Log.Message ("  slot: " + bs.slot.ToString ());
			Log.Message ("  Title: " + bs.Title);
			Log.Message ("  TitleShort: " + bs.TitleShort);
			Log.Message ("  baseDesc: " + bs.baseDesc);
			Log.Message ("  skillGains: " + ((bs.skillGains == null) ? "null" : bs.skillGains.ToString ()));
			Log.Message ("  skillGainsResolved: " + ((bs.skillGainsResolved == null) ? "null" : bs.skillGainsResolved.ToString ()));
			Log.Message ("  workDisables: " + bs.workDisables.ToString ());
			Log.Message ("  requiredWorkTags: " + bs.requiredWorkTags.ToString ());
			Log.Message ("  spawnCategories: " + bs.spawnCategories.ToString ());
			Log.Message ("  bodyTypeGlobal: " + bs.bodyTypeGlobal.ToString ());
			Log.Message ("  bodyTypeFemale: " + bs.bodyTypeFemale.ToString ());
			Log.Message ("  bodyTypeMale: " + bs.bodyTypeMale.ToString ());
			Log.Message ("  forcedTraits: " + ((bs.forcedTraits == null) ? "null" : bs.forcedTraits.ToString ()));
			Log.Message ("  disallowedTraits: " + ((bs.disallowedTraits == null) ? "null" : bs.disallowedTraits.ToString ()));
			Log.Message ("  shuffleable: " + bs.shuffleable.ToString ());
		}

		static First ()
		{
			inject_sexualizer ();
			//inject_genitals ();
			inject_recipes ();

			xxx.init (); // Must only be called after injections are complete
			nymph_backstories.init ();
			std.init ();
			bondage_gear_tradeability.init ();

			var har = HarmonyInstance.Create ("rjw");
			har.PatchAll (Assembly.GetExecutingAssembly ());
			PATCH_Pawn_ApparelTracker_TryDrop.apply (har);
		}
	}
}
