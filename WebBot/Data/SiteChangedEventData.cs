using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetFunctions.Sites;

namespace WebBot.Data
{
    public class SiteChangedEventData : EventArgs
    {
        public BaseSite site;
    }
}
