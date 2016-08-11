using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDAS.Models.Entities.Domain
{
    public class Artist :  AuditEntity<long>
    {
        [Index("IX_ArtistName", 1, IsUnique = true)]
        [Required]
        [StringLength(100)]
        public string ArtistName { get; set; }

        [Required]
        public int GenderPId { get; set; }
        [ForeignKey("GenderPId"), Column(Order = 0)]
        public virtual Param GenderParam
        {
            get; set;
        }

        public int? YearOfBirth { get; set; }
        public int? YearOfDeath { get; set; }
    }
}
