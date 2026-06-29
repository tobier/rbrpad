namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Normalized telemetry for the mapper.
    /// </summary>
    public class RumbleInputs
    {
        /// <summary>Whether the game is running and producing telemetry.</summary>
        public bool GameRunning;

        /// <summary>Per-wheel damper piston velocity (m/s).</summary>
        public WheelValues DamperVelocity;

        /// <summary>Per-wheel strut force (N, negative = load).</summary>
        public WheelValues StrutForce;

        /// <summary>Vertical (heave) acceleration of the car body (g, gravity-compensated so ≈0 when steady).</summary>
        public float HeaveAccel;
    }
}