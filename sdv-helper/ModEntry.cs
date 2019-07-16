using StardewModdingAPI;
using StardewModdingAPI.Events;
using sdv_helper.Detectors;
using sdv_helper.Config;
using sdv_helper.Labels;
using StardewValley;
using sdv_helper.Menu;

namespace sdv_helper
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        private static Detector detector;
        private static Settings settings;
        private static LabelDrawingManager drawingManager;
        private static ConfigMenu configMenu;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            settings = new Settings(Helper);
            configMenu = new ConfigMenu(settings);
            drawingManager = new LabelDrawingManager(settings);
            detector = new Detector(settings);
            detector.AddDetector("NPC")
                .AddDetector("Object")
                .AddDetector("FarmAnimal")
                .AddDetector("WaterEntity");

            Helper.Events.Display.RenderingHud += OnRenderingHud;
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            detector.Detect();
            drawingManager.LabelEntities(detector);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == settings.LoadKey)
            {
                settings.LoadSettings();
                drawingManager.SendHudMessage("Loaded settings from file", 4);
            }
            else if (e.Button == settings.MenuKey)
                Game1.activeClickableMenu = configMenu;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                detector.SetLocation(e.NewLocation);
        }
    }
}