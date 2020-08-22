using Homebound.Services;
using NeighbourhoodJam2020.Data;
using NeighbourhoodJam2020.UserInterface;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace NeighbourhoodJam2020.Gameplay
{
    /// <summary>
    /// Main game script, handles the start of the game and first deck.
    /// </summary>
    public sealed class GameController : MonoBehaviour
    {
        private const string TUTORIAL_DONE_PREF = "tutorialDone";

        [SerializeField]
        private CardUi _cardUi;

        [SerializeField]
        private StatsUi _statsUi;

        [SerializeField]
        private TimeProgressionUi _timeOfDayUi;

        [SerializeField]
        private CardTextUi _cardTextUi;

        [SerializeField]
        private CardHolder _cardHolder;

        [SerializeField, Header("The first deck that is added to the pool when the game starts.")]
        private Deck _startingDeck = null;

        [SerializeField, Header("This card is drawn when there is no more cards left!")]
        private Card _endGameCard;

        [SerializeField, Header("These are the cards you see when you hit -100 on stats.")]
        private Card _noEnergyCard;
        [SerializeField]
        private Card _noHapinessCard, _noProductivityCard;

        [SerializeField, Header("Begin the game at day x in the y.")]
        private int _startDay = 1;
        [SerializeField]
        private TimeOfDay _startingTimeOfDay = TimeOfDay.AFTERNOON;

        [SerializeField, Range(-100, 100), Header("The game will start with these values.")]
        private int _startingHappiness;
        [SerializeField, Range(-100, 100)]
        private int _startingEnergy, _startingProductivity;

        [SerializeField]
        private Card _titleCard, _creditsCard, _tutorialCard, _secondTutorialCard;

        private List<Card> _forcedCards = new List<Card>();

        // Real-time variables.
        private Card _currentCard;
        private bool _gameEnded = false;
        private CardPool _cardPool = null;
        private TimeOfDay _currentTimeOfDay;
        private int _happiness, _energy, _productivity;
        private int _currentDay = 1;
        private int _currentDeckIndex = 0; // How many 'time progressions' have passed since the addition of the most recent deck.

        private static bool _launchedGame = true;
        private bool _tutorialDone = false;
        private bool _tutorialInProgress = false;

        float ogWidth = 540;
        float ogHeight = 960;
        float aspect;

        private void OnValidate()
        {
            Debug.Assert(_startingDeck != null, "Starting Deck is null!");
        }

        private void Awake()
        {
            aspect = ogWidth / ogHeight;

            Debug.Assert(_cardUi != null, "Card Ui reference is null!");
            Debug.Assert(_cardTextUi != null, "Card text Ui reference is null!");
            Debug.Assert(_statsUi != null, "Stats Ui reference is null!");
            Debug.Assert(_timeOfDayUi != null, "Time of day Ui reference is null!");
            Debug.Assert(_noEnergyCard != null, "Zero Energy Card reference is null!");
            Debug.Assert(_noHapinessCard != null, "Zero Hapiness Card reference is null!");
            Debug.Assert(_noProductivityCard != null, "Zero Productivity Card reference is null!");
            Debug.Assert(_endGameCard != null, "End Game card reference is null!");

            _tutorialDone = PlayerPrefs.GetInt(TUTORIAL_DONE_PREF, 0) == 0 ? false : true;
            _cardPool = new CardPool(_startingDeck);
            _cardUi.AttachCallbacks(ChoiceSelected);
        }

        private void Start()
        {
            _currentTimeOfDay = _startingTimeOfDay;
            _currentDay = _startDay;
            _timeOfDayUi.SetTime(_currentDay, _currentTimeOfDay);

            EffectStats(new Stats(_startingEnergy, _startingHappiness, _startingProductivity));

            AudioService.Instance.StartMusic();

            Debug.Log("Starting game...");
            PickupNextCard();
            _cardHolder.FlipCard();
        }

        float lastHeight;
        private void Update()
        {
            // Resoultion checker.
#if !UNITY_ANDROID && !UNITY_WEBGL

            if (Input.GetMouseButtonDown(0) && Camera.main.aspect != 0.5625)
            {
                Screen.SetResolution((int)(Screen.height * 0.5625f), Screen.height, false);
                lastHeight = Screen.height;
            }


#endif
        }

        private void PickupNextCard(Choice choice = null)
        {
            Card card = null;

            // Tutorial for first time players.
            if (!_tutorialDone && choice == null)
            {
                Debug.Log("Doing tutorial.");
                card = _tutorialCard;
                _tutorialInProgress = true;
                _tutorialDone = true;
                PlayerPrefs.SetInt(TUTORIAL_DONE_PREF, 1);
            }

            // If the game started, pickup the title card.
            if (_launchedGame && !_tutorialInProgress)
            {
                Debug.Log("Showing title card.");
                card = _titleCard;
                _launchedGame = false;
            }

            // Check for stats at zero.
            if (_gameEnded == false)
            {
                if (_energy <= -100) card = _noEnergyCard;
                if (_happiness <= -100) card = _noHapinessCard;
                if (_productivity <= -100) card = _noProductivityCard;
                if (card != null && card != _titleCard && card != _creditsCard && !_tutorialInProgress)
                {
                    _gameEnded = true;
                    Debug.Log($"One of your stats has gotten to low. Your stats: Happiness ({_happiness}), Productivity ({_productivity}), Energy ({_energy})");
                    AudioService.Instance.PlayLose();
                    _cardPool.ClearCardPool();
                }
            }

            // Check for a forced card.
            if (card == null)
            {
                Card forcedCard = _cardPool.MostRecentDeck.GetForcedCard(_currentDeckIndex, _currentCard);
                if (forcedCard != null && !_forcedCards.Contains(forcedCard))
                {
                    card = forcedCard;
                    _forcedCards.Add(forcedCard);
                }
            }

            // Check for followup card.
            if (card == null && choice != null) card = choice.GetFollowupCard();

            // Get random card.
            if (card == null) card = _cardPool.PickupRandomCard(_currentTimeOfDay, new Stats(_energy, _happiness, _productivity));

            // No more cards left avaliable.
            if (card == null)
            {
                if (!_gameEnded)
                {
                    _gameEnded = true;
                    card = _endGameCard;
                    AudioService.Instance.PlayWin();
                    Debug.Log("No more cards avaliable to pickup.");
                    _cardPool.ClearCardPool();
                }
                else
                {
                    Debug.Log("Game over.");
                    card = _creditsCard;
                }
            }

            // Execute card effectgs.
            CardEffects(card);

            // Draw the card
            _cardUi.DrawCard(card);
            _cardTextUi.DrawCard(card);

            _currentCard = card;
        }

        private void CardEffects(Card card)
        {
            // Fail game?
            if (card.FailCard)
            {
                _gameEnded = true;
                Debug.Log($"Fail Card activated, no more cards left.");
                AudioService.Instance.PlayLose();
                _cardPool.ClearCardPool();
            }

            // Remove any cards out the active pool if this cards want to.
            if (card.RemoveCardsOnDraw != null && card.RemoveCardsOnDraw.Length >= 1) _cardPool.RemoveFromCardPool(card.RemoveCardsOnDraw);

            // Add any cards into the active pool if the card wants it to.
            if (card.AddCardsOnDraw != null && card.AddCardsOnDraw.Length >= 1) _cardPool.AddToCardPool(card.AddCardsOnDraw);
        }

        private void ChoiceSelected(Choice choice, ChoiceDirection direction, float cardUpdateDelay)
        {
            Debug.Log($"Player selected {direction} choice");

            // Reset game.
            if (_currentCard == _creditsCard)
            {
                Debug.Log("Restarting game...");
                StartCoroutine(RestartGame());
                return;
            }
            
            // End tutorial.
            if (_currentCard == _secondTutorialCard)
            {
                _tutorialInProgress = false;
            }

            // Effect stats.
            EffectStats(choice.GetStatEffects());
            
            // Progress time.
            if (choice.ContinueTime) ProgressTime();

            // Flush cards?
            if (choice.FlushCardPool || _cardPool.MostRecentDeck.ForceFlushCards(_currentDeckIndex)) _cardPool.ClearCardPool();

            // Add new deck from choice?
            if (choice.AddDeck) AddDeckToCardPool(choice.AddDeck);

            // Add new cards from choice?
            Card[] cards = choice.AddCardsOnChoiceSelect;
            if (cards != null && cards.Length >= 1)
            {
                _cardPool.AddToCardPool(cards);
            }

            // Add new deck from deck event?
            Deck forcedDeck = _cardPool.MostRecentDeck.GetForcedAddDeck(_currentDeckIndex);
            if (forcedDeck != null) AddDeckToCardPool(forcedDeck);

            // Add new cards into deck from deck event?
            Card[] newCards = _cardPool.MostRecentDeck.GetCardsForcedIntoDeck(_currentDeckIndex);
            if (newCards != null && newCards.Length >= 1)
            {
                _cardPool.AddToCardPool(newCards);
            }

            if (cardUpdateDelay != 0)
            {
                StartCoroutine(WaitToPickUpCard(choice, cardUpdateDelay));
            }
            else
            {
                // Continue game.
                PickupNextCard(choice);
            }
        }

        private IEnumerator RestartGame()
        {
            yield return new WaitForSeconds(0.4f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator WaitToPickUpCard(Choice choice, float delay)
        {
            yield return new WaitForSeconds(delay);
            // Continue game.
            PickupNextCard(choice);
        }

        private void ProgressTime()
        {
            if (_currentTimeOfDay == TimeOfDay.MORNING)
                _currentTimeOfDay = TimeOfDay.AFTERNOON;
            else if (_currentTimeOfDay == TimeOfDay.AFTERNOON)
                _currentTimeOfDay = TimeOfDay.EVENING;
            else
            {
                _currentDay++;
                _currentTimeOfDay = TimeOfDay.MORNING;
            }

            Debug.Log($"Time progressed to {_currentTimeOfDay}.");
            _currentDeckIndex++;
            _timeOfDayUi.SetTime(_currentDay, _currentTimeOfDay);
        }

        private void EffectStats(Stats stats)
        {
            _happiness = Mathf.Clamp(_happiness += stats.happiness, -100, 100);
            _productivity = Mathf.Clamp(_productivity += stats.productivity, -100, 100);
            _energy = Mathf.Clamp(_energy += stats.energy, -100, 100);

            Debug.Log($"Stats effected: Happiness ({stats.happiness}), Productivity ({stats.productivity}), Energy ({stats.energy})");
            _statsUi.SetStats(new Stats(_energy, _happiness, _productivity));
        }

        private void AddDeckToCardPool(Deck deck)
        {
            if (deck == null) return;
            _currentDeckIndex = 0;
            _cardPool.AddDeckToPool(deck);
        }
    }
}