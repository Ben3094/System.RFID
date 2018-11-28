using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID.UHFEPC
{
    public class ISO14816Tag : Tag
    {
        public sealed override ISO15693ClassIdentifier ISO15693Class => ISO15693ClassIdentifier.ISO14816;

        public ISO14816Tag(byte[] uid) : base(uid) { }
    }
}
