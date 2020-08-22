using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeighbourhoodJam2020.UserInterface
{
    /// <summary>
    /// Scales the canvas for notchs and other BS.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class CanvasScaler : MonoBehaviour
    {
        private void Start()
        {
            Vector2 anchorMin = Screen.safeArea.position;
            Vector2 anchorMax = Screen.safeArea.position + Screen.safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            var rect = GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }
}