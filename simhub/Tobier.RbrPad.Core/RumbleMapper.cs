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

            // Hum on the fine (right) motor that rises with engine RPM.
            float rpmFraction = inputs.MaxRpm > 0f ? Clamp01(inputs.Rpm / inputs.MaxRpm) : 0f;
            float right = rpmFraction * 0.5f;
            float left = 0f;

            float master = Clamp01(settings.MasterIntensity);
            return new RumbleOutput(left * master, right * master);
        }

        private static float Clamp01(float value)
        {
            if (value <= 0f) return 0f;
            if (value >= 1f) return 1f;
            return value;
        }
    }
}