using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenDotNet.RFID.UHFEPC
{
    #region Core definition
    public enum CommandType { Mandatory, Optional, Proprietary, Custom }

    public abstract class GS1Command : BenDotNet.RFID.UHFEPC.Command
    {
        public GS1Command() { }
        public abstract CommandType Type { get; }
        public abstract BitArray CommandCode { get; }
    }

    public abstract class GS1Reply : BenDotNet.RFID.UHFEPC.Reply
    {
        public GS1Reply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public GS1Reply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply) { }

        public const bool ERROR_HEADER = true;
        public const bool SUCCESS_HEADER = false;
        /// <summary>
        /// The header for an error code is a 1-bit, unlike the header for a success response which is a 0-bit
        /// </summary>
        public virtual bool Header => SUCCESS_HEADER;
    }

    public class ErrorReply : GS1Reply
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

    public class DelayedTagReply : GS1Reply
    {
        public DelayedTagReply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public DelayedTagReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }
    }
    public class InProcessTagReply : GS1Reply
    {
        public InProcessTagReply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public InProcessTagReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }
    }
    #endregion

    #region Select commands
    public abstract class BaseSelectCommand : GS1Command
    {
    }

    public class SelectCommand : BaseSelectCommand
    {
        public override CommandType Type => CommandType.Mandatory;
        
        public static bool[] SELECT_COMMAND_CODE = new bool[] { true, false, true, false };
        public override BitArray CommandCode => new BitArray(SELECT_COMMAND_CODE);
    }
    #endregion

    #region Inventory commands
    public abstract class BaseInventoryCommand : GS1Command
    {

    }

    public abstract class BaseQueryCommand : BaseInventoryCommand
    {
        public enum Session
        {
            S0 = 0b00,
            S1 = 0b01,
            S2 = 0b10,
            S3 = 0b11
        }
    }
    public class QueryReply : GS1Reply
    {
        public QueryReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public QueryReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    [ReplyType(typeof(QueryReply))]
    public class QueryCommmand : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERY_COMMAND_CODE = new bool[] { true, false, false, false };
        public override BitArray CommandCode => new BitArray(QUERY_COMMAND_CODE);
    }

    [ReplyType(typeof(QueryReply))]
    public class QueryAdjustCommand : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERYADJUST_COMMAND_CODE = new bool[] { true, false, false, true };
        public override BitArray CommandCode => new BitArray(QUERYADJUST_COMMAND_CODE);
    }

    [ReplyType(typeof(QueryReply))]
    public class QueryRepCommmand : BaseQueryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] QUERYREP_COMMAND_CODE = new bool[] { false, false };
        public override BitArray CommandCode => new BitArray(QUERYREP_COMMAND_CODE);
    }

    public class ACKCommand : BaseInventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public static bool[] ACK_COMMAND_CODE = new bool[] { false, true };
        public override BitArray CommandCode => new BitArray(ACK_COMMAND_CODE);
    }
    public class ACKReply : GS1Reply
    {
        public ACKReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public ACKReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    public class NCKCommand : BaseInventoryCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte NCK_COMMAND_CODE = 0b11000000;
        public override BitArray CommandCode => new BitArray(NCK_COMMAND_CODE);
    }
    #endregion

    #region Access commands
    public abstract class BaseAccessCommand : GS1Command
    {
    }

    [ReplyType(typeof(ReqRNReply))]
    public class ReqRNCommand : BaseAccessCommand
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

    public abstract class BaseMemoryAccessCommand : BaseAccessCommand
    {
        public BaseMemoryAccessCommand(GS1Tag.MemoryBank memoryBank) { this.MemoryBank = memoryBank; }
        public BaseMemoryAccessCommand(GS1Tag.MemoryBank memoryBank, int offset) { this.MemoryBank = memoryBank; this.Offset = offset; }
        public readonly GS1Tag.MemoryBank MemoryBank;

        public int Offset;
        public byte[] WordPtr
        {
            get => Helpers.CompileExtensibleBitVector(this.Offset);
            set => this.Offset = Helpers.ParseExtensibleBitVector(value);
        }
    }

    [ReplyType(typeof(ReadReply))]
    public class ReadCommand : BaseMemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;
        public const byte READ_COMMAND_CODE = 0b11000010;
        public override BitArray CommandCode => new BitArray(READ_COMMAND_CODE);

        public ReadCommand(GS1Tag.MemoryBank memoryBank, byte wordCount = 0) : base(memoryBank) { this.WordCount = wordCount; }
        public ReadCommand(GS1Tag.MemoryBank memoryBank, int offset, byte wordCount) : base(memoryBank, offset) { this.WordCount = wordCount; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// If 0, read the whole bank
        /// </remarks>
        public readonly byte WordCount;
    }
    public class ReadReply : GS1Reply
    {
        static ReadReply() { AssociatedCommandType = typeof(ReadCommand); }
        public ReadReply(ref object originalReply, ref RFID.Command associatedCommand) : base(ref associatedCommand, ref originalReply) { }

        public char[] MemoryWords;
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class WriteCommand : BaseMemoryAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;
        public const byte WRITE_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(WRITE_COMMAND_CODE);

        public WriteCommand(GS1Tag.MemoryBank memoryBank, ref char data, int offset = 0) : base(memoryBank, offset) { this.Data = data; }

        public readonly char Data;
    }

    [ReplyType(typeof(ReqRNReply))]
    public class KillCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte KILL_COMMAND_CODE = 0b11000100;
        public override BitArray CommandCode => new BitArray(KILL_COMMAND_CODE);
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class LockCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Mandatory;

        public const byte LOCK_COMMAND_CODE = 0b11000101;
        public override BitArray CommandCode => new BitArray(LOCK_COMMAND_CODE);
    }

    [ReplyType(typeof(ReqRNReply))]
    public class AccessCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte ACCESS_COMMAND_CODE = 0b11000110;
        public override BitArray CommandCode => new BitArray(ACCESS_COMMAND_CODE);
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class BlockWriteCommand : BaseMemoryAccessCommand
    {
        public override CommandType Type => CommandType.Optional;
        public const byte BLOCKWRITE_COMMAND_CODE = 0b11000111;
        public override BitArray CommandCode => new BitArray(BLOCKWRITE_COMMAND_CODE);

        public BlockWriteCommand(GS1Tag.MemoryBank memoryBank, ref char[] data, int offset = 0) : base(memoryBank, offset) { this.Data = data; }

        public byte WordCount { get { return (byte)this.Data.Length; } }
        public readonly char[] Data;
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class BlockEraseCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11001000;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }
    
    public class BlockPermalockCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKPERMALOCK_COMMAND_CODE = 0b11001001;
        public override BitArray CommandCode => new BitArray(BLOCKPERMALOCK_COMMAND_CODE);
    }

    public class ReadBufferCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11010010;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    [ReplyType(typeof(DelayedTagReply))]
    public class UntraceableCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000000;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileOpenCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte BLOCKERASE_COMMAND_CODE = 0b11010011;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileListCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000001;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }
    public class FileListReply : GS1Reply
    {
        public FileListReply(ref RFID.Command associatedCommand) : base(ref associatedCommand)
        {
        }

        public FileListReply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply)
        {
        }
    }

    public class FilePrivilegeCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000100;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class FileSetupCommand : BaseAccessCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000101;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }
    #endregion
}
