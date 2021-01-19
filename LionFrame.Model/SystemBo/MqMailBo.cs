using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Model.SystemBo
{
    public class MqMailBo
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }

        public MailBo MailBo { get; set; }
        
    }
}
