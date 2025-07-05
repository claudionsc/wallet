
namespace Wallet.Model
{
    public class ResponseModel<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HttpStatusCode { get; set; } = true;
    }
}
