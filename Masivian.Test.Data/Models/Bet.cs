using System;
using System.ComponentModel.DataAnnotations;
namespace Masivian.Test.Data.Models
{
    public class Bet
    {
        public Guid Id { get; set; }
        [Range(minimum: 0, maximum: 36, ErrorMessage = "The number must be between 0 and 36.")]
        public int Number { get; set; }
        public string Color { get; set; }
        [Range(minimum: 0, maximum: 10000, ErrorMessage = "The amount to bet must be less than (USD)10.000")]
        public double Amount { get; set; }
        [Required]
        public Guid RouletteId { get; set; }
        public string IdUsuario { get; set; }
    }
}