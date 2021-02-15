using System;
using Dms.Cms.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSUB_Remote_Monitor.Services
{
    internal class BidiSharedPipe : IBidiMessagePipe
    {
        public readonly SharedPipe Recv = new SharedPipe();
        public readonly SharedPipe Send = new SharedPipe();

        internal BidiSharedPipe()
        {
            this.Recv.MessageReceived += ReceiveMessageReceived;
            this.Recv.PipeDropped += ReceivePipeDropped;
        }

        #region IBidiMessagePipe Members

        public IMessagePipe SendPipe
        {
            get { return this.Send; }
        }

        public IMessagePipe ReceivePipe { get; set; }

        #endregion

        private bool ReceiveMessageReceived(Message message)
        {
            IMessagePipe receivePipe = this.ReceivePipe;

            return receivePipe != null && receivePipe.PostMessage(message);
        }

        private void ReceivePipeDropped(object sender, EventArgs e)
        {
            var receivePipe = this.ReceivePipe;

            if (receivePipe != null)
                receivePipe.DropPipe();
        }
    }
}
