using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventManager : MonoBehaviour
{
  private static EventManager instance;
  public static EventManager Instance { get { return instance; } }
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else Destroy(gameObject);
  }
  private EventHolder MyEventHolder;
  private Dictionary<string, Experience> MyEXP = new Dictionary<string, Experience>();  //경험 딕셔너리

  private void Start()
  {
    MyEventHolder = GameManager.Instance.EventHolder;
    MyEXP = GameManager.Instance.ExpDic;
  }
  /// <summary>
  /// 정착지 진입과 퀘스트 이벤트 실행
  /// </summary>
  /// <param name="_settledata"></param>
  public void SetSettleEvent(TileInfoData _settledata)
  {

    GameManager.Instance.SetSettlementPlace();
  }

  /// <summary>
  /// 정착지에서 장소를 선택해 이벤트 실행
  /// </summary>
  /// <param name="place"></param>
  public void SetSettleEvent(PlaceType place)
    {
    GameManager.Instance.MyGameData.AddPlaceEffectBeforeStartEvent(place);

    TileInfoData _tiledta = GameManager.Instance.MyGameData.CurrentSettlement.TileInfoData;
    EventDataDefulat _event = MyEventHolder.ReturnPlaceEvent(_tiledta.Settlement.Type, place, _tiledta.EnvirList); ;
    GameManager.Instance.SelectEvent(_event);

  }

  /// <summary>
  /// 야외 타일에서 이벤트 실행
  /// </summary>
  /// <param name="_tiledata"></param>
  public void SetOutsideEvent(TileInfoData _tiledata)
  {
        EventDataDefulat _event = null;
        _event = MyEventHolder.ReturnQuestEvent(_tiledata);
        if (_event == null) _event = MyEventHolder.ReturnOutsideEvent(_tiledata.EnvirList);
        //퀘스트가 존재한다면 해당 퀘스트 이벤트를 받아오고 적합한 퀘스트 이벤트가 없을 시 평범한 이벤트를

        GameManager.Instance.SetOuterEvent(_event);

    }
}
