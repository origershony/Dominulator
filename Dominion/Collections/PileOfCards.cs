﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class PileOfCards
        : ListOfCards
    {
        private Card protoType;      

        public PileOfCards(CardGameSubset gameSubset, Card protoType, int count)
            : base(gameSubset)
        {
            this.AddNCardsToTop(protoType, count);           
            this.protoType = protoType;            
        }

        public PileOfCards(CardGameSubset gameSubset, Card protoType)
            : base(gameSubset)
        {
            this.protoType = protoType;
        }

        public bool IsType(Card card)
        {
            if (this.protoType.Is(card))
                return true;

            if (card.isRuins && this.protoType.Equals(Card.Type<CardTypes.Ruins>()))
                return true;            

            return false;
        }

        public bool IsType<T>()
            where T: Card, new()
        {
            return IsType(Card.Type<T>());
        }

        public Card ProtoTypeCard
        {
            get
            {
                return this.protoType;
            }
        }

    }
}
