﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic = System.Collections.Generic;

namespace Win8Client
{
    class CardConstraintMatcher
    {
        private readonly bool requirePlusBuy;
        private readonly bool requireVillage;
        private readonly bool requirePlusCard;
        private readonly bool requireTrashing;
        private readonly bool requireAttack;
        private readonly bool allowAttack;
        private readonly bool requireReactionWhenAttack;
        private readonly bool requireAtLeast3FromAnExpansion;
        private readonly Dominion.Card[] RequiredCards;
        private readonly Dominion.Card[] RestrictedCards;

        List<Dominion.Expansion> enabledExpansions;
        List<Dominion.Card> currentCards;
        List<Dominion.Card> missingRequiredCards;
        UniqueCardPicker uniqueCardPicker;

        public CardConstraintMatcher(
            bool requirePlusBuy,
            bool requireVillage,
            bool requirePlusCard,
            bool requireTrashing,
            bool requireAttack,
            bool requireReactionWhenAttack,
            bool requireAtLeast3FromAnExpansion,
            Dominion.Card[] requiredCards,
            Dominion.Card[] restrictedCards,
            Dominion.Card[] startingCards,
            IEnumerable<Dominion.Card> allCards
            )
        {
            this.requirePlusBuy = requirePlusBuy;
            this.requireVillage = requireVillage;
            this.requirePlusCard = requirePlusCard;
            this.requireTrashing = requireTrashing;
            this.requireAttack = requireAttack;
            this.requireReactionWhenAttack = requireReactionWhenAttack;
            this.requireAtLeast3FromAnExpansion = requireAtLeast3FromAnExpansion;
            this.RequiredCards = requiredCards;
            this.RestrictedCards = restrictedCards;

            this.enabledExpansions = new List<Dominion.Expansion>();
            this.currentCards = new List<Dominion.Card>();
            this.missingRequiredCards = new List<Dominion.Card>();
            this.uniqueCardPicker = new UniqueCardPicker(allCards);

            this.uniqueCardPicker.ExcludeCards(requiredCards);
            this.uniqueCardPicker.ExcludeCards(restrictedCards);
            this.uniqueCardPicker.ExcludeCards(startingCards);

            foreach (var card in startingCards)
                this.currentCards.Add(card);

            foreach(var card in requiredCards)
            {
                if (!this.currentCards.Contains(card))
                {
                    this.missingRequiredCards.Add(card);                    
                }
            }
        }

        bool CurrentConstraint(Dominion.Card card)
        {
            foreach(var current in RequiredCards)
            {
                if (!currentCards.Contains(current))
                {
                    return card == current;
                }
            }

            return true;
        }        

        public Dominion.Card GetCard()
        {
            if (this.missingRequiredCards.Count > 0)
            {
                var result = this.missingRequiredCards[this.missingRequiredCards.Count - 1];
                this.missingRequiredCards.RemoveAt(this.missingRequiredCards.Count - 1);                
                return result;
            }

            return null;
        }

        bool MatchesOtherConstraints(Dominion.Card card)
        {
            if (!this.allowAttack && card.isAttack)
                return false;
            if (!this.enabledExpansions.Contains(card.expansion))
                return false;

            return true;
        }

        bool MatchesPlusBuy(Dominion.Card card)
        {
            return card.plusBuy > 0;
        }

        bool MatchesVillage(Dominion.Card card)
        {
            return card.plusAction > 0;
        }

        bool MatchesPlusCard(Dominion.Card card)
        {
            return card.plusCard > 1;
        }

        bool MatchesAttack(Dominion.Card card)
        {
            return card.isAttack;
        }
    }

    class UniqueCardPicker
    {        
        Generic.Dictionary<Dominion.Card, bool> excludes;
        Dominion.Card[] allCards;
        int[] remainingCards;
        int maxIndex;

        public UniqueCardPicker(IEnumerable<Dominion.Card> allCards)
        {
            this.allCards = allCards.ToArray();
            this.remainingCards = new int[this.allCards.Length];
            for (int i = 0; i < remainingCards.Length; ++i)
                remainingCards[i] = i;

            this.maxIndex = remainingCards.Length - 1;

            this.excludes = new Generic.Dictionary<Dominion.Card, bool>();
        }

        public void ExcludeCards(Generic.IEnumerable<Dominion.Card> excludes)
        {
            foreach (var card in excludes)
                this.excludes[card] = true;
        }            

        public bool IsExcluded(Dominion.Card card)
        {
            bool unused;
            if (this.excludes.TryGetValue(card, out unused))
                return true;

            return false;
        }

        public Dominion.Card GetCard(Func<Dominion.Card, bool> meetConstraint)
        {
            int curIndex = this.maxIndex;
            while (curIndex >= 0)
            {
                int resultCardIndex = NumberBetweenInclusive(MainPage.random, 0, maxIndex);
                Dominion.Card currentCard = this.allCards[remainingCards[resultCardIndex]];                

                if (!IsExcluded(currentCard) && meetConstraint(currentCard))
                {
                    remainingCards[resultCardIndex] = remainingCards[maxIndex];
                    --maxIndex;
                    return currentCard;                    
                }
                --curIndex;    
            }

            return null;
        }

        private static int NumberBetweenInclusive(Random random, int lowerBoundInclusive, int upperBoundInclusive)
        {
            int count = upperBoundInclusive - lowerBoundInclusive + 1;
            return random.Next(count) + lowerBoundInclusive;
        }
    }    
}
