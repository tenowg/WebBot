using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.BetActions.Enums
{
    [DataContract]
    public enum ActionValue
    {
        [EnumMember]
        Win,
        [EnumMember]
        Lose,
        [EnumMember]
        Bet
    }
}
