using StardewModdingAPI;
using StardewModdingAPI.Events;
using sdv_helper.Detectors;
using sdv_helper.Settings;
using sdv_helper.Graphics;

namespace sdv_helper
{
    public class ModEntry : Mod
    {
        private static Detector detector;
        private static Settings.Settings settings;
        private static DrawingManager drawingManager;

        public override void Entry(IModHelper helper)
        {
            settings = new Settings.Settings(Helper);
            detector = new Detector();
            detector.AddDetector("NPC")
                .AddDetector("Object")
                .AddDetector("FarmAnimal");
            drawingManager = new DrawingManager(settings);

            Helper.Events.Display.Rendered += Display_Rendered;
            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.L)
            {
                settings.LoadSettings();
                drawingManager.SendHudMessage("Loaded settings from file", 4);
            }
            else if (e.Button == SButton.K)
            {
                settings.SaveSettings();
                drawingManager.SendHudMessage("Saved settings to file", 5);
            }
        }

        private void Display_Rendered(object sender, RenderedEventArgs e)
        {
            detector.Detect();
            drawingManager.LabelEntities(detector);
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                detector.SetLocation(e.NewLocation);
        }
    }
}