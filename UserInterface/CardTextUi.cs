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
    public class CardTextUi : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Color _textColor;

        [SerializeField]
        private Color _invisibleColor;

        [SerializeField]
        private float _fadeInTime = 0.3f;

        [SerializeField]
        private float _textHeight = 0.1f;

        private Coroutine _textCorotutine;

        private Vector2 _original_anchorMin;
        private Vector2 _original_anchorMax;
        private Vector2 _high_anchorMin;
        private Vector2 _high_anchorMax;

        private RectTransform _textTransform;

        private Coroutine _removeCoroutine;

        private void OnValidate()
        {
            Debug.Assert(_text, "Text field is null.");
            _textColor = _text.color;
            _invisibleColor = new Color(_textColor.r, _textColor.g, _textColor.b, 0);

            _textTransform = _text.GetComponent<RectTransform>();
            _original_anchorMin = _textTransform.anchorMin;
            _original_anchorMax = _textTransform.anchorMax;

            _high_anchorMin = _original_anchorMin + new Vector2(0, _textHeight);
            _high_anchorMax = _original_anchorMax + new Vector2(0, _textHeight);
        }


        /// <summary>
        /// Draws the passed card onto the ui game object.
        /// </summary>
        public void DrawCard(Card card)
        {
            if (_removeCoroutine != null) StopCoroutine(_removeCoroutine);

            if (card == null) throw new ArgumentException("Card can't be null.");
            TextVarientCollection texts = card.GetTextVarientCollection();

            if (_text.text == texts.text) return;
            _text.text = string.Empty;
            if (card.Image == null) return;

            if (_textCorotutine != null) StopCoroutine(_textCorotutine);
            _textCorotutine = StartCoroutine(AnimateTextIn(texts.text));
        }

        private IEnumerator AnimateTextIn(string text)
        {
            //_text.color = _invisibleColor;
            _text.color = _textColor;
            _text.text = text;
            
            float t = 0;
            float y = _textTransform.position.y;

            while (t < 1)
            {
                //_text.color = Color.Lerp(_invisibleColor, _textColor, t);


                Vector2 currentAnchorMin = Vector2.Lerp(_high_anchorMin, _original_anchorMin, t);
                Vector2 currentAnchorMax = Vector2.Lerp(_high_anchorMax, _original_anchorMax, t);

                _textTransform.anchorMin = currentAnchorMin;
                _textTransform.anchorMax = currentAnchorMax;

                t += Time.deltaTime / _fadeInTime;
                yield return null;
            }

           _textTransform.anchorMin = _original_anchorMin;
           _textTransform.anchorMax = _original_anchorMax;

            _text.color = _textColor;
        }

        /// <summary>
        /// Destroys itself.
        /// </summary>
        public void RemoveCard()
        {
            if (_removeCoroutine != null) StopCoroutine(_removeCoroutine);
            _removeCoroutine = StartCoroutine(HideText());
        }

        private IEnumerator HideText()
        {
            float t = 0;

            while (t < .5f)
            {
                _text.color = Color.Lerp(_textColor, _invisibleColor, t*2);
                t += Time.deltaTime / _fadeInTime;
                yield return null;
            }

            _text.text = string.Empty;
        }
    }
}