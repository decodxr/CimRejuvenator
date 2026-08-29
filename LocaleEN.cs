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
                { m_Setting.GetOptionGroupLocaleID(Setting.kStatsGroup), "Statistics" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMod)), "Enable Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMod)), "Master switch. Disabling the mod does not undo citizens already rejuvenated." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenationChance)), "Rejuvenation chance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenationChance)), "Percentage of elderly cims that return to Adult in this life cycle. 100% practically prevents old-age deaths, but illness and accidents can still kill." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetAgeDays)), "Internal age after rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetAgeDays)), "Simulation age assigned after returning to Adult. 40 is a good starting point." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreHealth)), "Restore minimum health on rejuvenation" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreHealth)), "Raises health to at least 80 when rejuvenated. Existing sickness or injury is not removed." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Maximum rejuvenations per day" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Safety throttle so a huge retired population does not re-enter the workforce all at once." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SeniorsLastScan)), "Living seniors in last scan" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SeniorsLastScan)), "Living elderly citizens found by the last mod sweep." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedToday)), "Rejuvenated today" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedToday)), "Citizens rejuvenated during the current simulation day." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedSession)), "Rejuvenated this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedSession)), "Total citizens rejuvenated since the game was opened." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetStatistics)), "Reset statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetStatistics)), "Reset the counters shown on this page." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetStatistics)), "Reset this session's counters?" },
            };
        }

        public void Unload() { }
    }
}
