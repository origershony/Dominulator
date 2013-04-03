﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class Card
    {
        public readonly string name;
        private readonly int coinCost;
        public readonly int plusAction;
        public readonly int plusBuy;
        public readonly int plusCard;
        public readonly int plusCoin;
        public readonly int plusVictoryToken;
        public readonly int defualtSupplyCount;
        protected VictoryPointCounter victoryPointCounter;        // readonly
        public readonly bool isAction;
        public readonly bool isAttack;
        public readonly bool attackDependsOnPlayerChoice;
        public readonly bool isCurse;
        public readonly bool isReaction;
        public readonly bool isRune;
        public readonly bool isTreasure;

        internal Card(
            string name,
            int coinCost,
            int plusActions = 0,
            int plusBuy = 0,
            int plusCards = 0,
            int plusCoins = 0,
            VictoryPointCounter victoryPoints = null,
            int victoryTokenCount = 0,
            int defaultSupplyCount = 10,
            bool isAction = false,
            bool isAttack = false,
            bool attackDependsOnPlayerChoice = true,
            bool isCurse = false,
            bool isReaction = false,
            bool isRune = false,
            bool isTreasure = false)
        {
            this.name = name;
            this.coinCost = coinCost;
            this.plusAction = plusActions;
            this.plusBuy = plusBuy;
            this.plusCard = plusCards;
            this.plusCoin = plusCoins;
            this.victoryPointCounter = victoryPoints;
            this.plusVictoryToken = victoryTokenCount;
            this.isAction = isAction;
            this.isAttack = isAttack;
            this.attackDependsOnPlayerChoice = attackDependsOnPlayerChoice;
            this.isCurse = isCurse;
            this.isReaction = isReaction;
            this.isRune = isRune;
            this.isTreasure = isTreasure;
            this.defualtSupplyCount = defaultSupplyCount;
        }

        public bool Is(Type card)
        {
            return this.GetType().Equals(card);
        }

        public bool Is<T>()
            where T : Card
        {
            return this.Is(typeof(T));
        }

        public bool IsSameType(Card second)
        {
            return this.Is(second.GetType());
        }

        public bool isVictory
        {
            get
            {
                return this.victoryPointCounter != null;
            }
        }

        public int VictoryPoints(PlayerState playerState)
        {
            return this.victoryPointCounter(playerState);
        }

        public bool Equals(Card other)
        {
            return this.GetType() == other.GetType();
        }

        public int CurrentCoinCost(PlayerState player)
        {
            int effectiveCost = this.coinCost - player.cardCoinDiscount;
            return (effectiveCost >= 0) ? effectiveCost : 0;
        }

        virtual public void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
        }

        // return true if blocks attack;
        virtual public bool DoReactionToAttack(PlayerState currentPlayer, GameState gameState)
        {
            // by default, all cards are affected by attack
            return false;
        }

        virtual public void DoSpecializedTrash(PlayerState currentPlayer, GameState gameState)
        {
        }

        virtual public void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
        }

        virtual public void DoSpecializedCleanup(PlayerState currentPlayer, GameState gameState)
        {

        }
    }
}