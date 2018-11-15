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
        public abstract BitArray CommandCode { get; }
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

        public const byte READ_COMMAND_CODE = 0b11000010;
        public override BitArray CommandCode => new BitArray(READ_COMMAND_CODE);

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

        public const byte WRITE_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(WRITE_COMMAND_CODE);

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

        public const byte KILL_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(KILL_COMMAND_CODE);
    }

    public class Lock : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte LOCK_COMMAND_CODE = 0b11000101;
        public override BitArray CommandCode => new BitArray(LOCK_COMMAND_CODE);
    }

    public class Access : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte ACCESS_COMMAND_CODE = 0b11000110;
        public override BitArray CommandCode => new BitArray(ACCESS_COMMAND_CODE);
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

        public const byte BLOCKWRITE_COMMAND_CODE = 0b11000111;
        public override BitArray CommandCode => new BitArray(BLOCKWRITE_COMMAND_CODE);

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

        public const byte BLOCKERASE_COMMAND_CODE = 0b11001000;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class BlockPermalock : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKPERMALOCK_COMMAND_CODE = 0b11001001;
        public override BitArray CommandCode => new BitArray(BLOCKPERMALOCK_COMMAND_CODE);
    }

    public class Authenticate : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte AUTHENTICATE_COMMAND_CODE = 0b11010101;
        public override BitArray CommandCode => new BitArray(AUTHENTICATE_COMMAND_CODE);
    }

    public class AuthComm : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte AUTHCOMM_COMMAND_CODE = 0b11010111;
        public override BitArray CommandCode => new BitArray(AUTHCOMM_COMMAND_CODE);
    }

    public class SecureComm : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte SECURECOMM_COMMAND_CODE = 0b11010110;
        public override BitArray CommandCode => new BitArray(SECURECOMM_COMMAND_CODE);
    }

    public class KeyUpdate : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000010;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class TagPrivilege : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000011;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class ReadBuffer : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11010010;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class Untraceable : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000000;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileOpen : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11010011;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileList : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000001;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FilePrivilege : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000100;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileSetup : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000101;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }
    #endregion
}
