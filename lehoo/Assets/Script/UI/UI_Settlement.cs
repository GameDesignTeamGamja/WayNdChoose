using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics.Tracing;
using Google.Apis.Json;

public class UI_Settlement : UI_default
{
  private float UIOpenMoveTime = 0.8f;
  private WaitForSeconds LittleWait = new WaitForSeconds(0.2f);
  private float UICloseMoveTime = 0.6f;

  [SerializeField] private Image SettlementIcon = null;
  [SerializeField] private TextMeshProUGUI SettlementNameText = null;
  [SerializeField] private TextMeshProUGUI DiscomfortText = null;
  [SerializeField] private List<PlaceIconScript> SectorIcons=new List<PlaceIconScript>();
  private PlaceIconScript GetSectorIconScript(SectorTypeEnum sectortype)
  {
    for(int i=0;i< SectorIcons.Count;i++)
    {
      if (SectorIcons[i].MyType == sectortype) return SectorIcons[i];
    }
    return null;
  }
  [SerializeField] private TextMeshProUGUI SectorName = null;
  [SerializeField] private TextMeshProUGUI SectorSelectDescription = null;
  [SerializeField] private GameObject RestbuttonHolder = null;
  [SerializeField] private Button RestButton_Sanity = null;
  [SerializeField] private Button RestButton_Gold = null;
  [SerializeField] private TextMeshProUGUI RestDescription = null;
  private Settlement CurrentSettlement = null;
  private SectorTypeEnum SelectedSector = SectorTypeEnum.NULL;
  public void OpenUI()
  {
    IsOpen = true;
    UIManager.Instance.AddUIQueue(openui());
  }
  private IEnumerator openui()
  {
    if (DefaultGroup.interactable == true) DefaultGroup.interactable = false;
    if(DefaultRect.anchoredPosition!=Vector2.zero)DefaultRect.anchoredPosition = Vector2.zero;

    if(RestbuttonHolder.activeInHierarchy==true) RestbuttonHolder.SetActive(false);
    QuestSectorInfo = 0;
    SelectedSector = SectorTypeEnum.NULL;
    CurrentSettlement = GameManager.Instance.MyGameData.CurrentSettlement;
    SettlementNameText.text = CurrentSettlement.Name;
    DiscomfortText.text = CurrentSettlement.Discomfort.ToString();
    RestDescription.text = "";

    Sprite _settlementicon = null;
    int _placecount = 0;
    switch (CurrentSettlement.SettlementType)
    {
      case SettlementType.Village: _placecount = 2;_settlementicon = GameManager.Instance.ImageHolder.VillageIcon_black; break;
      case SettlementType.Town:_placecount = 3; _settlementicon = GameManager.Instance.ImageHolder.TownIcon_black; break;
      case SettlementType.City:_placecount = 4; _settlementicon = GameManager.Instance.ImageHolder.CityIcon_black; break;
    }
    for (int i = 0; i < SectorIcons.Count; i++)
    {
      if (i < _placecount)
      {
        if (SectorIcons[i].gameObject.activeInHierarchy == false) SectorIcons[i].gameObject.SetActive(true);
        SectorIcons[i].OpenIcon();
      }
      else
      {
        if (SectorIcons[i].gameObject.activeInHierarchy == true) SectorIcons[i].gameObject.SetActive(false);
      }
    }

    SettlementIcon.sprite = _settlementicon;
    SectorName.gameObject.SetActive(false);
    SectorName.text = GameManager.Instance.GetTextData("SELECTPLACE");
    if (SectorSelectDescription.gameObject.activeInHierarchy == false) SectorSelectDescription.gameObject.SetActive(true);
    SectorSelectDescription.text = "";
    RestButton_Gold.interactable = false;
    RestButton_Sanity.interactable = false;
    LayoutRebuilder.ForceRebuildLayoutImmediate(SectorName.transform.parent.transform as RectTransform);
    LayoutRebuilder.ForceRebuildLayoutImmediate(SettlementIcon.transform.parent.transform as RectTransform);
    LayoutRebuilder.ForceRebuildLayoutImmediate(RestbuttonHolder.transform as RectTransform);

    string _rectname = "nameholder";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).OutisdePos, GetPanelRect(_rectname).InsidePos, UIOpenMoveTime, true));
    yield return LittleWait;
    _rectname = "placepanel";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).OutisdePos, GetPanelRect(_rectname).InsidePos, UIOpenMoveTime, true));
    yield return LittleWait;
    _rectname = "description";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).OutisdePos, GetPanelRect(_rectname).InsidePos, UIOpenMoveTime, true));
    yield return LittleWait;
    UIManager.Instance.MapButton.Open(0, this);

    DefaultGroup.interactable = true;
  }
  public override void CloseUI()
  {
    IsOpen = false;
    UIManager.Instance.AddUIQueue(closeui());
  }
  private IEnumerator closeui()
  {
    DefaultGroup.interactable = false;
    if (UIManager.Instance.MapButton.IsOpen) UIManager.Instance.MapButton.Close();

    string _rectname = "description";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).InsidePos, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
    yield return LittleWait;
    _rectname = "nameholder";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).InsidePos, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
    yield return LittleWait;
    _rectname = "placepanel";
   yield return StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).InsidePos, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
  }
  public override void CloseForGameover()
  {
    IsOpen = false;

    UIManager.Instance.MapButton.CloseForGameover();

    string _rectname = "description";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).Rect.anchoredPosition, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
    _rectname = "nameholder";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).Rect.anchoredPosition, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
    _rectname = "placepanel";
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect(_rectname).Rect, GetPanelRect(_rectname).Rect.anchoredPosition, GetPanelRect(_rectname).OutisdePos, UICloseMoveTime, false));
  }
  [HideInInspector] public int QuestSectorInfo = 0;
  public void SelectPlace(int index)  //Sectortype은 0이 NULL임
  {
    if (SelectedSector == (SectorTypeEnum)index) return;

    if(SelectedSector!=SectorTypeEnum.NULL) GetSectorIconScript(SelectedSector).SetIdleColor();
    SelectedSector = (SectorTypeEnum)index;

    if(SectorName.gameObject.activeInHierarchy==false) SectorName.gameObject.SetActive(true);
    if(SectorSelectDescription.gameObject.activeInHierarchy==false) SectorSelectDescription.gameObject.SetActive(true);

    switch (GameManager.Instance.MyGameData.QuestType)
    {
      case QuestType.Cult:
        GetSectorIconScript(SelectedSector).SetSelectColor();

        SectorName.text = GameManager.Instance.GetTextData(SelectedSector, 0);
        string _effect = GameManager.Instance.GetTextData(SelectedSector, 3);
        switch (SelectedSector)
        {
          case SectorTypeEnum.Residence:
            _effect = string.Format(_effect, ConstValues.SectorEffect_residence);
            break;
          case SectorTypeEnum.Temple:
            _effect = string.Format(_effect, ConstValues.SectorEffect_temple);
            break;
          case SectorTypeEnum.Marketplace:
            _effect = string.Format(_effect, ConstValues.SectorEffect_marketSector);
            break;
          case SectorTypeEnum.Library:
            _effect = string.Format(_effect, ConstValues.SectorEffect_Library);
            break;
          case SectorTypeEnum.Theater:
            //서비스 종료다...!
            break;
          case SectorTypeEnum.Academy:
            //    _effect = string.Format(_effect, ConstValues.SectorDuration, ConstValues.SectorEffect_acardemy);
            break;
        }
        QuestSectorInfo=GameManager.Instance.MyGameData.Cult_IsSabbat(SelectedSector);
        string _sabbatdescription = "";
        switch (QuestSectorInfo)
        {
          case 0:
            SectorSelectDescription.text = _effect;
            break;
          case 1:
            _sabbatdescription = "<br><br>" + string.Format(GameManager.Instance.GetTextData("Quest0_Progress_Sabbat_Effect"),
WNCText.GetDiscomfortColor(ConstValues.Quest_Cult_SabbatDiscomfort)) + "<br>" + string.Format(GameManager.Instance.GetTextData("Quest0_Progress_Sabbat"), ConstValues.Quest_Cult_Progress_Sabbat);
            SectorSelectDescription.text = _effect + _sabbatdescription;
            break;
          case 2:
            break;
        }

        RestButton_Sanity.interactable = true;

        int _goldpayvalue = SelectedSector != SectorTypeEnum.Marketplace ?
          GameManager.Instance.MyGameData.SettleRestCost_Gold :
          GameManager.Instance.MyGameData.SettleRestCost_Gold * ConstValues.SectorEffect_marketSector / 100;

        if (GameManager.Instance.MyGameData.Gold >= _goldpayvalue) RestButton_Gold.interactable = true;
        else RestButton_Gold.interactable = false;
        break;
    }

    if (RestbuttonHolder.activeInHierarchy == false) RestbuttonHolder.SetActive(true);
    LayoutRebuilder.ForceRebuildLayoutImmediate(RestbuttonHolder.transform as RectTransform);
    LayoutRebuilder.ForceRebuildLayoutImmediate(SectorName.transform.parent.transform as RectTransform);
  }
  public void OnPointerRestType(StatusTypeEnum type)
  {
    if (UIManager.Instance.IsWorking) return;

    int _movepointvalue = 0;
    int _discomfortvalue = 0;
    switch (CurrentSettlement.SettlementType)
    {
      case SettlementType.Village:
        _movepointvalue = ConstValues.RestMovePoint_Village;
        _discomfortvalue = ConstValues.RestDiscomfort_Village;
        break;
      case SettlementType.Town:
        _movepointvalue = ConstValues.RestMovePoint_Town;
        _discomfortvalue = ConstValues.RestDiscomfort_Town;
        break;
      case SettlementType.City:
        _movepointvalue = ConstValues.RestMovePoint_City;
        _discomfortvalue = ConstValues.RestDiscomfort_City;
        break;
    }
    if (SelectedSector == SectorTypeEnum.Residence) _movepointvalue++;

    switch (GameManager.Instance.MyGameData.QuestType)
    {
      case QuestType.Cult:
        if(QuestSectorInfo==1)
          _discomfortvalue += ConstValues.Quest_Cult_SabbatDiscomfort;
        break;
    }


    switch (type)
    {
      case StatusTypeEnum.Sanity:

        RestDescription.text= string.Format(GameManager.Instance.GetTextData("Restbutton_Sanity"),
      WNCText.GetSanityColor("-" + (SelectedSector != SectorTypeEnum.Marketplace ?
          GameManager.Instance.MyGameData.SettleRestCost_Sanity :
          GameManager.Instance.MyGameData.SettleRestCost_Sanity * ConstValues.SectorEffect_marketSector / 100)),
      WNCText.GetMovepointColor("+" + _movepointvalue),
      WNCText.GetDiscomfortColor("+" + _discomfortvalue));
        break;
      case StatusTypeEnum.Gold:
        int _goldpayvalue = SelectedSector != SectorTypeEnum.Marketplace ?
          GameManager.Instance.MyGameData.SettleRestCost_Gold :
          GameManager.Instance.MyGameData.SettleRestCost_Gold * ConstValues.SectorEffect_marketSector / 100;
        if (GameManager.Instance.MyGameData.Gold < _goldpayvalue) return;

        RestDescription.text = string.Format(GameManager.Instance.GetTextData("Restbutton_Gold"),
  WNCText.GetGoldColor("-" + _goldpayvalue),
  WNCText.GetMovepointColor("+" + _movepointvalue),
  WNCText.GetDiscomfortColor("+" + _discomfortvalue));
        break;

    }
  }
  public void OnExitRestType(StatusTypeEnum type)
  {
    return;
  }
  public void StartRest_Sanity()
    {
    if (UIManager.Instance.IsWorking) return;
    CloseUI();
    GameManager.Instance.RestInSector(SelectedSector, true, QuestSectorInfo);
  }
  public void StartRest_Gold()
  {
    if (UIManager.Instance.IsWorking) return;
    CloseUI();
    GameManager.Instance.RestInSector(SelectedSector, false, QuestSectorInfo);
  }
}
