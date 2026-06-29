namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Mapper tuning only — no host/device config (controller selection lives in the plugin's
    /// settings). Plain JSON-serializable POCO.
    /// </summary>
    public class RumbleSettings
    {
        /// <summary>Master on/off. When false the mapper returns <see cref="RumbleOutput.None"/>.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Master intensity multiplier applied to both motors (0..1).</summary>
        public float MasterIntensity { get; set; } = 1.0f;

        /// <summary>Per-effect toggle for surface roughness.</summary>
        public bool RoughnessEnabled { get; set; } = true;

        /// <summary>Surface-roughness contribution to the heavy motor (0..1+).</summary>
        public float RoughnessGain { get; set; } = 1.0f;

        /// <summary>Response exponent for roughness: &gt;1 keeps light chatter quiet, lets bumps stand out.</summary>
        public float RoughnessCurve { get; set; } = 2.0f;

        /// <summary>Damper velocity (m/s) below which roughness is silent.</summary>
        public float DamperVelocityFloor { get; set; } = 0.1f;

        /// <summary>Damper velocity (m/s) that produces full roughness.</summary>
        public float DamperVelocityFull { get; set; } = 2.0f;

        /// <summary>Wheel load (N) below which the roughness gate is fully closed.</summary>
        public float LoadGateLow { get; set; } = 300f;

        /// <summary>Wheel load (N) at or above which the roughness gate is fully open.</summary>
        public float LoadGateHigh { get; set; } = 2000f;
    }
}