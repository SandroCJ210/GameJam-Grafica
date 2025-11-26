using System.Collections.Generic;
using UnityEngine;

public enum GamblerChoice
{
    None,
    Pass,
    Draw
}
public abstract class Gambler : MonoBehaviour
{
    protected List<int> deck = new List<int>();
    protected int totalCardsValue;
    protected int money;
    public GamblerChoice gamblerChoice = GamblerChoice.None;
    
    public List<int> Deck
    {
        get { return new List<int>(deck); }
    }
    
    public abstract void PlayTurn();
    //Request card from GameManager
    public abstract void DrawCard();
    public abstract void Pass();
    //Receive new card
    public virtual void ReceiveCard(int cardValue)
    {
        deck.Add(cardValue);
        totalCardsValue += cardValue; 
    }

    

}
