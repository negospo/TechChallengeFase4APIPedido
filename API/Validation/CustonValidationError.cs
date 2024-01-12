namespace API.Validation
{
    public class CustonValidationError
    {
        public string Field { get; }

        public string Message { get; }

        public CustonValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}
