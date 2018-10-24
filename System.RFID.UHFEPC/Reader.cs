using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using BitStreams;

namespace System.RFID.UHFEPC
{
    public abstract class Reader : RFID.Reader
    {
        public const char READ_COMMAND = (char)0b11000010;
        //public virtual byte[] ReadMemory(Tag targetTag, Tag.MemoryBank memBank, int wordPtr, byte wordCount)
        //{
        //    //byte[] EBVWordPtr = GetExtensibleBitVector(wordPtr);
        //    //byte[] arguments = (new byte[] { (byte)Tag.MemoryBank.User }).Concat(EBVWordPtr).Concat(new byte[] { wordCount }).ToArray();
        //    //return ExecuteAccessCommand(targetTag, READ_COMMAND, arguments);
        //}
        //public virtual byte[] ExecuteAccessCommand(Tag targetTag, char commandCode, byte[] arguments)
        //{
        //    //Compile command code
        //    List<byte> queryCommand = new List<byte>();
        //    if (commandCode > byte.MaxValue)
        //        queryCommand.Add((byte)(commandCode >> 8));
        //    queryCommand.Add((byte)commandCode);

        //    byte[] query = queryCommand.Concat(arguments).ToArray(); //Compile query //TODO: Add handler and CRC
        //    byte[] response = this.Execute(targetTag, query); //Send the query

        //    //Parse response
        //    if (response[0] != 0) //Detect error
        //    {
        //        if (response[0] == 1) //GS1 error signal
        //        {
        //            //TODO: Parse error code
        //        }
        //        else throw new ArgumentException("Reponse is misformed, data are present ");
        //    }
        //    return null;
        //}
        //public virtual BitStream ExecuteAccessCommand(Tag targetTag, char commandCode, BitStream arguments)
        //{
        //    //Compile command code
        //    List<byte> queryCommand = new List<byte>();
        //    if (commandCode > byte.MaxValue)
        //        queryCommand.Add((byte)(commandCode >> 8));
        //    queryCommand.Add((byte)commandCode);
        //    BitStream queryCommandStream = new BitStream(queryCommand.ToArray());
        //    arguments.CopyStreamTo(queryCommandStream);
            

        //    //TODO: Add handler and CRC
        //    BitStream response = this.Execute(targetTag, queryCommandStream); //Send the query
        //    StreamWriter responseReader = new StreamWriter(response);
        //    responseReader.A
        //    //Parse response
        //    if (response[0] != 0) //Detect error
        //    {
        //        if (response[0] == 1) //GS1 error signal
        //        {
        //            //TODO: Parse error code
        //        }
        //        else throw new ArgumentException("Reponse is misformed, data are present ");
        //    }
        //    return null;
        //}
    }

    /// <summary>
    /// 4 most-significant bits of the 8-bits "Cryptography Suite Indicator" (CSI) used by Challenge and Authenticate commands
    /// </summary>
    public enum CryptographicSuiteAssigningAuthority
    {
        ISO29167_1 = 0b00000000,
        ISO29167_2 = 0b00010000,
        ISO29167_3 = 0b00100000,
        ISO29167_4 = 0b00110000,
        TagManufacturer = 0b11010000,
        GS1 = 0b11100000
    }
}
