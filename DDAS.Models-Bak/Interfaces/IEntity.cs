namespace DDAS.Models.Interfaces
{
    interface IEntity<T>
    {
        T RecId { get; set; }
    }
}
