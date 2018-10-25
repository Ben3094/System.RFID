using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.RFID.UHFEPC
{
    public class Tag : RFID.Tag
    {
        public const string WRONG_UID_INITIATION = "Not a genuine EPC tag";
        public enum ISO15693ClassIdentifier
        {
            ISO7816 = 0xE0,
            ISO14816 = 0xE1,
            GS1 = 0xE2
        }
        public Tag(byte[] uid) : base(uid)
        {
            switch (uid[0])
            {
                case (byte)ISO15693ClassIdentifier.ISO7816:
                case (byte)ISO15693ClassIdentifier.ISO14816:
                case (byte)ISO15693ClassIdentifier.GS1:
                    break;

                default:
                    throw new ArgumentException(WRONG_UID_INITIATION);
            }
        }

        public const byte MEMORY_BANK_MAX_VALUE = 0b11;
        public enum MemoryBank
        {
            Reserved = 0b00,
            EPC = 0b01,
            TID = 0b10,
            User = 0b11
        }

        //public byte[] Execute(byte GS1EPCCommand, bool[] arguments)
        //{
        //    this.DetectingAntennas[0].ContainerReader.Execute(this, arguments);
        //}
        public override Stream Memory => throw new NotImplementedException();
    }

    public static partial class TIDParsing
    {
        public const byte TID_WORD_LENGTH = 4;

        public enum ISO15693ClassIdentifier
        {
            ISO7816 = 0xE0,
            ISO14816 = 0xE1,
            GS1 = 0xE2
        }
        public static byte GetISO15693ClassIdentifier(this byte[] TID) { return TID[0]; }

        public const byte EXTENDED_TID_START_INDEX = 0x08;
        public const byte EXTENDED_TID_MAX_VALUE = 0b1;
        public static bool IsTIDExtended(this byte[] TID) { return (TID[1] & EXTENDED_TID_MAX_VALUE) > 0; }

        public const byte SECURITY_FLAG_START_INDEX = 0x09;
        public const byte SECURITY_FLAG_MAX_VALUE = 0b1;
        public static bool HasSecurityFlag(this byte[] TID) { return ((TID[1] >> (SECURITY_FLAG_START_INDEX - 8)) & SECURITY_FLAG_MAX_VALUE) > 0; }
        
        public const byte FILE_FLAG_START_INDEX = 0x0A;
        public const byte FILE_FLAG_MAX_VALUE = 0b1;
        public static bool HasFileflag(this byte[] TID) { return ((TID[1] >> (FILE_FLAG_START_INDEX - 8)) & FILE_FLAG_MAX_VALUE) > 0; }

        public const byte DESIGNER_IDENTIFIER_START_INDEX = 0x0B;
        public const ushort DESIGNER_IDENTIFIER_MAX_VALUE = 0b111111111;
        public static ushort GetDesignedIdentifierRaw(this byte[] TID)
        {
            const int byteIndex = DESIGNER_IDENTIFIER_START_INDEX / 8;
            const int byteShift = DESIGNER_IDENTIFIER_START_INDEX % 8;
            byte firstPart = (byte)(TID[byteIndex] << byteShift);
            byte secondPart = (byte)(TID[byteIndex + 1] >> (8 - byteShift));
            return (ushort)(firstPart & secondPart);
        }
        public static MaskDesignerIdentifier GetDesignerIdentifier(this byte[] TID)
        {
            return (MaskDesignerIdentifier)(TID.GetDesignedIdentifierRaw());
        }

        public const byte MODEL_IDENTIFIER_START_INDEX = 0x14;
        public const byte MODEL_IDENTIFIER_VALUE_LENGTH = 12;
        public const ushort MODEL_IDENTIFIER_MAX_VALUE = 0b111111111111;
        public static ushort GetModelIdentifier(this byte[] TID)
        {
            const int byteIndex = MODEL_IDENTIFIER_START_INDEX / 8;
            const int byteShift = MODEL_IDENTIFIER_START_INDEX % 8;
            byte firstPart = (byte)(TID[byteIndex] << byteShift);
            byte secondPart = (byte)(TID[byteIndex + 1] >> (8 - byteShift));
            return (byte)(firstPart & secondPart);
        }
    }
}
