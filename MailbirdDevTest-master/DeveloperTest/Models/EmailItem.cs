using System;

namespace DeveloperTest.Models
{
    public class EmailItem
    {
        public object Uid { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Body { get; set; }

        public EmailItem()
        {

        }

        public EmailItem(object uid, string from, string subject, DateTime? dateTime)
        {
            Uid = uid;
            From = from;
            Subject = subject;
            CreatedOn = dateTime;
        }
    }

}
