﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Curse : Card { public Curse() : base("Curse", coinCost: 0, isCurse: true) { } }
    public class Estate : Card { public Estate() : base("Estate", coinCost: 2, victoryPoints: playerState => 1) { } }
    
    public class Duchy 
        : Card 
    { 
        public Duchy() 
            : base("Duchy", coinCost: 5, victoryPoints: playerState => 3) 
        { 
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            if (gameState.DoesSupplyHaveCard<Duchess>())
            {
                currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.Is<Duchess>(), "may gain a duchess", isOptional: true);                
            }

            return DeckPlacement.Default;
        }
    }

    public class Province : Card { public Province() : base("Province", coinCost: 8, victoryPoints: playerState => 6) { } }
    public class Copper : Card { public Copper() : base("Copper", coinCost: 0, plusCoins: 1, isTreasure: true) { } }
    public class Silver : Card { public Silver() : base("Silver", coinCost: 3, plusCoins: 2, isTreasure: true) { } }
    public class Gold : Card { public Gold() : base("Gold", coinCost: 6, plusCoins: 3, isTreasure: true) { } }

    public class Festival : Card { public Festival() : base("Festival", coinCost: 5, plusActions: 2, plusBuy: 1, plusCoins: 2, isAction: true) { } }
    public class Laboratory : Card { public Laboratory() : base("Laboratory", coinCost: 5, plusCards: 2, plusActions: 1, isAction: true) { } }
    public class Market : Card { public Market() : base("Market", coinCost: 5, plusCards: 1, plusActions: 1, plusBuy: 1, plusCoins: 1, isAction: true) { } }
    public class Smithy : Card { public Smithy() : base("Smithy", coinCost: 4, plusCards: 3, isAction: true) { } }
    public class Village : Card { public Village() : base("Village", coinCost: 3, plusCards: 1, plusActions: 2, isAction: true) { } }
    public class WoodCutter : Card { public WoodCutter() : base("WoodCutter", coinCost: 3, plusBuy: 1, plusCoins: 2, isAction: true) { } }

    public class Adventurer
        : Card
    {
        public Adventurer()
            : base("Adventurer", coinCost: 6, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countTreasureFound = 0;
            while (countTreasureFound < 2)
            {
                Card card = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (card == null)
                {
                    break;
                }

                if (card.isTreasure)
                {
                    countTreasureFound += 1;
                    currentPlayer.MoveRevealedCardToHand(card);
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Bureaucrat :
       Card
    {
        public Bureaucrat()
            : base("Bureaucrat", coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply<Silver>(gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            CardPredicate acceptableCard = card => card.isVictory;
            Card cardTopDecked = otherPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard, isOptional: false);
            if (cardTopDecked == null)
            {
                otherPlayer.RevealHand();
            }
            else
            {
                otherPlayer.RevealCard(cardTopDecked, DeckPlacement.TopOfDeck);
            }
        }
    }

    public class Cellar
        : Card
    {
        public Cellar()
            : base("Cellar", coinCost: 2, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            int countCardsDiscarded = 0;
            while (!currentPlayer.hand.IsEmpty)
            {
                if (!currentPlayer.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: true))
                {
                    break;
                }
                ++countCardsDiscarded;
            }

            currentPlayer.DrawAdditionalCardsIntoHand(countCardsDiscarded);
        }
    }

    public class Chancellor
        : Card
    {
        public Chancellor()
            : base("Chancellor", coinCost: 3, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldPutDeckInDiscard(gameState))
            {
                currentPlayer.MoveDeckToDiscard();
            }
        }
    }

    public class Chapel
      : Card
    {
        public Chapel()
            : base("Chapel", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 4, isOptional: true);
        }
    }

    public class CouncilRoom
       : Card
    {
        public CouncilRoom()
            : base("CouncilRoom", coinCost: 5, plusCards: 4, plusBuy: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState player in gameState.players.OtherPlayers)
            {
                player.DrawAdditionalCardsIntoHand(1);
            }
        }
    }

    public class Feast
       : Card
    {
        public Feast()
            : base("Feast", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                card => card.CurrentCoinCost(currentPlayer) < 5
                , "cost of card < 5");
        }
    }

    public class Gardens
        : Card
    {
        public Gardens()
            : base("Gardens", coinCost: 4)
        {
            this.victoryPointCounter = delegate(PlayerState playerState)
            {
                return playerState.AllOwnedCards.Count() / 10;
            };
        }
    }

    public class Library
        : Card
    {
        public Library()
            : base("Library", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            while (currentPlayer.hand.Count < 7)
            {
                Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck();
                if (revealedCard == null)
                {
                    break;
                }

                bool putCardInHand = !revealedCard.isAction || currentPlayer.actions.ShouldPutCardInHand(gameState, revealedCard);

                if (putCardInHand)
                {
                    currentPlayer.MoveRevealedCardToHand(revealedCard);
                }
            }

            currentPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class Militia
        : Card
    {
        public Militia()
            : base("Militia", coinCost: 4, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
            }
        }
    }

    public class Mine
        : Card
    {
        public Mine()
            : base("Mine", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                        gameState,
                        card => card.isTreasure,
                        CostConstraint.UpTo,
                        3,
                        CardRelativeCost.RelativeCost,
                        isOptionalToTrash: false,
                        isOptionalToGain: false,
                        defaultLocation: DeckPlacement.Hand);
        }
    }

    public class Moat
        : Card
    {
        public Moat()
            : base("Moat", coinCost: 2, plusCards: 2, isAction: true)
        {
        }

        public override bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.RequestPlayerRevealCardFromHand(card => card.Is<CardTypes.Moat>(), gameState);
            currentPlayer.MoveRevealedCardToHand(revealedCard);
            return revealedCard != null;
        }
    }

    public class Moneylender
       : Card
    {
        public Moneylender()
            : base("Moneylender", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.TrashCardFromHandOfType(gameState, typeof(Copper), guaranteeInHand: false);
            if (card != null)
            {
                currentPlayer.AddCoins(3);
            }
        }
    }

    public class Remodel
       : Card
    {
        public Remodel()
            : base("Remodel", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                        gameState,
                        card => true,
                        CostConstraint.UpTo,
                        2,
                        CardRelativeCost.RelativeCost);
        }
    }

    public class Spy
      : Card
    {
        public Spy()
            : base("Spy", coinCost: 4, plusCards: 1, plusActions: 1, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            ApplySpyEffect(currentPlayer, currentPlayer, gameState);
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            ApplySpyEffect(otherPlayer, currentPlayer, gameState);
        }

        private static void ApplySpyEffect(PlayerState playerAffected, PlayerState decidingPlayer, GameState gameState)
        {
            Card revealedCard = playerAffected.DrawAndRevealOneCardFromDeck();
            if (revealedCard != null)
            {
                if (decidingPlayer.actions.ShouldPlayerDiscardCardFromDeck(gameState, playerAffected, revealedCard))
                {
                    playerAffected.MoveRevealedCardsToDiscard();
                }
                else
                {
                    playerAffected.MoveRevealedCardToTopOfDeck(revealedCard);
                }
            }
        }
    }

    public class Thief
      : Card
    {
        public Thief()
            : base("Thief", coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.RevealCardsFromDeck(2);

            Card cardtoTrash = null;
            CardPredicate acceptableCards = card => card.isTreasure;
            if (otherPlayer.cardsBeingRevealed.HasCard(acceptableCards))
            {
                Type cardTypeToTrash = currentPlayer.actions.GetCardFromRevealedCardsToTrash(gameState, otherPlayer, acceptableCards);

                cardtoTrash = otherPlayer.cardsBeingRevealed.RemoveCard(cardTypeToTrash);
                if (cardtoTrash == null)
                {
                    throw new Exception("Must choose a revealed card to trash");
                }

                if (!acceptableCards(cardtoTrash))
                {
                    throw new Exception("Player Must choose a treasure card to trash");
                }

                otherPlayer.MoveCardToTrash(cardtoTrash, gameState);
            }

            if (cardtoTrash != null)
            {
                if (currentPlayer.actions.ShouldGainCard(gameState, cardtoTrash))
                {
                    Card cardToGain = gameState.trash.RemoveCard(cardtoTrash.GetType());
                    currentPlayer.GainCard(gameState, cardToGain, DeckPlacement.Discard);
                }
            }

            otherPlayer.MoveRevealedCardsToDiscard();
        }
    }

    public class ThroneRoom
        : Card
    {
        public ThroneRoom()
            : base("Throne Room", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Type actionTypeToPlay = currentPlayer.actions.GetActionFromHandToPlay(gameState, false);

            Card cardToPlay = currentPlayer.RequestPlayerChooseActionToRemoveFromHandForPlay(gameState, isOptional: false);
            if (cardToPlay != null)
            {
                currentPlayer.DoPlayAction(cardToPlay, gameState, countTimes: 2);
            }
        }
    }

    public class Witch
        : Card
    {
        public Witch()
            : base("Witch", coinCost: 5, plusCards: 2, isAction: true, isAttack: true)
        {

        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.GainCardFromSupply<Curse>(gameState);
        }
    }

    public class Workshop
        : Card
    {
        public Workshop()
            : base("Workshop", coinCost: 3, isAction: true)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4,
                "Card must cost up to 4");
        }
    }

}