using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetFunctions.Sites;

namespace WebBot.BetActions.Interfaces
{
    public interface IActionType
    {
        DictionaryPropertyObject Properties { get; set; }
        DictionaryPropertyObject FiringParameters { get; set; }

        string GetName();
        void Execute(BaseSite site);
        bool CanFire();
    }
}
