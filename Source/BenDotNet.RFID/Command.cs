using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BenDotNet.RFID
{
    public abstract class Command
    {
    }

    public class Reply
    {
        public Reply(ref Command associatedCommand)
        {
            this.OriginalReply = null;
            this.AssociatedCommand = associatedCommand;
        }
        public Reply(ref Command associatedCommand, ref object originalReply)
        {
            this.AssociatedCommand = associatedCommand;
            this.OriginalReply = originalReply;
        }

        public readonly object OriginalReply;
        public readonly Command AssociatedCommand;

        public static Type AssociatedCommandType = null;

        //TODO : How to handle readonly reply received by the reader ?
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ReplyTypeAttribute : Attribute
    {
        public readonly Type PossibleReplyTypes;

        public ReplyTypeAttribute(Type possibleReplyTypes)
        {
            if (typeof(Reply).IsAssignableFrom(possibleReplyTypes) && (possibleReplyTypes != typeof(Reply)))
                this.PossibleReplyTypes = possibleReplyTypes;
            else
                throw new ArgumentException("Is not a Reply derived type");
        }
    }
}
