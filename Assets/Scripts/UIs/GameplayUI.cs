using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameplayUI : BaseUI
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private TMP_Text levelText;
    protected override void AddUIAction(IUIData data = null)
    {
        if (data == null && data is not GameplayUIData) return;
        GameplayUIData uiData = (GameplayUIData)data;
        pauseBtn.onClick.AddListener(uiData.pauseGameAction);
    }

    protected override void RemoveUIAction()
    {
        pauseBtn.onClick.RemoveAllListeners();
    }

    public override void UpdateUI(IUIData data = null)
    {
        if (data == null && data is not GameplayUIUpdateData) return;
        GameplayUIUpdateData updateData = (GameplayUIUpdateData)data;
        levelText.text = $"Level: {updateData.level}";
    }
}

public struct GameplayUIData : IUIData
{
    public UnityAction pauseGameAction;
}

public struct GameplayUIUpdateData : IUIData
{
    public int level;
}

