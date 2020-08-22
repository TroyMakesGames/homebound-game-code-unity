using UnityEditor;
using UnityEngine;

namespace Homebound.Editor
{
    public class PlayerPrefsResetter : MonoBehaviour
    {
        [MenuItem("Tools/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            Debug.Log("Wiped Player prefs! c:");
            PlayerPrefs.DeleteAll();
        }
    }
}