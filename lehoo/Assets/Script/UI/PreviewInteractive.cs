using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PreviewPanelType { Turn,HP,Sanity,Gold,Map,Quest,Trait,Theme,Skill,EXP_long,EXP_short,Tendency,Selection,
  RewardHP,RewardSanity,RewardGold,RewardTrait,RewardTheme,RewardSkill,RewardExp,RewardSkillSelect,RewardExpSelect_long,RewardExpSelect_short,Discomfort,
Place,Environment,MadnessAccept,MadnessRefuse,MoveCostSanity,MoveCostGold,RestSanity,RestGold,CultPanel_Sabbat,CultPanel_Ritual,MovePoint,MoveCostGoldNogold,
CultSidePanel,TileInfo,}
public class PreviewInteractive :MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public PreviewPanelType PanelType=PreviewPanelType.Turn;
  public RectTransform OtherRect = null;
  public RectTransform OtherRect_other = null;
  [Space(15)]
    public TendencyTypeEnum MyTendency = TendencyTypeEnum.None;
  public TendencyTypeEnum MySelectionTendency= TendencyTypeEnum.None;
  public bool MySelectionTendencyDir = false;
  public SkillTypeEnum Myskill = SkillTypeEnum.Conversation;
  public int RewardValue = 0;
  public int ExpIndex = 0;
  public Experience MyEXP = null;
  public SectorTypeEnum MyPlaceType = SectorTypeEnum.NULL;
  public EnvironmentType MyEnvironmentType = EnvironmentType.NULL;
  public TileData MyTileData = null;
  public bool IsCultSidePanel = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
    UIManager.Instance.PreviewManager.ClosePreview();
    Experience _exp = null;
    switch (PanelType)
    {
      case PreviewPanelType.Turn:UIManager.Instance.PreviewManager.OpenTurnPreview(OtherRect==null?transform as RectTransform : OtherRect);break;
        case PreviewPanelType.HP:UIManager.Instance.PreviewManager.OpenHPPreview(OtherRect==null?transform as RectTransform : OtherRect);break;
      case PreviewPanelType.Sanity:UIManager.Instance.PreviewManager.OpenSanityPreview(OtherRect==null?transform as RectTransform : OtherRect);break;
        case PreviewPanelType.Gold:UIManager.Instance.PreviewManager.OpenGoldPreview(OtherRect==null?transform as RectTransform : OtherRect);break;
      case PreviewPanelType.Map:UIManager.Instance.PreviewManager.OpenMapPreview();break;
      case PreviewPanelType.Quest:UIManager.Instance.PreviewManager.OpenQuestPreview();break;
      case PreviewPanelType.Skill:UIManager.Instance.PreviewManager.OpenSkillPreview(Myskill, OtherRect==null?transform as RectTransform : OtherRect);break;
      case PreviewPanelType.EXP_long:
         _exp = GameManager.Instance.MyGameData.LongExp;
        if (_exp != null) UIManager.Instance.PreviewManager.OpenExpPreview(_exp, OtherRect==null?transform as RectTransform : OtherRect);
        else UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(GameManager.Instance.GetTextData("NoExp"), new Vector2(1.0f, 0.5f), OtherRect==null?transform as RectTransform : OtherRect);
        break;
        case PreviewPanelType.EXP_short:
         _exp = ExpIndex==0? GameManager.Instance.MyGameData.ShortExp_A: GameManager.Instance.MyGameData.ShortExp_B;
        if (_exp != null) UIManager.Instance.PreviewManager.OpenExpPreview(_exp, OtherRect==null?transform as RectTransform : OtherRect);
        else UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(GameManager.Instance.GetTextData("NoExp"),new Vector2(1.0f,0.5f), OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.Tendency:
        if (GameManager.Instance.MyGameData.GetTendencyLevel(MyTendency) == 0) return;
        UIManager.Instance.PreviewManager.OpenTendencyPreview(MyTendency, OtherRect==null?transform as RectTransform : OtherRect);break;
      case PreviewPanelType.Selection:
        SelectionData _selection = null;
        switch (MySelectionTendency)
        {
          case TendencyTypeEnum.None:
            _selection = GameManager.Instance.MyGameData.CurrentEvent.SelectionDatas[0];
            break;
          case TendencyTypeEnum.Head:
             _selection = GameManager.Instance.MyGameData.CurrentEvent.SelectionDatas[MySelectionTendencyDir==true?0:1];
            break;
          case TendencyTypeEnum.Body:
            _selection = GameManager.Instance.MyGameData.CurrentEvent.SelectionDatas[MySelectionTendencyDir == true ? 0 : 1];
            break;
        }
        switch (_selection.ThisSelectionType)
        {
          case SelectionTargetType.None: UIManager.Instance.PreviewManager.OpenSelectionNonePreview(_selection, MySelectionTendency, MySelectionTendencyDir, OtherRect);break;
          case SelectionTargetType.Pay: UIManager.Instance.PreviewManager.OpenSelectionPayPreview(_selection, MySelectionTendency, MySelectionTendencyDir, OtherRect); break;
          case SelectionTargetType.Check_Single:case SelectionTargetType.Check_Multy:
            UIManager.Instance.PreviewManager.OpenSelectionCheckPreview_skill(_selection, MySelectionTendency, MySelectionTendencyDir, OtherRect); break;
          default: UIManager.Instance.PreviewManager.OpenSelectionElsePreview(_selection, MySelectionTendency, MySelectionTendencyDir, OtherRect); break;
        }
        break;
      case PreviewPanelType.RewardHP:
        UIManager.Instance.PreviewManager.OpenRewardStatusPreview(StatusTypeEnum.HP, GameManager.Instance.MyGameData.RewardHPValue, OtherRect==null?transform as RectTransform : OtherRect); break;
      case PreviewPanelType.RewardSanity:
        UIManager.Instance.PreviewManager.OpenRewardStatusPreview(StatusTypeEnum.Sanity, GameManager.Instance.MyGameData.RewardSanityValue, OtherRect==null?transform as RectTransform : OtherRect); break;
      case PreviewPanelType.RewardGold:
        UIManager.Instance.PreviewManager.OpenRewardStatusPreview(StatusTypeEnum.Gold, GameManager.Instance.MyGameData.RewardGoldValue, OtherRect==null?transform as RectTransform : OtherRect); break;
      case PreviewPanelType.RewardSkill:
        UIManager.Instance.PreviewManager.OpenRewardSkillPreview(Myskill, OtherRect==null?transform as RectTransform : OtherRect);  break;
      case PreviewPanelType.RewardExp:
        UIManager.Instance.PreviewManager.OpenRewardExpPreview(MyEXP, OtherRect==null?transform as RectTransform : OtherRect); break;
      case PreviewPanelType.RewardExpSelect_long:
        _exp = UIManager.Instance.ExpRewardUI.CurrentExp;
        UIManager.Instance.PreviewManager.OpenExpSelectionEmptyPreview(_exp, true, OtherRect==null?transform as RectTransform : OtherRect);
        break;
        if (MyEXP == null) UIManager.Instance.PreviewManager.OpenExpSelectionEmptyPreview(_exp, true, OtherRect==null?transform as RectTransform : OtherRect);
        else
        {
          UIManager.Instance.PreviewManager.OpenExpSelectionExistPreview(MyEXP, _exp, false, OtherRect==null?transform as RectTransform : OtherRect);
        }
        break;
      case PreviewPanelType.RewardExpSelect_short:
        _exp = UIManager.Instance.ExpRewardUI.CurrentExp;
        UIManager.Instance.PreviewManager.OpenExpSelectionEmptyPreview(_exp, false, OtherRect==null?transform as RectTransform : OtherRect);
        break;
        if (MyEXP == null) UIManager.Instance.PreviewManager.OpenExpSelectionEmptyPreview(_exp, false, OtherRect==null?transform as RectTransform : OtherRect);
        else
        {
          UIManager.Instance.PreviewManager.OpenExpSelectionExistPreview(MyEXP, _exp, false, OtherRect==null?transform as RectTransform : OtherRect);
        }
        break;
      case PreviewPanelType.Discomfort:
        UIManager.Instance.PreviewManager.OpenDisComfortPanel( OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.Environment:
        UIManager.Instance.PreviewManager.OpenEnvirPanel(MyEnvironmentType);
        break;
      case PreviewPanelType.MadnessAccept:
        break;
      case PreviewPanelType.MadnessRefuse:
        break;
      case PreviewPanelType.MoveCostSanity:
        string _sanitycosttext = string.Format(GameManager.Instance.GetTextData("MoveCost_Santiy"),
          WNCText.GetSanityColor(UIManager.Instance.MapUI.MoveCost_Sanity)+(UIManager.Instance.MapUI.IsMad?WNCText.GetMadnessColor("?"):""));
        if (UIManager.Instance.MapUI.TotalSupplyCost > GameManager.Instance.MyGameData.Supply)
          _sanitycosttext += string.Format(string.Format(GameManager.Instance.GetTextData("MoveCost_Penalty"),
            UIManager.Instance.MapUI.TotalSupplyCost - GameManager.Instance.MyGameData.Supply,
           WNCText.GetSanityColor((UIManager.Instance.MapUI.TotalSupplyCost - GameManager.Instance.MyGameData.Supply) * GameManager.Instance.MyGameData.PenaltyCost)));

        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(
          _sanitycosttext
          ,new Vector2(0.5f,-0.05f)
          ,OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.MoveCostGold:
        int _defaultvalue = UIManager.Instance.MapUI.MoveCost_Sanity;
        int _requiregold = UIManager.Instance.MapUI.MoveCost_Gold;
        string _goldcosttext = "";
        if (GameManager.Instance.MyGameData.Gold >= _requiregold)
          _goldcosttext = string.Format(GameManager.Instance.GetTextData("MoveCost_Gold_full"), WNCText.GetGoldColor(_requiregold) + (UIManager.Instance.MapUI.IsMad ? WNCText.GetMadnessColor("?") : ""));
        else
          _goldcosttext = string.Format(GameManager.Instance.GetTextData("MoveCost_Gold_lack"),
            WNCText.GetGoldColor(GameManager.Instance.MyGameData.Gold),
           WNCText.GetSanityColor(_defaultvalue - GameManager.Instance.MyGameData.Gold * 2)) + (UIManager.Instance.MapUI.IsMad ? WNCText.GetMadnessColor("?") : "");

        if (UIManager.Instance.MapUI.TotalSupplyCost > GameManager.Instance.MyGameData.Supply)
          _goldcosttext += string.Format(GameManager.Instance.GetTextData("MoveCost_Penalty"),
            UIManager.Instance.MapUI.TotalSupplyCost - GameManager.Instance.MyGameData.Supply,
            WNCText.GetSanityColor((UIManager.Instance.MapUI.TotalSupplyCost - GameManager.Instance.MyGameData.Supply) * GameManager.Instance.MyGameData.PenaltyCost));

        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(
  _goldcosttext
  , new Vector2(0.5f, -0.05f)
  , OtherRect == null ? transform as RectTransform : OtherRect);

        break;
      case PreviewPanelType.RestSanity:
        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(GameManager.Instance.GetTextData("REST_SANITY"), OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.RestGold:
        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(GameManager.Instance.GetTextData("REST_GOLD"), OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.MovePoint:
        UIManager.Instance.PreviewManager.OpenMovePointPreview(OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.MoveCostGoldNogold:
        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(GameManager.Instance.GetTextData("NOGOLD_TEXT"), OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.CultSidePanel:
        string _cultinfo = string.Format(GameManager.Instance.GetTextData("Cult_Preview_progress"),Mathf.FloorToInt(GameManager.Instance.MyGameData.Quest_Cult_Progress));
        string _sametext = "";
        switch (GameManager.Instance.MyGameData.Quest_Cult_Phase)
        {
          case 0:
            _sametext = GameManager.Instance.GetTextData("Village");
            _cultinfo += string.Format(GameManager.Instance.GetTextData("Cult_Preview_Settlement"),
              _sametext, _sametext, ConstValues.Quest_Cult_Progress_Village, GameManager.Instance.MyGameData.Cult_CoolTime);
            break;
          case 1:
            _sametext = GameManager.Instance.GetTextData("Town");
            _cultinfo += string.Format(GameManager.Instance.GetTextData("Cult_Preview_Settlement"),
             _sametext, _sametext, ConstValues.Quest_Cult_Progress_Town, GameManager.Instance.MyGameData.Cult_CoolTime);
            break;
          case 2:
            _sametext = GameManager.Instance.GetTextData("City");
            _cultinfo += string.Format(GameManager.Instance.GetTextData("Cult_Preview_Settlement"),
            _sametext, _sametext, ConstValues.Quest_Cult_Progress_City, GameManager.Instance.MyGameData.Cult_CoolTime);
            break;
          case 3:
            _sametext = GameManager.Instance.GetTextData(GameManager.Instance.MyGameData.Cult_SabbatSector,0);
            _cultinfo += string.Format(GameManager.Instance.GetTextData("Cult_Preview_Sabbat"),
           _sametext, _sametext,ConstValues.Quest_Cult_Progress_Sabbat,GameManager.Instance.MyGameData.Cult_CoolTime,ConstValues.Quest_Cult_Sabbat_PenaltyDiscomfort);
            break;
          case 4:
            _cultinfo += string.Format(GameManager.Instance.GetTextData("Cult_Preview_Ritual"),
            ConstValues.Quest_Cult_Progress_Ritual, GameManager.Instance.MyGameData.Cult_CoolTime,ConstValues.Quest_Cult_Ritual_PenaltySupply);
            break;
        }
        UIManager.Instance.PreviewManager.OpenJustDescriptionPreview(_cultinfo, IsCultSidePanel?new Vector2(1.05f,0.5f):new Vector2(0.5f,1.05f),OtherRect==null?transform as RectTransform : OtherRect);
        break;
      case PreviewPanelType.TileInfo:
        UIManager.Instance.PreviewManager.OpenTileInfoPreveiew( MyTileData,
          MyTileData.TileSettle != null && (MyTileData.Coordinate.y - GameManager.Instance.MyGameData.Coordinate.y == -3) ? OtherRect_other : MyTileData.Coordinate.y <= GameManager.Instance.MyGameData.Coordinate.y ? OtherRect:OtherRect_other);
        break;

    }
  }
    public void OnPointerExit(PointerEventData eventData) 
    {
    UIManager.Instance.PreviewManager.ClosePreview();
  }


}
