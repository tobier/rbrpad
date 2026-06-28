namespace Tobier.RbrPad.Core
{
    /// <summary>
    /// Result of the mapping: normalized intensities (0..1) for the two motors.
    /// <see cref="Left"/> = heavy / low-frequency, <see cref="Right"/> = fine / high-frequency.
    /// </summary>
    public readonly struct RumbleOutput
    {
        public readonly float Left;
        public readonly float Right;

        public RumbleOutput(float left, float right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>Both motors off.</summary>
        public static RumbleOutput None => new RumbleOutput(0f, 0f);
    }
}