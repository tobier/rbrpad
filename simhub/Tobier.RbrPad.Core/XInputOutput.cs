using System.Collections.Generic;

namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// XInput enumeration, connection checks and rumble. Motor intensities are normalized
    /// floats (0..1), scaled to the native ushort range here so callers never see 0..65535.
    /// </summary>
    public static class XInputOutput
    {
        /// <summary>XInput supports user indices 0..3.</summary>
        public const int MaxControllers = 4;

        private const float MaxMotorSpeed = ushort.MaxValue;

        /// <summary>True if a controller is present in the given XInput slot (0..3).</summary>
        public static bool IsConnected(int userIndex)
        {
            if (userIndex < 0 || userIndex >= MaxControllers)
                return false;

            var state = new XInputInterop.XINPUT_STATE();
            return XInputInterop.XInputGetState((uint)userIndex, ref state) == XInputInterop.ErrorSuccess;
        }

        /// <summary>Returns the indices of all currently connected controllers.</summary>
        public static IReadOnlyList<int> GetConnectedIndices()
        {
            var connected = new List<int>();
            for (int i = 0; i < MaxControllers; i++)
            {
                if (IsConnected(i))
                    connected.Add(i);
            }
            return connected;
        }

        /// <summary>
        /// Sets motor intensities for a controller. <paramref name="left"/> is the heavy /
        /// low-frequency motor, <paramref name="right"/> is the fine / high-frequency motor.
        /// Values are clamped to 0..1. Safe to call on a disconnected slot (no-op result).
        /// </summary>
        public static void SetRumble(int userIndex, float left, float right)
        {
            if (userIndex < 0 || userIndex >= MaxControllers)
                return;

            var vibration = new XInputInterop.XINPUT_VIBRATION
            {
                wLeftMotorSpeed = ToMotorSpeed(left),
                wRightMotorSpeed = ToMotorSpeed(right),
            };
            XInputInterop.XInputSetState((uint)userIndex, ref vibration);
        }

        /// <summary>Stops both motors on the given controller.</summary>
        public static void Stop(int userIndex) => SetRumble(userIndex, 0f, 0f);

        private static ushort ToMotorSpeed(float value)
        {
            if (value <= 0f) return 0;
            if (value >= 1f) return (ushort)MaxMotorSpeed;
            return (ushort)(value * MaxMotorSpeed);
        }
    }
}