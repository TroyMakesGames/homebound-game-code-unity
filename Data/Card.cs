using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeighbourhoodJam2020.Data
{
    /// <summary>
    /// A card includes some text, and the choices the player can make.
    /// </summary>
    [CreateAssetMenu(fileName = "Card.asset", menuName = "Card")]
    public sealed class Card : ScriptableObject
    {
        [SerializeField, Header("The choice given to the player.")]
        private string _text = string.Empty;
        [SerializeField, Tooltip("Add here for extra varients.")]
        private string[] _alternateText;

        [SerializeField, Header("Card image (if null, card will be 'text card').")]
        private Sprite _image = null;
        public Sprite Image { get => _image; }

        [SerializeField, Header("The time of day this card can appear.")]
        private bool _morning = true;
        [SerializeField]
        private bool _afternoon = true;
        [SerializeField]
        private bool _evening = true;

        [SerializeField, Header("Stats required to draw this card.")]
        private StatsRequiredToBeDrawn _statsRequiredToBeDrawn;

        [SerializeField, Header("Remove other card from active pool.")]
        private Card[] _removeCardsOnDraw = null;
        public Card[] RemoveCardsOnDraw { get => _removeCardsOnDraw; }

        [SerializeField, Header("Adds these cards to the deck when this card is drawn.")]
        private Card[] _addCardsOnDraw = null;
        public Card[] AddCardsOnDraw { get => _addCardsOnDraw; }

        [SerializeField, Header("Mark this if this card fails the game.")]
        private bool _failCard = false;
        public bool FailCard { get => _failCard; }

        [SerializeField, Header("The choices the player has.")]
        private Choice _leftChoice;
        public Choice LeftChoice { get => _leftChoice; }

        [SerializeField]
        private Choice _rightChoice;
        public Choice RightChoice { get => _rightChoice; }

        [Serializable]
        private class StatsRequiredToBeDrawn
        {
            [SerializeField, Range(-100, 100), Header("Happiness")]
            private int _minHappiness = -100;
            public int MinHappiness { get => _minHappiness; }
            [SerializeField, Range(-100, 100)]
            private int _maxHappiness = 100;
            public int MaxHappiness { get => _maxHappiness; }

            [SerializeField, Range(-100, 100), Header("Energy")]
            private int _minEnergy = -100;
            public int MinEnergy { get => _minEnergy; }
            [SerializeField, Range(-100, 100)]
            private int _maxEnergy = 100;
            public int MaxEnergy { get => _maxEnergy; }

            [SerializeField, Range(-100, 100), Header("Productivity")]
            private int _minProductivity = -100;
            public int MinProductivity { get => _minProductivity; }
            [SerializeField, Range(-100, 100)]
            private int _maxProductivity = 100;
            public int MaxProductivity { get => _maxProductivity; }

            /// <summary>
            /// Called in the editor by the Card class to validate this choice.
            /// </summary>
            public void OnValidate(string cardName)
            {
                Debug.Assert(_minHappiness <= _maxHappiness, $"Card '{cardName}' min stats required for happiness is HIGHER and max stats required.");
                Debug.Assert(_minEnergy <= _maxEnergy, $"Card '{cardName}' min stats required for Energy is HIGHER and max stats required.");
                Debug.Assert(_minProductivity <= _maxProductivity, $"Card '{cardName}' min stats required for Productivity is HIGHER and max stats required.");
            }
        }

        private void OnValidate()
        {
            //Debug.Assert(!string.IsNullOrEmpty(_text), $"Text is empty on card '{name}'.");
            for (int i = 0; i < _alternateText.Length; i++) Debug.Assert(!string.IsNullOrEmpty(_alternateText[i]), $"The {i}'s alternate card text in '{name}'s card  is null.");
            Debug.Assert(_morning || _afternoon || _evening, $"'{name}' Card needs at least one time of day.");
            Debug.Assert(_leftChoice != null, $"Left choice is null on card '{name}'.");
            Debug.Assert(_rightChoice != null, $"Right choice is null on card '{name}'.");
            if (_removeCardsOnDraw != null)
            {
                for (int x = 0; x < _removeCardsOnDraw.Length; x++) Debug.Assert(_removeCardsOnDraw[x] != null, $"The {x}th card in the list for removal of cards on draw in '{name}'s card is null.");
            }
            if (_addCardsOnDraw != null)
            {
                for (int x = 0; x < _addCardsOnDraw.Length; x++) Debug.Assert(_addCardsOnDraw[x] != null, $"The {x}th card in the list cards to add on draw in '{name}' is null.");
            }
            _leftChoice?.OnValidate(name, _image, ChoiceDirection.LEFT, _alternateText != null ? _alternateText.Length : 0);
            _rightChoice?.OnValidate(name, _image, ChoiceDirection.RIGHT, _alternateText != null ? _alternateText.Length : 0);
        }

        /// <summary>
        /// Gets the text varient for text, and choice text. Only call this one as it's random.
        /// </summary>
        public TextVarientCollection GetTextVarientCollection()
        {
            TextVarientCollection texts = new TextVarientCollection();

            if (_alternateText == null || _alternateText.Length == 0)
            {
                texts.text = _text;
                texts.rightChoiceText = _rightChoice.GetText();
                texts.leftChoiceText = _leftChoice.GetText();
            }
            else
            {
                List<string> possibleTexts = _alternateText.ToList();
                possibleTexts.Add(_text);
                int textIndex = UnityEngine.Random.Range(0, possibleTexts.Count);
                texts.text = possibleTexts[textIndex];
                texts.rightChoiceText = _rightChoice.GetText(textIndex);
                texts.leftChoiceText = _leftChoice.GetText(textIndex);
            }

            return texts;
        }

        /// <summary>
        /// Can this card be picked up at the given time of day.
        /// </summary>
        public bool AvaliableForPickup(TimeOfDay timeOfDay, Stats stats)
        {
            bool timeOfDayCorrect = (timeOfDay == TimeOfDay.MORNING && _morning)
            || (timeOfDay == TimeOfDay.AFTERNOON && _afternoon)
            || (timeOfDay == TimeOfDay.EVENING && _evening);

            if (!timeOfDayCorrect) return false;

            if (stats.energy < _statsRequiredToBeDrawn.MinEnergy) return false;
            if (stats.happiness < _statsRequiredToBeDrawn.MinHappiness) return false;
            if (stats.productivity < _statsRequiredToBeDrawn.MinProductivity) return false;

            if (stats.energy > _statsRequiredToBeDrawn.MaxEnergy) return false;
            if (stats.happiness > _statsRequiredToBeDrawn.MaxHappiness) return false;
            if (stats.productivity > _statsRequiredToBeDrawn.MaxProductivity) return false;

            return true;
        }
    }
}