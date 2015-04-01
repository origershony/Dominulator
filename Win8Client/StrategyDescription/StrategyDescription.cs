﻿
namespace Win8Client
{
    class StrategyDescription
    {
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> CardAcceptanceDescriptions { get; private set; }

        public StrategyDescription()
        {
            this.CardAcceptanceDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();            
        }

        public Dominion.Strategy.Description.StrategyDescription ConvertToDominionStrategy()
        {
            return new Dominion.Strategy.Description.StrategyDescription(
                new Dominion.Strategy.Description.PickByPriorityDescription(ConvertPurchaseOrderToDominionStrategy()));
        }

        private Dominion.Strategy.Description.CardAcceptanceDescription[] ConvertPurchaseOrderToDominionStrategy()
        {
            var result = new Dominion.Strategy.Description.CardAcceptanceDescription[this.CardAcceptanceDescriptions.Count];

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = CardAcceptanceDescriptions[i].ConvertToDominionStrategy();
            }

            return result;
        }
    }
}