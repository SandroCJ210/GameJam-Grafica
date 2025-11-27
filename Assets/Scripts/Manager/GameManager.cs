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
    private ClosedEyesAudioFeedback _closedEyesAudioFeedback;
    private ProbabilityManager probabilityManager;

    private List<int> _playerDeck = new List<int>();
    private List<int> _wizardDeck = new List<int>();
    private Stack<int> _drawDeck = new Stack<int>(); // This is the deck where the gamblers draw cards

    private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    private int currentRound = 1;
    private bool _isRoundOver = false;
    private bool hasFinishedTurn = false;
    private Turn _currentTurn = Turn.PlayerTurn;
    private List<string> _wizardDecisions = new List<string>(); // Rastrea decisiones del mago
    private Coroutine roundCoroutine;


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
        roundCoroutine = StartCoroutine(Round());
    }

    private void InitializeVariables()
    {
        foreach (GameObject card in cardPrefabs)
        {
            _prefabDictionary[card.name] = card;
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _wizard = GameObject.FindGameObjectWithTag("Wizard").GetComponent<Wizard>();
        
        // Buscar ClosedEyesAudioFeedback en el Player o en la escena
        _closedEyesAudioFeedback = _player.GetComponent<ClosedEyesAudioFeedback>();
        if (_closedEyesAudioFeedback == null)
        {
            _closedEyesAudioFeedback = FindFirstObjectByType<ClosedEyesAudioFeedback>();
        }
        
        // Buscar ProbabilityManager en la escena
        probabilityManager = FindFirstObjectByType<ProbabilityManager>();
        if (probabilityManager != null)
        {
            Debug.Log("GameManager: ProbabilityManager encontrado");
        }
        else
        {
            Debug.Log("GameManager: ProbabilityManager no encontrado");
        }
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
        
        // Crear mazo con valores del 1 al 11, con múltiples copias
        // Si numberOfCards es 44, significa 4 copias de cada valor (1-11) = 44 cartas
        int copiesPerValue = numberOfCards / 11;
        var deckList = new List<int>();
        
        for (int value = 1; value <= 11; value++)
        {
            for (int copy = 0; copy < copiesPerValue; copy++)
            {
                deckList.Add(value);
            }
        }
        
        // Si sobran cartas (por ejemplo, si numberOfCards no es múltiplo de 11),
        // agregar las restantes como valores aleatorios del 1 al 11
        while (deckList.Count < numberOfCards)
        {
            deckList.Add(rand.Next(1, 12)); // 1 al 11 inclusive
        }

        // Barajar el mazo
        for (int i = deckList.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (deckList[i], deckList[j]) = (deckList[j], deckList[i]);
        }

        _drawDeck = new Stack<int>(deckList);
        Debug.Log($"GameManager: Mazo barajado con {_drawDeck.Count} cartas (valores del 1 al 11)");
    }

    public void DealCard(Turn turn)
    {
        int tempCard;
        
        // Si es turno del mago y existe ProbabilityManager, usar selección ponderada
        if (turn == Turn.WizardTurn && probabilityManager != null)
        {
            tempCard = probabilityManager.GetNextCard(Target.Wizard, _player.areEyesOpen, _wizard.TotalCardsValue);
            Debug.Log($"GameManager: DealCard usando ProbabilityManager -> carta {tempCard}");
            
            // Intentar mantener sincronización con _drawDeck: buscar y remover una instancia de esta carta
            bool foundInDeck = false;
            if (_drawDeck.Count > 0)
            {
                // Convertir Stack a List temporalmente para buscar
                List<int> deckList = new List<int>(_drawDeck);
                int index = deckList.IndexOf(tempCard);
                if (index >= 0)
                {
                    deckList.RemoveAt(index);
                    _drawDeck = new Stack<int>(deckList);
                    foundInDeck = true;
                }
            }
            
            if (!foundInDeck)
            {
                Debug.LogWarning($"GameManager: Carta {tempCard} obtenida de ProbabilityManager no encontrada en _drawDeck. Los mazos pueden estar desincronizados.");
            }
        }
        else
        {
            tempCard = PopCard();
            if (turn == Turn.WizardTurn)
            {
                Debug.Log($"GameManager: DealCard usando mazo tradicional -> carta {tempCard}");
            }
        }

        if (turn == Turn.WizardTurn)
        {
            _wizard.ReceiveCard(tempCard);
            _wizardDeck = _wizard.Deck;
            _wizardDecisions.Add($"Mago robó carta: {tempCard} (Total: {_wizard.TotalCardsValue})");
            
            // Verificar si el mago se pasó de 21
            if (_wizard.TotalCardsValue > 21)
            {
                Debug.Log($"¡El mago se pasó de 21! Total: {_wizard.TotalCardsValue}");
                _wizardDecisions.Add("¡BUST! El mago se pasó de 21");
                _isRoundOver = true;
            }
        }
        else if (turn == Turn.PlayerTurn)
        {
            _player.ReceiveCard(tempCard);
            _playerDeck = _player.Deck;
            
            // Verificar si el jugador se pasó de 21
            if (_player.TotalCardsValue > 21)
            {
                Debug.Log($"¡El jugador se pasó de 21! Total: {_player.TotalCardsValue}");
                _isRoundOver = true;
            }
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

    /// <summary>
    /// Mira la próxima carta del mazo sin robarla.
    /// Útil para mostrar el preview de audio antes de que el jugador elija.
    /// </summary>
    public int PeekNextCard()
    {
        if (_drawDeck.Count == 0)
        {
            Debug.LogWarning("El mazo está vacío!");
            return 0;
        }
        return _drawDeck.Peek(); // Peek() mira sin remover
    }

    /// <summary>
    /// Obtiene el preview de la próxima carta y reproduce el sonido si los ojos están cerrados.
    /// Retorna el valor de la carta y si es favorable.
    /// </summary>
    public (int cardValue, bool isFavorable) GetCardPreview()
    {
        int nextCard = PeekNextCard();
        int playerCurrentTotal = _player.TotalCardsValue;
        bool isFavorable = false;

        // Solo reproducir sonido si los ojos están cerrados
        if (_closedEyesAudioFeedback != null)
        {
            isFavorable = _closedEyesAudioFeedback.PlayCardPreviewFeedback(nextCard, playerCurrentTotal);
        }
        else
        {
            // Si no hay audio feedback, evaluar manualmente
            CardQualityEvaluator evaluator = _player.GetComponent<CardQualityEvaluator>();
            if (evaluator != null)
            {
                isFavorable = evaluator.IsGoodCard(nextCard, playerCurrentTotal);
            }
        }

        return (nextCard, isFavorable);
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
                
                // NO resetear elecciones aquí - necesitamos mantenerlas para verificar si ambos pasaron consecutivamente
                
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _player.PlayTurn();
                }
                else if (_currentTurn == Turn.WizardTurn)
                {
                    _wizard.PlayTurn();
                    
                    // Registrar decisión del mago
                    if (_wizard.gamblerChoice == GamblerChoice.Pass)
                    {
                        _wizardDecisions.Add($"Mago pasó (Total: {_wizard.TotalCardsValue})");
                    }
                }
                
                yield return new WaitUntil(() => hasFinishedTurn);

                // Verificar si alguien se pasó de 21 (ya se verifica en DealCard, pero por si acaso)
                if (_player.TotalCardsValue > 21 || _wizard.TotalCardsValue > 21)
                {
                    _isRoundOver = true;
                    break;
                }

                // Verificar condición de fin de ronda: ambos deben pasar CONSECUTIVAMENTE
                // Esto significa que después de este turno, ambos tienen Pass como elección
                bool endOfRoundCondition = _player.gamblerChoice == GamblerChoice.Pass &&
                                           _wizard.gamblerChoice == GamblerChoice.Pass;
                if (endOfRoundCondition)
                {
                    Debug.Log("Ambos pasaron consecutivamente. Terminando ronda...");
                    _isRoundOver = true;
                    break;
                }
                
                // Si alguien robó carta, resetear su elección para el próximo turno
                // Pero mantener Pass si pasó, para poder detectar cuando ambos pasan consecutivamente
                if (_currentTurn == Turn.PlayerTurn && _player.gamblerChoice == GamblerChoice.Draw)
                {
                    _player.gamblerChoice = GamblerChoice.None;
                }
                else if (_currentTurn == Turn.WizardTurn && _wizard.gamblerChoice == GamblerChoice.Draw)
                {
                    _wizard.gamblerChoice = GamblerChoice.None;
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
        int playerTotal = _player.TotalCardsValue;
        int wizardTotal = _wizard.TotalCardsValue;
        
        string winner = "";
        string reason = "";
        
        // Caso 1: Jugador se pasó de 21
        if (playerTotal > 21)
        {
            winner = "Mago";
            reason = $"El jugador se pasó de 21 (Total: {playerTotal})";
        }
        // Caso 2: Mago se pasó de 21
        else if (wizardTotal > 21)
        {
            winner = "Jugador";
            reason = $"El mago se pasó de 21 (Total: {wizardTotal})";
        }
        // Caso 3: Ambos se pasaron (no debería pasar, pero por si acaso)
        else if (playerTotal > 21 && wizardTotal > 21)
        {
            winner = "Empate";
            reason = "Ambos se pasaron de 21";
        }
        // Caso 4: Ambos pasaron - el más cercano a 21 gana
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
        
        Debug.Log($"=== FIN DE RONDA {currentRound} ===");
        Debug.Log($"Jugador: {playerTotal} | Mago: {wizardTotal}");
        Debug.Log($"Ganador: {winner} - {reason}");
        
        // Notificar al UI Manager para mostrar el panel de resultados
        RoundResultUIManager resultUI = FindFirstObjectByType<RoundResultUIManager>();
        if (resultUI != null)
        {
            resultUI.ShowRoundResult(winner, playerTotal, wizardTotal, reason);
        }
    }

    public void ResetRound()
    {
        // Limpiar cartas de ambos jugadores usando el método público
        if (_player != null)
        {
            _player.ResetDeck();
        }
        if (_wizard != null)
        {
            _wizard.ResetDeck();
        }
        
        // Limpiar decisiones del mago
        _wizardDecisions.Clear();
        
        // Notificar al UI Manager para limpiar el panel de decisiones
        WizardDecisionsUIManager decisionsUI = FindFirstObjectByType<WizardDecisionsUIManager>();
        if (decisionsUI != null)
        {
            decisionsUI.ClearDecisions();
        }
        
        Debug.Log("Ronda reseteada. Cartas y decisiones limpiadas.");
    }
    
    /// <summary>
    /// Reinicia todo el juego cuando el jugador presiona el botón Continue.
    /// </summary>
    public void RestartGameFromUI()
    {
        Debug.Log("GameManager: Reiniciando juego completo desde ContinueButton...");
        
        if (roundCoroutine != null)
        {
            StopCoroutine(roundCoroutine);
        }
        
        currentRound = 1;
        _currentTurn = Turn.PlayerTurn;
        _isRoundOver = false;
        hasFinishedTurn = false;
        
        ResetRound();
        roundCoroutine = StartCoroutine(Round());
    }
    
    /// <summary>
    /// Obtiene las decisiones del mago durante la ronda actual.
    /// </summary>
    public List<string> GetWizardDecisions()
    {
        return new List<string>(_wizardDecisions);
    }
}

