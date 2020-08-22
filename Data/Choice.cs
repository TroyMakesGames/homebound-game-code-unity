using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NeighbourhoodJam2020.Data
{
    /// <summary>
    /// Each card has two choices which contain text and the consequences of that choice.
    /// </summary>
    [Serializable]
    public sealed class Choice
    {
        [SerializeField, Header("The text for this selection.")]
        private string _text = string.Empty;
        [SerializeField, Tooltip("Add here for extra varients (index matches card text index).")]
        private string[] _alternateText;

        [SerializeField, Header("Selecting this option will continue time.")]
        private bool _continueTime = true;
        public bool ContinueTime { get => _continueTime; }

        [Header("How much this choice effects the stats.")]
        [SerializeField, Range(-100, 100)]
        private int _energyEffect = 0;
        [SerializeField, Range(-100, 100)]
        private int _happinessEffect = 0;
        [SerializeField, Range(-100, 100)]
        private int _productivityEffect = 0;

        [SerializeField, Header("Which cards will this choice lead to? Leave empty for random card from pool.")]
        private List<Card> _nextCards = new List<Card>();


        [SerializeField, Header("Adds these cards to the deck when this choice is selected.")]
        private Card[] _addCardsOnChoiceSelect = null;
        public Card[] AddCardsOnChoiceSelect { get => _addCardsOnChoiceSelect; }

        [SerializeField, Header("Replace the deck?")]
        private Deck _addDeck = null;
        public Deck AddDeck { get => _addDeck; }

        [SerializeField, Header("Remove all the cards from active card pool.")]
        private bool _flushCardPool = false;
        public bool FlushCardPool { get => _flushCardPool; }

        /// <summary>
        /// Called in the editor by the Card class to validate this choice.
        /// </summary>
        public void OnValidate(string cardName, Sprite cardImage, ChoiceDirection direction, int alternateTextLength)
        {
            //Debug.Assert(cardImage == null || !string.IsNullOrEmpty(_text), $"Text is empty on {direction} choice on card '{cardName}' (card is NOT a text card).");
            for (int i = 0; i < _alternateText.Length; i++) Debug.Assert(!string.IsNullOrEmpty(_alternateText[i]), $"The {i}'s alternate choice text in '{cardName}'s card {direction} choice is null.");
            Debug.Assert(_nextCards != null, $"Next cards is null on {direction} choice on '{cardName}' Deck.");
            for (int i = 0; i < _nextCards.Count; i++) Debug.Assert(_nextCards[i] != null, $"There are null cards in '{cardName}'s {direction} choice card {i} is null.");
            //if (_flushCardPool && !endca) Debug.Assert(_addDeck != null, $"Flush deck is enabled on '{cardName}'s {direction} choice but there is no new deck to replace it.");
            Debug.Assert(alternateTextLength == (_alternateText != null ? _alternateText.Length : 0), $"Card '{cardName}'s {direction} alternate text choices list size does not match the cards alternate text choices.");
            if (_addCardsOnChoiceSelect != null)
            {
                for (int x = 0; x < _addCardsOnChoiceSelect.Length; x++) Debug.Assert(_addCardsOnChoiceSelect[x] != null, $"The {x}th card in the list cards to add on {direction} choice in '{cardName}' is null.");
            }
        }

        /// <summary>
        /// Gets the text for this choice, pass in an alt index for an alternate text.
        /// </summary>
        public string GetText(int? alternative = null)
        {
            if (alternative == null || _alternateText.Length == 0 || alternative >= _alternateText.Length) return _text;
            List<string> texts = _alternateText.ToList();
            texts.Add(_text);
            return texts[(int)alternative];
        }

        /// <summary>
        /// Returns a struct with the effects of this choice on the stats.
        /// </summary>
        public Stats GetStatEffects()
        {
            return new Stats(_energyEffect, _happinessEffect, _productivityEffect);
        }

        /// <summary>
        /// Returns the next card that should be played after this choice, null if random.
        /// </summary>
        public Card GetFollowupCard()
        {
            if (_nextCards == null || _nextCards.Count == 0) return null;
            return _nextCards[UnityEngine.Random.Range(0, _nextCards.Count)];
        }
    }
}