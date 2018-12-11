using System.Linq;
using System.IO;
using System.Collections.Generic;
using BenDotNet.Numerics;

namespace BenDotNet.RFID.UHFEPC
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

            if (this.IsTIDExtended)
            {
                this.ExtendedTID = new byte[uid.Length - EXTENDED_TID_START_BYTES_INDEX];
                Array.Copy(uid, EXTENDED_TID_START_BYTES_INDEX, this.ExtendedTID, 0, this.ExtendedTID.Length); //TODO: Convert it to span when available
            }

            //TODO: Check permalock of TID memory

            this.UserMemoryBankStream = new MemoryBankStream(this, MemoryBank.User);
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
        public const byte EXTENDED_TID_START_BIT_INDEX = 0x1F;
        public const byte EXTENDED_TID_START_BYTES_INDEX = EXTENDED_TID_START_BIT_INDEX / 8;
        public readonly byte[] ExtendedTID;

        public enum MemoryBank
        {
            Reserved = 0b00,
            EPC = 0b01,
            TID = 0b10,
            User = 0b11
        }

        //TODO: Avoid this zone by making the memory stream resizable (except for those known)
        public const int RESERVED_MEMORY_BANK_OFFSET = 0;
        protected Range<int> reservedMemoryBankLimits;
        /// <remarks>
        /// In words
        /// </remarks>
        public Range<int> ReservedMemoryBankLimits
        {
            get
            {
                if (reservedMemoryBankLimits == null)
                    this.getMemoryBanksLimits();

                return reservedMemoryBankLimits;
            }
            set => this.reservedMemoryBankLimits = value;
        }
        protected Range<int> epcMemoryBankLimits;
        /// <remarks>
        /// In words
        /// </remarks>
        public Range<int> EPCMemoryBankLimits
        {
            get
            {
                if (epcMemoryBankLimits == null)
                    this.getMemoryBanksLimits();

                return epcMemoryBankLimits;
            }
            set => this.epcMemoryBankLimits = value;
        }
        protected Range<int> tidMemoryBankLimits;
        /// <remarks>
        /// In words
        /// </remarks>
        public Range<int> TIDMemoryBankLimits
        {
            get
            {
                if (tidMemoryBankLimits == null)
                    this.getMemoryBanksLimits();

                return tidMemoryBankLimits;
            }
            set => this.tidMemoryBankLimits = value;
        }
        protected Range<int> userMemoryBankLimits;
        /// <remarks>
        /// In words
        /// </remarks>
        public Range<int> UserMemoryBankLimits
        {
            get
            {
                if (userMemoryBankLimits == null)
                    this.getMemoryBanksLimits();

                return userMemoryBankLimits;
            }
            set => this.userMemoryBankLimits = value;
        }
        public readonly MemoryBankStream UserMemoryBankStream;

        private void getMemoryBanksLimits()
        {
            this.reservedMemoryBankLimits = new Range<int>(RESERVED_MEMORY_BANK_OFFSET, this.getMemoryBankSize(MemoryBank.Reserved));
            int EPC_MEMORY_BANK_OFFSET = reservedMemoryBankLimits.MaxValue + 1;
            this.epcMemoryBankLimits = new Range<int>(EPC_MEMORY_BANK_OFFSET, EPC_MEMORY_BANK_OFFSET + this.getMemoryBankSize(MemoryBank.EPC));
            int TID_MEMORY_BANK_OFFSET = epcMemoryBankLimits.MaxValue + 1;
            this.tidMemoryBankLimits = new Range<int>(TID_MEMORY_BANK_OFFSET, this.getMemoryBankSize(MemoryBank.TID));
            int USER_MEMORY_BANK_OFFSET = tidMemoryBankLimits.MaxValue + 1;
            this.userMemoryBankLimits = new Range<int>(USER_MEMORY_BANK_OFFSET, this.getMemoryBankSize(MemoryBank.User));
        }
        private int getMemoryBankSize(MemoryBank memoryBank)
        {
            return ((ReadReply)this.Execute(new ReadCommand(memoryBank))).MemoryWords.Length;
        }
    }


    /// <summary>
    /// Building zone...
    /// </summary>
    public class MemoryBankStream : Stream
    {
        public MemoryBankStream(GS1Tag tag, GS1Tag.MemoryBank memoryBank) { this.tag = tag; this.memoryBank = memoryBank; }
        private readonly GS1Tag tag;
        private readonly GS1Tag.MemoryBank memoryBank;

        private List<Tuple<int, IEnumerable<byte>>> pendingBuffers = new List<Tuple<int, IEnumerable<byte>>>();

        public bool AutoFlush = true;

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => true;

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            foreach (Tuple<int, IEnumerable<byte>> pendingBuffer in pendingBuffers)
            {
                IEnumerable<byte> pendingBytes = pendingBuffer.Item2;
                if (Helpers.IsMissingByteForConvertionInWords(pendingBuffer.Item2))
                {
                    int lastByteOffset = pendingBuffer.Item1 + pendingBuffer.Item2.Count();
                    ReadCommand readCommand = new ReadCommand(this.memoryBank, lastByteOffset, 1);
                    ReadReply readReply = (ReadReply)this.tag.Execute(readCommand);
                    byte lastByte = Helpers.GetBytesFromWords(readReply.MemoryWords).ElementAt(1);
                    pendingBytes = new List<byte>(pendingBytes);
                    ((List<byte>)pendingBytes).Add(lastByte);
                }

                char[] words = Helpers.GetWordsFromBytes(pendingBytes).ToArray();
                BlockWriteCommand blockWriteCommand = new BlockWriteCommand(this.memoryBank, words, pendingBuffer.Item1);
                this.tag.Execute(blockWriteCommand);
            }
            //CAUTION: If odd, buffer in byte can override the last byte !
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int originalWordsOffset = offset / 2;
            int wordsOffset = originalWordsOffset;
            int wordsCount = count / 2;
            bool hasPreviousHalfWord = wordsOffset != (float)(offset / 2f);
            bool hasAfterHalfWord = wordsCount != (float)(count / 2f);

            try
            {
                if (hasPreviousHalfWord)
                {
                    Array.Copy(readWord(wordsOffset, 1).ToArray(), 1, buffer, 0, 1);
                    wordsOffset++;
                }

                //Maximum word memory readable for a command
                for (; wordsOffset < wordsOffset + ((wordsCount / byte.MaxValue) * wordsCount); wordsOffset += byte.MaxValue)
                    readWord(wordsOffset).ToArray().CopyTo(buffer, wordsOffset - originalWordsOffset);

                //Uncomplete value of memory 
                IEnumerable<byte> BYTES = readWord(wordsOffset, (byte)(wordsCount % byte.MaxValue));
                BYTES.ToArray().CopyTo(buffer, wordsOffset - originalWordsOffset);

                if (hasAfterHalfWord)
                {
                    wordsOffset += BYTES.Count();
                    buffer[wordsOffset - originalWordsOffset] = readWord(wordsOffset, 1).Last();
                }
            }
            catch (IndexOutOfRangeException) {  }

            return ((wordsOffset - originalWordsOffset) * 2)
                - (hasPreviousHalfWord ? 1 : 0)
                + (hasAfterHalfWord ? 1 : 0);

            IEnumerable<byte> readWord(int readWordOffset, byte wordCount = byte.MaxValue)
            {
                ReadCommand readCommand = new ReadCommand(this.memoryBank, readWordOffset, wordCount);
                ReadReply readReply = (ReadReply)this.tag.Execute(readCommand);
                return Helpers.GetBytesFromWords(readReply.MemoryWords);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.pendingBuffers.Add(new Tuple<int, IEnumerable<byte>>(offset, buffer));

            if (this.AutoFlush)
                this.Flush();
        }
    }
}
