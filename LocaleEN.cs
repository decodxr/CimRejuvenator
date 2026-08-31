using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kMainGroup), "General" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kRejuvenationGroup), "Rejuvenation" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kSafetyGroup), "Safety" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPerformanceGroup), "Performance" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kStatsGroup), "Statistics" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMod)), "Enable Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMod)), "Master switch. Disabling the mod does not undo citizens already rejuvenated." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenationChance)), "Rejuvenation chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenationChance)), "Percentage of eligible elderly cims that return to Adult. The chance roll stays stable during the same elderly life cycle." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetAgeDays)), "Internal age after rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetAgeDays)), "Simulation age assigned after returning to Adult. 40 is a good starting point." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreHealth)), "Restore minimum health on rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreHealth)), "Raises health to at least 80 when rejuvenated. Existing sickness or injury is not removed." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenateNow)), "REJUVENATE NOW" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenateNow)), "Queues an immediate sweep. It runs as soon as the simulation is active and still respects chance, daily limit, per-sweep limit and demographic protection." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Maximum rejuvenations per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Total cap per simulation day. Up to 250,000; very high values can change your city's economy extremely quickly." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerSweep)), "Maximum per sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerSweep)), "Limits how many elderly cims may be rejuvenated in one scan, preventing an instant conversion of tens of thousands of citizens." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.KeepMinimumSeniorShare)), "Keep a minimum elderly share" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.KeepMinimumSeniorShare)), "When enabled, rejuvenation stops before the elderly population drops below the selected percentage." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MinimumSeniorPercent)), "Minimum elderly percentage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MinimumSeniorPercent)), "Optional demographic protection. 15% is a reasonable balanced-city starting point." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SweepsPerDay)), "Automatic sweeps per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SweepsPerDay)), "How often the full population is checked. 64 is the default. Higher values react faster but perform more CPU work." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CitizensLastScan)), "Citizens in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CitizensLastScan)), "Citizen entities examined during the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SeniorsLastScan)), "Living seniors in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SeniorsLastScan)), "Living elderly citizens found by the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ElderlyPercentLastScan)), "Elderly in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ElderlyPercentLastScan)), "Approximate elderly percentage found by the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedLastSweep)), "Rejuvenated in last sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedLastSweep)), "Citizens actually rejuvenated during the most recent sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedToday)), "Rejuvenated today" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedToday)), "Citizens rejuvenated during the current simulation day." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedSession)), "Rejuvenated this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedSession)), "Total citizens rejuvenated since the game was opened." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SweepsSession)), "Sweeps this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SweepsSession)), "Full rejuvenation sweeps completed during this session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LastSimulationDay)), "Last simulation day scanned" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LastSimulationDay)), "Internal simulation day of the latest completed sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetStatistics)), "Reset statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetStatistics)), "Reset the counters shown on this page." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetStatistics)), "Reset this session's counters?" },
            };
        }

        public void Unload() { }
    }
}
