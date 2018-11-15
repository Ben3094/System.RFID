using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Range;

namespace System.RFID.NFC
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
        public const float MAX_ALLOWED_FREQUENCY = 13567 * 10 ^ 3;
        public override Range<float> AllowedFrequencies => new Range<float>(MIN_ALLOWED_FREQUENCY, MAX_ALLOWED_FREQUENCY);
    }
}
