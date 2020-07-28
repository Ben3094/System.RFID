using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BenDotNet.RFID.UHFEPC
{
    public abstract partial class Tag : RFID.Tag
    {
        public const string NOT_EPC_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT_FORMAT = "{0} UHF EPC";
        public static string NOT_EPC_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT = String.Format(NOT_GENUINE_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT, NOT_EPC_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT_FORMAT);
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
                throw new ArgumentException(String.Format(NOT_EPC_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT, this.ISO15693Class));

            this.ReservedMemoryBankStream = new MemoryBankStream(this, MemoryBank.Reserved);
            this.EPCMemoryBankStream = new MemoryBankStream(this, MemoryBank.EPC);
            this.TIDMemoryBankStream = new MemoryBankStream(this, MemoryBank.TID);
            this.UserMemoryBankStream = new MemoryBankStream(this, MemoryBank.User);
            //this.memoryStream = new CompositeMemoryStream();
        }

        public enum MemoryBank
        {
            Reserved = 0b00,
            EPC = 0b01,
            TID = 0b10,
            User = 0b11
        }

        public IEnumerable<char> Read(MemoryBank memoryBank, int offset = 0, byte length = 0)
        {
            ReadCommand readCommand = new ReadCommand(memoryBank, offset, length);
            ReadReply readReply = (ReadReply)this.Execute(readCommand);
            return readReply.MemoryWords;
        }
        public void Write(MemoryBank memoryBank, ref IEnumerable<char> data, int offset = 0)
        {
            try //TODO: Avoid this try-catch by knowing tag capabilities
            {
                this.Execute(new BlockWriteCommand(memoryBank, ref data, offset));
            }
            catch (ErrorRepliedException) //TODO: Correct this exception to correctly handle exceptions
            {
                Helpers.FallbackBlockWrite(this, memoryBank, ref data, offset);
            }
        }
        public void Write(MemoryBank memoryBank, IEnumerable<char> data, int offset = 0)
        {
            this.Write(memoryBank, ref data, offset);
        }
        public void Write(MemoryBank memoryBank, int offset = 0, params char[] data)
        {
            this.Write(memoryBank, data, offset);
        }

        public const int DEFAULT_RESERVED_MEMORY_BANK_OFFSET = 0;
        public readonly MemoryBankStream ReservedMemoryBankStream;
        public long ReservedMemoryBankWordsLength { get => this.ReservedMemoryBankStream.Length; protected set => this.ReservedMemoryBankStream.length = value; }

        #region EPC
        public readonly MemoryBankStream EPCMemoryBankStream;
        public long EPCMemoryBankWordsLength { get => this.EPCMemoryBankStream.Length; protected set => this.EPCMemoryBankStream.length = value; }
        public const byte STOREDCRC_OFFSET = 0; //in bytes
        public const byte STOREDCRC_LENGTH = 1; //in bytes
        public char StoredCRC
        {
            get => this.Read(MemoryBank.EPC, STOREDCRC_OFFSET, STOREDCRC_LENGTH).First();
            set => this.Write(MemoryBank.EPC, STOREDCRC_OFFSET, value);
        }

        #region StoredPC
        public const byte STOREDPC_OFFSET = 1; //in bytes
        public const byte STOREDPC_LENGTH = 1; //in bytes
        public char StoredPC
        {
            get => this.Read(MemoryBank.EPC, STOREDPC_OFFSET, STOREDPC_LENGTH).First();
        }

        #region Parsing
        public const byte EPC_LENGTH_OFFSET = 0; //in bits
        public const byte EPC_LENGTH_LENGTH = 5; //in bits
        public const byte EPC_LENGTH_MASK = (1 << EPC_LENGTH_LENGTH) - 1;
        /// <summary>
        /// EPC query length, written by an Interrogator, specifies the length of the EPC that a Tag backscatters in response to an ACK
        /// </summary>
        /// <remarks>
        /// If a Tag does not support Extended EPC Word 1 then the maximum value for the EPC length field in the StoredPC shall be 0b11111 (allows a 496-bit EPC)
        /// If a Tag supports Extended EPC Word 1 then the maximum value for the EPC length field in the StoredPC shall be 0b11101 (allows a 464-bit EPC)
        /// A Tag that supports Extended EPC Word 1 shall not execute a Write, BlockWrite, or Untraceable that attempts to write an EPC length field larger than 0b11101 and shall instead treat the command’s parameters as unsupported
        /// </remarks>
        public byte EPCLength
        {
            get => (byte)((this.StoredPC >> EPC_LENGTH_OFFSET) | EPC_LENGTH_MASK);
        }
        public const byte USER_MEMORY_INDICATOR_OFFSET = 6; //in bits
        /// <summary>
        /// Tag is capable of allocating memory to File_0
        /// </summary>
        /// <remarks>
        /// Fixed or computed by the tag
        /// For a computed user memory indicator, if an Interrogator deallocates File_0 then the Tag shall set to false upon deallocation
        /// Also for a computed user memory indicator, the untraceability status of User memory shall not change the user memory indicator value (i.e. if user memory capable tag is traceable then user memory indicator shall remain true even if an interrogator instructs a tag to untraceably hide User memory)
        /// </remarks>
        public bool UserMemoryIndicator => this.StoredPC.IsTrue(USER_MEMORY_INDICATOR_OFFSET);
        public const byte EXTENDED_PROTOCOL_CONTROL_WORD_1_INDICATOR_OFFSET = 7;
        /// <summary>
        /// Tag implements Extended Protocol Control Word 1
        /// </summary>
        public bool ExtendedProtocolControlWord1Indicator => this.StoredPC.IsTrue(EXTENDED_PROTOCOL_CONTROL_WORD_1_INDICATOR_OFFSET);
        public const byte NUMBERING_SYSTEM_IDENTIFIER_TOGGLE_OFFSET = 8;
        public bool NumberingSystemIdentifierToggle => this.StoredPC.IsTrue(NUMBERING_SYSTEM_IDENTIFIER_TOGGLE_OFFSET);
        #endregion
        #endregion
        public const byte EPC_OFFSET = 2; //in bytes
        public const byte EPC_MAX_LENGTH = 30; //in bytes
        public byte[] EPC
        {
            get => UHFEPC.Helpers.GetBytesFromWords(this.Read(MemoryBank.EPC, EPC_OFFSET, EPC_MAX_LENGTH)).ToArray();
            set => this.Write(MemoryBank.EPC, UHFEPC.Helpers.GetWordsFromBytes(value), EPC_OFFSET);
        }
        #region Extended Protocol Control Word 1 
        public const byte XPC_W1_OFFSET = 32; //in bytes
        public const byte XPC_W1_LENGTH = 1; //in bytes
        public char ExtendedProtocolControlWord1
        {
            get => this.Read(MemoryBank.EPC, XPC_W1_OFFSET, XPC_W1_LENGTH).First();
            set => this.Write(MemoryBank.EPC, XPC_W1_OFFSET, value);
        }
        #region Parsing
        public const byte XPC_W2_INDICATOR_OFFSET = 0; //in bits
        public bool XPCW2Indicator => this.ExtendedProtocolControlWord1.IsTrue(XPC_W2_INDICATOR_OFFSET);
        public const byte BATTERY_ASSISTED_PASSIVE_INDICATOR_OFFSET = 8; //in bits
        public bool BatteryAssistedPassiveIndicator => this.ExtendedProtocolControlWord1.IsTrue(BATTERY_ASSISTED_PASSIVE_INDICATOR_OFFSET);
        public const byte COMPUTED_RESPONSE_INDICATOR_OFFSET = 9; //in bits
        public bool ComputedResponseIndicator => this.ExtendedProtocolControlWord1.IsTrue(COMPUTED_RESPONSE_INDICATOR_OFFSET);
        public const byte SL_INDICATOR_OFFSET = 10; //in bits
        public bool SLIndicator => this.ExtendedProtocolControlWord1.IsTrue(SL_INDICATOR_OFFSET);
        public const byte NOTIFICATION_INDICATOR_OFFSET = 11; //in bits
        public bool NotificationIndicator => this.ExtendedProtocolControlWord1.IsTrue(NOTIFICATION_INDICATOR_OFFSET);
        public const byte UNTRACEABLE_INDICATOR_OFFSET = 12; //in bits
        public bool UntraceableIndicator => this.ExtendedProtocolControlWord1.IsTrue(UNTRACEABLE_INDICATOR_OFFSET);
        public const byte KILLABLE_INDICATOR_OFFSET = 13; //in bits
        public bool KillableIndicator => this.ExtendedProtocolControlWord1.IsTrue(KILLABLE_INDICATOR_OFFSET);
        public const byte NONREMOVABLE_INDICATOR_OFFSET = 14; //in bits
        public bool NonremovableIndicator => this.ExtendedProtocolControlWord1.IsTrue(NONREMOVABLE_INDICATOR_OFFSET);
        public const byte HAZMAT_INDICATOR_OFFSET = 15; //in bits
        public bool HazmatIndicator => this.ExtendedProtocolControlWord1.IsTrue(HAZMAT_INDICATOR_OFFSET);
        #endregion
        #endregion
        public const byte XPC_W2_OFFSET = 32; //in bytes
        public const byte XPC_W2_LENGTH = 1; //in bytes
        public char XPC_W2
        {
            get => this.Read(MemoryBank.EPC, XPC_W2_OFFSET, XPC_W1_LENGTH).First();
            set => this.Write(MemoryBank.EPC, XPC_W2_OFFSET, value);
        }
        #endregion

        public readonly MemoryBankStream TIDMemoryBankStream;
        public long TIDMemoryBankWordsLength { get => this.TIDMemoryBankStream.Length; protected set => this.TIDMemoryBankStream.length = value; }
        public readonly MemoryBankStream UserMemoryBankStream;
        public long UserMemoryBankWordsLength { get => this.UserMemoryBankStream.Length; protected set => this.UserMemoryBankStream.length = value; }
        public override Stream Memory => this.UserMemoryBankStream;
    }
}
