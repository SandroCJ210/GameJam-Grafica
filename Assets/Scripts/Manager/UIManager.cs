using UnityEngine;

public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private GameObject eyesPanel;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private TMPro.TMP_Text turnText;
    private Player _player;
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
}