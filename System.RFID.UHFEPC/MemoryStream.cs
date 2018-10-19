using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.RFID.UHFEPC
{
    //public class MemoryStream : Stream
    //{
    //    public MemoryStream(Tag tag) { this.tag = tag; }
    //    private readonly Tag tag;

    //    public bool AutoFlush = true;

    //    public override bool CanRead => throw new NotImplementedException();

    //    public override bool CanSeek => throw new NotImplementedException();

    //    public override bool CanWrite => throw new NotImplementedException();

    //    public override long Length => throw new NotImplementedException();

    //    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //    public override void Flush()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private static double LOG_2 = Math.Log(2);
    //    public static byte GetOccupiedBits(byte value)
    //    {
    //        return (byte)(Math.Ceiling(Math.Log(value) / LOG_2));
    //    }

    //    public override int Read(byte[] buffer, int offset, int count)
    //    {
    //        byte MEMORY_BANK_BITS_LENGTH = GetOccupiedBits(Tag.MEMORY_BANK_MAX_VALUE);
    //        byte BYTE_BITS_LENGTH = GetOccupiedBits(byte.MaxValue);
    //        int blocksToRead = ((int)(count / byte.MaxValue));
    //        int blockIndex;
    //        //Complete 32 bits blocks to read
    //        for (blockIndex = 0; blockIndex < blocksToRead; blockIndex++)
    //        {
    //            ReadMemoryBlocks(offset + blockIndex, byte.MaxValue);
    //        }
    //        ReadMemoryBlocks(offset + blockIndex, (byte)(count % byte.MaxValue)); //Read remaining 8 bits blocks
    //        return blockIndex;

    //        byte[] ReadMemoryBlocks(int wordPtr, byte wordCount)
    //        {
    //            byte[] EBVWordPtr = GetExtensibleBitVector(wordPtr);
    //            int argumentsBitsLength = MEMORY_BANK_BITS_LENGTH + EBVWordPtr.Length + BYTE_BITS_LENGTH;
    //            byte[] arguments = (new byte[] { (byte)Tag.MemoryBank.User }).Concat(EBVWordPtr).Concat(new byte[] { wordCount }).ToArray();
    //            BitArray argumentss = new BitArray(arguments);
    //            argumentss.Cast<bool>().Skip()
    //            this.tag.Execute(Tag.READ_COMMAND, )
    //        }
    //    }

    //    public override long Seek(long offset, SeekOrigin origin)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetLength(long value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        throw new NotImplementedException();

    //        if (this.AutoFlush)
    //            this.Flush();
    //    }
    //}
}
