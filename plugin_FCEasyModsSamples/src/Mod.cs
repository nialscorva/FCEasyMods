using nialsorva.FCEEasyMods;
using System;
using System.Collections.Generic;

namespace nialscorva.FCEasyModsSamples {

    public class Mod : FortressCraftMod
    {

        private const string LOGGER_PREFIX = "[nialscorva.FCEasyModsSamples]  ";
        private const bool DEBUG = true;
        public static void log(String msg, params object[] args)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log(LOGGER_PREFIX + ": " + String.Format(msg, args));
            }
        }

        protected ModLoader modLoader = new ModLoader();

        public override ModRegistrationData Register()
        {
            return modLoader.ScanAssembly(GetType().Assembly);
        }

        public override ModCreateSegmentEntityResults CreateSegmentEntity(ModCreateSegmentEntityParameters parameters)
        {
            return modLoader.CreateSegmentEntity(parameters);
        }
        public override ModItemActionResults PerformItemAction(ModItemActionParameters parameters)
        {
            ModItemActionResults miar = new ModItemActionResults();
            
            return miar;
        }

        public override void CheckForCompletedMachine(ModCheckForCompletedMachineParameters parameters)
        {

        }
        public override void CreateMobEntity(ModCreateMobParameters parameters, ModCreateMobResults results)
        {

        }

        public override void HandleCustomRegistrationData(ICustomModRegistrationData registrationData)
        {

        }
        public override void LowFrequencyUpdate()
        {

        }
    }
}