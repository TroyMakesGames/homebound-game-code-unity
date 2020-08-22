using System;
using UnityEngine;

namespace NeighbourhoodJam2020.Data
{
    /// <summary>
    /// An event that occurs after a certain amount of cards after a certain time has passed.
    /// </summary>
    [Serializable]
    public class DeckEvent
    {
        [SerializeField]
        private string _eventName = "New Deck Event";

        [SerializeField, Header("How many 'time passes' to activate this event.")]
        private int _eventOccursAfter = 0;
        public int EventOccursAfter { get => _eventOccursAfter; }

        [SerializeField, Header("Force this card to be drawn (optional).")]
        private Card _forceDrawCard;
        public Card ForceDrawCard { get => _forceDrawCard; }

        [SerializeField, Header("Adds these cards to the deck when this event is triggured.")]
        private Card[] _addCardsToDeck = null;
        public Card[] AddCardsToDeck { get => _addCardsToDeck; }

        [SerializeField, Header("Clear the active card pool (USE WITH CAUTION!).")]
        private bool _flushCardPool;
        public bool FlushCardPool { get => _flushCardPool; }

        [SerializeField, Header("Add this deck to the active card pool (optional) (USE WITH CAUTION!).")]
        private Deck _addDeck;
        public Deck AddDeck { get => _addDeck; }

        /// <summary>
        /// Called in the editor by the Card class to validate this choice.
        /// </summary>
        public void OnValidate(Deck deck, int index)
        {
            Debug.Assert(_eventOccursAfter >= 0, $"Event occurs after.. is too long of a number in {deck.name} deck, index: {index}.");
            if (_flushCardPool) Debug.Assert(_addDeck != null, $"{deck.name} deck event {index} is flushing the card pool but NOT replacing it with another deck.");
            if (_addCardsToDeck != null)
            {
                for (int x = 0; x < _addCardsToDeck.Length; x++) Debug.Assert(_addCardsToDeck[x] != null, $"The {x}th card in the list cards to add in deck event '{_eventName}' is null.");
            }
        }
    }
}