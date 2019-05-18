using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32UWPApp.UWP
{
    class SendingMessage
    {
        public SendingMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
        public string ReplyText { get; set; }
    }
}
