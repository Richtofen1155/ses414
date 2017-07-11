
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using UnityEngine;

using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

using RimWorldChildren;

namespace rjw
{
    public static class xxx {
        public static BindingFlags ins_public_or_no = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static config config = DefDatabase<config>.GetNamed("the_one");

        public const float base_sat_per_fuck = 0.40f;

        public const float base_attraction = 0.60f;

        public const float no_parter_ability = 0.65f;

        public static TraitDef nymphomaniac = TraitDef.Named("Nymphomaniac");
        public static TraitDef rapist = TraitDef.Named("Rapist");
        public static TraitDef necrophiliac = TraitDef.Named("Necrophiliac");

        public static HediffDef immunodeficiency = DefDatabase<HediffDef>.GetNamed("Immunodeficiency");

        // Will be set in init. Can't be set earlier because the genitals part has to be injected first.
        public static BodyPartRecord genitals = null;
        public static BodyPartRecord breasts = null;
        public static BodyPartRecord anus = null;

        public static ThoughtDef saw_rash_1 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates1");
        public static ThoughtDef saw_rash_2 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates2");
        public static ThoughtDef saw_rash_3 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates3");

        public static ThoughtDef got_raped = DefDatabase<ThoughtDef>.GetNamed("GotRaped");
        public static ThoughtDef got_raped_by_animal = DefDatabase<ThoughtDef>.GetNamed("GotRapedByAnimal");
        public static ThoughtDef masochist_got_raped = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRaped");
        public static ThoughtDef masochist_got_raped_by_animal = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRapedByAnimal");
        public static ThoughtDef hate_my_rapist = DefDatabase<ThoughtDef>.GetNamed("HateMyRapist");
        public static ThoughtDef kinda_like_my_rapist = DefDatabase<ThoughtDef>.GetNamed("KindaLikeMyRapist");
        public static ThoughtDef allowed_me_to_get_raped = DefDatabase<ThoughtDef>.GetNamed("AllowedMeToGetRaped");
        public static ThoughtDef stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("StoleSomeLovin");
        public static ThoughtDef bloodlust_stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("BloodlustStoleSomeLovin");

        public static JobDef gettin_loved = DefDatabase<JobDef>.GetNamed("GettinLoved");
        public static JobDef nymph_rapin = DefDatabase<JobDef>.GetNamed("NymphJoinInBed");
        public static JobDef gettin_raped = DefDatabase<JobDef>.GetNamed("GettinRaped");
        public static JobDef comfort_prisoner_rapin = DefDatabase<JobDef>.GetNamed("ComfortPrisonerRapin");

        public static ThingDef mote_noheart = ThingDef.Named("Mote_NoHeart");

        public static StatDef sex_stat = StatDef.Named("SexAbility");
        public static StatDef vulnerability_stat = StatDef.Named("Vulnerability");

        public static ThingDef cum = ThingDef.Named("FilthCum");

        private static readonly SimpleCurve attractiveness_from_age_male = new SimpleCurve
        {
            new CurvePoint(00f, 0.00f),
            new CurvePoint(5f,  0.6f),
            new CurvePoint(15f, 0.8f),
            new CurvePoint(22f, 1.0f),
            new CurvePoint(30f, 0.9f),
            new CurvePoint(45f, 0.8f),
            new CurvePoint(60f, 0.7f),
            new CurvePoint(80f, 0.6f)
        };

        private static readonly SimpleCurve attractiveness_from_age_female = new SimpleCurve
        {
            new CurvePoint(00f, 0.00f),
            new CurvePoint(5f,  0.8f),
            new CurvePoint(14f, 1.0f),
            new CurvePoint(20f, 0.9f),
            new CurvePoint(30f, 0.8f),
            new CurvePoint(45f, 0.7f),
            new CurvePoint(60f, 0.5f),
            new CurvePoint(80f, 0.3f)
        };

        public static void init() {
            genitals = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Genitals"));
            breasts = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Chest"));
            anus = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Anus"));
        }

