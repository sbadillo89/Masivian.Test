using Masivian.Test.Data.Models;
using Masivian.Test.Data.Repository;
using Masivian.Test.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
namespace Masivian.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        readonly IBetRepository betRepository;
        public BetController(IBetRepository repository)
        {
            this.betRepository = repository;
        }
        [HttpPost]
        public async Task<ActionResult<Bet>> Post([FromBody] Bet newBet)
        {
            try
            {
                bool success = false;
                Request.Headers.TryGetValue("userId", out var userId);
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(Utils.CreateMessageError(message: "No se encontró {userId} válido en la cabecera."));
                newBet.Id = Guid.NewGuid();
                newBet.IdUsuario = userId;
                await Task.Run(() =>
                {
                    success = betRepository.CreateBet(bet: newBet);
                });
                if (!success)
                    return BadRequest();

                return Ok(newBet);
            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }
        }
        [HttpPost("Close/{rouletteId:Guid}")]
        public async Task<ActionResult<Bet>> CloseBets(Guid rouletteId)
        {
            try
            {
                ResultBet result = null;
                await Task.Run(() =>
                {
                    result = betRepository.CloseBet(rouletteID: rouletteId);
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}