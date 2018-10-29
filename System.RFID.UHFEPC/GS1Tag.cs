using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID.UHFEPC
{
    public partial class GS1Tag : Tag
    {
        public sealed override ISO15693ClassIdentifier ISO15693Class => ISO15693ClassIdentifier.GS1;

        public GS1Tag(byte[] uid) : base(uid)
        {
            this.IsTIDExtended = ((uid[EXTENDED_TID_BYTE_INDEX] >> EXTENDED_TID_BYTE_SHIFT) & EXTENDED_TID_MAX_VALUE) > 0;
            this.HasSecurityFlag = ((uid[SECURITY_FLAG_BYTE_INDEX] >> SECURITY_FLAG_BYTE_SHIFT) & SECURITY_FLAG_MAX_VALUE) > 0;
            this.HasFileflag = ((uid[SECURITY_FLAG_BYTE_INDEX] >> SECURITY_FLAG_BYTE_SHIFT) & SECURITY_FLAG_MAX_VALUE) > 0;

            byte firstPart = (byte)(uid[DESIGNER_IDENTIFIER_BYTE_INDEX] << DESIGNER_IDENTIFIER_BYTE_SHIFT);
            byte secondPart = (byte)(uid[DESIGNER_IDENTIFIER_BYTE_INDEX + 1] >> (8 - DESIGNER_IDENTIFIER_BYTE_SHIFT));
            this.DesignedIdentifier = (ushort)(firstPart & secondPart);

            firstPart = (byte)(uid[MODEL_IDENTIFIER_BYTE_INDEX] << MODEL_IDENTIFIER_BYTE_SHIFT);
            secondPart = (byte)(uid[MODEL_IDENTIFIER_BYTE_INDEX + 1] >> (8 - MODEL_IDENTIFIER_BYTE_SHIFT));
            this.ModelIdentifier = (byte)(firstPart & secondPart);

            //TODO: Implement XTID

            //TODO: Check permalock of TID memory
        }

        public const byte EXTENDED_TID_INDEX = 0x08;
        public const byte EXTENDED_TID_BYTE_INDEX = EXTENDED_TID_INDEX / 8;
        public const byte EXTENDED_TID_BYTE_SHIFT = EXTENDED_TID_INDEX % 8;
        public const byte EXTENDED_TID_MAX_VALUE = 0b1;
        public readonly bool IsTIDExtended;

        public const byte SECURITY_FLAG_INDEX = 0x09;
        public const byte SECURITY_FLAG_BYTE_INDEX = SECURITY_FLAG_INDEX / 8;
        public const byte SECURITY_FLAG_BYTE_SHIFT = SECURITY_FLAG_INDEX % 8;
        public const byte SECURITY_FLAG_MAX_VALUE = 0b1;
        public readonly bool HasSecurityFlag;

        public const byte FILE_FLAG_INDEX = 0x0A;
        public const byte FILE_FLAG_BYTE_INDEX = FILE_FLAG_INDEX / 8;
        public const byte FILE_FLAG_BYTE_SHIFT = FILE_FLAG_INDEX % 8;
        public const byte FILE_FLAG_MAX_VALUE = 0b1;
        public readonly bool HasFileflag;

        public const byte DESIGNER_IDENTIFIER_START_INDEX = 0x0B;
        public const byte DESIGNER_IDENTIFIER_BYTE_INDEX = DESIGNER_IDENTIFIER_START_INDEX / 8;
        public const byte DESIGNER_IDENTIFIER_BYTE_SHIFT = DESIGNER_IDENTIFIER_START_INDEX % 8;
        public const ushort DESIGNER_IDENTIFIER_MAX_VALUE = 0b111111111;
        public readonly ushort DesignedIdentifier;
        /// <remarks>Prefer a method than a readonly field because if the designer mask is not available then just the method fails instead of the constructor</remarks>
        public MaskDesignerIdentifier KnownDesignerIdentifier
        {
            get { return (MaskDesignerIdentifier)(this.DesignedIdentifier); }
        }

        public const byte MODEL_IDENTIFIER_START_INDEX = 0x14;
        const int MODEL_IDENTIFIER_BYTE_INDEX = MODEL_IDENTIFIER_START_INDEX / 8;
        const int MODEL_IDENTIFIER_BYTE_SHIFT = MODEL_IDENTIFIER_START_INDEX % 8;
        public const ushort MODEL_IDENTIFIER_MAX_VALUE = 0b111111111111;
        public readonly ushort ModelIdentifier;
    }
}
