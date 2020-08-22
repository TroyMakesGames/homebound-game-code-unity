using NeighbourhoodJam2020.Data;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeighbourhoodJam2020.UserInterface
{
    /// <summary>
    /// Controls the Ui updating on screen for the time of day.
    /// </summary>
    public sealed class TimeProgressionUi : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _dayCounterText;

        [SerializeField]
        private Image _timeOfDayImage;

        [SerializeField]
        private Sprite _moriningSprite, _afternoonSprite, _eveningSprite;

        [SerializeField]
        private Color _morningColor, _afternoonColor, _eveningColor;

        [SerializeField]
        private Image _bannerImage;

        [SerializeField]
        private Image _bannerAnimator;
        private RectTransform _bannerAnimationRect;
        [SerializeField] private float _bannerAnimateTime = 0.1f;
        private Coroutine _bannerCoroutine;

        private Coroutine _textCoroutine;

        private void OnValidate()
        {
            Debug.Assert(_timeOfDayImage, "_timeOfDayImage Ui reference is null.");
            Debug.Assert(_dayCounterText, "DayCounterText Ui reference is null.");
            Debug.Assert(_moriningSprite, "_moriningSprite Ui reference is null.");
            Debug.Assert(_afternoonSprite, "_afternoonSprite Ui reference is null.");
            Debug.Assert(_eveningSprite, "_eveningSprite Ui reference is null.");
        }

        private void Awake()
        {
            _bannerAnimationRect = _bannerAnimator.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Updates the Ui on screen for the time of day and day count.
        /// </summary>
        public void SetTime(int day, TimeOfDay timeOfDay)
        {
            if (_textCoroutine != null) StopCoroutine(_textCoroutine);
            _textCoroutine = StartCoroutine(UpdateText(day,timeOfDay));

            if (_bannerCoroutine != null) StopCoroutine(_bannerCoroutine);
            _bannerCoroutine = StartCoroutine(UpdateColorAnimation(GetColor(timeOfDay)));
        }

        private Sprite GetSprite(TimeOfDay timeOfDay)
        {
            switch (timeOfDay)
            {
                default:
                case TimeOfDay.MORNING:
                    return _moriningSprite;
                case TimeOfDay.AFTERNOON:
                    return _afternoonSprite;
                case TimeOfDay.EVENING:
                    return _eveningSprite;
            }
        }

        private Color GetColor(TimeOfDay timeOfDay)
        {
            switch (timeOfDay)
            {
                default:
                case TimeOfDay.MORNING:
                    return _morningColor;
                case TimeOfDay.AFTERNOON:
                    return _afternoonColor;
                case TimeOfDay.EVENING:
                    return _eveningColor;
            }
        }

        private IEnumerator UpdateText(int day, TimeOfDay timeOfDay)
        {
            yield return new WaitForSeconds(_bannerAnimateTime / 2);
            _dayCounterText.text = $"DAY {day}";
            _timeOfDayImage.sprite = GetSprite(timeOfDay);
        }

        private IEnumerator UpdateColorAnimation(Color color)
        {
            _bannerAnimationRect.localScale = new Vector3(1, 0, 1);
            _bannerAnimator.color = color;
            _bannerAnimationRect.gameObject.SetActive(true);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / _bannerAnimateTime;
                _bannerAnimationRect.localScale = new Vector3(1 , Mathf.Lerp(0, 1, t) ,1);
                yield return null;
            }

            _bannerImage.color = color;
            _bannerAnimationRect.gameObject.SetActive(false);
        }
    }
}