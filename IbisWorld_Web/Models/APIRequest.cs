using static IbisWorld_Utility.SD;

namespace IbisWorld_Web.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public string? Uri { get; set; }
        public object? Data { get; set; }
    }
}
