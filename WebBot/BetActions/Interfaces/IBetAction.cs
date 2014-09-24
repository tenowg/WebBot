using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.BetActions.Interfaces
{
    public interface IBetAction
    {
        BetActionProperties BetActionProperties { get; set; }
    }
}