        public static void bootstrap(Pawn p, Map m) {
            if (m.GetComponent<MapCom_Injector>() == null)
                m.components.Add(new MapCom_Injector(m));
        }

        public static bool is_psychopath(Pawn pawn) {
            if (pawn.story == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDefOf.Psychopath);
        }

        public static bool is_bloodlust(Pawn pawn) {
            if (pawn.story == null || pawn.story.traits == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDefOf.Bloodlust);
        }

        public static bool is_rapist(Pawn pawn) {
            //if (pawn.story == null || pawn.story.traits == null) {
            //    return false;
            //}
            return pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDef.Named("Rapist"));
        }

        public static bool is_necrophiliac(Pawn pawn) {
            return (pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(TraitDef.Named("Necrophiliac")));
        }

        public static bool is_brawler(Pawn pawn) {
            if (pawn.story == null || pawn.story.traits == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDefOf.Brawler);
        }

        public static bool is_kind(Pawn pawn) {
            if (pawn.story == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDefOf.Kind);
        }

        public static bool is_masochist(Pawn pawn) {
            if (pawn.story == null || pawn.story.traits == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

        }

        public static bool is_nympho(Pawn pawn) {
            if (pawn.story == null || pawn.story.traits == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(nymphomaniac);
        }

        public static bool is_gay(Pawn pawn) {
            if (pawn.story == null) {
                return false;
            }

            return pawn.story.traits.HasTrait(TraitDefOf.Gay);
        }

        public static bool is_animal(Pawn pawn) {
            return !pawn.RaceProps.Humanlike;
        }

        public static bool is_tooluser(Pawn pawn) {
            return pawn.RaceProps.intelligence >= Intelligence.ToolUser;
        }

        public static Gender opposite_gender(Gender g) {
            if (g == Gender.Male)
                return Gender.Female;
            else if (g == Gender.Female)
                return Gender.Male;
            else
                return Gender.None;
        }

        public static float get_sex_ability(Pawn p) {
            return p.GetStatValue(sex_stat, false);
        }

        /*
        public static bool genitals_blocked(Pawn pawn) {
            return Genital_Helper.genitals_blocked(pawn);
        }
        */

        public static bool can_fuck(Pawn pawn) {
            if (!xxx.is_animal(pawn)) {
                return (pawn.ageTracker.AgeBiologicalYears >= HugsLibInj.sex_minimum_age) && (pawn.ageTracker.AgeBiologicalYears >= HugsLibInj.sex_free_for_all_age) && (get_sex_ability(pawn) > 0.0f) && (!Genital_Helper.genitals_blocked(pawn));
            } else if (xxx.is_animal(pawn) && xxx.config.animals_enabled) {
                //return true;
                return (pawn.ageTracker.AgeBiologicalYears >= xxx.config.sex_minimum_age) && (get_sex_ability(pawn) > 0.0f);
            }
            return false;
        }

        public static bool can_get_raped(Pawn pawn) {
            // Pawns can still get raped even if their genitals are destroyed/removed.
            // Animals can always be raped regardless of age
            if (!xxx.is_animal(pawn)) {
                var age = pawn.ageTracker.AgeBiologicalYears;
                return (age >= HugsLibInj.sex_minimum_age) && (age >= HugsLibInj.sex_free_for_all_age) && (!Genital_Helper.genitals_blocked(pawn));
            } else if (xxx.is_animal(pawn) && xxx.config.animals_enabled) {
                return true;
            }
            return false;
        }

        // Returns how fuckable 'fucker' thinks 'p' is on a scale from 0.0 to 1.0
        public static float would_fuck(Pawn fucker, Pawn p, bool invert_opinion = false) {
            var fucker_age = fucker.ageTracker.AgeBiologicalYears;
            var p_age = p.ageTracker.AgeBiologicalYears;
            //var log_msg = "would_fuck() - fucker = " + fucker.Name + ", pawn = " + p.Name;
            //Log.Message(log_msg);

            bool age_ok;
            {
                //var ffa_age = config.sex_free_for_all_age;
                //if (xxx.is_animal(fucker) && xxx.config.animals_enabled && (p_age >= xxx.config.sex_free_for_all_age)) {
                if (xxx.is_animal(fucker) && xxx.config.animals_enabled && (p_age >= HugsLibInj.sex_free_for_all_age)) {
                    age_ok = true;
                } else if (xxx.is_animal(p) && xxx.config.animals_enabled && (fucker_age >= HugsLibInj.sex_free_for_all_age)) {
                    // don't check the age of animals when they are the victim
                    age_ok = true;
                    //} else if ((fucker_age >= xxx.config.sex_free_for_all_age) && (p_age >= xxx.config.sex_free_for_all_age)) {
                } else if ((fucker_age >= HugsLibInj.sex_free_for_all_age) && (p_age >= HugsLibInj.sex_free_for_all_age)) {
                    age_ok = true;
                    //} else if ((fucker_age < xxx.config.sex_minimum_age) || (p_age < xxx.config.sex_minimum_age)) {
                } else if ((fucker_age < HugsLibInj.sex_minimum_age) || (p_age < HugsLibInj.sex_minimum_age)) {
                    age_ok = false;
                } else {
                    age_ok = Math.Abs(fucker.ageTracker.AgeBiologicalYearsFloat - p.ageTracker.AgeBiologicalYearsFloat) < 2.05f;
                }
            }

            //Log.Message("would_fuck() - age_ok = " + age_ok.ToString());
            if (age_ok) {
                if ((!(fucker.Dead || p.Dead)) &&
                    (!(fucker.needs.food.Starving || p.needs.food.Starving)) &&
                    (fucker.health.hediffSet.BleedRateTotal <= 0.0f) && (p.health.hediffSet.BleedRateTotal <= 0.0f)) {

                    float orientation_factor;
                    {
                        Gender seeking = (!is_gay(fucker)) ? opposite_gender(fucker.gender) : fucker.gender;
                        if (p.gender == seeking)
                            orientation_factor = 1.0f;
                        else {
                            if ((fucker.gender == Gender.Female) && (p.gender == Gender.Female)) // High chance for casual lesbianism so e.g. nymphs are willing to fuck each other
                                orientation_factor = 0.4f;
                            else
                                orientation_factor = 0.1f;
                        }
                    }
                    //Log.Message("would_fuck() - orientation_factor = " + orientation_factor.ToString());

                    float age_factor = (p.gender == Gender.Male) ? attractiveness_from_age_male.Evaluate(p_age) : attractiveness_from_age_female.Evaluate(p_age);
                    //Log.Message("would_fuck() - age_factor = " + age_factor.ToString());

                    if (xxx.is_animal(fucker)) {
                        age_factor = 1.0f;
                    }
                    //Log.Message("would_fuck() - animal age_factor = " + age_factor.ToString());

                    float body_factor;
                    {
                        if (p.story != null) {
                            if (p.story.bodyType == BodyType.Female) body_factor = 1.25f;
                            else if (p.story.bodyType == BodyType.Fat) body_factor = 0.50f;
                            else body_factor = 1.00f;

                            if (RelationsUtility.IsDisfigured(p))
                                body_factor *= 0.7f;
                        } else {
                            body_factor = 1.0f;
                        }
                    }
                    //Log.Message("would_fuck() - body_factor = " + body_factor.ToString());

                    float trait_factor;
                    {
                        if (p.story != null) {
                            var deg = p.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                            trait_factor = 1.0f + 0.25f * (float)deg;
                        } else {
                            trait_factor = 1.0f;
                        }
                    }
                    //Log.Message("would_fuck() - trait_factor = " + trait_factor.ToString());

                    float opinion_factor;
                    {
                        if (p.relations != null) {
                            var opi = (float)((!invert_opinion) ? fucker.relations.OpinionOf(p) : 100 - fucker.relations.OpinionOf(p)); // -100 to 100
                            opinion_factor = 0.5f + (opi + 100.0f) * (0.8f / 200.0f); // 0.5 to 1.3
                        } else {
                            opinion_factor = 1.0f;
                        }
                    }
                    //Log.Message("would_fuck() - opinion_factor = " + opinion_factor.ToString());

                    float horniness_factor;
                    {
                        var need_sex = fucker.needs.TryGetNeed<Need_Sex>();
                        if (need_sex != null) {
                            if (need_sex.CurLevel <= need_sex.thresh_frustrated()) horniness_factor = 2.5f;
                            else if (need_sex.CurLevel <= need_sex.thresh_horny()) horniness_factor = 1.7f;
                            else horniness_factor = 1.0f;
                        } else
                            horniness_factor = 1.0f;
                    }
                    //Log.Message("would_fuck() - horniness_factor = " + horniness_factor.ToString());

                    var prenymph_att = Mathf.Clamp01(base_attraction * orientation_factor * age_factor * body_factor * trait_factor * opinion_factor * horniness_factor);
                    var final_att = (!is_nympho(fucker)) ? prenymph_att : 0.25f + 0.75f * prenymph_att;
                    //Log.Message("would_fuck( " + fucker.Name + ", " + p.Name + " ) - prenymph_att = " + prenymph_att.ToString() + ", final_att = " + final_att.ToString());

                    return final_att;

                } else
                    return 0.0f;
            } else
                return 0.0f;
        }

        // Adds satisfaction and joy after sex. "partner" can be null in case the pawn is fapping
        public static void satisfy(Pawn p, Pawn partner, Pawn rapist = null) {
            float sat;
            {
                var my_abi = get_sex_ability(p);
                var par_abi = (partner != null) ? get_sex_ability(partner) : no_parter_ability;
                sat = (is_nympho(p) ? 1.5f : 1.0f) * ((my_abi + par_abi) / 2.0f) * base_sat_per_fuck;
            }

            var need_sex = p.needs.TryGetNeed<Need_Sex>();
            if (need_sex != null)
                need_sex.CurLevel += sat;

            if (p.needs.joy != null) {
                float sat_to_joy;
                {
                    if ((partner != null) && (rapist == partner)) sat_to_joy = 0.00f; // p was raped
                    else if ((p == rapist) && (xxx.is_bloodlust(p))) sat_to_joy = 1.50f; // p raped someone and has bloodlust
                    else sat_to_joy = 0.65f; // normal sex or rape w/o bloodlust
                }
                p.needs.joy.CurLevel += sat * sat_to_joy;
            }
        }

        // Adds private parts to the pawn if it doesn't already have some
        // These functions were moved to Genital_Helper
        /*
		public static void sexualize (Pawn pawn)
		{
			Genital_Helper.sexualize (pawn);
		}

		public static void sexualize_everyone ()
		{
			Genital_Helper.sexualize_everyone ();
		}

		public static bool is_sexualized (Pawn pawn)
		{
			return Genital_Helper.is_sexualized (pawn);
		}

		public static bool pawns_require_sexualization ()
		{
			int count_sexualized = 0;
			bool found_one = false;

			foreach (var p in Find.WorldPawns.AllPawnsAliveOrDead)
				if ((!p.Dead) || p.Spawned)
					if (Genital_Helper.is_sexualized (p)) {
						++count_sexualized;
						if (count_sexualized > 50) // Late game worlds can have thousands of pawns. There's probably no point in checking all of them, and doing so could cause poor performance
							break;
					} else {
						found_one = true;
						break;
					}

			if (!found_one)
				foreach (var m in Find.Maps) {
					count_sexualized = 0;
					foreach (var p in m.mapPawns.AllPawns) {
						if (Genital_Helper.is_sexualized (p)) {
							++count_sexualized;
							if (count_sexualized > 50)
								break;
						} else {
							found_one = true;
							break;
						}
					}
				}

			return found_one;
		}
        */

        public static bool bed_has_at_least_two_occupants(Building_Bed bed) {
            int occupantc = 0;
            foreach (var occ in bed.CurOccupants)
                if (++occupantc >= 2)
                    break;
            return occupantc >= 2;
        }

        public static bool is_laying_down_alone(Pawn p) {
            if ((p.CurJob == null) ||
                (p.jobs.curDriver.layingDown == LayingDownState.NotLaying))
                return false;

            Building_Bed bed = null;

            if (p.jobs.curDriver is JobDriver_LayDown) {
                bed = ((JobDriver_LayDown)p.jobs.curDriver).Bed;
            }

            if (bed != null)
                return !bed_has_at_least_two_occupants(bed);
            else
                return true;
        }

        public static int generate_min_ticks_to_next_lovin(Pawn pawn) {
            if (!DebugSettings.alwaysDoLovin) {
                float interval = rjw_CORE_EXPOSED.JobDriver_Lovin.LovinIntervalHoursFromAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
                float rinterval = Math.Max(0.5f, Rand.Gaussian(interval, 0.3f));

                float tick = 1.0f;

                if (xxx.is_animal(pawn) || xxx.is_nympho(pawn)) {
                    tick = 0.5f;
                }

                return (int)(tick * rinterval * 2500.0f);
            } else {
                return 100;
            }
        }

        public static bool is_wasting_away(Pawn p) {
            var id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            return ((id != null) && (id.CurStageIndex > 0));
        }

        public static void update_immunodeficiency(Pawn p) {
            var min_bf_for_id = 1.0f - immunodeficiency.minSeverity;
            var id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            var bf = p.health.capacities.GetLevel(PawnCapacityDefOf.BloodFiltration);
            var has = (id != null);
            var should_have = (bf <= min_bf_for_id);

            if (has && (!should_have)) {
                p.health.RemoveHediff(id);
                id = null;
            } else if ((!has) && should_have) {
                p.health.AddHediff(immunodeficiency);
                id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
            }

            if (id != null) {
                id.Severity = 1.0f - bf;

                // Roll for and apply opportunistic infections:
                // Pawns will have a 90% chance for at least one infection each year at 0% filtration, and a 0%
                // chance at 40% filtration, scaling linearly.
                // Let x = chance infected per roll
                // Then chance not infected per roll = 1 - x
                // And chance not infected on any roll in one day = (1 - x) ^ (60000 / 150) = (1 - x) ^ 400
                // And chance not infected on any roll in one year = (1 - x) ^ (400 * 60) = (1 - x) ^ 24000
                // So 0.10 = (1 - x) ^ 24000
                //    log (0.10) = 24000 log (1 - x)
                //    x = 0.00009593644334648975435114691213 = ~96 in 1 million
                if ((Rand.RangeInclusive(1, 1000000) <= 100000) && (Rand.Value < bf / min_bf_for_id)) {
                    BodyPartRecord part;
                    {
                        var rv = Rand.Value;
                        if (rv < 0.25f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "Jaw"));
                        else if (rv < 0.50f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "LeftLung"));
                        else if (rv < 0.75f)
                            part = BodyDefOf.Human.AllParts.Find((BodyPartRecord bpr) => String.Equals(bpr.def.defName, "RightLung"));
                        else
                            part = BodyDefOf.Human.AllParts.RandomElement();
                    }

                    if ((part != null) &&
                        (!p.health.hediffSet.PartIsMissing(part)) && (!p.health.hediffSet.HasDirectlyAddedPartFor(part)) &&
                        (p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WoundInfection) == null) && // If the pawn already has a wound infection, we can't properly set the immunity for the new one
                        (p.health.immunity.GetImmunity(HediffDefOf.WoundInfection) <= 0.0f)) { // Dont spawn infection if pawn already has immunity

                        p.health.AddHediff(HediffDefOf.WoundInfection, part);
                        p.health.HealthTick(); // Creates the immunity record
                        var ir = p.health.immunity.GetImmunityRecord(HediffDefOf.WoundInfection);
                        if (ir != null)
                            ir.immunity = xxx.config.opp_inf_initial_immunity;
                        Find.LetterStack.ReceiveLetter("Opportunistic Infection", p.NameStringShort + " has developed an infection due to " + (p.gender == Gender.Male ? "his" : "her") + " weakened immune system.", LetterDefOf.BadNonUrgent, null);

                    }
                }
            }
        }

        public static void sexTick(Pawn pawn, Pawn partner) {
            pawn.Drawer.rotator.Face(partner.DrawPos);

            if (xxx.config.sounds_enabled) {
                SoundDef.Named("Sex").PlayOneShot(new TargetInfo(partner.Position, partner.Map, false));
            }

            pawn.Drawer.Notify_MeleeAttackOn(partner);
            pawn.Drawer.rotator.FaceCell(partner.Position);
        }

        // By default pawns who fuck receive a +10 relations boost. This method adds more relations based on the stuff
        // added in this mod.
        public static void think_after_sex(Pawn pawn, Pawn partner, Pawn rapist = null) {
            // Add thoughts if pawn did a rape
            if (rapist == pawn && !xxx.is_animal(pawn)) {
                var rapist_tho = (!xxx.is_bloodlust(pawn)) ? xxx.stole_some_lovin : xxx.bloodlust_stole_some_lovin;
                pawn.needs.mood.thoughts.memories.TryGainMemory(rapist_tho);

                // Add thoughts if p was raped
            } else if (rapist == partner && !xxx.is_animal(pawn)) {
                bool pawn_is_masochist = xxx.is_masochist(pawn);
                ThoughtDef raped_tho;

                if (xxx.is_animal(rapist)) {
                    raped_tho = (!pawn_is_masochist) ? got_raped_by_animal : masochist_got_raped_by_animal;
                } else {
                    raped_tho = (!pawn_is_masochist) ? got_raped : masochist_got_raped;
                }
                pawn.needs.mood.thoughts.memories.TryGainMemory(raped_tho);

                if (!xxx.is_animal(pawn) && !xxx.is_animal(partner)) {
                    var about_rapist_tho = (!pawn_is_masochist) ? hate_my_rapist : kinda_like_my_rapist;
                    pawn.needs.mood.thoughts.memories.TryGainMemory(about_rapist_tho, partner);
                }

                if (!pawn_is_masochist)
                    foreach (var bystander in pawn.Map.mapPawns.SpawnedPawnsInFaction(partner.Faction))
                        if ((bystander != pawn) && (bystander != partner) && !xxx.is_animal(bystander))
                            pawn.needs.mood.thoughts.memories.TryGainMemory(xxx.allowed_me_to_get_raped, bystander);

            }

            if (!xxx.is_animal(pawn) && !xxx.is_animal(partner)) {
                // Add negative relation for visible diseases on the genitals
                ThoughtDef tho_def;
                {
                    int net_sev = std.genital_rash_severity(partner) - std.genital_rash_severity(pawn);
                    if (net_sev == 1) tho_def = saw_rash_1;
                    else if (net_sev == 2) tho_def = saw_rash_2;
                    else if (net_sev >= 3) tho_def = saw_rash_3;
                    else tho_def = null;
                }

                if (tho_def != null) {
                    var tho = (Thought_Memory)ThoughtMaker.MakeThought(tho_def);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(tho, partner);
                }
            }
        }

        // Should be called after "pawn" has fucked "partner"
        // "rapist" can be set to either "pawn" or "partner", or null if no rape occurred
        public static void aftersex(Pawn pawn, Pawn partner, Pawn rapist = null) {
            // Roll for and apply food poisoning from rimming
            if ((rapist != pawn) &&
                (Rand.Value < xxx.config.chance_to_rim) &&
                (!pawn.health.hediffSet.HasHediff(HediffDefOf.FoodPoisoning))) {
                pawn.health.AddHediff(HediffDefOf.FoodPoisoning);
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                    Messages.Message(pawn.NameStringShort + " has gotten food poisoning by rimming " + partner.NameStringShort, pawn, MessageSound.Negative);
            }

            pawn.Drawer.rotator.Face(partner.DrawPos);
            pawn.Drawer.Notify_MeleeAttackOn(partner);
            pawn.Drawer.rotator.FaceCell(partner.Position);

            if (xxx.config.sounds_enabled) {
                SoundDef.Named("Cum").PlayOneShot(new TargetInfo(partner.Position, pawn.Map, false));
            }

            if (xxx.config.cum_enabled) {
                if (xxx.is_animal(pawn)) {
                    FilthMaker.MakeFilth(pawn.PositionHeld, pawn.MapHeld, cum, pawn.LabelIndefinite(), 3);
                }
                FilthMaker.MakeFilth(pawn.PositionHeld, pawn.MapHeld, cum, pawn.LabelIndefinite(), (int)(pawn.RaceProps.lifeExpectancy / pawn.ageTracker.AgeBiologicalYears));
            }

            satisfy(pawn, partner, rapist);
            think_after_sex(pawn, partner, rapist);

            if (pawn.gender == Gender.Female && partner.gender == Gender.Male) {
                try {
                    impregnate(pawn, partner);
                } catch {
                    Log.Message("RimworldChildren not present, can't impregnate :(");
                }
            }

            std.roll_to_catch(pawn, partner);
        }

        public static void impregnate(Pawn female, Pawn male) {
            BodyPartRecord torso = female.RaceProps.body.AllParts.Find(x => x.def == BodyPartDefOf.Torso);
            HediffDef contraceptive = HediffDef.Named("Contraceptive");

            // Make sure the woman is not pregnanct and not using a contraceptive
            if (female.health.hediffSet.HasHediff(HediffDefOf.Pregnant, torso) || female.health.hediffSet.HasHediff(contraceptive, null) || male.health.hediffSet.HasHediff(contraceptive, null)) {
                return;
            }
            // Check the pawn's age to see how likely it is she can carry a fetus
            // 25 and below is guaranteed, 50 and above is impossible, 37.5 is 50% chance
            float preg_chance = Math.Max(1 - (Math.Max(female.ageTracker.AgeBiologicalYearsFloat - 25, 0) / 25), 0) * 0.33f;
            float rand_value = Rand.Value;
            if (preg_chance < rand_value) {
                Log.Message("Impregnation failed. Chance was " + rand_value + " vs " + preg_chance);
                return;
            }
            Log.Message("Impregnation succeeded. Chance was " + rand_value + " vs " + preg_chance);
            // Spawn a bunch of hearts. Sharp eyed players may notice this means impregnation occurred.
            for (int i = 0; i <= 3; i++) {
                MoteMaker.ThrowMetaIcon(male.Position, male.MapHeld, ThingDefOf.Mote_Heart);
                MoteMaker.ThrowMetaIcon(female.Position, male.MapHeld, ThingDefOf.Mote_Heart);
            }

            // Do the actual impregnation. We apply it to the torso because Remove_Hediff in operations doesn't work on WholeBody (null body part)
            // for whatever reason.
            Hediff_HumanPregnancy hediff_Pregnant = (Hediff_HumanPregnancy)HediffMaker.MakeHediff(HediffDef.Named("HumanPregnancy"), female, torso);
            hediff_Pregnant.father = male;
            female.health.AddHediff(hediff_Pregnant, torso, null);
        }

        public static bool AttemptAnalRape(Pawn rapist, Pawn victim) {
            Log.Message(rapist.NameStringShort + " is attempting to anally rape " + victim.NameStringShort);
            return true;
        }
    }

}
