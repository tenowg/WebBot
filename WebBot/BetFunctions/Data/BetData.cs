using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;

namespace WebBot.BetFunctions.Data
{
    public class BetData
    {
        public long CurrentBetNum { get; set; }
        public decimal BetAmount { get; set; }
        public decimal Chance { get; set; }
        public decimal Multiplier { get; set; }
        public decimal PossiblePayout { get; set; }
        public decimal TotalWagered { get; set; }
        public WinType Result { get; set; }
        public decimal Profit { get; set; }
        public decimal TotalProfit { get; set; }
        public string Message { get; set; }
        public decimal Balance { get; set; }
        public decimal PossibleProfit { get; set; }
    }
}
