namespace Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException (string entityName, object id)
            : base($"La entidad {entityName} con ID {id} no fue encontrada.") { }
    }
}
