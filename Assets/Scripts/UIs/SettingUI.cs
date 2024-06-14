using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingUI : BaseUI
{
    [Header("Buttons:")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle vfsToggle;
    [SerializeField] private Button exitBtn;


    

    public override void Show(IUIData data = null)
    {
        base.Show(data);
        if (data == null && data is not SettingUIData) return;
        SettingUIData uiData = (SettingUIData)data;

        musicToggle.isOn = uiData.isMuteMussic;
        vfsToggle.isOn = uiData.isMuteVfs;
    }

    protected override void AddUIAction(IUIData data = null)
    {
        if (data == null && data is not SettingUIData) return;
        SettingUIData uiData = (SettingUIData)data;
        musicToggle.onValueChanged.AddListener(uiData.musicAudioChange);
        vfsToggle.onValueChanged.AddListener(uiData.vfsAudioChange);
        exitBtn.onClick.AddListener(uiData.previousUIAction);
    }

   
    protected override void RemoveUIAction()
    {
        musicToggle.onValueChanged.RemoveAllListeners();
        vfsToggle.onValueChanged.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
    }
}

public struct SettingUIData : IUIData
{
    public bool isMuteMussic;
    public bool isMuteVfs;
    public UnityAction<bool> musicAudioChange;
    public UnityAction<bool> vfsAudioChange;
    public UnityAction previousUIAction;
}
