using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour, IUI
{
    protected bool IsShow;

    public virtual void Initialize()
    {
    }

    public virtual void Show(IUIData data = null)
    {
        if (IsShow) return;
        gameObject.SetActive(true);
        IsShow = true;
        AddUIAction(data);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        IsShow = false;
        RemoveUIAction();
    }

    public virtual void UpdateUI(IUIData data = null)
    {
    }

    protected virtual void AddUIAction(IUIData data = null)
    {

    }

    protected virtual void RemoveUIAction()
    {

    }
}
