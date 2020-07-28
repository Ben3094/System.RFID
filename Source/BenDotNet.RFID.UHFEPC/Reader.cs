namespace BenDotNet.RFID.UHFEPC
{
    public abstract class Reader : RFID.Reader
    {
        public const float MIN_ALLOWED_FREQUENCY = 865 * 10 ^ 6; //in Hertz
        public override float MinAllowedFrequency => MIN_ALLOWED_FREQUENCY;
        public const float MAX_ALLOWED_FREQUENCY = 928 * 10 ^ 6; //in Hertz
        public override float MaxAllowedFrequency => MAX_ALLOWED_FREQUENCY;

        public const float MAX_ALLOWED_POWER = 30; //in decibel-milliwatt
        public override float MaxAllowedPower => MAX_ALLOWED_POWER;
    }
}
