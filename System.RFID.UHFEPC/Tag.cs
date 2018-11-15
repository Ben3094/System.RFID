using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.RFID.UHFEPC
{
    public abstract class Tag : RFID.Tag
    {
        public const string WRONG_UID_INITIATION = "Not a genuine {0} EPC tag";
        public enum ISO15693ClassIdentifier
        {
            ISO7816 = 0xE0,
            ISO14816 = 0xE1,
            GS1 = 0xE2
        }
        public abstract ISO15693ClassIdentifier ISO15693Class { get; }
        public Tag(byte[] uid) : base(uid)
        {
            if (uid[0] != (byte)this.ISO15693Class)
                throw new ArgumentException(String.Format(Tag.WRONG_UID_INITIATION, this.ISO15693Class));

            //EPC verification
            //TODO

        }

        public const byte MEMORY_BANK_MAX_VALUE = 0b11;
        public enum MemoryBank
        {
            Reserved = 0b00,
            EPC = 0b01,
            TID = 0b10,
            User = 0b11
        }

        public byte[] Execute(byte GS1EPCCommand, bool[] arguments)
        {
            this.DetectingAntennas[0].ContainerReader.Execute(this, arguments);
        }
        public override Stream Memory => throw new NotImplementedException();
    }
}
