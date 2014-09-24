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
        public WinType Result { get; set; }
        public decimal Profit { get; set; }
        public decimal TotalProfit { get; set; }
        public string Message { get; set; }
    }
}
