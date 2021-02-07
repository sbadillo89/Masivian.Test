using Masivian.Test.Data.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Masivian.Test.Data.Repository
{
    public class BetRepository : IBetRepository
    {
        readonly IRepository repository;
        readonly IRouletteRepository rouletteRepository;
        readonly string BetKey = Environment.GetEnvironmentVariable("BetKey");
        ResultBet resultBets = null;
        public BetRepository(IRepository repo, IRouletteRepository rouletteRepo)
        {
            this.repository = repo;
            this.rouletteRepository = rouletteRepo;
        }
        public bool CreateBet(Bet bet)
        {
            if (!IsValidRoulette(idRoulette: bet.RouletteId))
                throw new Exception(Utils.Utils.CreateMessageError(message: "La apuesta se intenta realizar en una ruleta que no existe o no esta abierta."));
            if (!ValidateColor(color: bet.Color))
                throw new Exception(Utils.Utils.CreateMessageError(message: "Los colores válidos son [Rojo] y [Negro]"));
            string betJson = JsonConvert.SerializeObject(bet);
            string _fieldKey = $"{bet.RouletteId}:{bet.Id}";

            return repository.Post(hashKey: BetKey, fieldKey: _fieldKey, betJson);
        }
        public ResultBet CloseBet(Guid rouletteID)
        {
            var betsRoulette = GetAllByRouletteId(rouletteId: rouletteID);
            rouletteRepository.CloseRoulette(rouletteId: rouletteID);
            SelectWinner(betsList: betsRoulette);

            return resultBets;
        }
        private bool ValidateColor(string color)
        {
            if (string.IsNullOrEmpty(color))
                return false;
            switch (color.ToLower())
            {
                case "negro":
                case "rojo":

                    return true;
                default:

                    return false;
            }
        }
        private bool IsValidRoulette(Guid idRoulette)
        {
            Roulette roulette = rouletteRepository.GetById(rouletteId: idRoulette);

            return (roulette.Status == "Abierta");
        }
        private List<Bet> GetAllByRouletteId(Guid rouletteId)
        {
            HashEntry[] entries = repository.Get(BetKey);
            if (entries.Length <= 0)
                return new List<Bet>();

            List<Bet> Bets = ConvertHashToList(entries: entries);

            return (Bets.Where(b => b.RouletteId == rouletteId).ToList());
        }
        private List<Bet> ConvertHashToList(HashEntry[] entries)
        {
            List<Bet> bets = new List<Bet>();
            for (int i = 0; i < entries.Length; i++)
            {
                string data = entries[i].Value;
                bets.Add(JsonConvert.DeserializeObject<Bet>(data));
            }

            return bets;
        }
        private void SelectWinner(List<Bet> betsList)
        {
            List<Bet> betsByNumber = betsList.Where(x => x.Number != 0).ToList();
            resultBets = new ResultBet();
            int index = new Random().Next(betsByNumber.Count);
            Bet winningBet = betsByNumber[index];
            string winningColor = (winningBet.Number % 2 == 0 ? "Rojo" : "Negro");
            resultBets.WinnigColor = winningColor;
            resultBets.WinningNumber = winningBet.Number;
            resultBets.Awards.AddRange(CalculateProfitNumber(betsList));
            resultBets.Awards.AddRange(CalculateProfitColor(betsList));
        }
        private List<Award> CalculateProfitNumber(List<Bet> betsList)
        {
            double GainFactorPerNumber = Convert.ToDouble(Environment.GetEnvironmentVariable("GainFactorPerNumber"));

            return betsList.Where(b => b.Number == resultBets.WinningNumber)
                   .Select(c => new Award()
                   {
                       Number = c.Number,
                       Amount = c.Amount,
                       Profit = c.Amount * GainFactorPerNumber
                   }).ToList();
        }
        private List<Award> CalculateProfitColor(List<Bet> betsList)
        {
            double GainFactorPerColor = Convert.ToDouble(Environment.GetEnvironmentVariable("GainFactorPerColor"));

            return betsList.Where(b => b.Color == resultBets.WinnigColor)
                   .Select(c => new Award()
                   {
                       Color = c.Color,
                       Amount = c.Amount,
                       Profit = c.Amount * GainFactorPerColor
                   }).ToList();
        }
    }
}