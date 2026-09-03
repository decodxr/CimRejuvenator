using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    public sealed class LocaleDirectEN : IDictionarySource
    {
        private readonly CimRejuvenatorSetting m_Setting;

        public LocaleDirectEN(CimRejuvenatorSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMode)), "Continuous direct growth lock" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMode)), "For zero or positive targets, continuously protects a non-decreasing population floor and schedules normal resident households when the city falls behind. This no longer waits for a full day before reacting." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength)), "Direct correction strength" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength)), "Percentage of the detected shortfall corrected on each growth-lock check. 100% is recommended during severe death waves." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerDay)), "Maximum direct residents per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerDay)), "Daily safety limit for residents represented by directly scheduled vanilla households." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerCheck)), "Maximum direct residents per check" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerCheck)), "Limits one correction burst. Lower values spread recovery across the day; higher values react faster to extreme losses." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.EmergencyGrowthPreset)), "APPLY EMERGENCY GROWTH PRESET" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.EmergencyGrowthPreset)), "Enables direct growth lock with +5,000/day target, 100% correction, aggressive immigration and birth support, and no forced outflow." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendMode)), "Population trend mode" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendMode)), "Shows whether the controller is disabled, adaptive, or using the continuous direct growth lock." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendGrowthFloor)), "Protected population floor" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendGrowthFloor)), "Minimum established population currently protected by direct growth lock. The floor never moves downward while the lock is active." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendShortfallLastCheck)), "Growth-lock shortfall" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendShortfallLastCheck)), "Residents missing from the protected floor after partial credit for households already scheduled but not yet established." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendPendingDirectResidents)), "Pending direct residents" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendPendingDirectResidents)), "Estimated residents already scheduled by direct mode and still waiting to appear in the established-resident census." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectCorrectionRequested)), "Latest direct correction request" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectCorrectionRequested)), "Resident correction requested by the latest growth-lock check before household-size rounding." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsToday)), "Direct residents scheduled today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsToday)), "Estimated residents represented by vanilla household entities scheduled by direct mode today." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsSession)), "Direct residents scheduled this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsSession)), "Estimated residents represented by direct-mode household injections during this game session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectHouseholdsToday)), "Direct households scheduled today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectHouseholdsToday)), "Normal vanilla household entities scheduled by direct mode today." },
            };
        }

        public void Unload() { }
    }
}
