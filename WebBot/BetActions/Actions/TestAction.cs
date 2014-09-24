﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Sites;
using WebBot.Converters;

namespace WebBot.BetActions.Actions
{
    public class TestAction : CActionType
    {
        public TestAction() : base()
        {
        }

        public override string GetName()
        {
            return "Open to Edit (TestAction)";
        }

        public override void Execute(BaseSite site)
        {
            Console.WriteLine("Test Firing");
        }

        public override bool CanFire()
        {
            return base.CanFire();
        }
    }
}
