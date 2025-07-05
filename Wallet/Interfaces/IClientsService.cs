using Wallet.Model;
using Wallet.DTOs.Client;

namespace Wallet.Interfaces
{
    public interface IClientsService
    {
        Task<ResponseModel<ClientResponseDTO>> UpdateClient(int id, ClientsDTO updatedClient);
        Task<ResponseModel<ClientsModel>> SearchClient(string email);
        Task<ResponseModel<ClientResponseDTO>> CreateClient(ClientsDTO clientsDTO);
        
        Task<ResponseModel<ClientResponseDTO>> Login(ClientLoginDTO clientLoginDTO);
        
        Task<ResponseModel<ClientsModel>> DeleteClient(int id);
    }
}
