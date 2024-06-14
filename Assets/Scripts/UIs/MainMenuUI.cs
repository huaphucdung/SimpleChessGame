using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button settingBtn;

    protected override void AddUIAction(IUIData data = null)
    {
        if (data == null && data is not MainMenuUIData) return; 
        MainMenuUIData uiData = (MainMenuUIData)data;
        playBtn.onClick.AddListener(uiData.startGameAction);
        settingBtn.onClick.AddListener(uiData.settingAction);
    }
 
    protected override void RemoveUIAction()
    {
        playBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();
    }

}


public struct MainMenuUIData : IUIData {
    public UnityAction startGameAction;
    public UnityAction settingAction;
}
