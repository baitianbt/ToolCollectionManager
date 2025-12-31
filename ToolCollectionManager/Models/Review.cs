using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolCollectionManager.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int SoftwareId { get; set; }

        [ForeignKey(nameof(SoftwareId))]
        public virtual SoftwareItem Software { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
