using GoodTimeStudio.MyPhone.Extensions;
using GoodTimeStudio.MyPhone.OBEX;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodTimeStudio.MyPhone.Data
{
    public class Message : IIdentifiable<string>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = null!;

        public MessageStatus Status { get; set; }

        public string Type { get; set; } = null!;

        public string Folder { get; set; } = null!;

        public Contact Sender { get; set; } = null!;

        public string Body { get; set; } = null!;

        public static Message FromBMessage(string messageHandle, BMessage b)
        {
            return new Message
            {
                Body = b.Body,
                Status = b.Status == BMessageStatus.READ ? MessageStatus.Read : MessageStatus.Unread,
                Folder = b.Folder,
                Sender = b.Sender.ToContact(),
                Type = b.Type
            };
        }
    }

    public enum MessageStatus
    {
        Unread,
        Read
    }
}
