using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    /// <summary>
    /// English strings introduced by the direct population-trend controller.
    /// Kept separate from the base locale source so feature-specific UI text can evolve independently.
    /// </summary>
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
                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMode)), "Direct trend compensation" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMode)), "When the previous complete simulation day misses the selected population target, schedule normal resident households directly to compensate the shortfall instead of relying only on immigration demand and birth rates." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength)), "Direct correction strength" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength)), "Percentage of the measured daily shortfall to compensate. At 100%, direct mode attempts to cover the full shortfall, subject to the daily safety cap and population ceiling." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerDay)), "Maximum direct residents per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerDay)), "Safety cap for the estimated number of residents scheduled through direct household injection after one completed simulation day." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendMode)), "Population trend mode" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendMode)), "Shows whether the trend controller is disabled, using adaptive rate steering, or using direct daily compensation." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectCorrectionRequested)), "Direct correction requested" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectCorrectionRequested)), "Resident shortfall requested by the most recent direct correction before household-size rounding and other limits." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsToday)), "Direct residents scheduled today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsToday)), "Estimated residents represented by vanilla household entities scheduled by direct mode today. Final established-resident counts depend on normal household initialization and moving-in." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsSession)), "Direct residents scheduled this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectResidentsSession)), "Estimated residents represented by direct-mode household injections during this game session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(CimRejuvenatorSetting.TrendDirectHouseholdsToday)), "Direct households scheduled today" },
                { m_Setting.GetOptionDescLocaleID(nameof(CimRejuvenatorSetting.TrendDirectHouseholdsToday)), "Number of normal vanilla household entities scheduled by direct mode today." },
            };
        }

        public void Unload() { }
    }
}
