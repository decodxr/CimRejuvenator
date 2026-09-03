using System;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace CimRejuvenator
{
    public sealed class Mod : IMod
    {
        public const string ModId = "CimRejuvenator";
        public const string Version = "0.6.1";

        public static readonly ILog Log = LogManager
            .GetLogger(ModId)
            .SetShowsErrorsInUI(false);

        public static CimRejuvenatorSetting Setting { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"Loading Cim Rejuvenator v{Version}");

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                Log.Info($"Loaded from {asset.path}");
            }

            Setting = new CimRejuvenatorSetting(this);
            RegisterLocalization();
            AssetDatabase.global.LoadSettings(ModId, Setting, new CimRejuvenatorSetting(this));
            Setting.RegisterInOptionsUI();

            updateSystem.UpdateAt<PopulationManagementSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<PopulationFlowSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<PopulationTrendSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<BirthRateControlSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<ImmigrationControlSystem>(SystemUpdatePhase.GameSimulation);

            Log.Info("Registered population management, flow, trend, birth-rate, and immigration systems.");
        }

        private static void RegisterLocalization()
        {
            var manager = GameManager.instance.localizationManager;

            foreach (var localeId in manager.GetSupportedLocales())
            {
                if (localeId.StartsWith("pt", StringComparison.OrdinalIgnoreCase))
                {
                    manager.AddSource(localeId, new LocalePTBR(Setting));
                    Log.Info($"Registered Portuguese localization for {localeId}.");
                }
                else
                {
                    manager.AddSource(localeId, new LocaleEN(Setting));
                    manager.AddSource(localeId, new LocaleDirectEN(Setting));
                }
            }
        }

        public void OnDispose()
        {
            Log.Info("Disposing Cim Rejuvenator");

            if (Setting != null)
            {
                Setting.UnregisterInOptionsUI();
                Setting = null;
            }
        }
    }
}
