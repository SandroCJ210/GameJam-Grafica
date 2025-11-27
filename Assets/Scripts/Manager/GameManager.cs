using System.Collections;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using System.Collections.Generic;

public enum Turn
{
    WizardTurn = 0,
    PlayerTurn = 1
}

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private int numberOfCards;
    [SerializeField] private int numberOfRounds;
    [SerializeField] private List<GameObject> cardPrefabs;

    private Player _player;
    private Wizard _wizard;

    private List<int> _playerDeck = new List<int>();
    private List<int> _wizardDeck = new List<int>();
    private Stack<int> _drawDeck = new Stack<int>(); // This is the deck where the gamblers draw cards

    private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    private int currentRound = 1;
    private bool _isRoundOver = false;
    private bool hasFinishedTurn = false;
    private Turn _currentTurn = Turn.PlayerTurn;


    public List<int> PlayerDeck
    {
        get => new List<int>(_playerDeck);
        private set => _playerDeck = value;
    }

    public List<int> WizardDeck
    {
        get => new List<int>(_wizardDeck);
        private set => _wizardDeck = value;
    }


    private void Start()
    {
        InitializeVariables();
        StartCoroutine(Round());
    }

    private void InitializeVariables()
    {
        foreach (GameObject card in cardPrefabs)
        {
            _prefabDictionary[card.name] = card;
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _wizard = GameObject.FindGameObjectWithTag("Wizard").GetComponent<Wizard>();
    }

    private void StartRound()
    {
        _isRoundOver = false;
        ResetRound();
        ShuffleDeck();
        DealCard(Turn.WizardTurn);
        DealCard(Turn.PlayerTurn);
    }

    private void ShuffleDeck()
    {
        Random rand = new Random();
        var deckList = Enumerable.Range(1, numberOfCards).ToList();

        for (int i = numberOfCards - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (deckList[i], deckList[j]) = (deckList[j], deckList[i]);
        }

        _drawDeck = new Stack<int>(deckList);
    }

    public void DealCard(Turn turn)
    {
        int tempCard;
        tempCard = PopCard();

        if (turn == Turn.WizardTurn)
        {
            _wizard.ReceiveCard(tempCard);
            _wizardDeck = _wizard.Deck;
        }
        else if (turn == Turn.PlayerTurn)
        {
            _player.ReceiveCard(tempCard);
            _playerDeck = _player.Deck;
        }
    }

    public int PopCard()
    {
        int lastCard = _drawDeck.Pop();
        //TODO: Reproduce the audio feedback if is player's turn.
        return lastCard;
    }

    public void SwitchTurn()
    {
        _currentTurn = _currentTurn == Turn.PlayerTurn
            ? Turn.WizardTurn
            : Turn.PlayerTurn;
    }

    private IEnumerator Round()
    {
        while (currentRound <= numberOfRounds)
        {
            StartRound();
            _isRoundOver = false;

            while (!_isRoundOver)
            {
                hasFinishedTurn = false;
                
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _player.PlayTurn();
                }
                else if (_currentTurn == Turn.WizardTurn)
                {
                    _wizard.PlayTurn();
                }
                
                yield return new WaitUntil(() => hasFinishedTurn);

                bool endOfRoundCondition = _player.gamblerChoice == GamblerChoice.Pass &&
                                           _wizard.gamblerChoice == GamblerChoice.Pass;
                if (endOfRoundCondition)
                {
                    _isRoundOver = true;
                    break;
                }
                
                SwitchTurn();
            }

            DecideRoundWinner();
            currentRound++;
        }

    }

    public void SetEndofTurn()
    {
        hasFinishedTurn = true;
    }
    public void DecideRoundWinner()
    {

    }

    public void ResetRound()
    {
        //_player.resetDeck();
        //_wizard.resetDeck();
    }
}