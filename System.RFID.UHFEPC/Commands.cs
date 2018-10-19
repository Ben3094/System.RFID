using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.RFID.UHFEPC
{
    #region Core definition
    public enum CommandType { Mandatory, Optional, Proprietary, Custom }

    public abstract class Command : System.RFID.Command
    {
        public Command() { }
        public abstract CommandType Type { get; }
    }

    public abstract class Reply : System.RFID.Reply
    {
        public Reply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref originalReply, ref associatedCommand) { }
    }
    #endregion

    #region Select commands
    public abstract class SelectCommand : Command
    {
    }

    public class Select : SelectCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        //public static readonly bool[] CODE = { true, false, true, false };
        //public override bool[] Code => CODE;

    }

    public class Challenge : SelectCommand
    {
        public override CommandType Type => CommandType.Optional;

        //public static readonly bool[] CODE = { true, false, true, false };
        //public override bool[] Code => CODE;
    }
    #endregion

    #region Inventory commands
    public abstract class InventoryCommand : Command
    {
        public char TagHandle;

        public char CRC;
    }

    public class Query : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class QueryAdjust : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class QueryRep : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class ACK : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class NCK : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }
    #endregion

    #region Access commands
    public abstract class AccessCommand : Command
    {
    }

    public class Req_RN : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public abstract class MemoryAccessCommand : AccessCommand
    {
        public Tag.MemoryBank MemoryBank;

        public int Offset = 0;
        public byte[] WordPtr
        {
            get
            {
                return Helpers.CompileExtensibleBitVector(this.Offset);
            }
            set
            {
                this.Offset = Helpers.ParseExtensibleBitVector(value);
            }
        }
    }

    public class Read : MemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public byte WordCount = 0;
    }
    public class ReadReply : Reply
    {
        public ReadReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref originalReply, ref associatedCommand) { }
        static ReadReply() { AssociatedCommandType = typeof(Read); }

        public char[] MemoryWords;
    }

    public class Write : MemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public char Data;
    }
    public class WriteReply : Reply
    {
        public WriteReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref originalReply, ref associatedCommand) { }
        static WriteReply() { AssociatedCommandType = typeof(Write); }
    }

    public class Kill : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class Lock : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;
    }

    public class Access : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class BlockWrite : MemoryAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public byte WordCount
        {
            get
            {
                return (byte)this.Data.Length;
            }
        }

        public char[] Data;
    }
    public class BlockWriteReply : Reply
    {
        public BlockWriteReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref originalReply, ref associatedCommand) { }
        static BlockWriteReply() { AssociatedCommandType = typeof(BlockWriteReply); }
    }

    public class BlockErase : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class BlockPermalock : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class Authenticate : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class AuthComm : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class SecureComm : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class KeyUpdate : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class TagPrivilege : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class ReadBuffer : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class Untraceable : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class FileOpen : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class FileList : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class FilePrivilege : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }

    public class FileSetup : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;
    }
    #endregion
}
