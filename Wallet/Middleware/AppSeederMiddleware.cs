using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;
using Wallet.Context;
using Wallet.Model;

namespace Wallet.Middleware
{
    public class AppSeederMiddleware
    {
        private readonly RequestDelegate _next;

        public AppSeederMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = context.RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();

            if (!db.Clients.Any())
            {
                var clients = new List<ClientsModel>
                {
                    new ClientsModel
                    {
                        Name = "João Silva",
                        Email = "joao@email.com",
                        Password = HashPassword("123456"),
                        Balance = 1000.00m
                    },
                    new ClientsModel
                    {
                        Name = "Maria Souza",
                        Email = "maria@email.com",
                        Password = HashPassword("abcdef"),
                        Balance = 1500.00m
                    }
                };

                db.Clients.AddRange(clients);
                await db.SaveChangesAsync();

                // Transações de exemplo
                var transactions = new List<TransactionHistoryModel>
                {
                    new TransactionHistoryModel
                    {
                        ClientId = clients[0].Id,
                        Amount = 200.00m,
                        Type = "deposit"
                    },
                    new TransactionHistoryModel
                    {
                        ClientId = clients[1].Id,
                        ToClientId = clients[0].Id,
                        Amount = 100.00m,
                        Type = "transfer"
                    }
                };

                db.TransactionHistories.AddRange(transactions);
                await db.SaveChangesAsync();
            }

            await _next(context);
        }

        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
