using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUI {
    void Initialize();
    void Show(IUIData data = null);
    void Hide();
    void UpdateUI(IUIData data = null);
}


public interface IUIData { 
}


