using System.Collections.Generic;
using UnityEngine;

public abstract class Gambler : MonoBehaviour
{
    protected List<int> deck;
    protected int totalCardsValue;
    protected int money;
    
    public abstract void PlayTurn();
    public abstract int DrawCard();
    public abstract void Pass();
}
