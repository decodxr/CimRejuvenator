using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly CimRejuvenatorSetting m_Setting;

        public LocaleEN(CimRejuvenatorSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Cim Rejuvenator" },
                { m_Setting.GetOptionTabLocaleID(CimRejuvenatorSetting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kMainGroup), "General" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kRejuvenationGroup), "Rejuvenation" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kDemographicsGroup), "Demographics" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kImmigrationGroup), "Immigration" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kBirthGroup), "Births" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kPerformanceGroup), "Performance" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kStatsGroup), "Statistics" },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableMod)), "Enable Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableMod)), "Master switch for all population management features." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableRejuvenation)), "Enable rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableRejuvenation)), "Allows eligible elderly residents to return to the Adult life stage." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenationChance)), "Rejuvenation chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenationChance)), "Percentage of eligible elderly residents selected for rejuvenation. The selection is stable for the same elderly life cycle." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ResetAgeDays)), "Age after rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ResetAgeDays)), "Age in simulation days assigned after returning to Adult." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RestoreHealth)), "Restore minimum health" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RestoreHealth)), "Raises health to at least 80 when a resident is rejuvenated. Existing sickness or injury flags are not removed." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerDay)), "Maximum rejuvenations per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerDay)), "Daily safety cap for rejuvenation. The maximum selectable value is 250,000." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerSweep)), "Maximum rejuvenations per sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerSweep)), "Limits how many residents can be rejuvenated in one population sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.KeepMinimumSeniorShare)), "Keep a minimum elderly share" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.KeepMinimumSeniorShare)), "Stops rejuvenation before the elderly share falls below the configured minimum." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MinimumSeniorPercent)), "Minimum elderly share" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MinimumSeniorPercent)), "Minimum percentage of residents kept in the Elderly life stage by rejuvenation." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenateNow)), "REJUVENATE NOW" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenateNow)), "Queues a rejuvenation sweep for the next active simulation update. Safety limits still apply." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableDemographicBalancer)), "Enable demographic balancer" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableDemographicBalancer)), "Gradually moves the resident age distribution toward the configured target weights." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TargetChildPercent)), "Child target weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TargetChildPercent)), "Relative target weight for Child residents. All four target weights are normalized automatically." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TargetTeenPercent)), "Teen target weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TargetTeenPercent)), "Relative target weight for Teen residents." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TargetAdultPercent)), "Adult target weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TargetAdultPercent)), "Relative target weight for Adult residents." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TargetSeniorPercent)), "Elderly target weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TargetSeniorPercent)), "Relative target weight for Elderly residents." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxAgeConversionsPerSweep)), "Maximum age conversions per sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxAgeConversionsPerSweep)), "Limits demographic balancing work in a single sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ProtectWorkersWhenBalancing)), "Protect employed residents" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ProtectWorkersWhenBalancing)), "Prevents employed residents from being converted to Child, Teen, or Elderly while balancing." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.BalanceNow)), "BALANCE NOW" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.BalanceNow)), "Queues a demographic balancing sweep for the next active simulation update." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TargetWeightTotal)), "Target weight total" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TargetWeightTotal)), "The values do not need to total 100%; they are treated as relative weights." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableImmigrationControl)), "Enable immigration control" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableImmigrationControl)), "Controls the vanilla resident household spawn system." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ImmigrationIntensity)), "Immigration intensity" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ImmigrationIntensity)), "Approximate percentage of household-spawn opportunities left open. 100% keeps normal spawning available; 0% pauses it." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.UseImmigrationDailyCap)), "Use daily new-resident cap" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.UseImmigrationDailyCap)), "Pauses new household spawning after the detected number of incoming residents reaches the daily cap." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxNewResidentsPerDay)), "Maximum new residents per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxNewResidentsPerDay)), "Soft daily cap. A household already being created can make the final count exceed the value slightly." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.UsePopulationCeiling)), "Use population ceiling" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.UsePopulationCeiling)), "Pauses resident household spawning when the resident population reaches the configured ceiling." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.PopulationCeiling)), "Resident population ceiling" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.PopulationCeiling)), "Population threshold used by immigration control. Existing residents are never removed." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ShapeNewResidentAges)), "Control incoming age mix" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ShapeNewResidentAges)), "Reassigns newly detected resident immigrants to the configured Child, Teen, Adult, and Elderly weights." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.IncomingChildWeight)), "Incoming Child weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.IncomingChildWeight)), "Relative chance that a newly detected resident immigrant is assigned to the Child life stage." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.IncomingTeenWeight)), "Incoming Teen weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.IncomingTeenWeight)), "Relative chance that a newly detected resident immigrant is assigned to the Teen life stage." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.IncomingAdultWeight)), "Incoming Adult weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.IncomingAdultWeight)), "Relative chance that a newly detected resident immigrant is assigned to the Adult life stage." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.IncomingSeniorWeight)), "Incoming Elderly weight" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.IncomingSeniorWeight)), "Relative chance that a newly detected resident immigrant is assigned to the Elderly life stage." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.IncomingWeightTotal)), "Incoming weight total" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.IncomingWeightTotal)), "Incoming age values are normalized automatically and do not need to total 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableBirthControl)), "Enable birth control" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableBirthControl)), "Scales the game's base birth rate and adult-partner birth bonus." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.BirthRatePercent)), "Birth rate multiplier" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.BirthRatePercent)), "100% is the captured vanilla rate, 0% stops new births, and values above 100% increase births." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.UseBirthDailyCap)), "Use daily birth cap" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.UseBirthDailyCap)), "Sets the applied birth-rate multiplier to 0% after the detected daily birth count reaches the cap." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxBirthsPerDay)), "Maximum births per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxBirthsPerDay)), "Soft daily cap based on detected newborn citizen entities." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.BirthsRespectChildTarget)), "Stop births at Child target" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.BirthsRespectChildTarget)), "Temporarily applies a 0% birth rate when the current Child share reaches the normalized Child target weight." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SweepsPerDay)), "Population sweeps per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SweepsPerDay)), "Controls how often full resident census, rejuvenation, and demographic balancing sweeps run. Higher values react faster and use more CPU time." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ResidentCount)), "Residents in last census" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ResidentCount)), "Living non-tourist, non-commuter citizens found in the latest population sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ChildCount)), "Children" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ChildCount)), "Child residents and their share of the latest census." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TeenCount)), "Teens" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TeenCount)), "Teen residents and their share of the latest census." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.AdultCount)), "Adults" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.AdultCount)), "Adult residents and their share of the latest census." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SeniorCount)), "Elderly" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SeniorCount)), "Elderly residents and their share of the latest census." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedLastSweep)), "Rejuvenated in last sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedLastSweep)), "Residents rejuvenated during the latest population sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedToday)), "Rejuvenated today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedToday)), "Residents rejuvenated during the current simulation day." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedSession)), "Rejuvenated this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedSession)), "Residents rejuvenated since this game session started." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.AgeConvertedLastSweep)), "Demographic conversions in last sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.AgeConvertedLastSweep)), "Residents reassigned to another life stage by the demographic balancer in the latest sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.AgeConvertedSession)), "Demographic conversions this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.AgeConvertedSession)), "Total demographic life-stage conversions during this session." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.BirthsToday)), "Births detected today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.BirthsToday)), "Newborn residents detected during the current simulation day." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.BirthsSession)), "Births detected this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.BirthsSession)), "Newborn residents detected since the population flow baseline was initialized." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.NewResidentsToday)), "New residents detected today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.NewResidentsToday)), "New non-tourist, non-commuter resident citizens detected today, excluding newborns." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.NewResidentsSession)), "New residents this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.NewResidentsSession)), "New resident immigrants detected since the population flow baseline was initialized." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.AppliedBirthRate)), "Applied birth rate" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.AppliedBirthRate)), "Current birth-rate multiplier after daily-cap and Child-target rules are applied." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ImmigrationStatus)), "Immigration status" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ImmigrationStatus)), "Current state of the resident household spawn throttle." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SweepsSession)), "Population sweeps this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SweepsSession)), "Full population sweeps completed during this session." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.LastSimulationDay)), "Last simulation day scanned" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.LastSimulationDay)), "Internal simulation day of the latest population sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Reset statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Resets session counters shown on this page." },
                { m_Setting.GetOptionWarningLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Reset Cim Rejuvenator statistics?" },
            };
        }

        public void Unload() { }
    }
}
