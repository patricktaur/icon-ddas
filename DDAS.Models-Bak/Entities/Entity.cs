using DDAS.Models.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDAS.Models.Entities
{
    public abstract class BaseEntity
    {

    }

    public class Entity<T> : BaseEntity, IEntity<T>
    {
      
       public virtual T RecId { get; set; }
    }
}