using Homebound.Services;
using NeighbourhoodJam2020.Data;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeighbourhoodJam2020.UserInterface
{
    /// <summary>
    /// Visualizes the card on the canvas.
    /// </summary>
    public class CardUi : MonoBehaviour
    {
        [SerializeField]
        private Image _cardImage;

        [SerializeField]
        private TMP_Text _text, _choiceText;

        private Card _currentCard;
        private Action<Choice, ChoiceDirection, float> _onChoiceSelected;

        private TextVarientCollection _cardText;

        [SerializeField]
        private Transform _choiceTransform;

        private bool _choiceShowing = false;
        [SerializeField]
        private float _choiceShowTime = 0.1f;

        private void OnValidate()
        {
            Debug.Assert(_text, "Text field is null.");
            Debug.Assert(_cardImage, "Card image is null.");
        }

        private void Awake()
        {
            _choiceTransform.localScale = new Vector3(1, 0, 1);
        }

        /// <summary>
        /// Attaches the callback for card selection.
        /// </summary>
        public void AttachCallbacks(Action<Choice, ChoiceDirection, float> onChoiceSelected)
        {
            if (onChoiceSelected == null) throw new ArgumentException("On Choice Selected callback parameter is null.");
            _onChoiceSelected = onChoiceSelected;
        }

        /// <summary>
        /// Draws the passed card onto the ui game object.
        /// </summary>
        public void DrawCard(Card card)
        {
            if (card == null) throw new ArgumentException("Card can't be null.");
            _currentCard = card;
            TextVarientCollection texts = card.GetTextVarientCollection();
            _cardText = texts;
            _text.text = card.Image == null ? texts.text : string.Empty;
            _cardImage.color = new Color(1, 1, 1, card.Image != null ? 1 : 0);
            _cardImage.sprite = card.Image;
        }

        /// <summary>
        /// Destroys itself.
        /// </summary>
        public void RemoveCard()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Select the choice for the player.
        /// </summary>
        public void SelectChoice(ChoiceDirection choiceDirection, float swipeTime = 0)
        {
            _onChoiceSelected.Invoke(choiceDirection == ChoiceDirection.LEFT ? _currentCard.LeftChoice : _currentCard.RightChoice, choiceDirection, swipeTime);
        }

        private Coroutine choiceCoroutine;

        public void ShowChoice(ChoiceDirection choice)
        {
            if (string.IsNullOrEmpty(_cardText.rightChoiceText) || string.IsNullOrEmpty(_cardText.leftChoiceText)) return;
            _choiceText.text = choice == ChoiceDirection.RIGHT ? _cardText.rightChoiceText : _cardText.leftChoiceText;
            _choiceText.alignment = choice == ChoiceDirection.RIGHT ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;

            if (!_choiceShowing)
            {
                _choiceShowing = true;
                if (choiceCoroutine != null) StopCoroutine(choiceCoroutine);
                choiceCoroutine = StartCoroutine(ChangeChoiceScale(1));
            }
        }

        public void HideChoice(bool force = false)
        {
            if (force)
            {
                _choiceShowing = false;
                _choiceText.text = string.Empty;
                _choiceTransform.localScale = new Vector3(1, 0, 1);
            }
            else if (_choiceShowing)
            {
                _choiceShowing = false;
                if (choiceCoroutine != null) StopCoroutine(choiceCoroutine);
                choiceCoroutine = StartCoroutine(ChangeChoiceScale(0));
            }
        }

        private IEnumerator ChangeChoiceScale(float target)
        {
            AudioService.Instance.PlayShowChoice();
            float startHeight = _choiceTransform.localScale.y;
            float t = 0;
            while (t < 1)
            {
                _choiceTransform.localScale = new Vector3(1, Mathf.Lerp(startHeight, target, t), 1);
                t += Time.deltaTime / _choiceShowTime;
                yield return null;
            }

            _choiceTransform.localScale = new Vector3(1, target, 1);
        }
    }
}