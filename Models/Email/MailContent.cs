using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MailContent
    {
        public string To { get; set; }              // Địa chỉ gửi đến
        public string Subject { get; set; }         // Chủ đề (tiêu đề email)
        public string Body { get; set; }
    }

    public class MailContentOTP : MailContent
    {
        public string OTP { get; set; }
    }

    public enum MailType
    {
        Unknown
    }
}
