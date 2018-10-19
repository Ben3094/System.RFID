using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID
{
    public class Command
    {
    }

    public class Reply
    {
        public Reply(ref object originalReply, ref Command associatedCommand)
        {
            this.OriginalReply = originalReply;
            this.AssociatedCommand = associatedCommand;
        }

        public readonly object OriginalReply;
        public readonly Command AssociatedCommand;

        public static Type AssociatedCommandType = null;
    }
}
