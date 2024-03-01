using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class MessageModel
    {
        [Key]
        public Guid MessageId { get; set; }
        /// <summary>
        /// メッセージ群の大元になる最初のメッセージのID
        /// </summary>
        public Guid GroupedMessageId { get; set; }
        /// <summary>
        /// 返信先メッセージのID
        /// </summary>
        public Guid? ReplyMessageId { get; set; }
        public Guid ToUserId { get; set; }
        public string Message { get; set; } = String.Empty;
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
