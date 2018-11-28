using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.RFID.UHFEPC
{
    public class SecurityEnabledGS1Tag : GS1Tag
    {
        public SecurityEnabledGS1Tag(byte[] uid) : base(uid)
        {
            if (this.HasSecurityFlag != true)
                throw new ArgumentException("Not a security enabled GS1 tag");
        }

        #region Commands
        public class Challenge : SelectCommand
        {
            public override CommandType Type => CommandType.Optional;

            public const byte CHALLENGE_COMMAND_CODE = 0b11010100;
            public override BitArray CommandCode => new BitArray(CHALLENGE_COMMAND_CODE);
        }

        public class Authenticate : AccessCommand
        {
            public override CommandType Type => CommandType.Optional;

            public const byte AUTHENTICATE_COMMAND_CODE = 0b11010101;
            public override BitArray CommandCode => new BitArray(AUTHENTICATE_COMMAND_CODE);
        }
        #endregion
    }
}
