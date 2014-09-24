using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.BetActions.Enums
{
    [DataContract]
    public enum ProfitType
    {
        [EnumMember]
        Always,
        [EnumMember]
        Profit,
        [EnumMember]
        Loss,
        [EnumMember]
        EqualTo
    }
}
