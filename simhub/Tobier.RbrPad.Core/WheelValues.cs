namespace Tobier.RbrPad.Core
{
    /// <summary>One value per wheel.</summary>
    public struct WheelValues
    {
        public float FrontLeft;
        public float FrontRight;
        public float RearLeft;
        public float RearRight;

        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return FrontLeft;
                    case 1: return FrontRight;
                    case 2: return RearLeft;
                    default: return RearRight;
                }
            }
        }

        public const int Count = 4;
    }
}
