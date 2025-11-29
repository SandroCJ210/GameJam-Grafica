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

public enum Winner
{
    Wizard,
    Player,
    None
}

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private int numberOfCards;
    [SerializeField] private int numberOfRounds;
    [SerializeField] private List<GameObject> cardPrefabs;
    
    [HideInInspector]
    public Player _player;
    [HideInInspector]
    public Wizard _wizard;

    private List<int> _playerDeck = new List<int>();
    private List<int> _wizardDeck = new List<int>();
    private Stack<int> _drawDeck = new Stack<int>(); // This is the deck where the gamblers draw cards

    private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    private int currentRound = 1;
    private bool _isRoundOver = false;
    private bool hasFinishedTurn = false;
    private Turn _currentTurn = Turn.PlayerTurn;
    private Winner winner = Winner.None;


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
        _currentTurn = Turn.PlayerTurn;              
        UIManager.Instance.SetTurnText(_currentTurn);
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
        if (_drawDeck.Count == 0)
        {
            Debug.LogWarning("El mazo está vacío!");
            return 0;
        }
        int lastCard = _drawDeck.Pop();
        return lastCard;
    }
    
    public int PeekNextCard()
    {
        if (_drawDeck.Count == 0)
        {
            Debug.LogWarning("El mazo está vacío!");
            return 0;
        }
        return _drawDeck.Peek(); 
    }
    

    public void SwitchTurn()
    {
        _currentTurn = _currentTurn == Turn.PlayerTurn
            ? Turn.WizardTurn
            : Turn.PlayerTurn;
        UIManager.Instance.SetTurnText(_currentTurn);
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

        ResetGame();
    }

    public void SetEndofTurn()
    {
        hasFinishedTurn = true;
    }
    
    public void DecideRoundWinner()
    {
        int playerTotal = _player.TotalCardsValue;
        int wizardTotal = _wizard.TotalCardsValue;
        
        string winner = "";
        string reason = "";
        
        if (playerTotal > 21)
        {
            winner = "Mago";
            reason = $"El jugador se pasó de 21 (Total: {playerTotal})";
        }
        else if (wizardTotal > 21)
        {
            winner = "Jugador";
            reason = $"El mago se pasó de 21 (Total: {wizardTotal})";
        }
        else if (playerTotal > 21 && wizardTotal > 21)
        {
            winner = "Empate";
            reason = "Ambos se pasaron de 21";
        }
        else
        {
            int playerDistance = Mathf.Abs(21 - playerTotal);
            int wizardDistance = Mathf.Abs(21 - wizardTotal);
            
            if (playerDistance < wizardDistance)
            {
                winner = "Jugador";
                reason = $"Jugador más cercano a 21 ({playerTotal} vs {wizardTotal})";
            }
            else if (wizardDistance < playerDistance)
            {
                winner = "Mago";
                reason = $"Mago más cercano a 21 ({wizardTotal} vs {playerTotal})";
            }
            else
            {
                winner = "Empate";
                reason = $"Ambos tienen el mismo total ({playerTotal})";
            }
        }
        
        
    }

    public void ResetRound()
    {
        if (_player != null)
        {
            _player.ResetDeck();
        }
        if (_wizard != null)
        {
            _wizard.ResetDeck();
        }
    }

    public void ResetGame()
    {
        winner = Winner.None;
    }
    

}
