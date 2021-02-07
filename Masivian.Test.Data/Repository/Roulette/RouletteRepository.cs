using Masivian.Test.Data.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Masivian.Test.Data.Repository
{
    public class RouletteRepository : IRouletteRepository
    {
        readonly IRepository repository;
        readonly string RouletteKey = Environment.GetEnvironmentVariable(variable: "RouletteKey");
        public RouletteRepository(IRepository repo)
        {
            this.repository = repo;
        }
        public Roulette GetById(Guid rouletteId)
        {
            string result = repository.GetById(key: RouletteKey, fieldId: rouletteId.ToString());
            if (string.IsNullOrEmpty(result))
                throw new Exception(Utils.Utils.CreateMessageError($"No se encontró una ruleta con este id:{rouletteId}"));

            return JsonConvert.DeserializeObject<Roulette>(result);
        }
        public List<Roulette> GetAll()
        {
            HashEntry[] result = repository.Get(key: RouletteKey);
            if (result.Length <= 0)
                return new List<Roulette>();

            return ConvertHashToList(entries: result);
        }
        public bool CreateRoulette(Roulette roulette)
        {
            string rouletteJson = JsonConvert.SerializeObject(roulette);

            return repository.Post(hashKey: RouletteKey, fieldKey: roulette.Id.ToString(), value: rouletteJson);
        }
        public bool OpenRoulette(Roulette roulette)
        {
            ValidateOpenRoulette(rouleteId: roulette.Id);

            return Create(roulette: roulette);
        }
        public bool CloseRoulette(Guid rouletteId)
        {
            var roulette = GetById(rouletteId: rouletteId);
            if (roulette.Status == "Cerrada")
                throw new Exception(Utils.Utils.CreateMessageError($"La rouleta {rouletteId} ya se encuentra cerrada."));
            roulette.Status = "Cerrada";

            return Create(roulette: roulette);
        }
        private List<Roulette> ConvertHashToList(HashEntry[] entries)
        {
            List<Roulette> roulettes = new List<Roulette>();
            for (int i = 0; i < entries.Length; i++)
            {
                string data = entries[i].Value;
                roulettes.Add(JsonConvert.DeserializeObject<Roulette>(data));
            }

            return roulettes;
        }
        private bool Create(Roulette roulette)
        {
            string rouletteJson = JsonConvert.SerializeObject(roulette);

            return repository.Post(hashKey: RouletteKey, fieldKey: roulette.Id.ToString(), value: rouletteJson);
        }
        private void ValidateOpenRoulette(Guid rouleteId)
        {
            List<Roulette> rouletes = GetAll();
            if (rouletes.Any())
            {
                Roulette rouletteOpenedExists = rouletes.FirstOrDefault(x => x.Status == "Abierta");
                if (rouletteOpenedExists.Id == rouleteId)
                    throw new Exception(Utils.Utils.CreateMessageError("Esta ruleta ya se encuentra abierta."));
                if (rouletteOpenedExists != null)
                    throw new Exception(Utils.Utils.CreateMessageError("Ya existe una ruleta abierta, cierrela antes de abrir una nueva."));
            }
        }
    }
}