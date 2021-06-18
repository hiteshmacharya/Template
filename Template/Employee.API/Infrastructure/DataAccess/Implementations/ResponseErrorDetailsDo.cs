using Upscript.Services.Employee.API.DataAccess.Interfaces;

namespace Employee.API.Infrastructure.DataAccess.Implementations
{
    public class ResponseErrorDetailsDO : IErrorDO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ReferenceID { get; set; }
    }
}
