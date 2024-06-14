using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<string, BaseUI> uiDictioanry;

    [Header("Transition Screen:")]
    [SerializeField] private Image blackScreenTop;
    [SerializeField] private Image blackScreenBottom;
    [SerializeField] private float positionClose =  900f;
    [SerializeField] private float positionOpen = 2300f;

#if UNITY_EDITOR
    public TMP_Text fps;
#endif

    private IUI currentUI;
    private IUIData currentUIData;

    public IUI previousUI { get; private set; }
    public IUIData previousUIData { get; private set; }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        fps.text = $"{(int)(1.0f / Time.deltaTime)}FPS";
    }
#endif
    public void Initialize()
    {
        foreach (KeyValuePair<string, BaseUI> ui in uiDictioanry)
        {
            ui.Value.Initialize();
        }
#if UNITY_EDITOR
        fps.gameObject.SetActive(true);
#endif
    }

    public void ChangeUI<T>(IUIData data = null) where T : BaseUI
    {
        string uiKey = typeof(T).ToString();
        if (!uiDictioanry.ContainsKey(uiKey)) return;

        //Save previous UI and Data
        previousUI = currentUI;
        previousUIData = currentUIData;

        IUI ui = uiDictioanry[uiKey];
        currentUI?.Hide();

        //Save current UI and Data
        currentUI = ui;
        currentUIData = data;
        
        currentUI.Show(data);
    }

    public void HideCurrentUI()
    {
        currentUI?.Hide();
    }

    public void UpdateData(IUIData data)
    {
        currentUI?.UpdateUI(data);
    }

    public void SetDefault()
    {
        blackScreenTop.rectTransform.localPosition  = new Vector3(0, positionOpen, 0);
        blackScreenBottom.rectTransform.localPosition = new Vector3(0, -positionOpen, 0);
    }

    public void ChangeScreen(Action oneStepAction, Action finishAction)
    {
      
        int counter = 0;
        blackScreenBottom.rectTransform.DOLocalMoveY(-positionClose, 1.5f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo);
        blackScreenTop.rectTransform.DOLocalMoveY(positionClose, 1.5f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo).OnStepComplete(() =>
        {
            if (counter == 0)
            {
                HideCurrentUI();
                oneStepAction?.Invoke();
            }
            counter++;
        }).OnComplete(() =>
        {
            finishAction?.Invoke();
        });
    }

    public void ChangePreviousUI()
    {
        currentUI?.Hide();
        //Save current UI and Data
        currentUI = previousUI;
        currentUIData = previousUIData;
        currentUI.Show(currentUIData);
    }
}
