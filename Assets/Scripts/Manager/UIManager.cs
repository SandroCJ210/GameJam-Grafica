using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private GameObject eyesPanel;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private TMPro.TMP_Text turnText;
    [SerializeField] private List<Image> playerCardSlots;
    [SerializeField] private List<Sprite> cardSprites;
    private Player _player;

    protected override void Awake()
    {
        base.Awake();
        ClearAllSlots(playerCardSlots);
        HideAllPanels();
    }
    public void Start()
    {
        _player = GameManager.Instance._player;
    }

    public void ShowEyesPanel()
    {
        if (eyesPanel != null)
        {
            eyesPanel.SetActive(true);
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void ShowActionPanel()
    {
        if (eyesPanel != null)
        {
            eyesPanel.SetActive(false);
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(true);
        }
    }

    public void HideAllPanels()
    {
        if (eyesPanel != null)
        {
            eyesPanel.SetActive(false);
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }
    

    public void OnClickEyesOpen()
    {
        if (_player != null)
        {
            _player.SetEyesChoice(true);
        }
        ShowActionPanel();
    }

    public void OnClickEyesClosed()
    {
        if (_player != null)
        {
            _player.SetEyesChoice(false);
        }
        ShowActionPanel();
    }

    public void OnClickDraw()
    {
        if (_player != null)
        {
            _player.SetActionChoice(GamblerChoice.Draw);
        }
    }

    public void OnClickPass()
    {
        if (_player != null)
        {
            _player.SetActionChoice(GamblerChoice.Pass);
        }
        ShowEyesPanel();
    }

    public void SetTurnText(Turn turn)
    {
        switch (turn)
        {
            case Turn.PlayerTurn:
                turnText.text = "Player's Turn";
                break;
            case Turn.WizardTurn:
                turnText.text = "Wizard's Turn";
                break;
        }
    }
    
    public void ShowPlayerCard(int cardValue)
    {
        Image slot = GetFirstEmptySlot(playerCardSlots);
        if (slot == null)
        {
            Debug.LogWarning("No hay slots libres para cartas del jugador.");
            return;
        }

        Sprite sprite = GetSpriteForCard(cardValue);
        if (sprite == null)
        {
            Debug.LogWarning($"No se encontró sprite para la carta con valor {cardValue}.");
            return;
        }

        bool visible = _player == null || _player.areEyesOpen;

        slot.enabled = true;
        slot.sprite = sprite;

        Color c = slot.color;
        c.a = visible ? 1f : 0f; // 1 si ojos abiertos, 0 si cerrados
        slot.color = c;
    }

    
    private Sprite GetSpriteForCard(int cardValue)
    {
        if (cardSprites == null || cardSprites.Count == 0)
        {
            return null;
        }

        int index = cardValue - 1; // Asumimos que valor 1 → índice 0

        if (index < 0 || index >= cardSprites.Count)
        {
            return null;
        }

        return cardSprites[index];
    }
    
    private Image GetFirstEmptySlot(List<Image> slots)
    {
        if (slots == null)
        {
            return null;
        }

        foreach (Image img in slots)
        {
            if (img == null)
            {
                continue;
            }

            // Vacío SOLO si no tiene sprite
            if (img.sprite == null)
            {
                return img;
            }
        }

        return null; // No hay espacio libre
    }


    public void ResetPlayerCards()
    {
        ClearAllSlots(playerCardSlots);
    }

    private void ClearAllSlots(List<Image> slots)
    {
        if (slots == null)
        {
            return;
        }

        foreach (Image img in slots)
        {
            if (img == null)
            {
                continue;
            }

            img.sprite = null;
            img.enabled = false;

            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }

    
    public void SetPlayerCardsVisibility(bool visible)
    {
        float alpha = visible ? 1f : 0f;

        foreach (Image img in playerCardSlots)
        {
            if (img == null)
            {
                continue;
            }

            if (img.sprite == null)
            {
                // Slot vacío, no tocamos
                continue;
            }

            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }



}