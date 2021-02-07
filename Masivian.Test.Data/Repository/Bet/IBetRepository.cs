using Masivian.Test.Data.Models;
using System;
namespace Masivian.Test.Data.Repository
{
    public interface IBetRepository
    {
        bool CreateBet(Bet bet);
        ResultBet CloseBet(Guid rouletteID);
    }
}