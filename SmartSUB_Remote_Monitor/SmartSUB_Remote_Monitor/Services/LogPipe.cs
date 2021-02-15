using System;
using Dms.Cms.Messaging;

namespace SmartSUB_Remote_Monitor.Services
{
    internal class LogPipe : IMessagePipe
    {
        private readonly Action<string, string> log;
        private readonly string name;

        public LogPipe(string name, Action<string, string> log)
        {
            this.name = name;
            this.log = log;
        }

        #region IMessagePipe Members

        public bool PostMessage(Message message)
        {
            this.log(this.name, message.ToString());

            return true;
        }

        public void DropPipe()
        {
            this.log(this.name, "pipe dropped.");
        }

        #endregion
    }
}
