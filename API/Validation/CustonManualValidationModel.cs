namespace API.Validation
{
    public class CustonManualValidationModel
    {
        public CustonManualValidationModel()
        {
            this.Errors = new List<CustonValidationError>();
            this.Message = "Validation Failed";
        }

        public string Message { get; }

        public List<CustonValidationError> Errors { get; }
    }
}
