using BenDotNet.Numerics;

namespace BenDotNet.RFID.UHFEPC
{
    public abstract class Reader : RFID.Reader
    {
        public const float MIN_ALLOWED_FREQUENCY = 865 * 10 ^ 6; //in Hertz
        public const float MAX_ALLOWED_FREQUENCY = 928 * 10 ^ 6; //in Hertz
        public override Range<float> AllowedFrequencies => new Range<float>(MIN_ALLOWED_FREQUENCY, MAX_ALLOWED_FREQUENCY);

        public const float MIN_ALLOWED_POWER = 0; //in decibel-milliwatt
        public const float MAX_ALLOWED_POWER = 30; //in decibel-milliwatt
        public override Range<float> AllowedPowers => new Range<float>(MIN_ALLOWED_POWER, MAX_ALLOWED_POWER);
    }
}
