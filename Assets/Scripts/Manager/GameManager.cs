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
    [SerializeField] private int numberOfCards = 11;
    [SerializeField] private int numberOfRounds;
    
    [HideInInspector]
    public Player _player;
    [HideInInspector]
    public Wizard _wizard;

    private List<int> _playerDeck = new List<int>();
    private List<int> _wizardDeck = new List<int>();
    private Stack<int> _drawDeck = new Stack<int>();
    

    private int currentRound = 1;
    private bool _isRoundOver = false;
    private bool hasFinishedTurn = false;
    private Turn _currentTurn = Turn.PlayerTurn;
    private Winner winner = Winner.None;
    
    // ✅ Tracking de acciones en la mini-ronda ACTUAL
    private GamblerChoice _playerActionThisMiniRound = GamblerChoice.None;
    private GamblerChoice _wizardActionThisMiniRound = GamblerChoice.None;
    private Turn _miniRoundStarter; // Quién empezó esta mini-ronda

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
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _wizard = GameObject.FindGameObjectWithTag("Wizard").GetComponent<Wizard>();
    }

    private void StartRound()
    {
        Debug.Log($"========= RONDA {currentRound} INICIADA =========");
        
        _isRoundOver = false;
        _currentTurn = Turn.PlayerTurn;
        _miniRoundStarter = Turn.PlayerTurn;
        
        UIManager.Instance.ResetPlayerCards();
        _playerActionThisMiniRound = GamblerChoice.None;
        _wizardActionThisMiniRound = GamblerChoice.None;
        
        UIManager.Instance.SetTurnText(_currentTurn);
        ResetRound();
        ShuffleDeck();
        DealCard(Turn.WizardTurn);
        DealCard(Turn.PlayerTurn);
        
        Debug.Log($"Cartas iniciales - Player: {_player.TotalCardsValue}, Wizard: {_wizard.TotalCardsValue}");
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
        Debug.Log($"Mazo barajado: {_drawDeck.Count} cartas (1-{numberOfCards})");
    }

    public void DealCard(Turn turn)
    {
        int tempCard = PopCard();

        if (turn == Turn.WizardTurn)
        {
            _wizard.ReceiveCard(tempCard);
            _wizardDeck = _wizard.Deck;
            Debug.Log($"[Wizard] Recibió carta {tempCard} → Total: {_wizard.TotalCardsValue}");
        }
        else if (turn == Turn.PlayerTurn)
        {
            _player.ReceiveCard(tempCard);
            _playerDeck = _player.Deck;
            UIManager.Instance.ShowPlayerCard(tempCard);
            _player.OnCardReceived(tempCard);
            Debug.Log($"[Player] Recibió carta {tempCard} → Total: {_player.TotalCardsValue}");
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
        _currentTurn = _currentTurn == Turn.PlayerTurn ? Turn.WizardTurn : Turn.PlayerTurn;
        UIManager.Instance.SetTurnText(_currentTurn);
        Debug.Log($"--- Cambio de turno: {_currentTurn} ---");
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
                
                // Marcar inicio de mini-ronda
                _miniRoundStarter = _currentTurn;
                Debug.Log($"--- MINI-RONDA: Empieza {_miniRoundStarter} ---");
                
                // PRIMER JUGADOR de la mini-ronda
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _player.PlayTurn();
                }
                else
                {
                    _wizard.PlayTurn();
                }
                
                yield return new WaitUntil(() => hasFinishedTurn);
                
                // Guardar acción del primer jugador
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _playerActionThisMiniRound = _player.gamblerChoice;
                    Debug.Log($"[Player] Acción: {_playerActionThisMiniRound}");
                }
                else
                {
                    _wizardActionThisMiniRound = _wizard.gamblerChoice;
                    Debug.Log($"[Wizard] Acción: {_wizardActionThisMiniRound}");
                }
                
                // Verificar bust del primer jugador
                if (CheckBust())
                {
                    _isRoundOver = true;
                    break;
                }
                
                // Cambiar turno
                SwitchTurn();
                hasFinishedTurn = false;
                
                // SEGUNDO JUGADOR de la mini-ronda
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _player.PlayTurn();
                }
                else
                {
                    _wizard.PlayTurn();
                }
                
                yield return new WaitUntil(() => hasFinishedTurn);
                
                // Guardar acción del segundo jugador
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _playerActionThisMiniRound = _player.gamblerChoice;
                    Debug.Log($"[Player] Acción: {_playerActionThisMiniRound}");
                }
                else
                {
                    _wizardActionThisMiniRound = _wizard.gamblerChoice;
                    Debug.Log($"[Wizard] Acción: {_wizardActionThisMiniRound}");
                }
                
                // ✅ AQUÍ verificamos al FINAL de la mini-ronda
                if (CheckEndOfRound())
                {
                    _isRoundOver = true;
                    Debug.Log("========= FIN DE RONDA =========");
                    break;
                }
                
                // Resetear acciones para la próxima mini-ronda
                _playerActionThisMiniRound = GamblerChoice.None;
                _wizardActionThisMiniRound = GamblerChoice.None;
                
                SwitchTurn();
            }

            DecideRoundWinner();
            currentRound++;
        }

        ResetGame();
    }

    /// <summary>
    /// Verifica si alguien se pasó de 21.
    /// </summary>
    private bool CheckBust()
    {
        if (_player.TotalCardsValue > 21)
        {
            Debug.Log("¡Player se pasó de 21! Fin de ronda.");
            return true;
        }
        
        if (_wizard.TotalCardsValue > 21)
        {
            Debug.Log("¡Wizard se pasó de 21! Fin de ronda.");
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// ✅ Verifica si la ronda debe terminar al FINAL de una mini-ronda.
    /// Solo se llama después de que ambos jugadores hayan jugado.
    /// </summary>
    private bool CheckEndOfRound()
    {
        // Verificar bust
        if (CheckBust())
        {
            return true;
        }

        // Verificar si ambos pasaron en esta mini-ronda
        bool bothPassedThisMiniRound = 
            _playerActionThisMiniRound == GamblerChoice.Pass && 
            _wizardActionThisMiniRound == GamblerChoice.Pass;

        if (bothPassedThisMiniRound)
        {
            Debug.Log("¡Ambos pasaron en esta mini-ronda! Fin de ronda.");
            return true;
        }

        Debug.Log($"Mini-ronda completa: Player={_playerActionThisMiniRound}, Wizard={_wizardActionThisMiniRound}");
        return false;
    }

    public void SetEndofTurn()
    {
        hasFinishedTurn = true;
    }
    
    public void DecideRoundWinner()
    {
        int playerTotal = _player.TotalCardsValue;
        int wizardTotal = _wizard.TotalCardsValue;
        
        string winnerName = "";
        string reason = "";
        
        // Caso 1: Ambos se pasaron
        if (playerTotal > 21 && wizardTotal > 21)
        {
            winnerName = "Empate";
            reason = "Ambos se pasaron de 21";
            winner = Winner.None;
        }
        // Caso 2: Solo player se pasó
        else if (playerTotal > 21)
        {
            winnerName = "Mago";
            reason = $"El jugador se pasó de 21 (Total: {playerTotal})";
            winner = Winner.Wizard;
        }
        // Caso 3: Solo wizard se pasó
        else if (wizardTotal > 21)
        {
            winnerName = "Jugador";
            reason = $"El mago se pasó de 21 (Total: {wizardTotal})";
            winner = Winner.Player;
        }
        // Caso 4: Nadie se pasó - comparar cercanía a 21
        else
        {
            int playerDistance = 21 - playerTotal;
            int wizardDistance = 21 - wizardTotal;
            
            if (playerDistance < wizardDistance)
            {
                winnerName = "Jugador";
                reason = $"Jugador más cercano a 21 ({playerTotal} vs {wizardTotal})";
                winner = Winner.Player;
            }
            else if (wizardDistance < playerDistance)
            {
                winnerName = "Mago";
                reason = $"Mago más cercano a 21 ({wizardTotal} vs {playerTotal})";
                winner = Winner.Wizard;
            }
            else
            {
                winnerName = "Empate";
                reason = $"Ambos tienen el mismo total ({playerTotal})";
                winner = Winner.None;
            }
        }
        
        Debug.Log($"========= RESULTADO DE RONDA {currentRound} =========");
        Debug.Log($"Player: {playerTotal} | Wizard: {wizardTotal}");
        Debug.Log($"Ganador: {winnerName} - {reason}");
        Debug.Log("=============================================");
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
        
        Debug.Log("Mazos reseteados");
    }

    public void ResetGame()
    {
        winner = Winner.None;
        currentRound = 1;
        Debug.Log("Juego completamente reseteado");
    }
}