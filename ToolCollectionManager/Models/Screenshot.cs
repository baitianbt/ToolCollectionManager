using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolCollectionManager.Models
{
    public class Screenshot
    {
        [Key]
        public int Id { get; set; }

        public int SoftwareId { get; set; }

        [ForeignKey(nameof(SoftwareId))]
        public virtual SoftwareItem Software { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }
}
