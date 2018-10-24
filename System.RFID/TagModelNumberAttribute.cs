using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.RFID
{
    //public abstract class TagModelNumberAttribute : Attribute
    //{
    //    public abstract bool UIDCorrespond(byte[] uid);
        
        //public const byte STANDARD_ISO15693_ALLOCATION_CLASS_IDENTIFIER = 0xE2;

        //public TagModelNumberAttribute(char maskDesignerIdentifer, char tagModelNumber, byte iso15693AllocationClassIdentifier = STANDARD_ISO15693_ALLOCATION_CLASS_IDENTIFIER)
        //{
        //    this.ISO15693AllocationClassIdentifier = iso15693AllocationClassIdentifier;
        //    this.MaskDesignerIdentifier = maskDesignerIdentifer;
        //    this.TagModelNumber = tagModelNumber;
        //}

        //public byte ISO15693AllocationClassIdentifier { get; private set; }

        //private const string WRONG_LENGTH_MESSAGE = "{0} must not be longer than {1} bits";

        //public const string MASK_DESIGNER_IDENTIFIER_NAME = "Mask Designer Identifier (MDID)";
        //public const char MASK_DESIGNER_IDENTIFIER_MAX_VALUE = (char)((2 << 9) - 1);
        //public static string MASK_DESIGNER_IDENTIFIER_WRONG_LENGTH_MESSAGE = String.Format(WRONG_LENGTH_MESSAGE, MASK_DESIGNER_IDENTIFIER_NAME, MASK_DESIGNER_IDENTIFIER_MAX_VALUE);
        //private char maskDesignerIdentifier;
        //public char MaskDesignerIdentifier
        //{
        //    get { return this.maskDesignerIdentifier; }
        //    set
        //    {
        //        if (value < MASK_DESIGNER_IDENTIFIER_MAX_VALUE)
        //            this.maskDesignerIdentifier = value;
        //        else
        //            throw new ArgumentException(MASK_DESIGNER_IDENTIFIER_WRONG_LENGTH_MESSAGE);
        //    }
        //}

        //public const string TAG_MODEL_NUMBER_NAME = "Tag Model Number (TMN)";
        //public const char TAG_MODEL_NUMBER_MAX_VALUE = (char)((2 << 12) - 1);
        //public static string TAG_MODEL_NUMBER_WRONG_LENGTH_MESSAGE = String.Format(WRONG_LENGTH_MESSAGE, TAG_MODEL_NUMBER_NAME, TAG_MODEL_NUMBER_MAX_VALUE);
        //private char tagModelNumber;
        //public char TagModelNumber
        //{
        //    get { return this.tagModelNumber; }
        //    set
        //    {
        //        if (value < TAG_MODEL_NUMBER_MAX_VALUE)
        //            this.tagModelNumber = value;
        //        else
        //            throw new ArgumentException(TAG_MODEL_NUMBER_WRONG_LENGTH_MESSAGE);
        //    }
        //}
    //}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class TagModelNumberAttribute : Attribute
    {
        public abstract bool UIDCorrespond(byte[] uid);
    }
    //TODO: Avoid using Attribute and add field in Tag class

    public class ClassicTagModelNumberAttribute : TagModelNumberAttribute
    {
        public ClassicTagModelNumberAttribute(bool?[] model)
        {
            this.MODEL = model;
        }

        public readonly bool?[] MODEL;

        public override bool UIDCorrespond(byte[] uid)
        {
            for (int bitIndex = 0; bitIndex < MODEL.Length; bitIndex++)
            {
                try
                {
                    if (MODEL[bitIndex] == Convert.ToBoolean(uid[bitIndex % 8] >> (bitIndex / 8) & 0x01))
                        return false;
                }
                catch (NullReferenceException) { }
            }
            return true;
        }
    }
}
