using DDAS.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDAS.Models.Entities
{
    public abstract class BaseEntity
    {

    }

    public class Entity<T> : BaseEntity, IEntity<T>
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual T RecId { get; set; }
    }
}