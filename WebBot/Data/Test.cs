using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.Data
{
    public class Test : nsIDOMEventListener
    {
        public void HandleEvent(nsIDOMEvent @event)
        {
            Console.WriteLine("test");
        }
    }
}
