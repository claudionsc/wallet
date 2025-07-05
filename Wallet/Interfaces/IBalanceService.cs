using System.Threading.Tasks;
using Wallet.Model;
using Wallet.DTOs;
using Wallet.DTOs.Balance;

namespace Wallet.Interfaces
{
    public interface IBalanceService
    {
        Task<ResponseModel<BalanceDto>> AddBalanceAsync(int clientId, decimal amount);
        Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto transferRequest);
    }
}
