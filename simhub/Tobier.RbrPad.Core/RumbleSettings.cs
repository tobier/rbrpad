namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Mapper tuning only — no host/device config (controller selection lives in the plugin's
    /// settings). Plain JSON-serializable POCO.
    /// </summary>
    public class RumbleSettings
    {
        /// <summary>Master on/off. When false the mapper returns <see cref="RumbleOutput.None"/>.</summary>
        public bool Enabled = true;

        /// <summary>Master intensity multiplier applied to both motors (0..1).</summary>
        public float MasterIntensity = 1.0f;
    }
}