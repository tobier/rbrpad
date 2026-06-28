using System.Runtime.InteropServices;

namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Raw XInput P/Invoke surface, kept in one place so nothing else touches native signatures.
    /// xinput1_4.dll ships with Windows 8+ (xinput1_3.dll for Windows 7).
    /// </summary>
    internal static class XInputInterop
    {
        internal const uint ErrorSuccess = 0;

        [DllImport("xinput1_4.dll")]
        internal static extern uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);

        [DllImport("xinput1_4.dll")]
        internal static extern uint XInputSetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

        [StructLayout(LayoutKind.Sequential)]
        internal struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XINPUT_VIBRATION
        {
            public ushort wLeftMotorSpeed;   // low-frequency / heavy motor
            public ushort wRightMotorSpeed;  // high-frequency / fine motor
        }
    }
}