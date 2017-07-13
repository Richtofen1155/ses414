
using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Verse;
using RimWorld;

namespace rjw {
	public static class nymph_generator {
		
		private static bool is_trait_conflicting_or_duplicate (Pawn p, Trait t)
		{
			foreach (var existing in p.story.traits.allTraits)
				if ((existing.def == t.def) || (t.def.ConflictsWith (existing)))
					return true;
			return false;
		}
		
		// Replaces a pawn's backstory and traits to turn it into a nymph
		public static void set_story (Pawn p)
		{
			var gen_sto = nymph_backstories.generate ();
			
			p.story.childhood = gen_sto.child;
			p.story.adulthood = gen_sto.adult;
			
			Trait third_trait = (! p.story.traits.allTraits.NullOrEmpty ()) ? p.story.traits.allTraits.RandomElement () : null;
			
			p.story.traits.allTraits.Clear ();
			var trait_count = 0;
			foreach (var t in gen_sto.traits) {
				p.story.traits.GainTrait (t);
				++trait_count;
			}

			if ((trait_count < 3) && (! is_trait_conflicting_or_duplicate (p, third_trait)))
				p.story.traits.GainTrait (third_trait);
		}
		
		private static int sum_previous_gains (SkillDef def, Pawn_StoryTracker sto, Pawn_AgeTracker age)
		{
			int total_gain = 0;
			int gain;
			
			// Gains from backstories
			if (sto.childhood.skillGainsResolved.TryGetValue (def, out gain))
				total_gain += gain;
			if (sto.adulthood.skillGainsResolved.TryGetValue (def, out gain))
				total_gain += gain;
			
			// Gains from traits
			foreach (var trait in sto.traits.allTraits)
				if (trait.CurrentData.skillGains.TryGetValue (def, out gain))
					total_gain += gain;
			
			// Gains from age
			var rgain = Rand.Value * (float)total_gain * 0.35f;
			var age_factor = Mathf.Clamp01 ((age.AgeBiologicalYearsFloat - 17.0f) / 10.0f); // Assume nymphs are 17~27
			total_gain += (int)(age_factor * rgain);
			
			return Mathf.Clamp (total_gain, 0, 20);
		}
		
		// Set a nymph's initial skills & passions from backstory, traits, and age
		public static void set_skills (Pawn p)
		{
			foreach (var skill_def in DefDatabase<SkillDef>.AllDefsListForReading) {
				var rec = p.skills.GetSkill (skill_def);
				if (! rec.TotallyDisabled) {
					rec.Level = sum_previous_gains (skill_def, p.story, p.ageTracker);
					rec.xpSinceLastLevel = rec.XpRequiredForLevelUp * Rand.Range (0.10f, 0.90f);
					
					var pas_cha = nymph_backstories.get_passion_chances (p.story.childhood, p.story.adulthood, skill_def);
					var rv = Rand.Value;
					if      (rv < pas_cha.major) rec.passion = Passion.Major;
					else if (rv < pas_cha.minor) rec.passion = Passion.Minor;
					else 						 rec.passion = Passion.None;

				} else
					rec.passion = Passion.None;
			}
		}
		
		public static Pawn spawn_new (IntVec3 around_loc, Map m)
		{
			PawnKindDef pkd; {
				pkd = PawnKindDef.Named ("Nymph");
				pkd.minGenerationAge = 20;
				pkd.maxGenerationAge = 27;
			}
			
			var req = new PawnGenerationRequest (pkd,
			                                     Faction.OfPlayer,
			                                     PawnGenerationContext.NonPlayer,
			                                     //null, // Map
			                                     -1,
			                                     false, // Force generate new pawn
			                                     false, // Newborn
			                                     false, // Allow dead
			                                     false, // Allow downed
			                                     false, // Can generate pawn relations
			                                     false, // Must be capable of violence
			                                     0.0f, // Colonist relation chance factor
			                                     false, // Force add free warm layer if needed
			                                     true, // Allow gay
			                                     true, // Allow food
			                                     true, // Inhabitant
												 false, // Been in Cryosleep
			                                     c => (c.story.bodyType == BodyType.Female) || (c.story.bodyType == BodyType.Thin), // Validator
			                                     null, // Fixed biological age
			                                     null, // Fixed chronological age
			                                     Gender.Female, // Fixed gender
			                                     null, // Fixed melanin
			                                     null); // Fixed last name
			
			IntVec3 spawn_loc = CellFinder.RandomSpawnCellForPawnNear (around_loc, m);
			var p = PawnGenerator.GeneratePawn (req);
			set_story (p);
			set_skills (p);
			GenSpawn.Spawn (p, spawn_loc, m);
			return p;
		}
		
	}
}
