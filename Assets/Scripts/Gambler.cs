using UnityEngine;

public abstract class Gambler : MonoBehaviour
{
    protected int[] deck;
    protected int totalCardsValue;
    protected int money;

    protected int[] Deck
    {
        get { return deck.Clone() as int[]; }
        private set { deck = value; }
    }
    
    protected abstract void PlayTurn();
    protected abstract void DrawCard();
    protected abstract void Pass();
}
