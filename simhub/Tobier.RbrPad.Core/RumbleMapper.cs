using System;

namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Map telemetry input with settings to create motor output. Stateful: transient effects
    /// (impacts) carry an envelope between frames, so call <see cref="Update"/> once per frame with dt.
    /// </summary>
    public class RumbleMapper
    {
        /// <summary>Decaying level of the current impact hit (0..1).</summary>
        private float _impactEnvelope;

        public RumbleOutput Update(RumbleInputs inputs, RumbleSettings settings, float dt)
        {
            if (settings == null || !settings.Enabled || inputs == null || !inputs.GameRunning)
            {
                _impactEnvelope = 0f;
                return RumbleOutput.None;
            }

            float roughness = Clamp01(settings.RoughnessEnabled ? SurfaceRoughness(inputs, settings) : 0f);
            float impact = Clamp01(settings.ImpactEnabled ? Impact(inputs, settings, dt) : 0f);

            // Layer by priority into the remaining headroom, highest first. The transient impact
            // ducks the continuous roughness beneath it; the sum can never exceed full, so the
            // motor doesn't saturate and a hit stays legible over a loud surface.
            float left = 0f;
            left = Layer(left, impact);
            left = Layer(left, roughness);
            float right = 0f;

            float master = Clamp01(settings.MasterIntensity);
            return new RumbleOutput(left * master, right * master);
        }

        /// <summary>
        /// Per-wheel damper piston velocity, gated by wheel load so a wheel going light over a
        /// crest or jump stops contributing. Averaged across the four wheels onto the heavy motor.
        /// </summary>
        private static float SurfaceRoughness(RumbleInputs inputs, RumbleSettings s)
        {
            float velRange = s.DamperVelocityFull - s.DamperVelocityFloor;
            if (velRange <= 0f)
                return 0f;

            float sum = 0f;
            for (int i = 0; i < WheelValues.Count; i++)
            {
                float load = Math.Max(0f, -inputs.StrutForce[i]);
                float gate = SmoothStep(s.LoadGateLow, s.LoadGateHigh, load);
                float rough = Clamp01((Math.Abs(inputs.DamperVelocity[i]) - s.DamperVelocityFloor) / velRange);
                if (s.RoughnessCurve > 0f && s.RoughnessCurve != 1f)
                    rough = (float)Math.Pow(rough, s.RoughnessCurve);
                sum += gate * rough;
            }

            return (sum / WheelValues.Count) * s.RoughnessGain;
        }

        /// <summary>
        /// Heave-acceleration spikes (landings, hard compressions) drive a short pulse on the heavy
        /// motor. The raw spike is a frame or two, so an attack-then-decay envelope makes it land.
        /// Ungated by wheel load — a landing fires exactly as the wheels reload.
        /// </summary>
        private float Impact(RumbleInputs inputs, RumbleSettings s, float dt)
        {
            float range = s.ImpactFull - s.ImpactThreshold;
            float hit = range > 0f
                ? Clamp01((Math.Abs(inputs.HeaveAccel) - s.ImpactThreshold) / range)
                : 0f;

            float decay = s.ImpactDecay > 0f ? (float)Math.Exp(-dt / s.ImpactDecay) : 0f;
            _impactEnvelope = Math.Max(hit, _impactEnvelope * decay);

            return _impactEnvelope * s.ImpactGain;
        }

        /// <summary>Adds a lower-priority effect into the headroom left by what's already accumulated.</summary>
        private static float Layer(float accumulated, float effect) => accumulated + effect * (1f - accumulated);

        private static float SmoothStep(float edge0, float edge1, float x)
        {
            if (edge1 <= edge0)
                return x >= edge1 ? 1f : 0f;

            float t = Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static float Clamp01(float value)
        {
            if (value <= 0f) return 0f;
            if (value >= 1f) return 1f;
            return value;
        }
    }
}
