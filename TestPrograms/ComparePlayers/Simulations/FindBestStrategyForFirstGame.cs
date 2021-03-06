﻿using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class FindBestStrategyForFirstGame
    {
        static void Run()
        {            
            StrategyOptimizer.FindBestStrategyForGame(GameSets.FirstGame);
        }      

        static void FindAndCompareBestStrategy(TestOutput testOutput)
        {
            EvaulateBestStrategyForFirstGame(testOutput);
            Run();

            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),                                   
        }

        static void EvaulateBestStrategyForFirstGame(TestOutput testOutput)
        {
            //FindBestStrategy currently finds the following, which is better than BigMoneySimple, but not as good as BigMoney
            //Province(1), Province, Gold, Market(1), Duchy(2), Militia(2), Silver, Estate(1),Workshop(1), Cellar(1),
            var player1 = new PlayerAction("Player 1",
                new CardPickByPriority(
                    CardAcceptance.For(Cards.Province),
                    CardAcceptance.For(Cards.Gold),
                    CardAcceptance.For(Cards.Market, gameState => Strategy.CountAllOwned(Cards.Market, gameState) < 1),
                    CardAcceptance.For(Cards.Duchy, gameState => Strategy.CountAllOwned(Cards.Duchy, gameState) < 2),
                    CardAcceptance.For(Cards.Militia, gameState => Strategy.CountAllOwned(Cards.Militia, gameState) < 2),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Estate, gameState => Strategy.CountAllOwned(Cards.Militia, gameState) < 1)
                    ));
            testOutput.ComparePlayers(player1, Strategies.BigMoneySimple.Player(), showVerboseScore: true);
            testOutput.ComparePlayers(player1, Strategies.BigMoney.Player(), showVerboseScore: true);
            testOutput.ComparePlayers(player1, Strategies.BigMoneySingleSmithy.Player(), showVerboseScore: true);
        }        
    }
}
