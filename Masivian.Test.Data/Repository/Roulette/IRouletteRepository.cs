using Masivian.Test.Data.Models;
using System;
using System.Collections.Generic;
namespace Masivian.Test.Data.Repository
{
    public interface IRouletteRepository
    {
        List<Roulette> GetAll();
        Roulette GetById(Guid rouletteId);
        bool CreateRoulette(Roulette roulette);
        bool OpenRoulette(Roulette roulette);
        bool CloseRoulette(Guid rouletteId);
    }
}