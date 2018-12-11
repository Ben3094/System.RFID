using System;
using System.Collections.Generic;
using System.Text;

namespace BenDotNet.RFID.UHFEPC
{
    public abstract class Command : BenDotNet.RFID.Command
    {
        public Command() { }
    }

    public abstract class Reply : BenDotNet.RFID.Reply
    {
        public Reply(ref RFID.Command associatedCommand) : base(ref associatedCommand) { }
        public Reply(ref RFID.Command associatedCommand, ref object originalReply) : base(ref associatedCommand, ref originalReply) { }
    }
}
