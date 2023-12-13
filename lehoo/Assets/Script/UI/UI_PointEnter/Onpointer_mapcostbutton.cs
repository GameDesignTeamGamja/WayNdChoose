using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Onpointer_mapcostbutton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
  public UI_map MapUI = null;
  public StatusTypeEnum MyStatusType = StatusTypeEnum.HP;
    public void OnPointerEnter(PointerEventData data)
  {
    if (UIManager.Instance.IsWorking) return;

    MapUI.EnterPointerStatus(MyStatusType);
  }
  public void OnPointerExit(PointerEventData data)
  {
    if (UIManager.Instance.IsWorking) return;

    MapUI.ExitPointerStatus(MyStatusType);
  }
}
