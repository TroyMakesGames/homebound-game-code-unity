using UnityEngine;

namespace Homebound.Services
{
    public class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        [SerializeField]
        private AudioSource _sfxAudioSource;

        [SerializeField]
        private AudioSource _sfxAudioSource2;

        [SerializeField]
        private AudioSource _musicAudioSource;

        [SerializeField]
        private AudioClip _buttonClick, _cardFlip, _loseGame, _newDeck, _statLow, _statUp, _winGame;

        [SerializeField]
        private AudioClip[] _cardSwipe;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayShowChoice()
        {
            _sfxAudioSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxAudioSource.PlayOneShot(_buttonClick, 0.85f);
        }

        public void PlayCardFlip()
        {
            _sfxAudioSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxAudioSource.PlayOneShot(_cardFlip);
        }

        public void PlayCardSwipe()
        {
            _sfxAudioSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxAudioSource.PlayOneShot(_cardSwipe[Random.Range(0, _cardSwipe.Length - 1)]);
        }

        public void PlayStatAtHighest()
        {
            _sfxAudioSource2.pitch = 1f;
            _sfxAudioSource2.PlayOneShot(_statUp);
        }

        public void PlayStatAtLowest()
        {
            _sfxAudioSource2.pitch = 1f;
            _sfxAudioSource2.PlayOneShot(_statLow);
        }

        public void PlayLose()
        {
            _sfxAudioSource2.pitch = 1f;
            _sfxAudioSource2.PlayOneShot(_loseGame);
            _musicAudioSource.Stop();
        }

        public void PlayWin()
        {
            _sfxAudioSource2.pitch = 1f;
            _sfxAudioSource2.PlayOneShot(_winGame);
            _musicAudioSource.Stop();
        }

        public void StartMusic()
        {
            _musicAudioSource.Play();
        }
    }
}