using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BenDotNet.RFID.UHFEPC
{
    public class ISO7816Tag : Tag
    {
        public sealed override ISO15693ClassIdentifier ISO15693Class => ISO15693ClassIdentifier.ISO7816;

        public override Stream Memory => throw new NotImplementedException();

        public ManufacturerIdentifierEnum KnownDesignerIdentifier => (ManufacturerIdentifierEnum)this.ManufacturerIdentifier;

        public ISO7816Tag(byte[] uid) : base(uid)
        {
            this.ManufacturerIdentifier = uid[MANUFACTURER_IDENTIFIER_BYTE_ADDRESS];
            this.SerialNumber = new byte[SERIAL_NUMBER_BYTE_ADDRESS_SPAN];
            Array.Copy(uid, SERIAL_NUMBER_BYTE_START_INDEX, this.SerialNumber, 0, SERIAL_NUMBER_BYTE_ADDRESS_SPAN);
        }

        public const byte MANUFACTURER_IDENTIFIER_BYTE_ADDRESS = 1;
        public readonly byte ManufacturerIdentifier; //TODO: Merge this with BitArray from GS1 tags

        public enum ManufacturerIdentifierEnum
        {
            AMS = 0x36
        }

        public const byte SERIAL_NUMBER_BYTE_START_INDEX = 2;
        public const byte SERIAL_NUMBER_BYTE_ADDRESS_SPAN = 3;
        public readonly byte[] SerialNumber;
    }
}
