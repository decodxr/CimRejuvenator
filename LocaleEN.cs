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
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kSafetyGroup), "Safety" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kPerformanceGroup), "Performance" },
                { m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kStatsGroup), "Statistics" },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EnableMod)), "Enable Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EnableMod)), "Master switch. Disabling the mod does not undo citizens already rejuvenated." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenationChance)), "Rejuvenation chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenationChance)), "Percentage of eligible elderly cims that return to Adult. The chance roll stays stable during the same elderly life cycle." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ResetAgeDays)), "Internal age after rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ResetAgeDays)), "Simulation age assigned after returning to Adult. 40 is a good starting point." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RestoreHealth)), "Restore minimum health on rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RestoreHealth)), "Raises health to at least 80 when rejuvenated. Existing sickness or injury is not removed." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenateNow)), "REJUVENATE NOW" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenateNow)), "Queues an immediate sweep. It runs as soon as the simulation is active and still respects chance, daily limit, per-sweep limit and demographic protection." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerDay)), "Maximum rejuvenations per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerDay)), "Total cap per simulation day. Up to 250,000; very high values can change your city's economy extremely quickly." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerSweep)), "Maximum per sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerSweep)), "Limits how many elderly cims may be rejuvenated in one scan, preventing an instant conversion of tens of thousands of citizens." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.KeepMinimumSeniorShare)), "Keep a minimum elderly share" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.KeepMinimumSeniorShare)), "When enabled, rejuvenation stops before the elderly population drops below the selected percentage." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.MinimumSeniorPercent)), "Minimum elderly percentage" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.MinimumSeniorPercent)), "Optional demographic protection. 15% is a reasonable balanced-city starting point." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SweepsPerDay)), "Automatic sweeps per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SweepsPerDay)), "How often the full population is checked. 64 is the default. Higher values react faster but perform more CPU work." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.CitizensLastScan)), "Citizens in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.CitizensLastScan)), "Citizen entities examined during the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SeniorsLastScan)), "Living seniors in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SeniorsLastScan)), "Living elderly citizens found by the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ElderlyPercentLastScan)), "Elderly in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ElderlyPercentLastScan)), "Approximate elderly percentage found by the latest sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedLastSweep)), "Rejuvenated in last sweep" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedLastSweep)), "Citizens actually rejuvenated during the most recent sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedToday)), "Rejuvenated today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedToday)), "Citizens rejuvenated during the current simulation day." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedSession)), "Rejuvenated this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.RejuvenatedSession)), "Total citizens rejuvenated since the game was opened." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.SweepsSession)), "Sweeps this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.SweepsSession)), "Full rejuvenation sweeps completed during this session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.LastSimulationDay)), "Last simulation day scanned" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.LastSimulationDay)), "Internal simulation day of the latest completed sweep." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Reset statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Reset the counters shown on this page." },
                { m_Setting.GetOptionWarningLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics)), "Reset this session's counters?" },
            };
        }

        public void Unload() { }
    }
}
