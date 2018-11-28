using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID.UHFEPC
{
    public class ISO7816Tag : Tag
    {
        public sealed override ISO15693ClassIdentifier ISO15693Class => ISO15693ClassIdentifier.ISO7816;

        public ISO7816Tag(byte[] uid) : base(uid) { }
    }
}
