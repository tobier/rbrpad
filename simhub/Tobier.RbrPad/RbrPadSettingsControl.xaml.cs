using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tobier.RbrPad.Core;

namespace Tobier.RbrPad
{
    /// <summary>
    /// Settings UI. Code-behind rather than data binding because settings are POCO fields
    /// (WPF binds to properties) and each change is pushed to the plugin immediately.
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

        public RbrPadSettingsControl(RbrPadPlugin plugin) : this()
        {
            _plugin = plugin;

            _initializing = true;
            EnabledCheck.IsChecked = _plugin.Settings.Rumble.Enabled;
            IntensitySlider.Value = _plugin.Settings.Rumble.MasterIntensity;
            UpdateIntensityLabel();
            _initializing = false;

            RefreshControllers();
        }

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

        private void UpdateIntensityLabel()
        {
            IntensityValue.Text = ((int)(IntensitySlider.Value * 100)).ToString(CultureInfo.InvariantCulture) + "%";
        }

        private void ControllerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initializing || ControllerCombo.SelectedItem is null)
                return;

            _plugin.Settings.ControllerIndex = ((ControllerOption)ControllerCombo.SelectedItem).Index;
            _plugin.SaveSettings();
            UpdateWarning();
        }

        private void EnabledCheck_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_initializing)
                return;

            _plugin.Settings.Rumble.Enabled = EnabledCheck.IsChecked == true;
            _plugin.SaveSettings();
        }

        private void IntensitySlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateIntensityLabel();

            if (_initializing)
                return;

            _plugin.Settings.Rumble.MasterIntensity = (float)IntensitySlider.Value;
            _plugin.SaveSettings();
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
