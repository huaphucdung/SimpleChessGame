using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private TMP_Text bestLevel;
    [SerializeField] private TMP_Text currentLevel;


    public override void Show(IUIData data = null)
    {
        base.Show(data);

        GameOverUIData uiData = (GameOverUIData)data;
        bestLevel.text = $"Your best level: {uiData.bestLevel}";
        currentLevel.text = $"{uiData.currentLevel}";
    }

    protected override void AddUIAction(IUIData data = null)
    {
        if (data == null && data is not GameOverUIData) return;
        GameOverUIData uiData = (GameOverUIData)data;
        replayBtn.onClick.AddListener(uiData.replayGameAction);
        menuBtn.onClick.AddListener(uiData.menuGameAction);
    }

    protected override void RemoveUIAction()
    {
        replayBtn.onClick.RemoveAllListeners();
        menuBtn.onClick.RemoveAllListeners(); 
    }
}


public struct GameOverUIData : IUIData
{
    public UnityAction replayGameAction;
    public UnityAction menuGameAction;
    public int currentLevel;
    public int bestLevel;
}
