namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Normalized telemetry for the mapper.
    /// </summary>
    public class RumbleInputs
    {
        /// <summary>Whether the game is running and producing telemetry.</summary>
        public bool GameRunning;

        /// <summary>Engine speed, revolutions per minute.</summary>
        public float Rpm;

        /// <summary>Engine redline / max RPM, used to normalize <see cref="Rpm"/>.</summary>
        public float MaxRpm;
    }
}