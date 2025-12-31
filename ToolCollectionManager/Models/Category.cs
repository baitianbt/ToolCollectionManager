using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolCollectionManager.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Color { get; set; } = "#0078D4";

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual Category? Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; } = new List<Category>();

        public virtual ICollection<SoftwareItem> SoftwareItems { get; set; } = new List<SoftwareItem>();
    }
}
