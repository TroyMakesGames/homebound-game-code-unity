using Homebound.Services;
using NeighbourhoodJam2020.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeighbourhoodJam2020.UserInterface
{
    /// <summary>
    /// Trashy UI code written on the last day of the jam.
    /// </summary>
    public class CardHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject _card;
        private RectTransform _cardRectTransform;
        private CardUi _cardUi;

        [SerializeField]
        private Transform _backCard;

        [SerializeField]
        private float _movingSpeed = 2;

        [SerializeField]
        private float _sideMargin = 5;

        [SerializeField]
        private float _verticalLerpAmount = 0.2f;

        [SerializeField]
        private CardTextUi _cardTextUi;

        private bool _mouseOverCard = false;

        private bool _canInput = true;
        private Vector2 _posOffset;
        private Vector3 _defaultPos;

        private Vector2 _cardOffset;
        private Vector2 _cardOrigin;

        [SerializeField]
        private float sidewaysPosX = 5;

        private bool _movingCard = false;
        private bool _directionSelected = false;

        private void Awake()
        {

            _cardUi = _card.GetComponent<CardUi>();
            _cardRectTransform = _card.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _defaultPos = _card.transform.position;
            _cardOrigin = _card.transform.position;
        }

        private void Update()
        {
            if (_card == null) return;

            if (_canInput && _mouseOverCard && Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _posOffset = pos - _cardOrigin;
                _movingCard = true;
            }

            if (_canInput && Input.GetMouseButtonUp(0))
            {
                _movingCard = false;
                CheckForSwipe();
            }

            // Card movement.
            if (_movingCard)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Input.GetMouseButtonDown(0))
                {
                    _posOffset = pos - _cardOrigin;
                }

                Vector2 actualPos = pos - _posOffset;

                float heightPos = Mathf.Lerp(_cardOrigin.y, actualPos.y, _verticalLerpAmount);

                _card.transform.position = new Vector2(actualPos.x, heightPos);
            }
            else if (!_directionSelected)
            {
                _card.transform.position = Vector2.MoveTowards(_card.transform.position, _defaultPos, _movingSpeed);
            }

            // Checking for swipe.
            if (!_directionSelected)
            {
                if (_card.transform.position.x > _sideMargin)
                {
                    _cardUi.ShowChoice(Data.ChoiceDirection.RIGHT);
                    //_cardUi.SelectChoice(Data.ChoiceDirection.RIGHT);
                }
                else if (_card.transform.position.x < -_sideMargin)
                {
                    _cardUi.ShowChoice(Data.ChoiceDirection.LEFT);
                    //_cardUi.SelectChoice(Data.ChoiceDirection.LEFT);
                }
                else
                {
                    _cardUi.HideChoice();
                }
            }

            // Card rotation.
            _card.transform.eulerAngles = new Vector3(0, 0, (-_card.transform.position.x) * 2);
        }

        private void CheckForSwipe()
        {
            if (_card.transform.position.x > _sideMargin)
            {
                _directionSelected = true;
                _canInput = false;
                SwipeAnimation(Data.ChoiceDirection.RIGHT);
            }
            else if (_card.transform.position.x < -_sideMargin)
            {
                _directionSelected = true;
                _canInput = false;

                SwipeAnimation(Data.ChoiceDirection.LEFT);
            }
        }

        public void MouseOverCard()
        {
            _mouseOverCard = true;
        }

        public void MouseOffOfCard()
        {
            _mouseOverCard = false;
        }

        float swipeTime = 0.4f;

        private void SwipeAnimation(Data.ChoiceDirection choiceDirection)
        {
            _cardUi.SelectChoice(choiceDirection, swipeTime);
            StartCoroutine(SwipeCard(choiceDirection));
        }
        
        private IEnumerator SwipeCard(Data.ChoiceDirection choiceDirection)
        {
            _cardTextUi.RemoveCard();

            AudioService.Instance.PlayCardSwipe();

            Vector2 goal = (Vector2) _card.transform.position + new Vector2(_cardOrigin.x + (choiceDirection == Data.ChoiceDirection.RIGHT ? (Screen.width* sidewaysPosX) : (-Screen.width * sidewaysPosX)), _card.transform.position.y);
            float t = 0;
            while (t < swipeTime)
            {
                _card.transform.position = Vector2.MoveTowards(_card.transform.position, goal, _movingSpeed * 10);
                t += Time.deltaTime;
                yield return null;
            }

            //_cardUi.SelectChoice(choiceDirection);
            _cardUi.HideChoice(true);
            StartCoroutine(FlipCardAnimation());
        }

        public void FlipCard()
        {
            StartCoroutine(FlipCardAnimation());
        }

        private IEnumerator FlipCardAnimation()
        {
            if (_card == null) yield break;

            AudioService.Instance.PlayCardFlip();

            _card.transform.position = _defaultPos;
            _card.transform.eulerAngles = new Vector3(0, 0, 0);
            _card.transform.localScale = new Vector3(0, 1, 1);
            _card.SetActive(false);
            _cardUi.HideChoice(true);

            float flipTime = 0.1f;

            float t = 0;

            while (t < 1)
            {
                _backCard.transform.localScale = new Vector3(Mathf.Lerp(1, 0, t), 1, 1); ;

                t += Time.deltaTime / flipTime;
                yield return null;
            }

            _backCard.transform.localScale = new Vector3(0, 1, 1);

            _card.SetActive(true);

             t = 0;

            while (t < 1)
            {
                _card.transform.localScale = new Vector3(Mathf.Lerp(0, 1, t), 1, 1); ;

                t += Time.deltaTime / flipTime;
                yield return null;
            }

            _card.transform.localScale = new Vector3(1, 1, 1);
            _backCard.transform.localScale = new Vector3(1, 1, 1);

            _directionSelected = false;
            _canInput = true;
        }
    }
}