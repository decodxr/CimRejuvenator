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
        public const string Version = "0.3.0";

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
            Setting.RegisterInOptionsUI();

            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Setting));
            AssetDatabase.global.LoadSettings(ModId, Setting, new CimRejuvenatorSetting(this));

            updateSystem.UpdateAt<PopulationManagementSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<PopulationFlowSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<BirthRateControlSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<ImmigrationControlSystem>(SystemUpdatePhase.GameSimulation);
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
