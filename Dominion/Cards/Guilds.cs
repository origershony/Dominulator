﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Advisor
        : Card
    {
        public static Advisor card = new Advisor();

        private Advisor()
            : base("Advisor", Expansion.Guilds, coinCost: 4, isAction: true,  plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(3, gameState);
            Card cardType = gameState.players.PlayerLeft.actions.BanCardToDrawnIntoHandFromRevealedCards(gameState);
            if (!currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                throw new Exception("Must ban a card currently being revealed");
            }
            currentPlayer.MoveRevealedCardToDiscard(cardType, gameState);
            currentPlayer.MoveAllRevealedCardsToHand();
        }
    }

    public class Baker
        : Card
    {
        public static Baker card = new Baker();

        private Baker()
            : base("Baker", Expansion.Guilds, coinCost: 5, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(1);
        }

        public override void DoSpecializedSetupIfInSupply(GameState gameState)
        {
            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                player.AddCoinTokens(1);
            }
        }
    }

    public class Butcher
        : Card
    {
        public static Butcher card = new Butcher();

        private Butcher()
            : base("Butcher", Expansion.Guilds, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(2);            

            int coinCount = currentPlayer.actions.GetCoinAmountToUseInButcher(gameState);
            if (coinCount > currentPlayer.AvailableCoinTokens)
                throw new Exception("Tried to use too many coins");

            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: true);
            if (trashedCard == null)
                return;

            currentPlayer.AddCoinTokens(-coinCount);

            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) == trashedCard.CurrentCoinCost(currentPlayer) + coinCount &&
                        card.potionCost == trashedCard.potionCost,
                "Must gain a card costing exactly equal to the cost of the card trashed plus any coin spent");
        }
    }

    public class CandlestickMaker
        : Card
    {
        public static CandlestickMaker card = new CandlestickMaker();

        private CandlestickMaker()
            : base("Candlestick Maker", Expansion.Guilds, coinCost: 2, isAction: true, plusActions:1, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(1);
        }
    }

    public class Doctor
        : Card
    {
        public static Doctor card = new Doctor();

        private Doctor()
            : base("Doctor", Expansion.Guilds, coinCost: 3, isAction: true, canOverpay: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card cardType = currentPlayer.RequestPlayerNameACard(gameState);
            currentPlayer.RevealCardsFromDeck(3, gameState);

            while (currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                currentPlayer.MoveRevealedCardToTrash(cardType, gameState);
            }
            
            currentPlayer.RequestPlayerPutRevealedCardsBackOnDeck(gameState);
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            for (int i = 0; i < overpayAmount; ++i)
            {
                if (!currentPlayer.deck.Any())
                    break;

                currentPlayer.LookAtCardsFromDeck(1, gameState);

                DeckPlacement deckPlacement = currentPlayer.actions.ChooseBetweenTrashTopDeckDiscard(gameState, currentPlayer.CardsBeingLookedAt.SomeCard());

                switch (deckPlacement)
                {
                    case DeckPlacement.Trash:
                        currentPlayer.RequestPlayerTrashLookedAtCard(gameState);
                        break;
                    case DeckPlacement.Discard:
                        currentPlayer.MoveLookedAtCardsToDiscard(gameState);
                        break;
                    case DeckPlacement.Deck:
                        currentPlayer.MoveLookedAtCardToTopOfDeck();
                        break;
                    default:
                        throw new Exception("Invalid option");
                }
            }
        }
    }

    public class Herald
       : Card
    {
        public static Herald card = new Herald();

        private Herald()
            : base("Herald", Expansion.Guilds, coinCost: 4, isAction: true, canOverpay: true, plusActions:1, plusCards:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
            if (revealedCard == null)
                return;

            if (revealedCard.isAction)
            {
                currentPlayer.cardsBeingRevealed.RemoveCard(revealedCard);
                currentPlayer.DoPlayAction(revealedCard, gameState);
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            for (int i = 0; i < overpayAmount; ++i)
            {
                currentPlayer.RequestPlayerTopDeckCardFromDiscard(gameState, isOptional: false);
            }            
        }
    }
    
    public class Journeyman
        : Card
    {
        public static Journeyman card = new Journeyman();

        private Journeyman()
            : base("Journeyman", Expansion.Guilds, coinCost:5, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card namedCard = currentPlayer.RequestPlayerNameACard(gameState);

            int cardFoundCount = 0;
            while(true)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
                if (revealedCard == null)
                    break;
                if (revealedCard == namedCard)
                    continue;
                cardFoundCount++;
                if (cardFoundCount >= 3)
                    break;
            }
           
            currentPlayer.MoveRevealedCardsToHand(card => card != namedCard);
            currentPlayer.MoveRevealedCardsToDiscard(gameState);                
        }
    }


    public class Masterpiece
        : Card
    {
        public static Masterpiece card = new Masterpiece();

        private Masterpiece()
            : base("Masterpiece", Expansion.Guilds, coinCost: 3, isTreasure:true, canOverpay: true, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {            
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            currentPlayer.GainCardsFromSupply(gameState, Cards.Silver, overpayAmount);
        }
    }

    public class MerchantGuild
        : Card
    {
        public static MerchantGuild card = new MerchantGuild();

        private MerchantGuild()
            : base("Merchant Guild", Expansion.Guilds, coinCost: 5, plusCoins: 1, plusBuy:1, isAction:true)
        {
            this.doSpecializedActionOnBuyWhileInPlay = DoSpecializedActionOnBuyWhileInPlay;
        }

        private new void DoSpecializedActionOnBuyWhileInPlay(PlayerState currentPlayer, GameState gameState, Card boughtCard)
        {
            currentPlayer.AddCoinTokens(1);            
        }
    }

    public class Plaza
        : Card
    {
        public static Plaza card = new Plaza();

        private Plaza()
            : base("Plaza", Expansion.Guilds, coinCost: 4, isAction: true, plusActions: 2, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.isTreasure, isOptional: true))
            {
                currentPlayer.AddCoinTokens(1);
            }
        }
    }    

    public class Soothsayer
        : Card
    {
        public static Soothsayer card = new Soothsayer();

        private Soothsayer()
            : base("Soothsayer", Expansion.Guilds, coinCost: 5, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Gold.card, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (otherPlayer.GainCardFromSupply(Curse.card, gameState))
            {
                otherPlayer.DrawAdditionalCardsIntoHand(1, gameState);
            }
        }
    }

    public class StoneMason
        : Card
    {
        public static StoneMason card = new StoneMason();

        private StoneMason()
            : base("StoneMason", Expansion.Guilds, coinCost: 2, isAction: true, canOverpay: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash => true, isOptional: false);
            if (card != null)
            {
                for (int i = 0; i < 2; ++i)
                {
                    currentPlayer.RequestPlayerGainCardFromSupply(
                        gameState,
                        acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) < card.CurrentCoinCost(currentPlayer) &&
                                          acceptableCard.potionCost <= card.potionCost,
                        "Must gain 2 cards less than the trashed card");
                }
            }
        }

        public override void OverpayOnPurchase(PlayerState currentPlayer, GameState gameState, int overpayAmount)
        {
            // todo:  over pay by potion
            // throw new NotImplementedException()
            for (int i = 0; i < 2; ++i)
            {
                currentPlayer.RequestPlayerGainCardFromSupply(
                    gameState,
                    acceptableCard => acceptableCard.isAction && acceptableCard.CurrentCoinCost(currentPlayer) == overpayAmount,
                    "Must gain 2 action cards costing the amount overpaid");
            }
        }
    }

    public class Taxman
       : Card
    {
        public static Taxman card = new Taxman();

        private Taxman()
            : base("Taxman", Expansion.Guilds, coinCost: 4, isAction: true, isAttack: true, attackDependsOnPlayerChoice: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => acceptableCard.isTreasure, isOptional: true);

            PlayerState.AttackAction attackAction = this.DoEmptyAttack;

            if (trashedCard != null)
            {
                attackAction = delegate(PlayerState currentPlayer2, PlayerState otherPlayer, GameState gameState2)
                {
                    if (otherPlayer.Hand.Count >= 5)
                    {
                        otherPlayer.DiscardCardFromHand(gameState, trashedCard);
                    }
                };

                currentPlayer.RequestPlayerGainCardFromSupply(gameState,
                    acceptableCard => acceptableCard.isTreasure && 
                                      acceptableCard.CurrentCoinCost(currentPlayer) <= trashedCard.CurrentCoinCost(currentPlayer) + 3 && 
                                      acceptableCard.potionCost == 0,
                    "Gain a card costing up to 3 more than the trashed card",
                    isOptional: false,
                    defaultLocation: DeckPlacement.TopOfDeck);
            }

            currentPlayer.AttackOtherPlayers(gameState, attackAction);
        }
       
    }   
}