using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace CimRejuvenator
{
    [FileLocation("ModsSettings/CimRejuvenator/CimRejuvenator")]
    [SettingsUIGroupOrder(kMainGroup, kRejuvenationGroup, kSafetyGroup, kStatsGroup)]
    [SettingsUIShowGroupName(kMainGroup, kRejuvenationGroup, kSafetyGroup, kStatsGroup)]
    public sealed class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kMainGroup = "General";
        public const string kRejuvenationGroup = "Rejuvenation";
        public const string kSafetyGroup = "Safety";
        public const string kStatsGroup = "Statistics";

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(kSection, kMainGroup)]
        public bool EnableMod { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int RejuvenationChance { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsDisabled))]
        [SettingsUISlider(min = 36, max = 70, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int ResetAgeDays { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool RestoreHealth { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsDisabled))]
        [SettingsUISlider(min = 100, max = 20000, step = 100, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kSafetyGroup)]
        public int MaxRejuvenationsPerDay { get; set; }

        [SettingsUISection(kSection, kStatsGroup)]
        public string SeniorsLastScan => RejuvenationSystem.SeniorsLastScan.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedToday => RejuvenationSystem.RejuvenatedToday.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedSession => RejuvenationSystem.RejuvenatedSession.ToString("N0");

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kStatsGroup)]
        public bool ResetStatistics
        {
            set { RejuvenationSystem.ResetStatistics(); }
        }

        public bool IsDisabled() => !EnableMod;

        public sealed override void SetDefaults()
        {
            EnableMod = true;
            RejuvenationChance = 80;
            ResetAgeDays = 40;
            RestoreHealth = true;
            MaxRejuvenationsPerDay = 5000;
        }
    }
}
