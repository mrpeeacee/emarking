using Newtonsoft.Json;

namespace Saras.eMarking.Domain.Entities.Exceptions
{
    public class ExceptionMessage
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ReferenceId { get; set; }
        public static bool IsError => true;
        public string Detail { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
