using System;

namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Map telemetry input with settings to create motor output.
    /// </summary>
    public static class RumbleMapper
    {
        public static RumbleOutput Map(RumbleInputs inputs, RumbleSettings settings)
        {
            if (settings == null || !settings.Enabled || inputs == null || !inputs.GameRunning)
                return RumbleOutput.None;

            float left = settings.RoughnessEnabled ? SurfaceRoughness(inputs, settings) : 0f;
            float right = 0f;

            float master = Clamp01(settings.MasterIntensity);
            return new RumbleOutput(Clamp01(left) * master, Clamp01(right) * master);
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
