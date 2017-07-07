using System;
using System.Collections.Generic;
using System.Text;

using Verse;
using RimWorld;

namespace rjw {
	public class config : Verse.Def {
		
		// Feature Toggles
		public bool animals_enabled;
		public bool comfort_prisoners_enabled;
        public bool colonists_can_be_comfort_prisoners;
		public bool cum_enabled;
		public bool rape_me_sticky_enabled;
		public bool sounds_enabled;
		public bool stds_enabled;
		public bool bondage_gear_enabled;
		public bool nymph_joiners_enabled;
        public bool whore_beds_enabled;

		// Display Toggles
		public bool show_regular_dick_and_vag;

		// STD config
		public bool std_show_roll_to_catch;
		public float std_min_severity_to_pitch;
		public float std_env_pitch_cleanliness_exaggeration;
		public float std_env_pitch_dirtiness_exaggeration;
		public float std_outdoor_cleanliness;

		// Age Config
		public int sex_free_for_all_age;
        public int sex_minimum_age;

		public float significant_pain_threshold;
		public float max_nymph_fraction;
		public float opp_inf_initial_immunity;
		public float comfort_prisoner_rape_mtbh_mul;
		public float nymph_spawn_with_std_mul;
		public float chance_to_rim;
	}
}
