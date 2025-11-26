using UnityEngine;

public class Wizard : Gambler
{
    [SerializeField] private Player player;
    [SerializeField] private ProbabilityManager probabilityManager;
    [SerializeField] private MageDifficultyConfig difficulty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new int[0];
        totalCardsValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void PlayTurn()
    {
        bool eyesOpen = player.areEyesOpen;
        int mageTotal = totalCardsValue;
        int playerVisible = eyesOpen ? player.TotalCardsValue : EstimatePlayerValue();

        // bool playerHasActedBeforeMage = gameManager.PlayerActedThisCycle;
        bool doHit = DecideAction(mageTotal, playerVisible, eyesOpen);

        if (doHit)
            DrawCard();
        else
            Pass();
    }

    protected override void DrawCard()
    {
        int card = probabilityManager.GetNextCard(Target.Wizard, player.areEyesOpen, totalCardsValue);

        int[] newDeck = new int[deck.Length + 1];
        deck.CopyTo(newDeck, 0);
        newDeck[deck.Length] = card;
        deck = newDeck;

        totalCardsValue += card;
    }

    protected override void Pass()
    {
        
    }

    private bool DecideAction(int mageTotal, int playerVisible, bool eyesOpen)
    {
        // CASO 1: JUGADOR CON OJOS ABIERTOS
        if (eyesOpen)
        {
            int diff = playerVisible - mageTotal;

            if (diff >= 3)
            {
                return Random.value < 0.8f; // 80% Hit
            }

            else if (diff <= -2)
            {
                return Random.value < 0.2f; // 20% Hit
            }
            else
            {
                return mageTotal < 17; // Hit hasta 16, Pass desde 17
            }
        }

        // CASO 2: JUGADOR CON OJOS CERRADOS 

        bool decisionBase;

        
        if (mageTotal <= 11)
        {
            decisionBase = true;
        }
        
        else if (mageTotal >= 19)
        {
            decisionBase = false;
        }
        else
        {
            // Nivel de audacia del mago
            float boldness = difficulty != null ? difficulty.boldness : 0.3f;

            
            float t = Mathf.InverseLerp(12f, 18f, mageTotal);
           
            float hitProbability = Mathf.Lerp(1f, 0f, t);
            
            hitProbability *= (0.5f + boldness);
           
            hitProbability = Mathf.Clamp01(hitProbability);

            decisionBase = Random.value < hitProbability;
        }

        // Nivel de dificultad: posibilidad de error cuando el jugador cierra los ojos
        float mistakeChance = difficulty != null ? difficulty.mistakeChanceEyesClosed : 0.2f;

        if (Random.value < mistakeChance)
        {
            decisionBase = !decisionBase;
        }

        return decisionBase;
    }


    private int EstimatePlayerValue()
    {
        int count = player.deckLength; 
        int estimate;

        switch (count)
        {
            case 1:
                estimate = Random.Range(3, 10);
                break;

            case 2:
                estimate = Random.Range(8, 15);
                break;

            case 3:
                estimate = Random.Range(12, 18);
                break;

            default:
                estimate = Random.Range(15, 21);
                break;
        }


        estimate += Random.Range(-1, 2);
        estimate = Mathf.Clamp(estimate, 4, 21);

        return estimate;
    }

}

