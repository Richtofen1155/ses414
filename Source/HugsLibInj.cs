using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HugsLib;
using HugsLib.Settings;
using UnityEngine.SceneManagement;
using HugsLib.Core;

namespace rjw
{
    public class HugsLibInj : ModBase
    {

        public override string ModIdentifier
        {
            get
            {
                return "RJW";
            }
        }

        public override VersionShort GetVersion() {
            Logger.Message("GetVersion() called");
            return base.GetVersion();
        }

        private SettingHandle<bool> nymphs_join;
        private SettingHandle<bool> STD_floor_catch;
        private SettingHandle<bool> reipu_beating;
        //private SettingHandle<bool> animals_enabled;
        //private SettingHandle<bool> comfort_prisoners_enabled;
        //private SettingHandle<bool> colonists_can_be_comfort_prisoners;
        //private SettingHandle<bool> cum_enabled;
        //private SettingHandle<bool> rape_me_sticky_enabled;
        //private SettingHandle<bool> sounds_enabled;
        //private SettingHandle<bool> stds_enabled;
        //private SettingHandle<bool> bondage_gear_enabled;
        //private SettingHandle<bool> show_regular_dick_and_vag;
        private SettingHandle<uint> option_sex_free_for_all_age;
        private SettingHandle<uint> option_sex_minimum_age;

        public static bool nymphos = true;
        public static bool std_floor = true;
        public static bool prisoner_beating = true;
        public static uint sex_free_for_all_age;
        public static uint sex_minimum_age;


        public override void Initialize() {
            Logger.Message("Initialize() called");
            base.Initialize();
        }

        public override void DefsLoaded()
        {
            Logger.Message("DefsLoaded() called");
            nymphs_join = Settings.GetHandle<bool>("nymphs_join", "Nymphs join", "Will nymphos join your colony", true);
            STD_floor_catch = Settings.GetHandle<bool>("STD_floor_catch", "STD from floors", "If enabled, STD will be catched not only from the persons carrying it, but also from the environment", true);
            reipu_beating = Settings.GetHandle<bool>("reipu_beating", "Prisoners beating", "Will comfort prisoners get beaten in the acts", true);
            option_sex_free_for_all_age = Settings.GetHandle<uint>("sex_free_for_all_age", "Sex free for all age", "Pawns younger than the free-for-all age will only have sex with others within 2 biological years of their own age", 15);
            option_sex_minimum_age = Settings.GetHandle<uint>("sex_minimum_age", "Sex minimum age", "Pawns must be this old to have sex at all (should be lower than free-for-all age)", 15);

            nymphos = nymphs_join.Value;
            std_floor = STD_floor_catch.Value;
            prisoner_beating = reipu_beating.Value;
            sex_free_for_all_age = option_sex_free_for_all_age.Value;
            sex_minimum_age = option_sex_minimum_age.Value;

        }

        public override void Update() {
            base.Update();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
        }

        public override void MapComponentsInitializing(Map map) {
            Logger.Message("MapComponentsInitializing() called");
            base.MapComponentsInitializing(map);
        }

        public override void MapDiscarded(Map map) {
            Logger.Message("MapDiscarded() called");
            base.MapDiscarded(map);
        }

        public override void MapGenerated(Map map) {
            Logger.Message("MapGenerated() called");
            base.MapGenerated(map);
        }

        public override void MapLoaded(Map map) {
            Logger.Message("MapLoaded() called");
            base.MapLoaded(map);
        }

        public override void OnGUI() {
            base.OnGUI();
        }

        public override void SceneLoaded(Scene scene) {
            Logger.Message("SceneLoaded() called");
            base.SceneLoaded(scene);
        }

        public override void SettingsChanged()
        {
            Logger.Message("SettingsChanged() called");
            nymphos = nymphs_join.Value;
            std_floor = STD_floor_catch.Value;
            prisoner_beating = reipu_beating.Value;
            sex_free_for_all_age = option_sex_free_for_all_age.Value;
            sex_minimum_age = option_sex_minimum_age.Value;
        }

        /*
        public override void Tick(int currentTick) {
            base.Tick(currentTick);
        }
        */

        public override void WorldLoaded() {
            Logger.Message("WorldLoaded() called");
            base.WorldLoaded();
        }

    }
}
