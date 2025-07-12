using System.Threading.Tasks;
using Wallet.Model;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.DTOs.Filter;

namespace Wallet.Interfaces
{
    public interface IBalanceService
    {
        Task<ResponseModel<BalanceDto>> AddBalanceAsync(int clientId, decimal amount);
        Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto transferRequest);
        Task<ResponseModel<PaginatedResultDto<TransactionHistoryModel>>> GetTransactionHistoryAsync(int clientId, int pageNumber, int pageSize, string? type = null, TransactionFilterDateDTO? filter = null);

    }
}
