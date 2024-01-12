using Microsoft.AspNetCore.Mvc;

namespace API.Validation
{
    public class CustonManualValidationResult : ObjectResult
    {
        public CustonManualValidationResult() : base(new CustonManualValidationModel())
        {
            this.StatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        public List<CustonValidationError> Errors
        {
            get { return ((CustonManualValidationModel)this.Value).Errors; }
        }
    }
}
