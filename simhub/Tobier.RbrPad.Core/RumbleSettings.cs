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

        /// <summary>Per-effect toggle for landing/impact hits.</summary>
        public bool ImpactEnabled { get; set; } = true;

        /// <summary>Impact contribution to the heavy motor (0..1+).</summary>
        public float ImpactGain { get; set; } = 1.0f;

        /// <summary>Body jolt (g) below which no impact fires. Above normal braking/cornering (~1 g).</summary>
        public float ImpactThreshold { get; set; } = 1.2f;

        /// <summary>Body jolt (g) that produces a full-strength impact.</summary>
        public float ImpactFull { get; set; } = 2.5f;

        /// <summary>Impact envelope decay time constant (s) — how long a hit rings out.</summary>
        public float ImpactDecay { get; set; } = 0.10f;
    }
}