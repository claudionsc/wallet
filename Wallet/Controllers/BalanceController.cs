using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wallet.DTOs;
using Wallet.DTOs.Balance;
using Wallet.Interfaces;
using Wallet.Model;

namespace Wallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        // POST: api/balance/add/1
        [HttpPost("add/{clientId}")]
        public async Task<ActionResult<ResponseModel<BalanceDto>>> AddBalance(int clientId, [FromBody] decimal amount)
        {
            var result = await _balanceService.AddBalanceAsync(clientId, amount);
            return result.HttpStatusCode ? Ok(result) : BadRequest(result);
        }

        // POST: api/balance/transfer
        [HttpPost("transfer")]
        public async Task<ActionResult<ResponseModel<TransferResultDto>>> TransferBalance([FromBody] TransferRequestDto request)
        {
            var result = await _balanceService.TransferBalanceAsync(request);
            return result.HttpStatusCode ? Ok(result) : BadRequest(result);
        }
    }
}
