using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.BetFunctions.Data
{
    public class BustCheck
    {
        private float _bet;
        private string _reason;
        private float _balance;
        private float _profit;

        public float Bet { get { return _bet; } }
        public string Reason { get { return _reason; } }
        public float Balance { get { return _balance; } }
        public float Profit { get { return _profit; } }

        public BustCheck(float bet, string reason, float balance, float profit)
        {
            _bet = bet;
            _reason = reason;
            _balance = balance;
            _profit = profit;
        }
    }
}
