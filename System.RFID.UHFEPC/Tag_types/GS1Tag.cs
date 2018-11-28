namespace System.RFID.UHFEPC
{
    public partial class GS1Tag : Tag
    {
        public sealed override ISO15693ClassIdentifier ISO15693Class => ISO15693ClassIdentifier.GS1;

        public GS1Tag(byte[] uid) : base(uid)
        {
            this.IsTIDExtended = ((uid[EXTENDED_TID_FLAG_BYTE_INDEX] >> EXTENDED_TID_FLAG_BYTE_SHIFT) & EXTENDED_TID_FLAG_MAX_VALUE) > 0;
            this.HasSecurityFlag = ((uid[SECURITY_FLAG_BYTE_INDEX] >> SECURITY_FLAG_BYTE_SHIFT) & SECURITY_FLAG_MAX_VALUE) > 0;
            this.HasFileflag = ((uid[SECURITY_FLAG_BYTE_INDEX] >> SECURITY_FLAG_BYTE_SHIFT) & SECURITY_FLAG_MAX_VALUE) > 0;
            
            ushort firstPart = (ushort)(uid[DESIGNER_IDENTIFIER_BYTE_INDEX] << DESIGNER_IDENTIFIER_BIT_IN_SECOND_PART);
            byte secondPart = (byte)(uid[DESIGNER_IDENTIFIER_BYTE_INDEX + 1] >> (8 - DESIGNER_IDENTIFIER_BIT_IN_SECOND_PART));
            this.DesignedIdentifier = (ushort)((firstPart | secondPart) & DESIGNER_IDENTIFIER_MAX_VALUE);
            
            firstPart = (ushort)(uid[MODEL_IDENTIFIER_BYTE_INDEX] << MODEL_IDENTIFIER_BIT_IN_SECOND_PART);
            secondPart = (byte)(uid[MODEL_IDENTIFIER_BYTE_INDEX + 1] >> (8 - MODEL_IDENTIFIER_BIT_IN_SECOND_PART));
            this.ModelIdentifier = (ushort)((firstPart | secondPart) & MODEL_IDENTIFIER_MAX_VALUE);

            //TODO: Check permalock of TID memory
        }

        public const byte EXTENDED_TID_FLAG_INDEX = 0x08;
        public const byte EXTENDED_TID_FLAG_BYTE_INDEX = EXTENDED_TID_FLAG_INDEX / 8;
        public const byte EXTENDED_TID_FLAG_BYTE_SHIFT = 7 - (EXTENDED_TID_FLAG_INDEX % 8);
        public const byte EXTENDED_TID_FLAG_MAX_VALUE = 0b1;
        public readonly bool IsTIDExtended;

        public const byte SECURITY_FLAG_INDEX = 0x09;
        public const byte SECURITY_FLAG_BYTE_INDEX = SECURITY_FLAG_INDEX / 8;
        public const byte SECURITY_FLAG_BYTE_SHIFT = 7 - (SECURITY_FLAG_INDEX % 8);
        public const byte SECURITY_FLAG_MAX_VALUE = 0b1;
        public readonly bool HasSecurityFlag;

        public const byte FILE_FLAG_INDEX = 0x0A;
        public const byte FILE_FLAG_BYTE_INDEX = FILE_FLAG_INDEX / 8;
        public const byte FILE_FLAG_BYTE_SHIFT = 7 - (FILE_FLAG_INDEX % 8);
        public const byte FILE_FLAG_MAX_VALUE = 0b1;
        public readonly bool HasFileflag;

        public const byte DESIGNER_IDENTIFIER_START_INDEX = 0x0B;
        public const byte DESIGNER_IDENTIFIER_BYTE_INDEX = DESIGNER_IDENTIFIER_START_INDEX / 8;
        public const byte DESIGNER_IDENTIFIER_BIT_IN_FIRST_PART = 8 - (DESIGNER_IDENTIFIER_START_INDEX % 8);
        public const byte DESIGNER_IDENTIFIER_MAX_BIT_LENGTH = 9;
        public const byte DESIGNER_IDENTIFIER_BIT_IN_SECOND_PART = DESIGNER_IDENTIFIER_MAX_BIT_LENGTH - DESIGNER_IDENTIFIER_BIT_IN_FIRST_PART;
        public const ushort DESIGNER_IDENTIFIER_MAX_VALUE = (1 << (DESIGNER_IDENTIFIER_MAX_BIT_LENGTH + 1)) - 1;
        public readonly ushort DesignedIdentifier;
        /// <remarks>Prefer a method than a readonly field because if the designer mask is not available then just the method fails instead of the constructor</remarks>
        public MaskDesignerIdentifier KnownDesignerIdentifier
        {
            get { return (MaskDesignerIdentifier)(this.DesignedIdentifier); }
        }

        public const byte MODEL_IDENTIFIER_START_INDEX = 0x14;
        public const int MODEL_IDENTIFIER_BYTE_INDEX = MODEL_IDENTIFIER_START_INDEX / 8;
        public const int MODEL_IDENTIFIER_BIT_IN_FIRST_PART = 8 - (MODEL_IDENTIFIER_START_INDEX % 8);
        public const byte MODEL_IDENTIFIER_MAX_BIT_LENGTH = 12;
        public const byte MODEL_IDENTIFIER_BIT_IN_SECOND_PART = MODEL_IDENTIFIER_MAX_BIT_LENGTH - MODEL_IDENTIFIER_BIT_IN_FIRST_PART;
        public const ushort MODEL_IDENTIFIER_MAX_VALUE = (1 << (MODEL_IDENTIFIER_MAX_BIT_LENGTH + 1)) - 1;
        public readonly ushort ModelIdentifier;

        //TODO: Implement XTID
        //public const byte EXTENDED_TID_START_INDEX = 0x1F;
        //public byte[] ExtendedTID
        //{
        //    get { new TimeSpan<}
        //}
    }
}
