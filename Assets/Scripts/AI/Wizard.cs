using System.Collections.Generic;
using UnityEngine;

public class Wizard : Gambler
{
    [SerializeField] private Player player;

    [Header("Caso: Ojos cerrados")]
    [Range(0f, 1f)] public float boldness = 0.3f;
    [Range(0f, 1f)] public float mistakeChanceEyesClosed = 0.2f;

    void Start()
    {
        deck = new List<int>();
        totalCardsValue = 0;

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public override void PlayTurn()
    {
        Debug.Log("=== TURNO DEL MAGO ===");

        bool eyesOpen = player.areEyesOpen;
        int mageTotal = totalCardsValue;
        int playerVisible = eyesOpen ? player.TotalCardsValue : EstimatePlayerValue();

        bool doHit = DecideAction(mageTotal, playerVisible, eyesOpen);

        if (doHit)
            DrawCard();
        else
            Pass();

        GameManager.Instance.SetEndofTurn();
    }

    public override void DrawCard()
    {
        gamblerChoice = GamblerChoice.Draw;
        GameManager.Instance.DealCard(Turn.WizardTurn);
        Debug.Log($"Mago robó carta. Total actual: {totalCardsValue}");
    }

    public override void Pass()
    {
        gamblerChoice = GamblerChoice.Pass;
        Debug.Log("Mago pasa.");
    }

    private bool DecideAction(int mageTotal, int playerVisible, bool eyesOpen)
    {
        
        if (mageTotal >= 20)
            return false; 

        // --- OJOS ABIERTOS ---
        if (eyesOpen)
        {
            int diff = playerVisible - mageTotal;

            if (diff >= 3)
                return Random.value < 0.8f;

            if (diff <= -2)
                return Random.value < 0.2f;

            return mageTotal < 17;
        }

        // --- OJOS CERRADOS ---
        bool baseDecision;

        if (mageTotal <= 11)
            baseDecision = true;
        else if (mageTotal >= 19)
            baseDecision = false;
        else
        {
            float t = Mathf.InverseLerp(12f, 18f, mageTotal);
            float hitProb = Mathf.Lerp(1f, 0f, t);
            hitProb *= (0.5f + boldness);
            hitProb = Mathf.Clamp01(hitProb);
            baseDecision = Random.value < hitProb;
        }

        
        if (Random.value < mistakeChanceEyesClosed)
            baseDecision = !baseDecision;

        return baseDecision;
    }

    private int EstimatePlayerValue()
    {
        int count = player.deckLength;
        int estimate;

        switch (count)
        {
            case 1: estimate = Random.Range(3, 10); break;
            case 2: estimate = Random.Range(8, 15); break;
            case 3: estimate = Random.Range(12, 18); break;
            default: estimate = Random.Range(15, 21); break;
        }

        estimate += Random.Range(-1, 2);
        return Mathf.Clamp(estimate, 4, 21);
    }
}


