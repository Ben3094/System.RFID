using System;
using System.Collections.Generic;

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
    }
}
