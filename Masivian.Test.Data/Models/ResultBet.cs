using System.Collections.Generic;
namespace Masivian.Test.Data.Models
{
    public class ResultBet
    {
        public ResultBet()
        {
            Awards = new List<Award>();
        }
        public int WinningNumber { get; set; }
        public string WinnigColor { get; set; }
        public List<Award> Awards { get; set; }
    }
}