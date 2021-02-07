using Masivian.Test.Data.Models;
using Masivian.Test.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Masivian.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        readonly IRouletteRepository repository;
        public RouletteController(IRouletteRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roulette>>> Get()
        {
            IEnumerable<Roulette> roulettes = null;
            await Task.Run(() =>
            {
                roulettes = repository.GetAll();
            });
            if (!roulettes.Any())
            {
                return NotFound();
            }

            return Ok(roulettes);
        }
        [HttpPost]
        public async Task<ActionResult<Roulette>> Post()
        {
            try
            {
                bool succes = false;
                Roulette roulette = new Roulette { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow };
                await Task.Run(() =>
                {
                    succes = repository.CreateRoulette(roulette: roulette);
                });
                if (!succes)
                    return BadRequest();

                return Ok(roulette.Id.ToString());
            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }
        }
        [HttpPost("Open/{rouletteId:Guid}")]
        public async Task<ActionResult<Roulette>> Open(Guid rouletteId)
        {
            try
            {
                bool open = false;
                Roulette currentRoulette = repository.GetById(rouletteId: rouletteId);
                currentRoulette.Status = "Abierta";
                await Task.Run(() =>
                {
                    open = !repository.OpenRoulette(roulette: currentRoulette);
                });
                if (open)
                {
                    return Ok(currentRoulette);
                }
                else
                { throw new Exception("No se pudo abrir la ruleta."); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}