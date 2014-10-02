using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.BetActions.Enums
{
    [DataContract]
    public enum PercentOrFixed
    {
        [EnumMember]
        Percent,
        [EnumMember]
        Fixed,
        [EnumMember]
        Reset,
        [EnumMember]
        Multiply,
        [EnumMember]
        Exactly
    }
}
