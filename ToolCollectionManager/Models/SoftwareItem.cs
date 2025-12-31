using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToolCollectionManager.Models
{
    public class SoftwareItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string IconPath { get; set; } = string.Empty;

        [Required]
        public string ExecutablePath { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string Developer { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }

        public virtual ICollection<Screenshot> Screenshots { get; set; } = new List<Screenshot>();

        public double Rating { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public DateTime InstallDate { get; set; } = DateTime.Now;

        public bool IsFavorite { get; set; }
    }
}