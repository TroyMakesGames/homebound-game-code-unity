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
    /// Controls the Ui updating on screen for the stats.
    /// </summary>
    public sealed class StatsUi : MonoBehaviour
    {
        [SerializeField]
        private Image _happinessUi, _energyUi, _productivityUi;

        [SerializeField]
        private Color _increaseColor, _decreaseColor;
        [SerializeField]
        private Color _defaultColor;

        [SerializeField]
        private float _transitionTimePerAmount = 0.1f;

        [SerializeField]
        private float _colorTransitionTime = 0.3f;

        [SerializeField]
        private float _imagePulseScale = 1.1f;

        [SerializeField]
        private float _imagePulseTime = 0.2f;

        private bool playFailSound = false;
        private bool playWinSound = false;

        private void OnValidate()
        {
            Debug.Assert(_happinessUi, "Hapiness Ui reference is null.");
            Debug.Assert(_energyUi, "Energy Ui reference is null.");
            Debug.Assert(_productivityUi, "Productivity Ui reference is null.");
        }

        /// <summary>
        /// Updates the Ui on screen for the stat values.
        /// </summary>
        public void SetStats(Stats stats)
        {
            playFailSound = false;
            playWinSound = false;

            float happinessPercentage = ((float)stats.happiness + 100) / 200;
            AdjustStatVisual(_happinessUi, happinessPercentage);

            float energyPercentage = ((float)stats.energy + 100) / 200;
            AdjustStatVisual(_energyUi, energyPercentage);

            float productivityPercentage = ((float)stats.productivity + 100) / 200;
            AdjustStatVisual(_productivityUi, productivityPercentage);

            if (playFailSound)
            {
                AudioService.Instance.PlayStatAtLowest();
            }
            else if (playWinSound)
            {
                AudioService.Instance.PlayStatAtHighest();
            }
        }

        private void AdjustStatVisual(Image image, float target)
        {
            if (image.fillAmount == target) return;

            if (target == 0)
            {
                playFailSound = true;
            } else if (target == 1)
            {
                playWinSound = true;
            }

            StartCoroutine(FillAnimation(image, target));
            StartCoroutine(UpdateColor(image, target));
        }

        private IEnumerator ScaleImage(Image image, float endScale)
        {
            Transform imageTransform = image.transform.parent;
            float startingScale = imageTransform.localScale.x;

            // Grow.
            float t = 0;
            while (t < 1)
            {
                float currentScale = Mathf.Lerp(startingScale, endScale, t);
                imageTransform.localScale = new Vector3(currentScale, currentScale, currentScale);
                t += Time.deltaTime / _imagePulseTime;
                yield return null;
            }
            imageTransform.localScale = new Vector3(endScale, endScale, endScale);

            // Shrink.
            if (endScale != 1) StartCoroutine(ScaleImage(image, 1));
        }

        private IEnumerator MorphColor(Image image, Color target)
        {
            Color startColor = image.color;

            // Transition to 
            float t = 0;
            while (t < 1)
            {
                image.color = Color.Lerp(startColor, target, t);
                t += Time.deltaTime / _colorTransitionTime;
                yield return null;
            }

            image.color = target;
        }

        private IEnumerator FillAnimation(Image image, float target)
        {
            float start = image.fillAmount;
            float t = 0;

            float distance = Mathf.Abs(start - target);
            float transitionTime = distance * _transitionTimePerAmount;

            //StartCoroutine(MorphColor(image, target > image.fillAmount ? _increaseColor : _decreaseColor));
            StartCoroutine(ScaleImage(image, _imagePulseScale));

            while (t < 1)
            {
                image.fillAmount = Mathf.Lerp(start, target, t);
                t += Time.deltaTime / transitionTime;
                yield return null;
            }
            image.fillAmount = target;

            //StartCoroutine(MorphColor(image, _defaultColor));
        }

        private IEnumerator UpdateColor(Image image, float target)
        {
            Color targetColor;
            if (target > 0.5f)
            {
                targetColor = Color.Lerp(_defaultColor, _increaseColor, (target - 0.5f) * 2);
            }
            else
            {
                targetColor = Color.Lerp(_decreaseColor, _defaultColor, target * 2);
            }

            float start = image.fillAmount;
            float distance = Mathf.Abs(start - target);
            float transitionTime = distance * _transitionTimePerAmount;

            Color startColor = image.color;
            float t = 0;
            while (t < 1)
            {
                image.color = Color.Lerp(startColor, targetColor, t);
                t += Time.deltaTime / transitionTime;
                yield return null;
            }

            image.color = targetColor;
        }
    }
}