namespace Application.CustomExceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string? message) : base(message)
        {
        }
    }
}
