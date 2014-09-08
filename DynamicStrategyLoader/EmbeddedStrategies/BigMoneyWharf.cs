﻿using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{

    public class BigMoneyWharf
        : Strategy
    {
        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Wharf, "BigMoneyWharf");
        }
    }
}