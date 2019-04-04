using System;
using System.Collections;

namespace BenDotNet.RFID.UHFEPC.GS1.Secured
{
    public class SecuredTag : Tag
    {
        public SecuredTag(byte[] uid) : base(uid)
        {
            if (this.HasSecurityFlag != true)
                throw new ArgumentException("Not a security enabled GS1 tag");
        }
    }

    #region Commands
    #region Select commands
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

    public class Challenge : BaseSelectCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte CHALLENGE_COMMAND_CODE = 0b11010100;
        public override BitArray CommandCode => new BitArray(CHALLENGE_COMMAND_CODE);

        /// <summary>
        /// Specifies whether a Tag concatenates response to its EPC when replying to an ACK
        /// </summary>
        public bool ImmediateEPC;

        /// <summary>
        /// Selects the cryptographic suite that Tag and Interrogator use for the Challenge
        /// </summary>
        public CryptographicSuiteAssigningAuthority CryptographicSuiteAssigningAuthority;
    }
    #endregion

    #region Security commands
    public abstract class SendResponseChoiceCommand : SecurityCommand
    {
    }

    public class Authenticate : SendResponseChoiceCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte AUTHENTICATE_COMMAND_CODE = 0b11010101;
        public override BitArray CommandCode => new BitArray(AUTHENTICATE_COMMAND_CODE);
    }

    public class AuthComm : SecurityCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte AUTHCOMM_COMMAND_CODE = 0b11010111;
        public override BitArray CommandCode => new BitArray(AUTHCOMM_COMMAND_CODE);
    }

    public class SecureComm : SendResponseChoiceCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const byte SECURECOMM_COMMAND_CODE = 0b11010110;
        public override BitArray CommandCode => new BitArray(SECURECOMM_COMMAND_CODE);
    }

    [ReplyType(typeof(InProcessTagReply))]
    public class KeyUpdate : SendResponseChoiceCommand
    {
        public override CommandType Type => CommandType.Optional;

        public const ushort BLOCKERASE_COMMAND_CODE = 0b1110001000000010;
        public override BitArray CommandCode => new BitArray(BLOCKERASE_COMMAND_CODE);
    }

    public class TagPrivilege : SendResponseChoiceCommand
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
    #endregion
    #endregion
}
