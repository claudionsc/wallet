using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Wallet.Context;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<BalanceDto>> AddBalanceAsync(int clientId, decimal amount)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
            {
                return new ResponseModel<BalanceDto>
                {
                    HttpStatusCode = false,
                    Message = "Cliente não encontrado",
                    Data = null
                };
            }

            client.Balance += amount;
            await _context.SaveChangesAsync();

            return new ResponseModel<BalanceDto>
            {
                HttpStatusCode = true,
                Message = "Saldo adicionado com sucesso",
                Data = new BalanceDto { Balance = client.Balance }
            };
        }

        public async Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto request)
        {
            if (request.Amount <= 0)
            {
                return new ResponseModel<TransferResultDto>
                {
                    HttpStatusCode = false,
                    Message = "O valor da transferência deve ser maior que zero",
                    Data = null
                };
            }

            if (request.FromClientId == request.ToClientId)
            {
                return new ResponseModel<TransferResultDto>
                {
                    HttpStatusCode = false,
                    Message = "Não é possível transferir para a mesma conta",
                    Data = null
                };
            }

            var fromClient = await _context.Clients.FindAsync(request.FromClientId);
            var toClient = await _context.Clients.FindAsync(request.ToClientId);

            if (fromClient == null || toClient == null)
            {
                return new ResponseModel<TransferResultDto>
                {
                    HttpStatusCode = false,
                    Message = "Cliente de origem ou destino não encontrado",
                    Data = null
                };
            }

            if (fromClient.Balance < request.Amount)
            {
                return new ResponseModel<TransferResultDto>
                {
                    HttpStatusCode = false,
                    Message = "Saldo insuficiente para realizar a transferência",
                    Data = null
                };
            }

            // Realiza a transferência
            fromClient.Balance -= request.Amount;
            toClient.Balance += request.Amount;

            await _context.SaveChangesAsync();

            var result = new TransferResultDto
            {
                FromClientId = fromClient.Id,
                FromNewBalance = fromClient.Balance,
                ToClientId = toClient.Id,
                ToNewBalance = toClient.Balance,
                TransferredAmount = request.Amount
            };

            return new ResponseModel<TransferResultDto>
            {
                HttpStatusCode = true,
                Message = "Transferência realizada com sucesso",
                Data = result
            };
        }
    }
}
