using NeighbourhoodJam2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeighbourhoodJam2020.Gameplay
{
    /// <summary>
    /// Responable for manages the avaliable cards that can be drawn.
    /// </summary>
    public sealed class CardPool
    {
        private List<Card> _activeCardPool = new List<Card>();
        private List<Deck> _addedDecks = new List<Deck>();

        /// <summary>
        /// The last deck added to the card pool.
        /// </summary>
        public Deck MostRecentDeck { get => _addedDecks[_addedDecks.Count - 1]; }

        /// <summary>
        /// Creates a new card pool, needs a starting deck.
        /// </summary>
        public CardPool(Deck startingDeck)
        {
            if (startingDeck == null) throw new ArgumentException("Starting deck is null.");
            AddDeckToPool(startingDeck);
        }

        /// <summary>
        /// Adds the passed deck into the active pool.
        /// </summary>
        public void AddDeckToPool(Deck deck)
        {
            if (deck == null) throw new ArgumentException("Deck is null.");

            if (_addedDecks.Contains(deck))
            {
                Debug.Log($"Tried adding deck '{deck.name}' to card pool but it has already been added before.");
                return;
            }

            _addedDecks.Add(deck);
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                _activeCardPool.Add(deck.Cards[i]);
            }

            Debug.Log($"Added deck '{deck.name}'.");
        }

        /// <summary>
        /// Retrutns a random card and removes it from the active card pool.
        /// </summary>
        public Card PickupRandomCard(TimeOfDay timeOfDay, Stats stats)
        {
            Debug.Log($"Picking up random card for {timeOfDay}...");

            List<Card> possibleCards = new List<Card>();
            for (int i = 0; i < _activeCardPool.Count; i++)
            {
                if (_activeCardPool[i].AvaliableForPickup(timeOfDay, stats))
                    possibleCards.Add(_activeCardPool[i]);
            }

            if (possibleCards.Count == 0)
            {
                Debug.Log("No more cards remaining to draw.");
                return null;
            }

            Card randomCard = possibleCards[UnityEngine.Random.Range(0, possibleCards.Count)];
            Debug.Log($"Drew {randomCard.name}!");

            _activeCardPool.Remove(randomCard);
            return randomCard;
        }

        /// <summary>
        /// Adds individual cards to the card pool.
        /// </summary>
        public void AddToCardPool(Card[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null && !_activeCardPool.Contains(cards[i]))
                {
                    Debug.Log($"Adding card '{cards[i].name}' to the card pool.");
                    _activeCardPool.Add(cards[i]);
                }
            }
        }

        /// <summary>
        /// Remove these cards from the card pool.
        /// </summary>
        public void RemoveFromCardPool(Card[] cards)
        {
            if (cards == null) throw new ArgumentException("Cards array is null.");

            for (int y = 0; y < cards.Length; y++)
            {
                for (int x = _activeCardPool.Count - 1; x >= 0; x--)
                {
                    if (cards[y] == _activeCardPool[x])
                    {
                        Debug.Log($"Removing card '${cards[y].name} from active card pool.'");
                        _activeCardPool.RemoveAt(x);
                    }
                }
            }
        }

        /// <summary>
        /// Emptys the card pool completely.
        /// </summary>
        public void ClearCardPool()
        {
            Debug.Log("Cleared card pool.");
            _activeCardPool = new List<Card>();
        }
    }
}