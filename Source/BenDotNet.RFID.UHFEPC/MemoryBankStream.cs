using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BenDotNet.RFID.UHFEPC
{
    public class MemoryBankStream : Stream
    {
        public MemoryBankStream(Tag tag, Tag.MemoryBank memoryBank) { this.tag = tag; this.memoryBank = memoryBank; }
        private readonly Tag tag;
        private readonly Tag.MemoryBank memoryBank;

        private List<Tuple<int, IEnumerable<byte>>> pendingBuffers = new List<Tuple<int, IEnumerable<byte>>>();

        public bool AutoFlush = true;

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => true;

        public override bool CanWrite => throw new NotImplementedException();

        private const long NOT_INITIALIZED_LENGTH_VALUE = -1;
        internal long length = NOT_INITIALIZED_LENGTH_VALUE;
        public override long Length
        {
            get
            {
                if (length == NOT_INITIALIZED_LENGTH_VALUE)
                    length = ((ReadReply)this.tag.Execute(new ReadCommand(this.memoryBank))).MemoryWords.Length * 2;

                return length;
            }
        }

        public const string NEGATIVE_POSITION_ARGUMENT_EXCEPTION_MESSAGE = "Position can not be negative";
        public const string POSITION_OVERFLOWS_END_OF_STREAM_EXCEPTION_MESSAGE = "Position exceeds memory length";
        private long position = 0;
        public override long Position
        {
            get => position;
            set
            {
                if (value < 0)
                    throw new ArgumentException(NEGATIVE_POSITION_ARGUMENT_EXCEPTION_MESSAGE);
                else if (value >= length)
                    throw new EndOfStreamException(POSITION_OVERFLOWS_END_OF_STREAM_EXCEPTION_MESSAGE);
                else
                    position = value;
            }
        }

        public override void Flush()
        {
            foreach (Tuple<int, IEnumerable<byte>> pendingBuffer in pendingBuffers)
            {
                List<byte> pendingBytes = new List<byte>();
                int wordOffset = pendingBuffer.Item1 / 2;
                bool hasPreviousRemainingWord = pendingBuffer.Item1 % 2 > 0;
                bool hasNextRemainingWord = ((pendingBuffer.Item2.Count() % 2) > 0) ^ hasPreviousRemainingWord;

                //CAUTION: Never implement a buffer for remaining words, it may introduce error when tag is accessed by multiple readers
                if (hasPreviousRemainingWord)
                    pendingBytes.Add(readWord(wordOffset, 1).First());
                pendingBytes.AddRange(pendingBuffer.Item2);
                if (hasNextRemainingWord)
                    pendingBytes.Add(readWord((pendingBuffer.Item1 + pendingBuffer.Item2.Count()) / 2, 1).Last());
            
                IEnumerable<char> words = Helpers.GetWordsFromBytes(pendingBytes).ToArray();

                this.tag.Write(this.memoryBank, ref words, wordOffset);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            bool hasPreviousRemainingWord = offset % 2 > 0;
            bool hasNextRemainingWord = ((count % 2) > 0) ^ hasPreviousRemainingWord;
            int previousRemainingWordOffset = hasPreviousRemainingWord ? 1 : 0;
            int wordsOffset = (int)Math.Ceiling(offset / 2f);
            int wordsCount = (int)Math.Floor((count - previousRemainingWordOffset) / 2f);

            int wordIndex = wordsOffset; //Initiate the position

            try
            {
                if (hasPreviousRemainingWord)
                    buffer[0] = readWord(wordsOffset - 1, 1).Last();

                //Maximum word memory readable for a command
                int maxFullBlockAddressable = wordsOffset + ((wordsCount / byte.MaxValue) * byte.MaxValue);
                for (; wordIndex < maxFullBlockAddressable; wordIndex += byte.MaxValue)
                    readWord(wordIndex).ToArray().CopyTo(buffer, previousRemainingWordOffset + ((wordIndex - wordsOffset) * 2));

                if (wordIndex < wordsOffset + wordsCount)
                    readWord(wordIndex, (byte)(wordsCount % byte.MaxValue)).ToArray().CopyTo(buffer, previousRemainingWordOffset + ((wordIndex - wordsOffset) * 2));
                wordIndex += wordsCount % byte.MaxValue;

                if (hasNextRemainingWord)
                    buffer[previousRemainingWordOffset + ((wordIndex - wordsOffset) * 2)] = readWord(wordsOffset + wordsCount + 1, 1).First();
            }
            catch (IndexOutOfRangeException) { }

            return ((wordIndex - wordsOffset) * 2)
                + previousRemainingWordOffset
                + (hasNextRemainingWord ? 1 : 0);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = offset;
            switch (origin)
            {
                case SeekOrigin.Current:
                    newPosition += this.Position;
                    break;
                case SeekOrigin.End:
                    newPosition += this.Length - 1;
                    break;
            }
            this.Position = newPosition;
            return newPosition;
        }

        public override void SetLength(long value)
        {
            //TODO redefine length
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.pendingBuffers.Add(new Tuple<int, IEnumerable<byte>>(offset, buffer.Take(count)));

            if (this.AutoFlush)
                this.Flush();
        }

        #region Helpers
        private IEnumerable<byte> readWord(int wordOffset, byte wordCount = byte.MaxValue)
        {
            ReadCommand readCommand = new ReadCommand(this.memoryBank, wordOffset, wordCount);
            RFID.Reply readReply = this.tag.Execute(readCommand);
            return Helpers.GetBytesFromWords(((ReadReply)readReply).MemoryWords);
        }
        #endregion
    }
}
