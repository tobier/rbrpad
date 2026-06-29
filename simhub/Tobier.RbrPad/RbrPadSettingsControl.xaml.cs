using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Tobier.RbrPad.Core;

namespace Tobier.RbrPad
{
    /// <summary>
    /// Settings UI. The tuning sections bind to <see cref="RbrPadPluginSettings.Rumble"/>; the
    /// controller picker is driven from code because it reflects live device state.
    /// </summary>
    public partial class RbrPadSettingsControl : UserControl
    {
        private readonly RbrPadPlugin _plugin;

        /// <summary>Suppresses change handlers while we populate controls programmatically.</summary>
        private bool _initializing;

        /// <summary>One selectable XInput slot.</summary>
        private sealed class ControllerOption
        {
            public int Index { get; set; }
            public string Display { get; set; }
        }

        public RbrPadSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>Polls the plugin's live telemetry for the readout. UI-thread, runs only while shown.</summary>
        private readonly DispatcherTimer _readoutTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

        /// <summary>Decaying peak/valley holds so the readout is legible instead of flickering at 60 Hz.</summary>
        private float _damperPeak, _loadValley, _loadPeak, _outputPeak;

        public RbrPadSettingsControl(RbrPadPlugin plugin) : this()
        {
            _plugin = plugin;
            Root.DataContext = _plugin.Settings;

            RefreshControllers();

            _readoutTimer.Tick += ReadoutTimer_Tick;
            Loaded += (s, e) => _readoutTimer.Start();
            Unloaded += (s, e) => _readoutTimer.Stop();
        }

        /// <summary>Persists whenever a bound tuning control writes its value back to the settings.</summary>
        private void Settings_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            _plugin.SaveSettings();
        }

        private void ReadoutTimer_Tick(object sender, EventArgs e)
        {
            RumbleInputs inputs = _plugin.LatestInputs;
            if (inputs == null)
            {
                _damperPeak = _loadValley = _loadPeak = _outputPeak = 0f;
                LiveDamperValue.Text = "—";
                LiveLoadValue.Text = "—";
                LiveOutputValue.Text = "—";
                return;
            }

            float maxDamper = 0f, minLoad = float.MaxValue, maxLoad = 0f;
            for (int i = 0; i < WheelValues.Count; i++)
            {
                float v = Math.Abs(inputs.DamperVelocity[i]);
                if (v > maxDamper) maxDamper = v;
                float load = Math.Max(0f, -inputs.StrutForce[i]);
                if (load < minLoad) minLoad = load;
                if (load > maxLoad) maxLoad = load;
            }

            _damperPeak = PeakHold(_damperPeak, maxDamper);
            _loadPeak = PeakHold(_loadPeak, maxLoad);
            _loadValley = ValleyHold(_loadValley, minLoad);
            _outputPeak = PeakHold(_outputPeak, _plugin.LatestHeavyOutput);

            LiveDamperValue.Text = _damperPeak.ToString("0.00", CultureInfo.InvariantCulture) + " m/s";
            LiveLoadValue.Text = _loadValley.ToString("0", CultureInfo.InvariantCulture) + "–" +
                                 _loadPeak.ToString("0", CultureInfo.InvariantCulture) + " N";
            LiveOutputValue.Text = (_outputPeak * 100f).ToString("0", CultureInfo.InvariantCulture) + "%";
        }

        /// <summary>Holds the recent maximum, decaying toward the current value so spikes linger briefly.</summary>
        private static float PeakHold(float held, float current) => Math.Max(current, held * 0.9f);

        /// <summary>Holds the recent minimum (dips over crests), drifting back up toward the current value.</summary>
        private static float ValleyHold(float held, float current) =>
            current < held ? current : held + (current - held) * 0.1f;

        /// <summary>Re-enumerates slots 0..3, marking which are connected, and reselects the saved one.</summary>
        private void RefreshControllers()
        {
            _initializing = true;

            var options = new List<ControllerOption>();
            for (int i = 0; i < XInputOutput.MaxControllers; i++)
            {
                bool connected = XInputOutput.IsConnected(i);
                options.Add(new ControllerOption
                {
                    Index = i,
                    Display = $"Slot {i} — {(connected ? "connected" : "not connected")}",
                });
            }

            ControllerCombo.ItemsSource = options;

            int selected = _plugin.Settings.ControllerIndex;
            if (selected < 0 || selected >= options.Count)
                selected = 0;
            ControllerCombo.SelectedIndex = selected;

            _initializing = false;

            UpdateWarning();
        }

        /// <summary>Shows the warning when the selected slot has no controller present.</summary>
        private void UpdateWarning()
        {
            bool connected = XInputOutput.IsConnected(_plugin.Settings.ControllerIndex);
            NotConnectedWarning.Visibility = connected
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        private void ControllerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initializing || ControllerCombo.SelectedItem is null)
                return;

            _plugin.Settings.ControllerIndex = ((ControllerOption)ControllerCombo.SelectedItem).Index;
            _plugin.SaveSettings();
            UpdateWarning();
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshControllers();
        }

        /// <summary>Brief rumble pulse on the selected slot so the user can confirm the right pad.</summary>
        private async void Test_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int index = _plugin.Settings.ControllerIndex;
            if (!XInputOutput.IsConnected(index))
            {
                UpdateWarning();
                return;
            }

            // Ramp both motors up, then down.
            for (int p = 0; p <= 100; p += 10)
            {
                XInputOutput.SetRumble(index, p / 100f, p / 100f);
                await Task.Delay(20);
            }
            for (int p = 100; p >= 0; p -= 10)
            {
                XInputOutput.SetRumble(index, p / 100f, p / 100f);
                await Task.Delay(20);
            }
            XInputOutput.Stop(index);
        }
    }
}
