using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class UI_dialogue : UI_default
{
  private enum DialogueTypeEnum { None,Event,Settlement}
  private DialogueTypeEnum CurrentDialogueType=DialogueTypeEnum.None;
  public float OpenTime = 1.5f;
  public float CloseTime = 1.0f;
  public RectTransform DialogueRect = null;
  public CanvasGroup DialogueAlpha = null;

  public Vector2 EventDialogueSize = new Vector2(475.0f, 480.0f);
  public Vector2 SettlementDialogueSize = new Vector2(475.0f, 780.0f);
  public Vector2 LeftPos = new Vector2(-1350.0f, 0.0f);
  public Vector2 CenterPos = Vector2.zero;
  public Vector2 RightPos = new Vector2(1250.0f, 0.0f);
  public Image SettlementBackground = null;
  [SerializeField] private Vector2 Descriptionpos_Outside = new Vector2(0.0f, 342.0f);
  [SerializeField] private Vector2 Descriptionpos_Inside = new Vector2(-600.0f, 342.0f);
  [Header("이벤트")]
  #region 이벤트
  public GameObject EventObjectHolder = null;
  [Space(10)]
  [SerializeField] private RectTransform IllustRect = null;
  [SerializeField] private ImageSwapScript Illust = null;
  public float FadeTime = 0.8f;
  [SerializeField] private Image IllustEffect_Image = null;
  [SerializeField] private CanvasGroup IllustEffect_Group = null;
  [SerializeField] private Color SuccessColor = Color.white;
  [SerializeField] private Color FailColor = Color.red;

  [SerializeField] private TextMeshProUGUI NameText = null;
  [SerializeField] private TextMeshProUGUI DescriptionText = null;
  public Scrollbar DescriptionScrollBar = null;
  public AnimationCurve ScrollbarCurve = new AnimationCurve();
  private IEnumerator updatescrollbar()
  {
    yield return new WaitForSeconds(0.05f);

    float _time = 0.0f;
    while (DescriptionScrollBar.value > 0.001f && _time < ConstValues.ScrollTime)
    {
      DescriptionScrollBar.value = Mathf.Lerp(DescriptionScrollBar.value, 0.0f, ConstValues.ScrollSpeed);
      _time += Time.deltaTime;
      yield return null;

    }
    DescriptionScrollBar.value = 0.0f;
  }
  [SerializeField] private CanvasGroup NextButtonGroup = null;
  private void SetNextButtonDisable() => NextButtonGroup.interactable = false;
  public void SetNextButtonActive()=> NextButtonGroup.interactable = true;
  [SerializeField] private CanvasGroup RewardButtonGroup = null;
  [SerializeField] private Image RewardIcon = null;
  [SerializeField] private TextMeshProUGUI RewardDescription = null;
  [SerializeField] private UI_RewardExp RewardExpUI = null;
  [SerializeField] private RectTransform MapbuttonPos_Event = null;
  [SerializeField] private CanvasGroup EndingButtonGroup = null;
  [SerializeField] private TextMeshProUGUI EndingButtonText = null;
  [SerializeField] private CanvasGroup EndingRefuseGroup = null;
  [SerializeField] private TextMeshProUGUI EndingRefuseText = null;
  [SerializeField] private CanvasGroup SelectionGroup = null;
  [SerializeField] private UI_Selection Selection_A = null;
  
  public int GetRequireValue(bool dir)
  {
    if (dir) return RequireValue_A;
    else return RequireValue_B;
  }
  private int RequireValue_A
  {
    get
    {
      int _value = 0;
      switch (CurrentEvent.SelectionDatas[0].ThisSelectionType)
      {
        default:return 0;
        case SelectionTargetType.Pay:
          switch (CurrentEvent.SelectionDatas[0].SelectionPayTarget)
          {
            case StatusTypeEnum.HP:
              return GameManager.Instance.MyGameData.PayHPValue(GetUsingExpEffectCount(ExpUsageDic_L, EffectType.HPLoss));
            case StatusTypeEnum.Sanity:
              return GameManager.Instance.MyGameData.PaySanityValue(GetUsingExpEffectCount(ExpUsageDic_L, EffectType.SanityLoss));
            case StatusTypeEnum.Gold:
              return GameManager.Instance.MyGameData.PayGoldValue;
            default: return 0;
          }
        case SelectionTargetType.Check_Single:
          switch (CurrentEvent.SelectionDatas[0].SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              _value =GameManager.Instance.MyGameData.Skill_Conversation.Level+GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value =GameManager.Instance.MyGameData.Skill_Force.Level+ GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value = GameManager.Instance.MyGameData.Skill_Wild.Level +GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value = GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          return _value;
        case SelectionTargetType.Check_Multy:
          switch (CurrentEvent.SelectionDatas[0].SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              _value = GameManager.Instance.MyGameData.Skill_Conversation.Level +GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value = GameManager.Instance.MyGameData.Skill_Force.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value = GameManager.Instance.MyGameData.Skill_Wild.Level +GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value = GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          switch (CurrentEvent.SelectionDatas[0].SelectionCheckSkill[1])
          {
            case SkillTypeEnum.Conversation:
              _value += GameManager.Instance.MyGameData.Skill_Conversation.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value += GameManager.Instance.MyGameData.Skill_Force.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value += GameManager.Instance.MyGameData.Skill_Wild.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value += GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_L, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          return _value;
      }
    }
  }
  private int RequireValue_B
  {
    get
    {
      int _value = 0;
      switch (CurrentEvent.SelectionDatas[1].ThisSelectionType)
      {
        default: return 0;
        case SelectionTargetType.Pay:
          switch (CurrentEvent.SelectionDatas[1].SelectionPayTarget)
          {
            case StatusTypeEnum.HP:
              return GameManager.Instance.MyGameData.PayHPValue(GetUsingExpEffectCount(ExpUsageDic_R, EffectType.HPLoss));
            case StatusTypeEnum.Sanity:
              return GameManager.Instance.MyGameData.PaySanityValue(GetUsingExpEffectCount(ExpUsageDic_R, EffectType.SanityLoss));
            case StatusTypeEnum.Gold:
              return GameManager.Instance.MyGameData.PayGoldValue;
            default: return 0;
          }
        case SelectionTargetType.Check_Single:
          switch (CurrentEvent.SelectionDatas[1].SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              _value = GameManager.Instance.MyGameData.Skill_Conversation.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value = GameManager.Instance.MyGameData.Skill_Force.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value = GameManager.Instance.MyGameData.Skill_Wild.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value = GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          return _value;
        case SelectionTargetType.Check_Multy:
          switch (CurrentEvent.SelectionDatas[1].SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              _value = GameManager.Instance.MyGameData.Skill_Conversation.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value = GameManager.Instance.MyGameData.Skill_Force.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value = GameManager.Instance.MyGameData.Skill_Wild.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value = GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          switch (CurrentEvent.SelectionDatas[1].SelectionCheckSkill[1])
          {
            case SkillTypeEnum.Conversation:
              _value += GameManager.Instance.MyGameData.Skill_Conversation.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Conversation);
              break;
            case SkillTypeEnum.Force:
              _value += GameManager.Instance.MyGameData.Skill_Force.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Force);
              break;
            case SkillTypeEnum.Wild:
              _value += GameManager.Instance.MyGameData.Skill_Wild.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Wild);
              break;
            case SkillTypeEnum.Intelligence:
              _value += GameManager.Instance.MyGameData.Skill_Intelligence.Level + GetUsingExpEffectCount(ExpUsageDic_R, EffectType.Intelligence);
              break;
            default:
              _value += 0;
              break;
          }
          return _value;
      }
    }
  }
  [SerializeField] private UI_Selection Selection_B = null;
  public Dictionary<Experience, int> ExpUsageDic_L = new Dictionary<Experience, int>();
  public Dictionary<Experience, int> ExpUsageDic_R = new Dictionary<Experience, int>();
  public int GetUsingExpEffectCount(Dictionary<Experience,int> dic, EffectType targettype)
  {
    int _count = 0;
    foreach(var _data in dic)
    {
      if (_data.Key.Effects.Contains(targettype)) _count += _data.Value;
    }
    return _count;
  }
  public void AddExp(bool isL, Experience exp)
  {
    SelectionData _targetselection = null;
    bool _enable = false;
    if (isL)
    {
      _targetselection = CurrentEvent.SelectionDatas[0];
      switch (_targetselection.ThisSelectionType)
      {
        case SelectionTargetType.None:
          break;
        case SelectionTargetType.Pay:
          if (_targetselection.SelectionPayTarget == StatusTypeEnum.HP && exp.Effects.Contains(EffectType.HPLoss)) _enable = true;
          else if (_targetselection.SelectionPayTarget == StatusTypeEnum.Sanity && exp.Effects.Contains(EffectType.SanityLoss)) _enable = true;
          break;
        case SelectionTargetType.Check_Single:
          switch (_targetselection.SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              if (exp.Effects.Contains(EffectType.Conversation)) _enable = true;
              break;
            case SkillTypeEnum.Force:
              if (exp.Effects.Contains(EffectType.Force)) _enable = true;
              break;
            case SkillTypeEnum.Wild:
              if (exp.Effects.Contains(EffectType.Wild)) _enable = true;
              break;
            case SkillTypeEnum.Intelligence:
              if (exp.Effects.Contains(EffectType.Intelligence)) _enable = true;
              break;
          }
          break;
        case SelectionTargetType.Check_Multy:
          foreach(var _skill in _targetselection.SelectionCheckSkill)
          {
            switch (_skill)
            {
              case SkillTypeEnum.Conversation:
                if (exp.Effects.Contains(EffectType.Conversation)) _enable = true;
                break;
              case SkillTypeEnum.Force:
                if (exp.Effects.Contains(EffectType.Force)) _enable = true;
                break;
              case SkillTypeEnum.Wild:
                if (exp.Effects.Contains(EffectType.Wild)) _enable = true;
                break;
              case SkillTypeEnum.Intelligence:
                if (exp.Effects.Contains(EffectType.Intelligence)) _enable = true;
                break;
            }
          }
          break;
      }

      if (_enable)
      {
        if (ExpUsageDic_L.ContainsKey(exp))
        {
          if (ExpUsageDic_L[exp] < exp.Duration -1) ExpUsageDic_L[exp]++;
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
        else if (ExpUsageDic_R.ContainsKey(exp))
        {
          if (exp.Duration > 1)
          {
            ExpUsageDic_R.Remove(exp);
            ExpUsageDic_L.Add(exp, 1);
          }
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
        else
        {
          if (exp.Duration > 1)
          {
            ExpUsageDic_L.Add(exp, 1);
          }
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
      }
    }
    else
    {
      _targetselection = CurrentEvent.SelectionDatas[1];
      switch (_targetselection.ThisSelectionType)
      {
        case SelectionTargetType.None:
          break;
        case SelectionTargetType.Pay:
          if (_targetselection.SelectionPayTarget == StatusTypeEnum.HP && exp.Effects.Contains(EffectType.HPLoss)) _enable = true;
          else if (_targetselection.SelectionPayTarget == StatusTypeEnum.Sanity && exp.Effects.Contains(EffectType.SanityLoss)) _enable = true;
          break;
        case SelectionTargetType.Check_Single:
          switch (_targetselection.SelectionCheckSkill[0])
          {
            case SkillTypeEnum.Conversation:
              if (exp.Effects.Contains(EffectType.Conversation)) _enable = true;
              break;
            case SkillTypeEnum.Force:
              if (exp.Effects.Contains(EffectType.Force)) _enable = true;
              break;
            case SkillTypeEnum.Wild:
              if (exp.Effects.Contains(EffectType.Wild)) _enable = true;
              break;
            case SkillTypeEnum.Intelligence:
              if (exp.Effects.Contains(EffectType.Intelligence)) _enable = true;
              break;
          }
          break;
        case SelectionTargetType.Check_Multy:
          foreach (var _skill in _targetselection.SelectionCheckSkill)
          {
            switch (_skill)
            {
              case SkillTypeEnum.Conversation:
                if (exp.Effects.Contains(EffectType.Conversation)) _enable = true;
                break;
              case SkillTypeEnum.Force:
                if (exp.Effects.Contains(EffectType.Force)) _enable = true;
                break;
              case SkillTypeEnum.Wild:
                if (exp.Effects.Contains(EffectType.Wild)) _enable = true;
                break;
              case SkillTypeEnum.Intelligence:
                if (exp.Effects.Contains(EffectType.Intelligence)) _enable = true;
                break;
            }
          }
          break;
      }

      if (_enable)
      {
        if (ExpUsageDic_R.ContainsKey(exp))
        {
          if (ExpUsageDic_R[exp] < exp.Duration -1) ExpUsageDic_R[exp]++;
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
        else if (ExpUsageDic_L.ContainsKey(exp))
        {
          if (exp.Duration > 1)
          {
            ExpUsageDic_L.Remove(exp);
            ExpUsageDic_R.Add(exp, 1);
          }
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
        else
        {
          if (exp.Duration > 1)
          {
            ExpUsageDic_R.Add(exp, 1);
          }
          else
          {
            UIManager.Instance.ExpUsingWarning(exp);
            return;
          }
        }
      }
    }
    if (_enable)
    {
      if (CurrentEvent.Selection_type == SelectionTypeEnum.Single)
      {
        Selection_A.UpdateValues();
      }
      else
      {
        Selection_A.UpdateValues();
        Selection_B.UpdateValues();
      }
      UIManager.Instance.UpdateExpUse();
    }

  }
  public void SubExp(Experience exp)
  {
    if (ExpUsageDic_L.ContainsKey(exp))
    {
      if (ExpUsageDic_L[exp] == 1) ExpUsageDic_L.Remove(exp);
      else ExpUsageDic_L[exp]--;

      if (CurrentEvent.Selection_type == SelectionTypeEnum.Single)
      {
        Selection_A.UpdateValues();
      }
      else
      {
        Selection_A.UpdateValues();
        Selection_B.UpdateValues();
      }

      UIManager.Instance.UpdateExpUse();
 }
    else if (ExpUsageDic_R.ContainsKey(exp))
    {
      if (ExpUsageDic_R[exp] == 1) ExpUsageDic_R.Remove(exp);
      else ExpUsageDic_R[exp]--;

      if (CurrentEvent.Selection_type == SelectionTypeEnum.Single)
      {
        Selection_A.UpdateValues();
      }
      else
      {
        Selection_A.UpdateValues();
        Selection_B.UpdateValues();
      }

      UIManager.Instance.UpdateExpUse();
 }
  }
  public GameObject RewardAskObject = null;
  public TextMeshProUGUI RewardAskText = null;
  public TextMeshProUGUI RewardText_Yes = null;
  public TextMeshProUGUI RewardText_No = null;
  private ReturnButton CurrentReturnButton = null;
  public void OpenRewardAsk(ReturnButton currentreturnbutton)
  {
    CurrentReturnButton=currentreturnbutton;
    RewardAskObject.SetActive(true);
  }
  public void RewardAskClick_Yes()
  {
    RemainReward = false;
    CurrentReturnButton.Clicked();
  }
  public void RewardAskClick_No()
  {
    RewardAskObject.SetActive(false);
  }

  private EventData CurrentEvent
  {
    get { return GameManager.Instance.MyGameData.CurrentEvent; }
  }

  private UI_Selection GetOppositeSelection(UI_Selection _selection)
  {
    if (_selection == Selection_A) return Selection_B;
    return Selection_A;
  }
  public IEnumerator OpenEventUI(bool dir)
  {
    if (PlayerPrefs.GetInt("Tutorial_Event") == 0) UIManager.Instance.TutorialUI.OpenTutorial_Event();
    if (RewardAskText.text == "")
    {
      RewardText_Yes.text = GameManager.Instance.GetTextData("YES");
      RewardText_No.text = GameManager.Instance.GetTextData("NO");
    }
    RewardAskText.text = string.Format(GameManager.Instance.GetTextData("NOREWARD"),
      GameManager.Instance.MyGameData.CurrentSettlement == null ? GameManager.Instance.GetTextData("Map") : GameManager.Instance.GetTextData("Settlement"));

    if (CurrentDialogueType == DialogueTypeEnum.None)
    {
      if (MadenssEffect.enabled) MadenssEffect.enabled = false;
      if (!DefaultGroup.interactable) DefaultGroup.interactable = true;
      if (RewardAskObject.activeInHierarchy) RewardAskObject.SetActive(false);
      if (QuitAskObject.activeInHierarchy) QuitAskObject.SetActive(false);
      if (DialogueAlpha.alpha == 0.0f) DialogueAlpha.alpha = 1.0f;
      ExpUsageDic_L.Clear();
      ExpUsageDic_R.Clear();
      IsOpen = true;

      UIManager.Instance.SettleButton.DeActive();
      UIManager.Instance.MapButton.DeActive();
      if (EventObjectHolder.activeInHierarchy == false) EventObjectHolder.SetActive(true);
      if (SettlementObjectHolder.activeInHierarchy == true) SettlementObjectHolder.SetActive(false);
      DialogueRect.sizeDelta = EventDialogueSize;

      if (CurrentEvent.AppearSpace == EventAppearType.Outer)
      {
        UIManager.Instance.UpdateBackground(CurrentEvent.EnvironmentType);
        if (SettlementBackground.enabled == true) SettlementBackground.enabled = false;
      }
      else
      {
        if (SettlementBackground.enabled == false) SettlementBackground.enabled = true;
      }


      EndingButtonGroup.alpha = 0.0f;
      EndingButtonGroup.interactable = false;
      EndingButtonGroup.blocksRaycasts = false;
      EndingRefuseGroup.alpha = 0.0f;
      EndingRefuseGroup.interactable = false;
      EndingRefuseGroup.blocksRaycasts = false;
      NextButtonGroup.alpha = 0.0f;
      NextButtonGroup.interactable = false;
      RewardButtonGroup.alpha = 0.0f;
      RewardButtonGroup.interactable = false;
      SelectionGroup.alpha = 0.0f;
      SelectionGroup.interactable = false;

      EventIllustHolderes = CurrentEvent.BeginningIllusts;
      EventDescriptions = CurrentEvent.BeginningDescriptions;
      IsBeginning = true;
      EventPhaseIndex = 0;

      NameText.text = CurrentEvent.Name;
      DescriptionText.text = "";

      UpdateSelections();

      DefaultGroup.interactable = false;
      StartCoroutine(displaynextindex(true));

      Vector2 _startpos = dir ? LeftPos : RightPos;
      yield return StartCoroutine(UIManager.Instance.moverect(DefaultRect, _startpos, CenterPos, OpenTime, UIManager.Instance.UIPanelOpenCurve));

      DefaultGroup.interactable = true;
    }
    else if (CurrentDialogueType == DialogueTypeEnum.Settlement && CurrentEvent.AppearSpace != EventAppearType.Outer)
    {
      DefaultGroup.interactable = false;
      UIManager.Instance.SettleButton.DeActive();
      ExpUsageDic_R.Clear();
      ExpUsageDic_L.Clear();
      StartCoroutine(UIManager.Instance.ChangeAlpha(DialogueAlpha, 0.0f, CloseTime, UIManager.Instance.UIPanelCLoseCurve));
      StartCoroutine(UIManager.Instance.moverect(DialogueRect, Descriptionpos_Outside, Descriptionpos_Inside, CloseTime, UIManager.Instance.UIPanelCLoseCurve));
      yield return new WaitForSeconds(CloseTime);
      if (MadenssEffect.enabled) MadenssEffect.enabled = false;
      SettlementObjectHolder.SetActive(false);
      EventObjectHolder.SetActive(true);
      DialogueRect.sizeDelta = EventDialogueSize;

      EndingButtonGroup.alpha = 0.0f;
      EndingButtonGroup.interactable = false;
      EndingButtonGroup.blocksRaycasts = false;
      EndingRefuseGroup.alpha = 0.0f;
      EndingRefuseGroup.interactable = false;
      EndingRefuseGroup.blocksRaycasts = false;
      NextButtonGroup.alpha = 0.0f;
      NextButtonGroup.interactable = false;
      RewardButtonGroup.alpha = 0.0f;
      RewardButtonGroup.interactable = false;
      SelectionGroup.alpha = 0.0f;
      SelectionGroup.interactable = false;

      EventIllustHolderes = CurrentEvent.BeginningIllusts;
      EventDescriptions = CurrentEvent.BeginningDescriptions;
      IsBeginning = true;
      IsOpen = true;
      EventPhaseIndex = 0;
      PhrIndex = -1;

      NameText.text = CurrentEvent.Name;
      DescriptionText.text = "";
      UpdateSelections();
      StartCoroutine(displaynextindex(true));

      StartCoroutine(UIManager.Instance.moverect(DialogueRect, Descriptionpos_Inside, Descriptionpos_Outside, OpenTime, UIManager.Instance.UIPanelOpenCurve));
      StartCoroutine(UIManager.Instance.ChangeAlpha(DialogueAlpha, 1.0f, OpenTime, UIManager.Instance.UIPanelOpenCurve));
      yield return new WaitForSeconds(OpenTime);
      DefaultGroup.interactable = true;
    }
    CurrentDialogueType = DialogueTypeEnum.Event;
    yield return null;
  }
  public IEnumerator OpenEventUI(bool issuccess, bool isleft, bool dir)
  {
    if (MadenssEffect.enabled) MadenssEffect.enabled = false;
    if (!DefaultGroup.interactable) DefaultGroup.interactable = true;
    if (DialogueAlpha.alpha == 0.0f) DialogueAlpha.alpha = 1.0f;
    ExpUsageDic_L.Clear();
    ExpUsageDic_R.Clear();
    IsOpen = true;
    if (RewardAskObject.activeInHierarchy) RewardAskObject.SetActive(false);
    if (QuitAskObject.activeInHierarchy) QuitAskObject.SetActive(false);
    if (EventObjectHolder.activeInHierarchy == false) EventObjectHolder.SetActive(true);
    if (SettlementObjectHolder.activeInHierarchy == true) SettlementObjectHolder.SetActive(false);
    if (SettlementBackground.enabled == true) SettlementBackground.enabled = false;
    UIManager.Instance.SettleButton.DeActive();
    DialogueRect.sizeDelta = EventDialogueSize;

    if (CurrentEvent.AppearSpace == EventAppearType.Outer)
    {
      UIManager.Instance.UpdateBackground(CurrentEvent.EnvironmentType);
      if (SettlementBackground.enabled == true) SettlementBackground.enabled = false;
    }
    else
    {
      if (SettlementBackground.enabled == false) SettlementBackground.enabled = true;
    }

    if (RewardAskText.text == "")
    {
      RewardText_Yes.text = GameManager.Instance.GetTextData("YES");
      RewardText_No.text = GameManager.Instance.GetTextData("NO");
    }
    RewardAskText.text = string.Format(GameManager.Instance.GetTextData("NOREWARD"),
      GameManager.Instance.MyGameData.CurrentSettlement == null ? GameManager.Instance.GetTextData("Map") : GameManager.Instance.GetTextData("Settlement"));

    EndingButtonGroup.alpha = 0.0f;
    EndingButtonGroup.interactable = false;
    EndingButtonGroup.blocksRaycasts = false;
    EndingRefuseGroup.alpha = 0.0f;
    EndingRefuseGroup.interactable = false;
    EndingRefuseGroup.blocksRaycasts = false;
    NextButtonGroup.alpha = 0.0f;
    NextButtonGroup.interactable = false;
    RewardButtonGroup.alpha = 0.0f;
    RewardButtonGroup.interactable = false;
    SelectionGroup.alpha = 0.0f;
    SelectionGroup.interactable = false;

    NameText.text = CurrentEvent.Name;
    DescriptionText.text = "";
    for (int i = 0; i < CurrentEvent.BeginningDescriptions.Count; i++)
    {
      for(int j = 0; j < CurrentEvent.BeginningDescriptions[i].Split('$').Length; j++)
      {
        if (i > 0||j>0) DescriptionText.text += "<br><br>";
        DescriptionText.text += CurrentEvent.BeginningDescriptions[i].Split('$')[j];
      }
    }

    IsBeginning = false;
    EventPhaseIndex = 0;
    if (issuccess)
    {
      if (isleft)
      {
        CurrentSuccessData = CurrentEvent.SelectionDatas[0].SuccessData;
      }
      else
      {
        CurrentSuccessData = CurrentEvent.SelectionDatas[1].SuccessData;
      }
      EventIllustHolderes = CurrentSuccessData.Illusts;
      EventDescriptions = CurrentSuccessData.Descriptions;
    }
    else
    {
      if (isleft)
      {
        CurrentFailData = CurrentEvent.SelectionDatas[0].FailData;
      }
      else
      {
        CurrentFailData = CurrentEvent.SelectionDatas[1].FailData;
      }
      EventIllustHolderes = CurrentFailData.Illusts;
      EventDescriptions = CurrentFailData.Descriptions;
    }
    PhrIndex = -1;

    DefaultGroup.interactable = false;
    Illust.Next(EventIllustHolderes[0].CurrentIllust);
    StartCoroutine(displaynextindex(true));

    Vector2 _startpos = dir ? LeftPos : RightPos;
    yield return StartCoroutine(UIManager.Instance.moverect(DefaultRect, _startpos, CenterPos, OpenTime, UIManager.Instance.UIPanelOpenCurve));

    DefaultGroup.interactable = true;

    CurrentDialogueType = DialogueTypeEnum.Event;

    yield return null;
  }
  [Space(15)]
  private List<EventIllustHolder> EventIllustHolderes = null;
  private List<string> EventDescriptions = null;
  public int EventPhaseIndex_max { get { return EventDescriptions!=null? EventDescriptions.Count : 0; } }
  public int EventPhaseIndex = 0;
  private List<string> CurrentDescription { get { return EventDescriptions[EventPhaseIndex].Split('$').ToList(); } }
  private int PhrIndex_max { get { return CurrentDescription != null ? CurrentDescription.Count : 0; } }
  private int PhrIndex = -1;
  private bool IsBeginning = false;
  public void NextDescription()
  {
    if (UIManager.Instance.IsWorking) return;

    UIManager.Instance.AddUIQueue(displaynextindex(true));
  }
  public void UpdateSelections()
  {
    switch (CurrentEvent.Selection_type)
    {
      case SelectionTypeEnum.Single:
        Selection_A.Setup(CurrentEvent.SelectionDatas[0]);
        if (Selection_B.gameObject.activeInHierarchy == true) Selection_B.gameObject.SetActive(false);
        break;
      case SelectionTypeEnum.Body:
        Selection_A.Setup(CurrentEvent.SelectionDatas[0]);
        if (Selection_B.gameObject.activeInHierarchy == false) Selection_B.gameObject.SetActive(true);
        Selection_B.Setup(CurrentEvent.SelectionDatas[1]);
        break;
      case SelectionTypeEnum.Head:
        Selection_A.Setup(CurrentEvent.SelectionDatas[0]);
        if (Selection_B.gameObject.activeInHierarchy == false) Selection_B.gameObject.SetActive(true);
        Selection_B.Setup(CurrentEvent.SelectionDatas[1]);
        break;
    }
  }
  private IEnumerator displaynextindex(bool dir)
  {
    SetNextButtonDisable();

    PhrIndex++;
    if (PhrIndex < PhrIndex_max-1)
    {
      if (PhrIndex == 0)
      {
        Illust.Next(EventIllustHolderes[EventPhaseIndex].CurrentIllust, FadeTime);
        yield return new WaitForSeconds(FadeTime);
      }

      DescriptionText.text += (EventPhaseIndex == 0 && PhrIndex == 0 ? "" : "<br><br>") + CurrentDescription[PhrIndex];
      LayoutRebuilder.ForceRebuildLayoutImmediate(DescriptionText.transform.parent.transform as RectTransform);
      yield return StartCoroutine(updatescrollbar());

      if (NextButtonGroup.alpha == 0.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 1.0f, FadeTime));
      SetNextButtonActive();
    }
    else
    {
      int _phasetype = 0;
      if (IsBeginning)
      {
        if (EventPhaseIndex == EventPhaseIndex_max - 1)              //선택지 단계에 도달
        {
          if (EventPhaseIndex == 0)     //UI 처음 열고 바로 선택지일때
          {
            _phasetype = 3;
          }
          else                                 //다음 버튼 눌러서 선택지에 도달할때
          {
            _phasetype = 2;
          }
        }
        else                                                                 //다음 내용으로 진행
        {
          if (EventPhaseIndex == 0)     //UI 처음 열고 설명 페이즈일때
          {
            _phasetype = 0;
          }
          else                                 //다음 버튼 눌러서 다음 내용 전개하기
          {
            _phasetype = 1;
          }
        }

        if (PhrIndex == 0)
        {
          Illust.Next(EventIllustHolderes[EventPhaseIndex].CurrentIllust, FadeTime);
          yield return new WaitForSeconds(FadeTime);
        }
        DescriptionText.text += (EventPhaseIndex == 0 && PhrIndex == 0 ? "" : "<br><br>") + CurrentDescription[PhrIndex];
        LayoutRebuilder.ForceRebuildLayoutImmediate(DescriptionText.transform.parent.transform as RectTransform);

        switch (_phasetype)
        {
          case 0:   //처음 열고 내용
            if (NextButtonGroup.alpha == 0.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 1.0f, FadeTime));

            yield return StartCoroutine(updatescrollbar());

            SetNextButtonActive();
            break;
          case 1:   //다음 버튼 눌러 내용
            if (NextButtonGroup.alpha == 0.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 1.0f, FadeTime));

            yield return StartCoroutine(updatescrollbar());

            SetNextButtonActive();
            break;
          case 2:   //다음 버튼 눌러 선택
            if (NextButtonGroup.alpha == 1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 0.0f, 0.5f));

            StartCoroutine(UIManager.Instance.ChangeAlpha(SelectionGroup, 1.0f, FadeTime));
            UIManager.Instance.UpdateExpButton(true);
            UIManager.Instance.SetExpUse(CurrentEvent.SelectionDatas.ToList());
            yield return StartCoroutine(updatescrollbar());

            break;
          case 3:   //처음 열고 선택
            if (NextButtonGroup.alpha == 1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 0.0f, 0.5f));

            StartCoroutine(UIManager.Instance.ChangeAlpha(SelectionGroup, 1.0f, FadeTime));
            UIManager.Instance.UpdateExpButton(true);
            UIManager.Instance.SetExpUse(CurrentEvent.SelectionDatas.ToList());
            yield return StartCoroutine(updatescrollbar());

            break;
        }

      }
      else
      {
        if (EventPhaseIndex == EventPhaseIndex_max - 1)             //보상 단계에 도달
        {
          if (EventPhaseIndex == 0)     //선택지 선택 후 바로 보상일때         (선택지 애니메이션은 완료)
          {
            _phasetype = 3;
          }
          else                                 //다음 버튼 눌러서 보상에 도달할때
          {
            _phasetype = 2;
          }
        }
        else                                                                 //다음 내용으로 진행
        {
          if (EventPhaseIndex == 0)     //선택지 선택 후 새로 설명으로 넘어갈때 
          {
            _phasetype = 0;
          }
          else                                 //다음 버튼 눌러서 다음 내용 전개하기
          {
            _phasetype = 1;
          }
        }

        if (SelectionGroup.alpha == 1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(SelectionGroup, 0.0f, 0.5f));

        if (PhrIndex == 0)
        {
          Illust.Next(EventIllustHolderes[EventPhaseIndex].CurrentIllust, FadeTime);
          yield return new WaitForSeconds(FadeTime);
        }

        if (EventDescriptions[EventPhaseIndex].Contains("#Penalty#"))
        {
          DescriptionText.text += "<br><br>" + EventDescriptions[EventPhaseIndex].Replace("#Penalty#", "");
          SetPenalty();
          if (CurrentFailData != null)
          {
            switch (CurrentFailData.Penelty_target)
            {
              case PenaltyTarget.None:
                break;
              case PenaltyTarget.EXP:
                break;
              case PenaltyTarget.Status:
                switch (CurrentFailData.StatusType)
                {
                  case StatusTypeEnum.HP:
                    DescriptionText.text += WNCText.SetSize(30, $"<br>{GameManager.Instance.GetTextData(StatusTypeEnum.HP, 2)} {-1 * PenaltyValue}");
                    break;
                  case StatusTypeEnum.Sanity:
                    DescriptionText.text += WNCText.SetSize(30, $"<br>{GameManager.Instance.GetTextData(StatusTypeEnum.Sanity, 2)} {-1 * PenaltyValue}");
                    break;
                  case StatusTypeEnum.Gold:
                    DescriptionText.text += WNCText.SetSize(30, $"<br>{GameManager.Instance.GetTextData(StatusTypeEnum.Gold, 2)} {-1 * PenaltyValue}");
                    break;
                }
                break;
            }
          }
        }
        else
        {
          DescriptionText.text += "<br><br>" + EventDescriptions[EventPhaseIndex];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(DescriptionText.transform.parent.transform as RectTransform);


        switch (_phasetype)
        {
          case 0:   //선택 누르고 내용
            if (NextButtonGroup.alpha == 0.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 1.0f, FadeTime));
            UIManager.Instance.SetExpUnuse();

            yield return StartCoroutine(updatescrollbar());
            SetNextButtonActive();
            break;
          case 1:   //다음 눌러서 내용
            if (NextButtonGroup.alpha == 0.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 1.0f, FadeTime));

            yield return StartCoroutine(updatescrollbar());
            SetNextButtonActive();
            break;
          case 2:   //다음 버튼 눌러서 보상
            if (NextButtonGroup.alpha == 1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 0.0f, 0.5f));

            if (CurrentSuccessData != null && CurrentEvent.EndingID != "")
            {
              CurrentEndingData = GameManager.Instance.ImageHolder.GetEndingData(CurrentEvent.EndingID);
              EndingButtonGroup.alpha = 0.0f;
              EndingButtonText.text = string.Format(GameManager.Instance.GetTextData("Ending"), CurrentEndingData.Name);
              EndingRefuseText.text = CurrentEndingData.Refuse_Name;
              LayoutRebuilder.ForceRebuildLayoutImmediate(EndingButtonGroup.transform as RectTransform);
              LayoutRebuilder.ForceRebuildLayoutImmediate(EndingRefuseGroup.transform as RectTransform);

              StartCoroutine(UIManager.Instance.ChangeAlpha(EndingButtonGroup, 1.0f, FadeTime));
              StartCoroutine(UIManager.Instance.ChangeAlpha(EndingRefuseGroup, 1.0f, FadeTime));
              yield return StartCoroutine(updatescrollbar());
            }
            else
            {
              yield return StartCoroutine(updatescrollbar());

              if (CurrentSuccessData != null) SetRewardButton();
              OpenReturnButton();
            }
            break;
          case 3:   //선택 눌러서 보상
            if (NextButtonGroup.alpha == 1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(NextButtonGroup, 0.0f, 0.5f));
            UIManager.Instance.SetExpUnuse();

            if (CurrentSuccessData != null && CurrentEvent.EndingID != "")
            {
              CurrentEndingData = GameManager.Instance.ImageHolder.GetEndingData(CurrentEvent.EndingID);
              EndingButtonGroup.alpha = 0.0f;
              EndingButtonText.text = string.Format(GameManager.Instance.GetTextData("Ending"), CurrentEndingData.Name);
              EndingRefuseText.text = CurrentEndingData.Refuse_Name;
              LayoutRebuilder.ForceRebuildLayoutImmediate(EndingButtonGroup.transform as RectTransform);
              LayoutRebuilder.ForceRebuildLayoutImmediate(EndingRefuseGroup.transform as RectTransform);

              StartCoroutine(UIManager.Instance.ChangeAlpha(EndingButtonGroup, 1.0f, FadeTime));
              StartCoroutine(UIManager.Instance.ChangeAlpha(EndingRefuseGroup, 1.0f, FadeTime));
              yield return StartCoroutine(updatescrollbar());
            }
            else
            {
              yield return StartCoroutine(updatescrollbar());

              if (CurrentSuccessData != null) SetRewardButton();
              OpenReturnButton();
            }
            break;
        }
      }

      EventPhaseIndex++;
      PhrIndex=-1;
    }
  }
  public void RefuseEnding()
  {
    if (UIManager.Instance.IsWorking) return;
    UIManager.Instance.AddUIQueue(refuseending());
  }
  private IEnumerator refuseending()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(EndingButtonGroup, 0.0f, 0.5f));
    StartCoroutine(UIManager.Instance.ChangeAlpha(EndingRefuseGroup, 0.0f, 0.5f));

    Illust.Next(CurrentEndingData.RefuseIllust,FadeTime);
    DescriptionText.text += "<br><br>" + CurrentEndingData.Refuse_Description;
    LayoutRebuilder.ForceRebuildLayoutImmediate(DescriptionText.transform.parent.transform as RectTransform);
    yield return StartCoroutine(updatescrollbar());

    SetRewardButton();
    OpenReturnButton();
  }
  private UI_Selection CurrentUISelection = null;
  /// <summary>
  /// 선택지 클릭했을때 선택지 스크립트에서 호출
  /// </summary>
  /// <param name="_selection"></param>
  public void SelectSelection(UI_Selection _selection)
  {
    if (_selection.MyTendencyType != TendencyTypeEnum.None)
    {
      GetOppositeSelection(_selection).DeActive();
    }
    //다른거 사라지게 만들고
    UIManager.Instance.UseExp(_selection.IsLeft);
    UIManager.Instance.UpdateExpButton(false);
    UIManager.Instance.AddUIQueue(selectionanimation(_selection));
    //성공, 실패 검사 실행 
  }
  private IEnumerator selectionanimation(UI_Selection _selection)
  {
    CurrentUISelection = _selection;

    SelectionData _selectiondata = _selection.MySelectionData;
    int _currentvalue = GetRequireValue(_selection.IsLeft);
    int  _requirevalue = 0;    //기술 체크에만 사용
    int _currentpercent = UnityEngine.Random.Range(1,101);
    int _requirepercent = 0;                   //성공 확률(골드 혹은 기술 체크) 
    bool _issuccess = false;  
                                               
    switch (_selectiondata.ThisSelectionType)
    {
      case SelectionTargetType.None:
        _issuccess = true;
        UIManager.Instance.AudioManager.PlaySFX(25);
        break;
      case SelectionTargetType.Pay:
        UIManager.Instance.AudioManager.PlaySFX(2);
        if (_selectiondata.SelectionPayTarget.Equals(StatusTypeEnum.HP))
        {
          yield return StartCoroutine(payanimation(_selection.PayIcon, _currentvalue, 0, _selection.PayInfo));

          _issuccess = true;
          UIManager.Instance.AudioManager.PlaySFX(25);
          GameManager.Instance.MyGameData.HP -= _currentvalue;
        }
        else if (_selectiondata.SelectionPayTarget.Equals(StatusTypeEnum.Sanity))
        {
          yield return StartCoroutine(payanimation(_selection.PayIcon, _currentvalue, 0, _selection.PayInfo));

          _issuccess = true;//체력,정신력 지불의 경우 남은 값과 상관 없이 일단 성공으로만 친다
          UIManager.Instance.AudioManager.PlaySFX(25);
          GameManager.Instance.MyGameData.Sanity -= _currentvalue;
        }
        else        //돈 지불일 경우 돈 적을 때 실행하는 뭔가 있어야 함
        {
          int _goldsuccesspercent = GameManager.Instance.MyGameData.Gold >= _currentvalue ? 100 : GameManager.Instance.MyGameData.RequireValue_Money(_currentvalue);

          if (GameManager.Instance.MyGameData.Gold >= _currentvalue)
          {
            yield return StartCoroutine(payanimation(_selection.PayIcon, _currentvalue, 0, _selection.PayInfo));

            GameManager.Instance.MyGameData.Gold -= _currentvalue;
            UIManager.Instance.AudioManager.PlaySFX(25);
            _issuccess = true;
          }
          else
          {
            if (_currentpercent >= _goldsuccesspercent)
            {
              int _elsevalue = _currentvalue - GameManager.Instance.MyGameData.Gold;

              yield return StartCoroutine(payanimation(_selection.PayIcon, _currentvalue, 0, _selection.PayInfo));

              _issuccess = true;
              UIManager.Instance.AudioManager.PlaySFX(25);
              GameManager.Instance.MyGameData.Gold = 0;
              GameManager.Instance.MyGameData.Sanity -= (int)(_elsevalue * ConstValues.GoldSanityPayAmplifiedValue);
       //       Debug.Log("정당한 값을 지불한 레후~");
            }//돈이 부족해 성공한 경우
            else
            {
              yield return StartCoroutine(payanimation(_selection.PayIcon, _currentvalue, _currentvalue - GameManager.Instance.MyGameData.Gold, _selection.PayInfo));

              _issuccess = false;
              UIManager.Instance.AudioManager.PlaySFX(26);
            }//돈이 부족해 실패한 경우
          }
        }

        break;
      case SelectionTargetType.Check_Single: //기술(단수) 선택지면 확률 검사
        UIManager.Instance.AudioManager.PlaySFX(2);

        _requirevalue = GameManager.Instance.MyGameData.CheckSkillSingleValue;
        _requirepercent = GameManager.Instance.MyGameData.RequireValue_SkillCheck(_currentvalue, _requirevalue);
        if (_currentpercent >= _requirepercent)
        {
          _issuccess = true;
        }
        else
        {
          _issuccess = false;
        }

        yield return StartCoroutine(checkanimation(_selection.SkillIcon_A,Mathf.Clamp(_currentpercent / (float)_requirepercent, 0.0f, 1.0f)));
        yield return new WaitForSeconds(0.5f);
        break;
      case SelectionTargetType.Check_Multy: //기술(복수) 선택지면 확률 검사
        UIManager.Instance.AudioManager.PlaySFX(2);
        
        _requirevalue = GameManager.Instance.MyGameData.CheckSkillMultyValue;
        _requirepercent = GameManager.Instance.MyGameData.RequireValue_SkillCheck(_currentvalue, _requirevalue);
        if (_currentpercent >= _requirepercent)
        {
          _issuccess = true;
        }
        else
        {
          _issuccess = false;
        }
        yield return StartCoroutine(checkanimation(_selection.SkillIcon_A, _selection.SkillIcon_B,Mathf.Clamp(_currentpercent / (float)_requirepercent, 0.0f, 1.0f)));
        yield return new WaitForSeconds(0.5f);
        break;
    }

    if (_issuccess) //성공하면 성공
    {
      Debug.Log("성공함");
      StartCoroutine(SetSuccess(CurrentEvent.SelectionDatas[_selection.Index].SuccessData,_selection.MyTendencyType,_selection.IsLeft));
    }
    else            //실패하면 실패
    {
      Debug.Log("실패함");
      StartCoroutine(SetFail(CurrentEvent.SelectionDatas[_selection.Index].FailData, _selection.MyTendencyType, _selection.IsLeft));
    }
    _selection.DeActive();

    yield return null;
  }//선택한 선택지 성공 여부를 계산하고 애니메이션을 실행시키는 코루틴
  //이 코루틴에서 SetSuccess 아니면 SetFail로 바로 넘어감
  [SerializeField] private float SelectionEffectTime_check = 3.5f;
  [SerializeField] private float SelectionEffectTime_pay = 1.0f;
  [SerializeField] private AnimationCurve SelectionCheckCurve = null;
  private IEnumerator payanimation(Image image, int payvalue, int targetvalue, TextMeshProUGUI tmp)
  {
    float _time = 0.0f;
    string _str = "";
    float _stoptime = SelectionEffectTime_pay * (1.0f - targetvalue / (float)payvalue);
    UIManager.Instance.AudioManager.PlaySFX(31, "payanimation");
    while (_time< _stoptime)
    {
      image.fillAmount = 1.0f - _time / SelectionEffectTime_pay;
      _str = ((int)Mathf.Lerp(payvalue, targetvalue, _time / SelectionEffectTime_pay)).ToString();
      tmp.text = _str;
      _time += Time.deltaTime;
      yield return null;
    }
    _str = targetvalue.ToString();
    tmp.text= _str;
    UIManager.Instance.AudioManager.StopSFX("payanimation");
    yield return new WaitForSeconds(0.25f);
  }
  private IEnumerator checkanimation(Image image,float successvalue)
  {
  //  Debug.Log(successvalue);
    float _time = 0.0f;
    UIManager.Instance.AudioManager.PlaySFX(32, "checkanimation");
    while (_time< SelectionEffectTime_check * successvalue)
    {
      image.fillAmount=Mathf.Lerp(1.0f,0.0f, SelectionCheckCurve.Evaluate(_time/ SelectionEffectTime_check));
      _time += Time.deltaTime;yield return null;
    }
    UIManager.Instance.AudioManager.StopSFX("checkanimation");
    if (successvalue == 1.0f) { image.fillAmount = 0.0f; UIManager.Instance.AudioManager.PlaySFX(25); }
    else { UIManager.Instance.AudioManager.PlaySFX(26); }

    yield return new WaitForSeconds(0.25f);
  }
  private IEnumerator checkanimation(Image image_L,Image image_R,float successvalue)
  {
    float _time = 0.0f;
    float _firstvalue = Mathf.Clamp(successvalue * 2.0f, 0.0f, 1.0f), _secondvalue = (successvalue - 0.5f) * 2.0f;

    UIManager.Instance.AudioManager.PlaySFX(32, "checkanimation");
    while (_time < (SelectionEffectTime_check/2)* _firstvalue)
    {
      image_L.fillAmount = Mathf.Lerp(1.0f, 0.0f, SelectionCheckCurve.Evaluate(_time / (SelectionEffectTime_check / 2)));
      _time += Time.deltaTime; yield return null;
    }
    UIManager.Instance.AudioManager.StopSFX("checkanimation");
    if (_firstvalue == 1.0f) { image_L.fillAmount = 0.0f; UIManager.Instance.AudioManager.PlaySFX(25); _time = 0.0f; }
    else { UIManager.Instance.AudioManager.PlaySFX(26); yield return new WaitForSeconds(0.25f); yield break; }


    UIManager.Instance.AudioManager.PlaySFX(32, "checkanimation");
    while (_time < (SelectionEffectTime_check / 2) *_secondvalue)
    {
      image_R.fillAmount = Mathf.Lerp(1.0f, 0.0f, SelectionCheckCurve.Evaluate(_time / (SelectionEffectTime_check / 2)));
      _time += Time.deltaTime;yield return null;
    }
    UIManager.Instance.AudioManager.StopSFX("checkanimation");
    if (_secondvalue == 1.0f) { image_R.fillAmount = 0.0f; UIManager.Instance.AudioManager.PlaySFX(25);  }
    else { UIManager.Instance.AudioManager.PlaySFX(26); }

    yield return new WaitForSeconds(0.25f);
  }
  private SuccessData CurrentSuccessData = null;
  public bool RemainReward = false;
  [SerializeField] private float ResultEffectTime = 2.0f;
  public IEnumerator SetSuccess(SuccessData _success,TendencyTypeEnum tendencytype,bool isleft)
  {
    CurrentSuccessData = _success;

    IsBeginning = false;
      EventPhaseIndex = 0;
      EventIllustHolderes = CurrentSuccessData.Illusts;
      EventDescriptions = CurrentSuccessData.Descriptions;

    IllustEffect_Image.color = SuccessColor;
    StartCoroutine (UIManager.Instance.ChangeAlpha(IllustEffect_Group, 0.0f, ResultEffectTime));

      UIManager.Instance.AddUIQueue(displaynextindex(true));

    GameManager.Instance.SuccessCurrentEvent(tendencytype, isleft);
    GameManager.Instance.SaveData();

    yield return null;
    //연계 이벤트고, 엔딩 설정이 돼 있는 상태에서 성공할 경우 엔딩 다이어로그 전개
  }//성공할 경우 보상 탭을 세팅하고 텍스트를 성공 설명으로 교체, 퀘스트 이벤트일 경우 진행도 ++
  public EndingDatas CurrentEndingData = null;
  private void SetRewardButton()
  {
    if (CurrentSuccessData.Reward_Type == RewardTypeEnum.None) return;

    RewardButtonGroup.alpha = 0.0f;

  //  Reward_Highlight.RemoveAllCall();
   // Reward_Highlight.Interactive = true;
    RemainReward = CurrentSuccessData.Reward_Type == RewardTypeEnum.None ? false : true;
    Sprite _icon = null;
    string _description = "";
    switch (CurrentSuccessData.Reward_Type)
    {
      case RewardTypeEnum.Status:
        switch (CurrentSuccessData.Reward_StatusType)
        {
          case StatusTypeEnum.HP:
            _icon = GameManager.Instance.ImageHolder.HPIcon;
            _description = $"+{WNCText.GetHPColor(GameManager.Instance.MyGameData.RewardHPValue)}";
        //    Reward_Highlight.SetInfo(HighlightEffectEnum.HP, GameManager.Instance.MyGameData.RewardHPValue);
            break;
          case StatusTypeEnum.Sanity:
            _icon = GameManager.Instance.ImageHolder.SanityIcon;
            _description = $"+{WNCText.GetSanityColor(GameManager.Instance.MyGameData.RewardSanityValue)}";
      //      Reward_Highlight.SetInfo(HighlightEffectEnum.Sanity, GameManager.Instance.MyGameData.RewardSanityValue);
            break;
          case StatusTypeEnum.Gold:
            _icon = GameManager.Instance.ImageHolder.GoldIcon;
            _description = $"+{WNCText.GetGoldColor(GameManager.Instance.MyGameData.RewardGoldValue)}";
        //    Reward_Highlight.SetInfo(HighlightEffectEnum.Gold, GameManager.Instance.MyGameData.RewardGoldValue);
            break;
        }
        break;
      case RewardTypeEnum.Experience:
        _icon = GameManager.Instance.ImageHolder.UnknownExpRewardIcon;
        //   _name = GameManager.Instance.GetTextData("EXP_NAME");
        _description = GameManager.Instance.ExpDic[CurrentSuccessData.Reward_EXPID].Name;
      //  Reward_Highlight.SetInfo(HighlightEffectEnum.Exp);
        break;
      case RewardTypeEnum.Skill:
        _icon = GameManager.Instance.ImageHolder.GetSkillIcon(CurrentSuccessData.Reward_SkillType, false);
        _description = $"{GameManager.Instance.GetTextData(CurrentSuccessData.Reward_SkillType, 0)} +1";
     //   Reward_Highlight.SetInfo(new List<SkillTypeEnum> { CurrentSuccessData.Reward_SkillType });
        break;
    }
    RewardIcon.sprite = _icon;
    RewardDescription.text = _description;

    StartCoroutine(UIManager.Instance.ChangeAlpha(RewardButtonGroup, 1.0f, 0.3f));
  }
  private FailData CurrentFailData = null;
  public IEnumerator SetFail(FailData _fail, TendencyTypeEnum tendencytype, bool isleft)
  {
    CurrentFailData = _fail;

    if (_fail.Penelty_target == PenaltyTarget.Status)
    {
      if (_fail.StatusType == StatusTypeEnum.HP && GameManager.Instance.MyGameData.HP <= GameManager.Instance.MyGameData.FailHPValue)
      {
        GameManager.Instance.GameOver();
        yield break;
      }
      else if(_fail.StatusType==StatusTypeEnum.Sanity&&
        GameManager.Instance.MyGameData.Sanity<=GameManager.Instance.MyGameData.FailSanityValue&&
        !GameManager.Instance.MyGameData.MadnessSafe)
      {
        GameManager.Instance.GameOver();
        yield break;
      }
    }

    IsBeginning = false;
    EventPhaseIndex = 0;
    EventIllustHolderes = CurrentFailData.Illusts;
    EventDescriptions = CurrentFailData.Descriptions;

    StartCoroutine(shakeillust());

    IllustEffect_Image.color = FailColor;
    StartCoroutine((UIManager.Instance.ChangeAlpha(IllustEffect_Group, 0.0f, ResultEffectTime)));

    UIManager.Instance.AddUIQueue(displaynextindex(true));

    GameManager.Instance.FailCurrentEvent(tendencytype, isleft);

    GameManager.Instance.SaveData();
    yield return null;
  }//실패할 경우 패널티를 부과하고 텍스트를 실패 설명으로 교체
  [SerializeField] private float IllustShakeDegree = 10;
  [SerializeField] private float IllustShakeTime = 0.7f;
  [SerializeField] private float IllustShakePeriod = 0.1f;
  [SerializeField] private float IllustRotateDegree = 2.5f;
  private IEnumerator shakeillust()
  {
    Vector2 _originpos = IllustRect.anchoredPosition;
    float _time = 0.0f;
    while (_time < IllustShakeTime)
    {
      IllustRect.anchoredPosition = _originpos + new Vector2(Random.Range(-IllustShakeDegree, IllustShakeDegree), Random.Range(-IllustShakeDegree, IllustShakeDegree));
      IllustRect.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.Range(-IllustRotateDegree, IllustRotateDegree)));

      yield return new WaitForSeconds(IllustShakePeriod);
      _time += IllustShakePeriod;
      yield return null;
    }
    IllustRect.anchoredPosition = _originpos;
    IllustRect.rotation = Quaternion.Euler(Vector3.zero);
  }
  public void OpenReturnButton()
  {
    if (GameManager.Instance.MyGameData.CurrentSettlement != null)
    {
      UIManager.Instance.SettleButton.SetCurrentUI(this, MapbuttonPos_Event,0.0f);
    }//정착지에서 이벤트를 끝낸 경우 정착지로 돌아가는 버튼 활성화
    else
    {
      UIManager.Instance.MapButton.SetCurrentUI(this, MapbuttonPos_Event,0.0f);
    }//야외에서 이벤트를 끝낸 경우 야외로 돌아가는 버튼 활성화
  }
  /// <summary>
  /// true:왼쪽으로 false:오른쪽으로
  /// </summary>
  /// <param name="dir"></param>
  public void CloseUI(bool dir)
  {
    UIManager.Instance.AddUIQueue(closeui_all(dir));
  }
  private IEnumerator closeui_all(bool dir)
  {
    CurrentDialogueType = DialogueTypeEnum.None;

    DefaultGroup.interactable = false;
    IsOpen = false;

    CurrentSuccessData = null;
    CurrentFailData = null;
    
    UIManager.Instance.SidePanelCultUI.SetSabbatEffect(false);


    if (RewardButtonGroup.alpha==1.0f) StartCoroutine(UIManager.Instance.ChangeAlpha(RewardButtonGroup, 0.0f, 0.3f));

    if (dir == true)
    {
      yield return StartCoroutine(UIManager.Instance.moverect(DefaultRect, DefaultRect.anchoredPosition, LeftPos, CloseTime, UIManager.Instance.UIPanelOpenCurve));
    }
    else
    {
      yield return StartCoroutine(UIManager.Instance.moverect(DefaultRect, DefaultRect.anchoredPosition, RightPos, CloseTime, UIManager.Instance.UIPanelOpenCurve));
    }
        Illust.Next(GameManager.Instance.ImageHolder.Transparent);
  }
  public void GetEnding()
  {
    GameManager.Instance.SubEnding(GameManager.Instance.ImageHolder.GetEndingData(GameManager.Instance.MyGameData.CurrentEvent.EndingID));
  }
  public void GetReward()
  {
    if (UIManager.Instance.IsWorking) return;
    if (CurrentSuccessData != null && CurrentSuccessData.Reward_Type == RewardTypeEnum.Experience)
      UIManager.Instance.AddUIQueue(getreward());
    else StartCoroutine(getreward());
  }
  private IEnumerator getreward()
  {
 //   Reward_Highlight.Interactive = false;

    if (CurrentSuccessData != null)
    {
      if (CurrentSuccessData.Reward_Type == RewardTypeEnum.Experience)
      {
        RewardExpUI.OpenUI_RewardExp(GameManager.Instance.ExpDic[CurrentSuccessData.Reward_EXPID]);
        yield break;
      }
      else
      {
        RewardButtonGroup.interactable = false;
        switch (CurrentSuccessData.Reward_Type)
        {
          case RewardTypeEnum.Status:
            switch (CurrentSuccessData.Reward_StatusType)
            {
              case StatusTypeEnum.HP:
                GameManager.Instance.MyGameData.HP += GameManager.Instance.MyGameData.RewardHPValue;
                break;
              case StatusTypeEnum.Sanity:
                GameManager.Instance.MyGameData.Sanity += GameManager.Instance.MyGameData.RewardSanityValue;
                break;
              case StatusTypeEnum.Gold:
                GameManager.Instance.MyGameData.Gold += GameManager.Instance.MyGameData.RewardGoldValue;
                break;
            }
            RemainReward = false;
            break;
          case RewardTypeEnum.Skill:
            RemainReward = false;
            GameManager.Instance.MyGameData.GetSkill(CurrentSuccessData.Reward_SkillType).LevelByDefault++;
            UIManager.Instance.AudioManager.PlaySFX(19);

            StartCoroutine(UIManager.Instance.SetIconEffect(CurrentSuccessData.Reward_SkillType));
            break;
        }

        StartCoroutine(UIManager.Instance.ChangeAlpha(RewardButtonGroup, 0.0f, 0.6f));
      }
    }
  }
  private int PenaltyValue = 0;
  public void ExpAcquired()
  {
    RemainReward = false;
    StartCoroutine(UIManager.Instance.ChangeAlpha(RewardButtonGroup, 0.0f, 0.6f));
  }
  public void SetPenalty()
  {
    switch (CurrentFailData.Penelty_target)
    {
      case PenaltyTarget.None:
        return;
      case PenaltyTarget.Status:
        switch (CurrentFailData.StatusType)
        {
          case StatusTypeEnum.HP:
            GameManager.Instance.MyGameData.HP -= GameManager.Instance.MyGameData.FailHPValue;
            PenaltyValue = GameManager.Instance.MyGameData.FailHPValue;
            break;
          case StatusTypeEnum.Sanity:
            GameManager.Instance.MyGameData.Sanity -= GameManager.Instance.MyGameData.FailSanityValue;
            PenaltyValue = GameManager.Instance.MyGameData.FailSanityValue;
            break;
          case StatusTypeEnum.Gold:
            if (GameManager.Instance.MyGameData.Gold >= GameManager.Instance.MyGameData.FailGoldValue)
            {
              GameManager.Instance.MyGameData.Gold -= GameManager.Instance.MyGameData.FailGoldValue;
              PenaltyValue = GameManager.Instance.MyGameData.FailGoldValue;
            }
            else
            {
              PenaltyValue = GameManager.Instance.MyGameData.Gold;
              GameManager.Instance.MyGameData.Gold = 0;
            }
            break;
        }
        break;
    }
  }//실패할 경우 패널티 부과하는 연출
  #endregion
  [Space(20)]
  [Header("정착지")]
  #region 정착지
  public GameObject SettlementObjectHolder = null;
  public Image MadenssEffect = null;
  [SerializeField] private RectTransform MapbuttonPos_Settlemtn = null;
  [SerializeField] private UnityEngine.UI.Image SettlementIcon = null;
  [SerializeField] private TextMeshProUGUI SettlementTypeText = null;
  [SerializeField] private TextMeshProUGUI DiscomfortText = null;
  [SerializeField] private TextMeshProUGUI RestCostValueText = null;
  [SerializeField] private List<PlaceIconScript> SectorIcons = new List<PlaceIconScript>();
  private PlaceIconScript GetSectorIconScript(SectorTypeEnum sectortype)
  {
    for (int i = 0; i < SectorIcons.Count; i++)
    {
      if (SectorIcons[i].MyType == sectortype) return SectorIcons[i];
    }
    return null;
  }
  [SerializeField] private UnityEngine.UI.Image SelectSectorIcon = null;
  [SerializeField] private TextMeshProUGUI SectorName = null;
  [SerializeField] private TextMeshProUGUI SectorEffect = null;
  [SerializeField] private GameObject RestButtonHolder = null;
  [SerializeField] private Onpointer_highlight CostHighlight_Sanity = null;
  [SerializeField] private UnityEngine.UI.Button Cost_Sanity = null;
  [SerializeField] private Onpointer_highlight CostHighlight_Gold = null;
  [SerializeField] private UnityEngine.UI.Button Cost_Gold = null;
  private Settlement CurrentSettlement{ get { return GameManager.Instance.MyGameData.CurrentSettlement; } }
  private SectorTypeEnum SelectedSector = SectorTypeEnum.NULL;
  private bool IsMad = false;
  public GameObject QuitAskObject = null;
  public TextMeshProUGUI QuitAskText = null;
  public TextMeshProUGUI QuitText_Yes = null;
  public TextMeshProUGUI QuitText_No = null;
  public void OpenQuitAsk() => QuitAskObject.SetActive(true);
  public void QuitAskClick_Yes() 
  { 
    GameManager.Instance.MyGameData.FirstRest = false;
    UIManager.Instance.MapButton.Clicked();
  }
  public void QuitAskClick_No()
  {
    QuitAskObject.SetActive(false);
  }
  public IEnumerator openui_settlement(bool dir)
  {
    if (GameManager.Instance.MyGameData.CurrentEvent != null)
    {
      GameManager.Instance.MyGameData.Turn++;
      GameManager.Instance.MyGameData.CurrentEvent = null;
      GameManager.Instance.SaveData();
    }
    if (QuitAskText.text == "")
    {
      QuitAskText.text = GameManager.Instance.GetTextData("NoRestQuit");
      QuitText_Yes.text = GameManager.Instance.GetTextData("YES");
      QuitText_No.text = GameManager.Instance.GetTextData("NO");
    }
    if (PlayerPrefs.GetInt("Tutorial_Settlement") == 0) UIManager.Instance.TutorialUI.OpenTutorial_Settlement();

        DefaultGroup.interactable = false;
        if (CurrentDialogueType == DialogueTypeEnum.Event)
        {
            StartCoroutine(UIManager.Instance.moverect(DialogueRect, Descriptionpos_Outside, Descriptionpos_Inside, CloseTime, UIManager.Instance.UIPanelCLoseCurve));
            StartCoroutine(UIManager.Instance.ChangeAlpha(DialogueAlpha, 0.0f, CloseTime, UIManager.Instance.UIPanelCLoseCurve));
            yield return new WaitForSeconds(CloseTime);
        }
    if (RewardAskObject.activeInHierarchy) RewardAskObject.SetActive(false);
    if (QuitAskObject.activeInHierarchy) QuitAskObject.SetActive(false);
    ExpUsageDic_L.Clear();
    ExpUsageDic_R.Clear();

    if (GameManager.Instance.MyGameData.Madness_Force && (GameManager.Instance.MyGameData.TotalRestCount % ConstValues.MadnessEffect_Force == ConstValues.MadnessEffect_Force-1))
    {
      Debug.Log("무력 광기 발동");
      UIManager.Instance.HighlightManager.Highlight_Madness(SkillTypeEnum.Force);
      UIManager.Instance.AudioManager.PlaySFX(27, "madness");
      if (!MadenssEffect.enabled) MadenssEffect.enabled = true;
      IsMad = true;
    }
    else
    {
      UIManager.Instance.AudioManager.PlaySFX(14, "preview");
      if (MadenssEffect.enabled) MadenssEffect.enabled = false;
      IsMad = false;
    }
    UIManager.Instance.OffBackground();

    if (EventObjectHolder.activeInHierarchy == true) EventObjectHolder.SetActive(false);
    if (SettlementObjectHolder.activeInHierarchy == false) SettlementObjectHolder.SetActive(true);
    if (SettlementBackground.enabled == false) SettlementBackground.enabled = true;
    DialogueRect.sizeDelta = SettlementDialogueSize;

    IsSelectSector = false;
    QuestSectorInfo = false;
    SelectedSector = SectorTypeEnum.NULL;
    Illust.Next(GameManager.Instance.ImageHolder.GetSettlementIllust(CurrentSettlement.SettlementType, GameManager.Instance.MyGameData.Turn));

    DiscomfortText.text = CurrentSettlement.Discomfort.ToString();
    RestCostValueText.text = string.Format(GameManager.Instance.GetTextData("RestCostValue"),
     (int)(GameManager.Instance.MyGameData.GetDiscomfortValue(CurrentSettlement.Discomfort) * 100));
    if (RestButtonHolder.gameObject.activeInHierarchy == true) RestButtonHolder.gameObject.SetActive(false);

    Sprite _settlementicon = null;
    switch (CurrentSettlement.SettlementType)
    {
      case SettlementType.Village: _settlementicon = GameManager.Instance.ImageHolder.VillageIcon_white; break;
      case SettlementType.Town:  _settlementicon = GameManager.Instance.ImageHolder.TownIcon_white; break;
      case SettlementType.City:_settlementicon = GameManager.Instance.ImageHolder.CityIcon_white; break;
    }
    for (int i = 0; i < SectorIcons.Count; i++)
    {
      if (CurrentSettlement.Sectors.Contains(SectorIcons[i].MyType))
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
    SettlementTypeText.text = GameManager.Instance.GetTextData(CurrentSettlement.SettlementType);
    SelectSectorIcon.sprite = GameManager.Instance.ImageHolder.UnknownSectorIcon;
    SectorName.text =IsMad?"": GameManager.Instance.GetTextData("SELECTPLACE");
    SectorEffect.text = "";
        UIManager.Instance.MapButton.SetCurrentUI(this, MapbuttonPos_Settlemtn, 1.0f);

        if (CurrentDialogueType == DialogueTypeEnum.None)
        {
            Vector2 _startpos_panel = dir ? LeftPos : RightPos, _endpos_panel = CenterPos;
            yield return StartCoroutine(UIManager.Instance.moverect(DefaultRect, _startpos_panel, _endpos_panel, OpenTime, UIManager.Instance.UIPanelOpenCurve));
        }
        else
        {
            StartCoroutine(UIManager.Instance.moverect(DialogueRect, Descriptionpos_Inside, Descriptionpos_Outside, OpenTime, UIManager.Instance.UIPanelOpenCurve));
            StartCoroutine(UIManager.Instance.ChangeAlpha(DialogueAlpha, 1.0f, OpenTime, UIManager.Instance.UIPanelOpenCurve));
            yield return new WaitForSeconds(OpenTime);
        }
        DefaultGroup.interactable = true;

    CurrentDialogueType = DialogueTypeEnum.Settlement;
        IsOpen = true;

    }
    [HideInInspector] public bool QuestSectorInfo = false;
  [HideInInspector] public int GoldCost = 0;
  [HideInInspector] public int SanityCost = 0;
  [HideInInspector] public int DiscomfortValue = 0;
  [HideInInspector] public int SupplyValue = 0;
  [HideInInspector] public bool IsSelectSector = false;

  public void OnPointerSector(SectorTypeEnum sector)
  {
    if (IsSelectSector == true) return;

    QuestSectorInfo = GameManager.Instance.MyGameData.Cult_SabbatSector == sector;

    SelectSectorIcon.sprite =IsMad?GameManager.Instance.ImageHolder.MadnessActive: GameManager.Instance.ImageHolder.GetSectorIcon(sector,false);
    SectorName.text = GameManager.Instance.GetTextData(sector, 0);
    string _effect = GameManager.Instance.GetTextData(sector, 3);
    int _discomfort_default = ConstValues.Rest_Discomfort;
    switch (sector)
    {
      case SectorTypeEnum.Residence:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect,
          WNCText.PositiveColor(ConstValues.SectorEffect_residence_discomfort.ToString()));

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue=IsMad? _discomfort_default : (_discomfort_default - ConstValues.SectorEffect_residence_discomfort) > 0 ? (_discomfort_default - ConstValues.SectorEffect_residence_discomfort) : 0;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Temple:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_temple);

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Marketplace:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_marketSector);

        GoldCost =IsMad? GameManager.Instance.MyGameData.RestCost_Gold: Mathf.FloorToInt(GameManager.Instance.MyGameData.RestCost_Gold * (1.0f - ConstValues.SectorEffect_marketSector / 100.0f));
        SanityCost = IsMad ? GameManager.Instance.MyGameData.RestCost_Sanity : Mathf.FloorToInt(GameManager.Instance.MyGameData.RestCost_Sanity * (1.0f - ConstValues.SectorEffect_marketSector / 100.0f));
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Library:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_Library);

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
    }

    string _sabbatdescription = "";
    switch (QuestSectorInfo)
    {
      case false:
        SectorEffect.text = _effect;
        break;
      case true:
        _sabbatdescription = "<br>" + string.Format(GameManager.Instance.GetTextData("Cult_Progress_Sabbat_Effect"),
        ConstValues.Quest_Cult_Progress_Sabbat);
        SectorEffect.text = _effect + _sabbatdescription;
        break;
    }
  }
  public void OutPointerSector()
  {
    if (IsSelectSector == true) return;

    SelectSectorIcon.sprite = GameManager.Instance.ImageHolder.UnknownSectorIcon;
    SectorName.text = GameManager.Instance.GetTextData("SELECTPLACE");
    SectorEffect.text = "";
  }
  public void SelectPlace(int index)  //Sectortype은 0이 NULL임
  {
    if (SelectedSector == (SectorTypeEnum)index) return;

    if (SelectedSector != SectorTypeEnum.NULL) GetSectorIconScript(SelectedSector).SetIdleColor();
    SelectedSector = (SectorTypeEnum)index;
    IsSelectSector = true;

    switch (SelectedSector)
    {
      case SectorTypeEnum.Residence:
        UIManager.Instance.AudioManager.PlaySFX(10,"sector");
        break;
      case SectorTypeEnum.Temple:
        UIManager.Instance.AudioManager.PlaySFX(11,"sector");
        break;
      case SectorTypeEnum.Marketplace:
        UIManager.Instance.AudioManager.PlaySFX(12,"sector");
        break;
      case SectorTypeEnum.Library:
        UIManager.Instance.AudioManager.PlaySFX(13,"sector");
        break;
    }

    Illust.Next(GameManager.Instance.ImageHolder.GetSectorIllust(CurrentSettlement.SettlementType, SelectedSector, GameManager.Instance.MyGameData.Turn));

    QuestSectorInfo = GameManager.Instance.MyGameData.Cult_SabbatSector==SelectedSector;

    SelectSectorIcon.sprite = IsMad ? GameManager.Instance.ImageHolder.MadnessActive: GameManager.Instance.ImageHolder.GetSectorIcon(SelectedSector, false);
    SectorName.text = GameManager.Instance.GetTextData(SelectedSector, 0);
    string _effect = GameManager.Instance.GetTextData(SelectedSector, 3);
    int _discomfort_default = ConstValues.Rest_Discomfort;
    switch (SelectedSector)
    {
      case SectorTypeEnum.Residence:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect,
          WNCText.PositiveColor(ConstValues.SectorEffect_residence_discomfort.ToString()));

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue = IsMad ? _discomfort_default : (_discomfort_default - ConstValues.SectorEffect_residence_discomfort) > 0 ? (_discomfort_default - ConstValues.SectorEffect_residence_discomfort) : 0;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Temple:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_temple);

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Marketplace:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_marketSector);

        GoldCost = IsMad ? GameManager.Instance.MyGameData.RestCost_Gold : Mathf.FloorToInt(GameManager.Instance.MyGameData.RestCost_Gold * (1.0f - ConstValues.SectorEffect_marketSector / 100.0f));
        SanityCost = IsMad ? GameManager.Instance.MyGameData.RestCost_Sanity : Mathf.FloorToInt(GameManager.Instance.MyGameData.RestCost_Sanity * (1.0f - ConstValues.SectorEffect_marketSector / 100.0f));
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
      case SectorTypeEnum.Library:
        _effect = IsMad ? GameManager.Instance.GetTextData("Madness_Force_Description") : string.Format(_effect, ConstValues.SectorEffect_Library);

        GoldCost = GameManager.Instance.MyGameData.RestCost_Gold;
        SanityCost = GameManager.Instance.MyGameData.RestCost_Sanity;
        DiscomfortValue = _discomfort_default;
        SupplyValue = ConstValues.Rest_Supply;
        break;
    }

    string _sabbatdescription = "";
    switch (QuestSectorInfo)
    {
      case false:
        SectorEffect.text = _effect;
        UIManager.Instance.SidePanelCultUI.SetSabbatEffect(false);
        break;
      case true:
        _sabbatdescription = "<br>" + string.Format(GameManager.Instance.GetTextData("Cult_Progress_Sabbat_Effect"),
        ConstValues.Quest_Cult_Progress_Sabbat);
        SectorEffect.text = _effect + _sabbatdescription;
        UIManager.Instance.SidePanelCultUI.SetSabbatEffect(true);
        break;
    }
    if (RestButtonHolder.gameObject.activeInHierarchy == false)
    {
      RestButtonHolder.gameObject.SetActive(true);
    }

    CostHighlight_Sanity.Interactive = true;
    CostHighlight_Sanity.SetInfo(HighlightEffectEnum.Sanity);

    bool _goldable = GameManager.Instance.MyGameData.Gold >= GoldCost;
    Cost_Gold.interactable = _goldable;
    Cost_Gold.GetComponent<CanvasGroup>().alpha=_goldable?1.0f:0.3f;
    CostHighlight_Gold.Interactive = _goldable;
    if (_goldable) CostHighlight_Gold.SetInfo(HighlightEffectEnum.Gold);
  }
  public void OnPointerRestType(StatusTypeEnum type)
  {
    if (UIManager.Instance.IsWorking) return;
  }
  public void OnExitRestType(StatusTypeEnum type)
  {
    return;
  }
  public void StartRest_Sanity()
  {
    if (UIManager.Instance.IsWorking) return;

    CostHighlight_Sanity.Interactive = false;
    CostHighlight_Gold.Interactive = false;

    UIManager.Instance.AddUIQueue(restinsector(StatusTypeEnum.Sanity));
  }
  public void StartRest_Gold()
  {
    if (UIManager.Instance.IsWorking) return;

    CostHighlight_Sanity.Interactive = false;
    CostHighlight_Gold.Interactive = false;

    UIManager.Instance.AddUIQueue(restinsector(StatusTypeEnum.Gold));
  }

  private IEnumerator restinsector(StatusTypeEnum statustype)
  {
    DefaultGroup.interactable = false;
    IsOpen = false;

    if (statustype == StatusTypeEnum.Sanity && GameManager.Instance.MyGameData.Sanity < SanityCost && !GameManager.Instance.MyGameData.MadnessSafe)
    {
      GameManager.Instance.GameOver();
      yield break;
    }


    GameManager.Instance.MyGameData.FirstRest = false;
    if (GameManager.Instance.MyGameData.Madness_Force)
    {
      GameManager.Instance.MyGameData.TotalRestCount++;
      UIManager.Instance.SetForceMadCount();
    }

    int _discomfortvalue = DiscomfortValue;
    switch (GameManager.Instance.MyGameData.QuestType)
    {
      case QuestType.Cult:
        switch (QuestSectorInfo)
        {
          case false:
            break;
          case true:
            UIManager.Instance.CultUI.AddProgress(3,GetSectorIconScript(SelectedSector).transform as RectTransform);
            break;
        }
        break;
    }

    float _restvalue = GameManager.Instance.MyGameData.GetDiscomfortValue(CurrentSettlement.Discomfort) * 100;
    switch (statustype)
    {
      case StatusTypeEnum.Sanity:
        GameManager.Instance.MyGameData.Sanity -= SanityCost;
        CurrentSettlement.Discomfort += _discomfortvalue;
        if (DiscomfortValue > 0)
        {
          StartCoroutine(discomfortanimation(_restvalue));
        }
        break;
      case StatusTypeEnum.Gold:

        GameManager.Instance.MyGameData.Gold -= GoldCost;
        GameManager.Instance.MyGameData.Sanity += ConstValues.RestSanityRestore;
        CurrentSettlement.Discomfort += _discomfortvalue;
        if (DiscomfortValue > 0)
        {
          StartCoroutine(discomfortanimation(_restvalue));
        }
        break;
    }
   if(GameManager.Instance.MyGameData.Supply < 0) GameManager.Instance.MyGameData.Supply = SupplyValue;
   else GameManager.Instance.MyGameData.Supply += SupplyValue;
    GameManager.Instance.MyGameData.ApplySectorEffect(IsMad? SectorTypeEnum.NULL: SelectedSector);

    UIManager.Instance.AudioManager.PlaySFX(2);

    yield return new WaitForSeconds(ConstValues.CountChangeTime_settlement);

    UIManager.Instance.AudioManager.PlaySFX(14);

    EventManager.Instance.SetSettlementEvent(SelectedSector);

  }
  public AnimationCurve DiscomfortAnimationCurve = new AnimationCurve();
  private IEnumerator discomfortanimation(float startrestvalue)
  {
    float _time = 0.0f;

    string _valuetext = GameManager.Instance.GetTextData("RestCostValue");
    float _startdisdcomfort = float.Parse(DiscomfortText.text), _enddiscomfort = CurrentSettlement.Discomfort;
    float _endrestvalue = GameManager.Instance.MyGameData.GetDiscomfortValue(CurrentSettlement.Discomfort) * 100;
    while (_time < ConstValues.CountChangeTime_settlement)
    {
      DiscomfortText.text = Mathf.RoundToInt(Mathf.Lerp(_startdisdcomfort, _enddiscomfort,_time/ ConstValues.CountChangeTime_settlement)).ToString();
      RestCostValueText.text = string.Format(_valuetext, Mathf.RoundToInt(Mathf.Lerp(startrestvalue, _endrestvalue, _time / ConstValues.CountChangeTime_settlement)));
      _time += Time.deltaTime;
      yield return null;
    }
    DiscomfortText.text = _enddiscomfort.ToString();
    RestCostValueText.text = string.Format(_valuetext, _endrestvalue);
  }
  #endregion
}
