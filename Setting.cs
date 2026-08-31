using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace CimRejuvenator
{
    [FileLocation("ModsSettings/CimRejuvenator/CimRejuvenator")]
    [SettingsUIGroupOrder(
        kMainGroup,
        kRejuvenationGroup,
        kDemographicsGroup,
        kImmigrationGroup,
        kBirthGroup,
        kPerformanceGroup,
        kStatsGroup)]
    [SettingsUIShowGroupName(
        kMainGroup,
        kRejuvenationGroup,
        kDemographicsGroup,
        kImmigrationGroup,
        kBirthGroup,
        kPerformanceGroup,
        kStatsGroup)]
    public sealed class CimRejuvenatorSetting : ModSetting
    {
        public const string kSection = "Main";
        public const string kMainGroup = "General";
        public const string kRejuvenationGroup = "Rejuvenation";
        public const string kDemographicsGroup = "Demographics";
        public const string kImmigrationGroup = "Immigration";
        public const string kBirthGroup = "Births";
        public const string kPerformanceGroup = "Performance";
        public const string kStatsGroup = "Statistics";

        public CimRejuvenatorSetting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(kSection, kMainGroup)]
        public bool EnableMod { get; set; }

        // Rejuvenation

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool EnableRejuvenation { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int RejuvenationChance { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISlider(min = 36, max = 70, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int ResetAgeDays { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool RestoreHealth { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISlider(min = 100, max = 250000, step = 1000, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int MaxRejuvenationsPerDay { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISlider(min = 100, max = 100000, step = 500, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int MaxRejuvenationsPerSweep { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool KeepMinimumSeniorShare { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsSeniorShareDisabled))]
        [SettingsUISlider(min = 0, max = 50, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public int MinimumSeniorPercent { get; set; }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsRejuvenationDisabled))]
        [SettingsUISection(kSection, kRejuvenationGroup)]
        public bool RejuvenateNow
        {
            set { PopulationManagementSystem.RequestImmediateRejuvenation(); }
        }

        // Demographics

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public bool EnableDemographicBalancer { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public int TargetChildPercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public int TargetTeenPercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public int TargetAdultPercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public int TargetSeniorPercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISlider(min = 100, max = 100000, step = 500, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public int MaxAgeConversionsPerSweep { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public bool ProtectWorkersWhenBalancing { get; set; }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDemographicBalancerDisabled))]
        [SettingsUISection(kSection, kDemographicsGroup)]
        public bool BalanceNow
        {
            set { PopulationManagementSystem.RequestImmediateBalance(); }
        }

        [SettingsUISection(kSection, kDemographicsGroup)]
        public string TargetWeightTotal =>
            (TargetChildPercent + TargetTeenPercent + TargetAdultPercent + TargetSeniorPercent).ToString("N0") + "%";

        // Immigration

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public bool EnableImmigrationControl { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsImmigrationControlDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int ImmigrationIntensity { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsImmigrationControlDisabled))]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public bool UseImmigrationDailyCap { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsImmigrationDailyCapDisabled))]
        [SettingsUISlider(min = 10, max = 250000, step = 100, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int MaxNewResidentsPerDay { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsImmigrationControlDisabled))]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public bool UsePopulationCeiling { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsPopulationCeilingDisabled))]
        [SettingsUISlider(min = 1000, max = 2000000, step = 1000, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int PopulationCeiling { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsImmigrationControlDisabled))]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public bool ShapeNewResidentAges { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsIncomingAgeMixDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int IncomingChildWeight { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsIncomingAgeMixDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int IncomingTeenWeight { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsIncomingAgeMixDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int IncomingAdultWeight { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsIncomingAgeMixDisabled))]
        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kImmigrationGroup)]
        public int IncomingSeniorWeight { get; set; }

        [SettingsUISection(kSection, kImmigrationGroup)]
        public string IncomingWeightTotal =>
            (IncomingChildWeight + IncomingTeenWeight + IncomingAdultWeight + IncomingSeniorWeight).ToString("N0") + "%";

        // Births

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISection(kSection, kBirthGroup)]
        public bool EnableBirthControl { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsBirthControlDisabled))]
        [SettingsUISlider(min = 0, max = 500, step = 10, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kBirthGroup)]
        public int BirthRatePercent { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsBirthControlDisabled))]
        [SettingsUISection(kSection, kBirthGroup)]
        public bool UseBirthDailyCap { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsBirthDailyCapDisabled))]
        [SettingsUISlider(min = 10, max = 100000, step = 100, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kBirthGroup)]
        public int MaxBirthsPerDay { get; set; }

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsBirthControlDisabled))]
        [SettingsUISection(kSection, kBirthGroup)]
        public bool BirthsRespectChildTarget { get; set; }

        // Performance

        [SettingsUIDisableByCondition(typeof(CimRejuvenatorSetting), nameof(IsDisabled))]
        [SettingsUISlider(min = 8, max = 256, step = 8, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kPerformanceGroup)]
        public int SweepsPerDay { get; set; }

        // Statistics

        [SettingsUISection(kSection, kStatsGroup)]
        public string ResidentCount => PopulationManagementSystem.ResidentCount.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string ChildCount => PopulationManagementSystem.ChildCount.ToString("N0") + " (" + PopulationManagementSystem.ChildPercent.ToString("F1") + "%)";

        [SettingsUISection(kSection, kStatsGroup)]
        public string TeenCount => PopulationManagementSystem.TeenCount.ToString("N0") + " (" + PopulationManagementSystem.TeenPercent.ToString("F1") + "%)";

        [SettingsUISection(kSection, kStatsGroup)]
        public string AdultCount => PopulationManagementSystem.AdultCount.ToString("N0") + " (" + PopulationManagementSystem.AdultPercent.ToString("F1") + "%)";

        [SettingsUISection(kSection, kStatsGroup)]
        public string SeniorCount => PopulationManagementSystem.SeniorCount.ToString("N0") + " (" + PopulationManagementSystem.SeniorPercent.ToString("F1") + "%)";

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedLastSweep => PopulationManagementSystem.RejuvenatedLastSweep.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedToday => PopulationManagementSystem.RejuvenatedToday.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string RejuvenatedSession => PopulationManagementSystem.RejuvenatedSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string AgeConvertedLastSweep => PopulationManagementSystem.AgeConvertedLastSweep.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string AgeConvertedSession => PopulationManagementSystem.AgeConvertedSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string BirthsToday => PopulationFlowSystem.BirthsToday.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string BirthsSession => PopulationFlowSystem.BirthsSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string NewResidentsToday => PopulationFlowSystem.NewResidentsToday.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string NewResidentsSession => PopulationFlowSystem.NewResidentsSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string AppliedBirthRate => BirthRateControlSystem.LastAppliedBirthRatePercent.ToString("N0") + "%";

        [SettingsUISection(kSection, kStatsGroup)]
        public string ImmigrationStatus => ImmigrationControlSystem.Status;

        [SettingsUISection(kSection, kStatsGroup)]
        public string SweepsSession => PopulationManagementSystem.SweepsSession.ToString("N0");

        [SettingsUISection(kSection, kStatsGroup)]
        public string LastSimulationDay => PopulationManagementSystem.LastSimulationDay < 0
            ? "-"
            : PopulationManagementSystem.LastSimulationDay.ToString("N0");

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kStatsGroup)]
        public bool ResetStatistics
        {
            set
            {
                PopulationManagementSystem.ResetStatistics();
                PopulationFlowSystem.ResetStatistics();
            }
        }

        public bool IsDisabled() => !EnableMod;
        public bool IsRejuvenationDisabled() => !EnableMod || !EnableRejuvenation;
        public bool IsSeniorShareDisabled() => IsRejuvenationDisabled() || !KeepMinimumSeniorShare;
        public bool IsDemographicBalancerDisabled() => !EnableMod || !EnableDemographicBalancer;
        public bool IsImmigrationControlDisabled() => !EnableMod || !EnableImmigrationControl;
        public bool IsImmigrationDailyCapDisabled() => IsImmigrationControlDisabled() || !UseImmigrationDailyCap;
        public bool IsPopulationCeilingDisabled() => IsImmigrationControlDisabled() || !UsePopulationCeiling;
        public bool IsIncomingAgeMixDisabled() => IsImmigrationControlDisabled() || !ShapeNewResidentAges;
        public bool IsBirthControlDisabled() => !EnableMod || !EnableBirthControl;
        public bool IsBirthDailyCapDisabled() => IsBirthControlDisabled() || !UseBirthDailyCap;

        public sealed override void SetDefaults()
        {
            EnableMod = true;

            EnableRejuvenation = true;
            RejuvenationChance = 80;
            ResetAgeDays = 40;
            RestoreHealth = true;
            MaxRejuvenationsPerDay = 20000;
            MaxRejuvenationsPerSweep = 5000;
            KeepMinimumSeniorShare = false;
            MinimumSeniorPercent = 15;

            EnableDemographicBalancer = false;
            TargetChildPercent = 15;
            TargetTeenPercent = 10;
            TargetAdultPercent = 60;
            TargetSeniorPercent = 15;
            MaxAgeConversionsPerSweep = 5000;
            ProtectWorkersWhenBalancing = true;

            EnableImmigrationControl = false;
            ImmigrationIntensity = 100;
            UseImmigrationDailyCap = false;
            MaxNewResidentsPerDay = 10000;
            UsePopulationCeiling = false;
            PopulationCeiling = 500000;
            ShapeNewResidentAges = false;
            IncomingChildWeight = 15;
            IncomingTeenWeight = 10;
            IncomingAdultWeight = 65;
            IncomingSeniorWeight = 10;

            EnableBirthControl = false;
            BirthRatePercent = 100;
            UseBirthDailyCap = false;
            MaxBirthsPerDay = 5000;
            BirthsRespectChildTarget = false;

            SweepsPerDay = 64;
        }
    }
}
