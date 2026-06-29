using Tobier.RbrPad.Core;

namespace Tobier.RbrPad
{
    /// <summary>
    /// Persisted plugin settings: the selected XInput controller plus the Core tuning block.
    /// The controller index is host config, so it lives here rather than in <see cref="RumbleSettings"/>.
    /// </summary>
    public class RbrPadPluginSettings
    {
        /// <summary>Selected XInput user index (0..3) to send rumble to.</summary>
        public int ControllerIndex { get; set; } = 0;

        /// <summary>Mapping tuning (master enable, intensity).</summary>
        public RumbleSettings Rumble { get; set; } = new RumbleSettings();
    }
}
