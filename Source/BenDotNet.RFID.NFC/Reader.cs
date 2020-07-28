using System;

namespace BenDotNet.RFID.NFC
{
    public abstract class Reader : RFID.Reader
    {        
        protected override void ConnectionMethod()
        {
            throw new NotImplementedException();
        }

        protected override void DisconnectionMethod()
        {
            throw new NotImplementedException();
        }

        public const float MIN_ALLOWED_FREQUENCY = 13533 * 10 ^ 3;
        public override float MinAllowedFrequency => MIN_ALLOWED_FREQUENCY;
        public const float MAX_ALLOWED_FREQUENCY = 13567 * 10 ^ 3;
        public override float MaxAllowedFrequency => MAX_ALLOWED_FREQUENCY;
    }
}
