using UnityEngine;

public class Player : Gambler
{
    public bool areEyesOpen { get; private set; } = true;
    public int TotalCardsValue => totalCardsValue;

    public int deckLength => deck != null ? deck.Count : 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void PlayTurn()
    {
        
    }

    public override int DrawCard()
    {
        return 0;
    }

    public override void Pass()
    {
        
    }
    
    
}
