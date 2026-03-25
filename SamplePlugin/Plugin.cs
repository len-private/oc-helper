using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;
using SamplePlugin.Services;

namespace SamplePlugin;

/// <summary>
/// Main plugin class for OC-Helper automation.
/// 
/// This class is responsible for:
/// - Initializing all services
/// - Managing configuration
/// - Hooking into Dalamud events
/// - Coordinating UI updates
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    private const string CommandName = "/oc";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("OCHelper");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    // Automation Services
    private TreasureHuntingService TreasureHuntingService { get; init; }
    private CombatAutomationService CombatAutomationService { get; init; }
    private PerformanceTrackerService PerformanceTrackerService { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // You might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        // Initialize automation services
        TreasureHuntingService = new TreasureHuntingService(Configuration, ObjectTable, ClientState, Log);
        CombatAutomationService = new CombatAutomationService(Configuration, ObjectTable, ClientState, Log);
        PerformanceTrackerService = new PerformanceTrackerService(Configuration, Log);

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle OC-Helper main UI"
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Hook into framework update to run automation logic every frame
        Framework.Update += OnFrameworkUpdate;

        Log.Information($"===OC-Helper Plugin Loaded===");
    }

    /// <summary>
    /// Called every frame. Updates all automation services.
    /// </summary>
    private void OnFrameworkUpdate(IFramework framework)
    {
        try
        {
            TreasureHuntingService.Update();
            CombatAutomationService.Update();
            PerformanceTrackerService.CheckAutoReset();
        }
        catch (Exception ex)
        {
            Log.Error($"Error during framework update: {ex}");
        }
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        Framework.Update -= OnFrameworkUpdate;
        
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        // Dispose services
        TreasureHuntingService?.Dispose();
        CombatAutomationService?.Dispose();
        PerformanceTrackerService?.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();

    // Public accessors for services (UI windows need them)
    public TreasureHuntingService GetTreasureService() => TreasureHuntingService;
    public CombatAutomationService GetCombatService() => CombatAutomationService;
    public PerformanceTrackerService GetPerformanceService() => PerformanceTrackerService;
}
