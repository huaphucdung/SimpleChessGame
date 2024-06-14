using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuUI : BaseUI
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button menuBtn;

    protected override void AddUIAction(IUIData data = null)
    {
        if (data == null && data is not PauseMenuUIData) return;
        PauseMenuUIData uiData = (PauseMenuUIData)data;
        continueBtn.onClick.AddListener(uiData.continueGameAction);
        replayBtn.onClick.AddListener(uiData.replayGameAction);
        optionsBtn.onClick.AddListener(uiData.settingAction);
        menuBtn.onClick.AddListener(uiData.menuAction);
    }

    protected override void RemoveUIAction()
    {
        continueBtn.onClick.RemoveAllListeners();
        replayBtn.onClick.RemoveAllListeners();
        optionsBtn.onClick.RemoveAllListeners();
        menuBtn.onClick.RemoveAllListeners();
    }
}

public struct PauseMenuUIData : IUIData
{
    public UnityAction continueGameAction;
    public UnityAction replayGameAction;
    public UnityAction settingAction;
    public UnityAction menuAction;
}

