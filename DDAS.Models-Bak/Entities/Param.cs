using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities
{
    public class Param
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RecId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public int? ParId { get; set; }
        [ForeignKey("ParId")]
        public virtual Param ParParam { get; set; }

        public virtual ICollection<Artist> Artists { get; set; }

    }
}
