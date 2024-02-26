using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class TrackingModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public int Key { get; set; }
        public string? Note { get; set; }
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Keyを「最終桁から2桁目まで」と「1桁目」に分け、前者を7で除した余りが後者と同じか確認する
        /// </summary>
        /// <returns></returns>
        public bool Check7DR()
        {
            int n = Key / 10;
            int x = Key % 10;
            return (n % 7) == x;
        }
    }

    public class TrackingModelDto
    {
        public int Key { get; set; }
        public string? Note { get; set; }
        public Guid CreatedBy { get; set; }

        public TrackingModel Create()
        {
            return new TrackingModel() { Key = Key, Note = Note, CreatedBy = CreatedBy };
        }
    }

    public enum Industory
    {
        Kuroneko = 0,
        Sagawa = 1,
        Nihon = 2,
        Seino = 3,
    }


}
