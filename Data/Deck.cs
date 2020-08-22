using System.Collections.Generic;
using UnityEngine;

namespace NeighbourhoodJam2020.Data
{
    /// <summary>
    /// A collection of cards, that can be added to the card pool.
    /// </summary>
    [CreateAssetMenu(fileName = "Deck.asset", menuName = "Deck")]
    public sealed class Deck : ScriptableObject
    {
        [SerializeField, Header("Cards that can be randomly drawn from this deck.")]
        private List<Card> _cards = new List<Card>();
        public List<Card> Cards { get => _cards; }

        [SerializeField, Header("Events that trigger after this deck has been added.")]
        private List<DeckEvent> _deckEvents = new List<DeckEvent>();
        public List<DeckEvent> DeckEvents { get => _deckEvents; }

        private void OnValidate()
        {
            Debug.Assert(_cards != null, $"Cards is null on '{name}' Deck.");
            Debug.Assert(_cards.Count > 0, $"Deck '{name}' has no cards.");
            for (int i = 0; i < _cards.Count; i++) Debug.Assert(_cards[i] != null, $"Deck '{name}' card {i} is null.");
        }

        /// <summary>
        /// 
        /// </summary>
        public Card[] GetCardsForcedIntoDeck(int deckIndex)
        {
            for (int i = 0; i < _deckEvents.Count; i++)
            {
                if (_deckEvents[i].EventOccursAfter == deckIndex && _deckEvents[i].AddCardsToDeck != null & _deckEvents[i].AddCardsToDeck.Length >= 1)
                    return _deckEvents[i].AddCardsToDeck;
            }
            return null;
        }

        /// <summary>
        /// If a deck event is forcing a specific card at a specific deck index, return that card.
        public Card GetForcedCard(int deckIndex, Card currentCard)
        {
            for (int i = 0; i < _deckEvents.Count; i++)
            {
                if (_deckEvents[i].EventOccursAfter == deckIndex && _deckEvents[i].ForceDrawCard != null && _deckEvents[i].ForceDrawCard != currentCard)
                    return _deckEvents[i].ForceDrawCard;
            }
            return null;
        }

        /// <summary>
        /// If there is a deck event to add a new deck, return that.
        /// </summary>
        public Deck GetForcedAddDeck(int deckIndex)
        {
            for (int i = 0; i < _deckEvents.Count; i++)
            {
                if (_deckEvents[i].EventOccursAfter == deckIndex && _deckEvents[i].AddDeck != null)
                    return _deckEvents[i].AddDeck;
            }
            return null;
        }

        /// <summary>
        /// Returns true if there was a deck event that needs to flush the cards.
        /// </summary>
        public bool ForceFlushCards(int deckIndex)
        {
            for (int i = 0; i < _deckEvents.Count; i++)
            {
                if (_deckEvents[i].EventOccursAfter == deckIndex && _deckEvents[i].FlushCardPool)
                    return true;
            }
            return false;
        }
    }
}