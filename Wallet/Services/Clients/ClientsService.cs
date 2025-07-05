using Microsoft.EntityFrameworkCore;
using Wallet.Context;
using Wallet.DTOs.Client;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Services.Clients
{
    public class ClientsService : IClientsService
    {
        private readonly AppDbContext _context;
        public ClientsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<ClientResponseDTO>> UpdateClient(int id, ClientsDTO updatedUser)
        {
            var resposta = new ResponseModel<ClientResponseDTO>();
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(u => u.Id == id);
                if (client == null)
                {
                    resposta.Message = "Usuário não encontrado.";
                    resposta.HttpStatusCode = false;
                    return resposta;
                }

                client.Name = updatedUser.Name;
                client.Email = updatedUser.Email;
                client.Password = updatedUser.Password;

                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                resposta.Data = new ClientResponseDTO
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                };
                resposta.Message = "Usuário atualizado com sucesso.";
                resposta.HttpStatusCode = true;
            }
            catch (Exception ex)
            {
                resposta.Message = ex.Message;
                resposta.HttpStatusCode = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientsModel>> SearchClient(string email)
        {
            var resposta = new ResponseModel<ClientsModel>();
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == email);

                if (client == null)
                {
                    resposta.Message = "Nenhum registro localizado";
                    resposta.HttpStatusCode = false;
                    return resposta;
                }

                resposta.Data = client;
                resposta.Message = "Usuário exibido com sucesso";
                resposta.HttpStatusCode = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.HttpStatusCode = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientResponseDTO>> CreateClient(ClientsDTO clientsDto)
        {
            var resposta = new ResponseModel<ClientResponseDTO>();

            try
            {
                var clientEntity = new ClientsModel()
                {
                    Name = clientsDto.Name,
                    Email = clientsDto.Email,
                    Password = clientsDto.Password,
                };

                _context.Clients.Add(clientEntity);
                await _context.SaveChangesAsync();

                resposta.Data = new ClientResponseDTO
                {
                    Id = clientEntity.Id,
                    Name = clientEntity.Name,
                    Email = clientEntity.Email,
                };
                resposta.Message = "Usuário criado com sucesso";
                resposta.HttpStatusCode = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.HttpStatusCode = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientResponseDTO>> Login(ClientLoginDTO clientLoginDTO)
        {
            var resposta = new ResponseModel<ClientResponseDTO>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == clientLoginDTO.Email);

                if (client == null)
                {
                    resposta.HttpStatusCode = false;
                    resposta.Message = "Usuário não existe";
                    return resposta;
                }

                
                if (client.Password != clientLoginDTO.Password)
                {
                    resposta.HttpStatusCode = false;
                    resposta.Message = "Senha incorreta";
                    return resposta;
                }

                resposta.Data = new ClientResponseDTO
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    Balance = client.Balance
                };

                resposta.Message = "Login realizado com sucesso";
                resposta.HttpStatusCode = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.HttpStatusCode = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<ClientsModel>> DeleteClient(int id)
        {
            var resposta = new ResponseModel<ClientsModel>();

            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);

                if (client == null)
                {
                    resposta.Message = "Usuário não encontrado";
                    resposta.HttpStatusCode = false;
                    return resposta;
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                resposta.Message = "Usuário excluído com sucesso";
                resposta.HttpStatusCode = true;
            }
            catch (Exception e)
            {
                resposta.Message = e.Message;
                resposta.HttpStatusCode = false;
            }

            return resposta;
        }
    }
}
