using RBR;
using Tobier.RbrPad.Core;

namespace Tobier.RbrPad
{
    /// <summary>
    /// Builds <see cref="RumbleInputs"/> from RBR's raw <see cref="RBRData"/> telemetry.
    /// </summary>
    internal static class RbrTelemetryAdapter
    {
        public static RumbleInputs ToInputs(RBRData raw)
        {
            var car = raw.NGPTelemetry.car;

            return new RumbleInputs
            {
                GameRunning = true,
                DamperVelocity = new WheelValues
                {
                    FrontLeft = car.suspensionLF.damper.pistonVelocity,
                    FrontRight = car.suspensionRF.damper.pistonVelocity,
                    RearLeft = car.suspensionLB.damper.pistonVelocity,
                    RearRight = car.suspensionRB.damper.pistonVelocity,
                },
                StrutForce = new WheelValues
                {
                    FrontLeft = car.suspensionLF.strutForce,
                    FrontRight = car.suspensionRF.strutForce,
                    RearLeft = car.suspensionLB.strutForce,
                    RearRight = car.suspensionRB.strutForce,
                },
            };
        }
    }
}
