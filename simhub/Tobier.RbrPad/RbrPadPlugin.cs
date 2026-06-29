using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Diagnostics;
using System.Windows.Media;
using Tobier.RbrPad.Core;

namespace Tobier.RbrPad
{
    [PluginDescription("A plugin for Richard Burns Rally that aims to improve the gamepad driving experience")]
    [PluginAuthor("Tobias Eriksson")]
    [PluginName("RBR Gamepad Companion")]
    public class RbrPadPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        private const string SettingsKey = "GeneralSettings";

        /// <summary>Last XInput slot we sent rumble to, or -1 if none (so we can stop it cleanly).</summary>
        private int _lastDrivenIndex = -1;

        private readonly RumbleMapper _mapper = new RumbleMapper();

        /// <summary>Measures real time between mapped frames so transient envelopes decay correctly.</summary>
        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();

        public RbrPadPluginSettings Settings;

        /// <summary>Most recent telemetry fed to the mapper, or null when no game data is flowing. For the UI readout.</summary>
        public RumbleInputs LatestInputs { get; private set; }

        /// <summary>Most recent heavy-motor level actually sent (0..1). For the UI readout.</summary>
        public float LatestHeavyOutput { get; private set; }

        public PluginManager PluginManager { get; set; }

        /// <summary>Left-menu icon. Must be 24x24 and readable in black and white.</summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        public string LeftMenuTitle => "RBR Gamepad";

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("RBR Gamepad Companion: starting");
            Settings = this.ReadCommonSettings(SettingsKey, () => new RbrPadPluginSettings());
        }

        /// <summary>On the critical path — must be fast and must not throw.</summary>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!data.GameRunning || data.NewData == null)
            {
                StopRumble();
                return;
            }

            if (data.NewData == data.OldData)
            {
                return; // Do nothing if data didn't change
            }

            // Only RBR produces RBRData — for any other game, leave the pad alone.
            if (!(data.NewData.GetRawDataObject() is RBR.RBRData raw))
            {
                return;
            }

            float dt = (float)Math.Min(_frameTimer.Elapsed.TotalSeconds, 0.1);
            _frameTimer.Restart();

            RumbleInputs inputs = RbrTelemetryAdapter.ToInputs(raw);
            RumbleOutput output = _mapper.Update(inputs, Settings.Rumble, dt);
            LatestInputs = inputs;
            LatestHeavyOutput = output.Left;

            int index = Settings.ControllerIndex;
            // If the user switched controllers mid-drive, stop the old one so it doesn't stick.
            if (_lastDrivenIndex >= 0 && _lastDrivenIndex != index)
                XInputOutput.Stop(_lastDrivenIndex);

            XInputOutput.SetRumble(index, output.Left, output.Right);
            _lastDrivenIndex = index;
        }

        public void End(PluginManager pluginManager)
        {
            StopRumble();
            SaveSettings();
        }

        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new RbrPadSettingsControl(this);
        }

        /// <summary>Persists settings now (the UI calls this on change; End also saves).</summary>
        public void SaveSettings()
        {
            this.SaveCommonSettings(SettingsKey, Settings);
        }

        /// <summary>Zero the motors on whatever slot we last drove, exactly once.</summary>
        private void StopRumble()
        {
            if (_lastDrivenIndex < 0)
                return;

            XInputOutput.Stop(_lastDrivenIndex);
            _lastDrivenIndex = -1;
            LatestInputs = null;
            LatestHeavyOutput = 0f;
        }
    }
}
