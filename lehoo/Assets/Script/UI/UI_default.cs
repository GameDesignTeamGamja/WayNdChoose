using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_default : MonoBehaviour
{
  public RectTransform MyRect = null;
  public CanvasGroup MyGroup = null;
  public bool IsOpen = false;
  public UIMoveDir MyDir = UIMoveDir.Horizontal;
  public virtual void OpenUI(bool _islarge)
  {
    if (IsOpen) { CloseUI();IsOpen = false; return; }
    IsOpen = true;
        UIManager.Instance.AddUIQueue(UIManager.Instance.OpenUI(MyRect, MyGroup, MyDir, _islarge, false));
  }
  public virtual void CloseUI()
  {
    UIManager.Instance.AddUIQueue(UIManager.Instance.CloseUI(MyRect, MyGroup, MyDir, false));
    IsOpen = false;
  }
  public virtual void CloseUiQuick()
  {
    MyGroup.alpha = 0.0f;
    MyGroup.interactable = false;
    MyGroup.blocksRaycasts = false;
  }
}
