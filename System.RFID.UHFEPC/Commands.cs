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
        public Reply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public Reply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply) { }

        public const bool ERROR_HEADER = true;
        public const bool SUCCESS_HEADER = false;
        /// <summary>
        /// The header for an error code is a 1-bit, unlike the header for a success response which is a 0-bit
        /// </summary>
        public virtual bool Header => SUCCESS_HEADER;
    }

    public class ErrorReply : Reply
    {
        public ErrorReply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public ErrorReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply) { }

        public override bool Header => ERROR_HEADER;

        /// <remarks>
        /// If a Tag supports error-specific codes then it shall use the error-specific codes
        /// If a Tag does not support error-specific codes then it shall backscatter error code 000011112 (indicating a non-specific error)
        /// A Tag shall backscatter error codes only from the open or secured states
        /// A Tag shall not backscatter an error code if it receives an invalid or improper access command, or an access command with an incorrect handle
        /// If an error is described by more than one error code then the more specific error code shall take precedence and shall be the code that the Tag backscatters
        /// </remarks>
        /// <summary>
        /// 
        /// </summary>
        public byte ErrorCode;

        public enum ErrorCodeEnum
        {
            OtherError = 0b00000000, //Catch-all for errors not covered by other codes
            NotSupported = 0b00000001, //The Tag does not support the specified parameters or feature
            InsufficientPrivileges = 0b00000010, //The Interrogator did not authenticate itself with sufficient privileges for the Tag to perform the operation
            MemoryOverrun = 0b00000011, //The Tag memory location does not exist, is too small, or the Tag does not support the specified EPC length
            MemoryLocked = 0b00000100, //The Tag memory location is locked or permalocked and is either not writeable or not readable
            CryptosuiteError = 0b00000101, //Catch-all for errors specified by the cryptographic suite
            CommandNotEncapsulated = 0b00000110, //The Interrogator did not encapsulate the command in an AuthComm or SecureComm as required
            ResponseBufferOverflow = 0b00000111, //The operation failed because the ResponseBuffer overflowed
            SecurityTimeout = 0b00001000, //The command failed because the Tag is in a security timeout
            InsufficientPower = 0b00001011, //The Tag has insufficient power to perform the operation
            NonSpecificError = 0b00001111 //The Tag does not support error-specific codes
        }
        public ErrorCodeEnum Error { get { return (ErrorCodeEnum)this.ErrorCode; } }
    }

    public class DelayedTagReply : Reply
    {
        public DelayedTagReply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public DelayedTagReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }
    }
    public class InProcessTagReply : Reply
    {
        public InProcessTagReply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public InProcessTagReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }
    }
    #endregion

    #region Select commands
    public abstract class SelectCommand : Command
    {
    }

    public class Select : SelectCommand
    {
        public override CommandType Type => CommandType.Mandatory;
        
        public static bool[] SELECT_COMMAND_CODE = new bool[] { true, false, true, false };
        public override BitArray CommandCode => new BitArray(SELECT_COMMAND_CODE);
    }
    #endregion

    #region Inventory commands
    public abstract class InventoryCommand : Command
    {

    }

    public abstract class BaseQueryCommand : InventoryCommand
    {
        public enum Session
        {
            S0 = 0b00,
            S1 = 0b01,
            S2 = 0b10,
            S3 = 0b11
        }
    }
    public class QueryReply : Reply
    {
        public QueryReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public QueryReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    [ReplyType(typeof(QueryReply))]
    public class Query : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERY_COMMAND_CODE = new bool[] { true, false, false, false };
        public override BitArray CommandCode => new BitArray(QUERY_COMMAND_CODE);
    }

    [ReplyType(typeof(QueryReply))]
    public class QueryAdjust : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERYADJUST_COMMAND_CODE = new bool[] { true, false, false, true };
        public override BitArray CommandCode => new BitArray(QUERYADJUST_COMMAND_CODE);
    }

    [ReplyType(typeof(QueryReply))]
    public class QueryRep : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERYREP_COMMAND_CODE = new bool[] { false, false };
        public override BitArray CommandCode => new BitArray(QUERYREP_COMMAND_CODE);
    }

    public class ACK : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] ACK_COMMAND_CODE = new bool[] { false, true };
        public override BitArray CommandCode => new BitArray(ACK_COMMAND_CODE);
    }
    public class ACKReply : Reply
    {
        public ACKReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public ACKReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    public class NCK : InventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte NCK_COMMAND_CODE = 0b11000000;
        public override BitArray CommandCode => new BitArray(NCK_COMMAND_CODE);
    }
    #endregion

    #region Access commands
    public abstract class AccessCommand : Command
    {
    }

    [ReplyType(typeof(ReqRNReply))]
    public class ReqRN : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte REQRN_COMMAND_CODE = 0b11000001;
        public override BitArray CommandCode => new BitArray(REQRN_COMMAND_CODE);
    }
    public class ReqRNReply : QueryReply
    {
        public ReqRNReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public ReqRNReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
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

    [ReplyType(typeof(ReadReply))]
    public class Read : MemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte READ_COMMAND_CODE = 0b11000010;
        public override BitArray CommandCode => new BitArray(READ_COMMAND_CODE);

        public byte WordCount = 0;
    }
    public class ReadReply : Reply
    {
        static ReadReply() { AssociatedCommandType = typeof(Read); }
        public ReadReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }

        public char[] MemoryWords;
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class Write : MemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte WRITE_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(WRITE_COMMAND_CODE);

        public char[] Data;
    }

    [ReplyType(typeof(ReqRNReply))]
    public class Kill : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte KILL_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(KILL_COMMAND_CODE);
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class Lock : AccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte LOCK_COMMAND_CODE = 0b11000101;
        public override BitArray CommandCode => new BitArray(LOCK_COMMAND_CODE);
    }

    [ReplyType(typeof(ReqRNReply))]
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
        public BlockWriteReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }
        static BlockWriteReply() { AssociatedCommandType = typeof(BlockWriteReply); }
    }

    [ReplyType(typeof(DelayedTagReply))]
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

    [ReplyType(typeof(InProcessTagReply))]
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
    public class TagPrivilegeReply : Reply
    {
        public TagPrivilegeReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public TagPrivilegeReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    public class ReadBuffer : AccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11010010;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    [ReplyType(typeof(DelayedTagReply))]
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
    public class FileListReply : Reply
    {
        public FileListReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public FileListReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
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
