using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace CimRejuvenator
{
    [FileLocation("ModsSettings/CimRejuvenator/CimRejuvenator")]
    [SettingsUIGroupOrder(kMainGroup, kRejuvenationGroup, kSafetyGroup, kPerformanceGroup, kStatsGroup)]
    [SettingsUIShowGroupName(kMainGroup, kRejuvenationGroup, kSafetyGroup, kPerformanceGroup, kStatsGroup)]
    public sealed class CimRejuvenatorSetting : ModSetting
    {
        public const string kSection = "Main";
        public const string kMainGroup = "General";
        public const string kRejuvenationGroup = "Rejuvenation";
        public const string kSafetyGroup = "Safety";
        public const string kPerformanceGroup = "Performance";
        public const string kStatsGroup = "Statistics";

        public CimRejuvenatorSetting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(kSection, kMainGroup)]
        public bool EnableMod { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int RejuvenationChance { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 36, max = 70, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int ResetAgeDays { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool RestoreHealth { get; set; }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool RejuvenateNow
        {
            set { RejuvenationSystem.RequestImmediateSweep(); }
        }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 100, max = 250000, step = 1000, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kSafetyGroup)]
        public int MaxRejuvenationsPerDay { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 100, max = 100000, step = 500, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kSafetyGroup)]
        public int MaxRejuvenationsPerSweep { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kSafetyGroup)]
        public bool KeepMinimumSeniorShare { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsSeniorShareDisabled))]
        [SettingsUISlider(min = 0, max = 50, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kSafetyGroup)]
        public int MinimumSeniorPercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 8, max = 256, step = 8, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kPerformanceGroup)]
        public int SweepsPerDay { get; set; }

        [SettingsUISection(kSection, kStatsGroup)]
        public string CitizensLastScan => RejuvenationSystem.CitizensLastScan.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string SeniorsLastScan => RejuvenationSystem.SeniorsLastScan.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string ElderlyPercentLastScan => RejuvenationSystem.ElderlyPercentLastScan.ToString("F1") + "%";

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedLastSweep => RejuvenationSystem.RejuvenatedLastSweep.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedToday => RejuvenationSystem.RejuvenatedToday.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedSession => RejuvenationSystem.RejuvenatedSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string SweepsSession => RejuvenationSystem.SweepsSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string LastSimulationDay => RejuvenationSystem.LastSimulationDay < 0
            ? "-"
            : RejuvenationSystem.LastSimulationDay.ToString("N0");

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kStatsGroup)]
        public bool ResetStatistics
        {
            set { RejuvenationSystem.ResetStatistics(); }
        }

        public bool IsDisabled() => !EnableMod;

        public bool IsSeniorShareDisabled() => !EnableMod || !KeepMinimumSeniorShare;

        public sealed override void SetDefaults()
        {
            EnableMod = true;
            RejuvenationChance = 80;
            ResetAgeDays = 40;
            RestoreHealth = true;

            MaxRejuvenationsPerDay = 20000;
            MaxRejuvenationsPerSweep = 5000;

            KeepMinimumSeniorShare = false;
            MinimumSeniorPercent = 15;

            SweepsPerDay = 64;
        }
    }
}
