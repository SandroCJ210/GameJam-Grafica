using System;
using System.Collections;
using UnityEngine;

public class Player : Gambler
{
    public bool areEyesOpen { get; private set; } = true;
    public int deckLength => deck != null ? deck.Count : 0;

    private bool isTakingTurn = false;
    private bool eyesChoiceDone = false;
    private bool actionChoiceDone = false;

    public override void PlayTurn()
    {
        if (!isTakingTurn)
        {
            Debug.Log("Player Turn");
            StartCoroutine(PlayerTurnRoutine());
        }
    }

    public void Update()
    {
        Debug.Log("EYES" + areEyesOpen);
        Debug.Log("CHOICE" + gamblerChoice);
    }

    private IEnumerator PlayerTurnRoutine()
    {
        isTakingTurn = true;
        eyesChoiceDone = false;
        actionChoiceDone = false;

        // Activar panel de decisión de ojos
        UIManager.Instance.ShowEyesPanel();

        yield return new WaitUntil(() => eyesChoiceDone);

        // Activar panel de decisión de jugada
        UIManager.Instance.ShowActionPanel();

        yield return new WaitUntil(() => actionChoiceDone);
        
        isTakingTurn = false;
        UIManager.Instance.HideAllPanels();
        GameManager.Instance.SetEndofTurn();
    }
    
    public void SetEyesChoice(bool open)
    {
        if (!isTakingTurn || eyesChoiceDone)
        {
            return;
        }

        areEyesOpen = open;
        eyesChoiceDone = true;
    }
    
    public void SetActionChoice(GamblerChoice choice)
    {
        if (!isTakingTurn || !eyesChoiceDone || actionChoiceDone)
        {
            return;
        }

        gamblerChoice = choice;

        if (choice == GamblerChoice.Draw)
        {
            DrawCard();
        }
        else if (choice == GamblerChoice.Pass)
        {
            Pass();
        }

        actionChoiceDone = true;
    }

    public override void DrawCard()
    {
        GameManager.Instance.DealCard(Turn.PlayerTurn);
    }

    public override void Pass()
    {
        
    }
}

