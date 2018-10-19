using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.RFID.NFC
{
    public class Tag : RFID.Tag
    {
        public Tag(byte[] uid) : base(uid)
        {
        }

        public override Stream Memory => throw new NotImplementedException();
    }
}
