using System;
using System.Collections.Generic;
using System.Linq;

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

        //public const float MIN_ALLOWED_FREQUENCY = 860 * 10 ^ 6;
        //public const float MAX_ALLOWED_FREQUENCY = 960 * 10 ^ 6;
        //public override float[] AllowedFrequencies => Enumerable.Range((int)MIN_ALLOWED_FREQUENCY, (int)MAX_ALLOWED_FREQUENCY).Cast<float>().ToArray();
    }
}
