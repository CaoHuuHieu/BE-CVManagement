namespace CVManagement.Exceptions
{
    public class EntityException : Exception
    {
        public EntityException(string? message) : base(message)
        {
        }        
        public EntityException()
        {
        }
    }
}
