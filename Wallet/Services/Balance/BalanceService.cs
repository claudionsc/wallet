using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            var response = new ResponseModel<BalanceDto>();

            try
            {
                if (amount <= 0)
                {
                    response.Success = false;
                    response.Message = "O valor adicionado deve ser maior que zero.";
                    return response;
                }

                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    response.Success = false;
                    response.Message = "Cliente não encontrado.";
                    return response;
                }

                client.Balance += amount;

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = client.Id,
                    Amount = amount,
                    Type = "add"
                });

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Saldo adicionado com sucesso.";
                response.Data = new BalanceDto { Balance = client.Balance };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao adicionar saldo: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<TransferResultDto>> TransferBalanceAsync(TransferRequestDto request)
        {
            var response = new ResponseModel<TransferResultDto>();

            try
            {
                if (request.Amount <= 0)
                {
                    response.Success = false;
                    response.Message = "O valor da transferência deve ser maior que zero.";
                    return response;
                }

                if (request.FromClientId == request.ToClientId)
                {
                    response.Success = false;
                    response.Message = "Não é possível transferir para a mesma conta.";
                    return response;
                }

                var fromClient = await _context.Clients.FindAsync(request.FromClientId);
                var toClient = await _context.Clients.FindAsync(request.ToClientId);

                if (fromClient == null || toClient == null)
                {
                    response.Success = false;
                    response.Message = "Cliente de origem ou destino não encontrado.";
                    return response;
                }

                if (fromClient.Balance < request.Amount)
                {
                    response.Success = false;
                    response.Message = "Saldo insuficiente para realizar a transferência.";
                    return response;
                }

                fromClient.Balance -= request.Amount;
                toClient.Balance += request.Amount;

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = fromClient.Id,
                    Amount = -request.Amount,
                    Type = "transfer",
                    ToClientId = toClient.Id
                });

                _context.TransactionHistories.Add(new TransactionHistoryModel
                {
                    ClientId = toClient.Id,
                    Amount = request.Amount,
                    Type = "transfer",
                    ToClientId = fromClient.Id
                });

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Transferência realizada com sucesso.";
                response.Data = new TransferResultDto
                {
                    FromClientId = fromClient.Id,
                    FromNewBalance = fromClient.Balance,
                    ToClientId = toClient.Id,
                    TransferredAmount = request.Amount
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao realizar transferência: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<PaginatedResultDto<TransactionHistoryModel>>> GetTransactionHistoryAsync(
            int clientId, int pageNumber, int pageSize, string? type = null)
        {
            var response = new ResponseModel<PaginatedResultDto<TransactionHistoryModel>>();

            try
            {
                var exists = await _context.Clients.AnyAsync(c => c.Id == clientId);
                if (!exists)
                {
                    response.Success = false;
                    response.Message = "Cliente não encontrado.";
                    return response;
                }

                var query = _context.TransactionHistories
                    .Where(t => t.ClientId == clientId);

                if (!string.IsNullOrEmpty(type))
                {
                    type = type.ToLower();
                    if (type != "add" && type != "transfer")
                    {
                        response.Success = false;
                        response.Message = "Tipo de transação inválido. Use 'add' ou 'transfer'.";
                        return response;
                    }

                    query = query.Where(t => t.Type == type);
                }

                var totalItems = await query.CountAsync();

                var transactions = await query
                    .OrderByDescending(t => t.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TransactionHistoryModel
                    {
                        Id = t.Id,
                        ClientId = t.ClientId,
                        Amount = t.Amount,
                        Type = t.Type,
                        ToClientId = t.ToClientId,
                        Timestamp = t.Timestamp
                    })
                    .ToListAsync();

                response.Success = true;
                response.Message = "Transações encontradas com sucesso.";
                response.Data = new PaginatedResultDto<TransactionHistoryModel>
                {
                    Items = transactions,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar transações: {ex.Message}";
            }

            return response;
        }
    }
}
