using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class UI_Selection : MonoBehaviour
{
  [SerializeField] private CanvasGroup MyGroup = null;
  [SerializeField] private Image MyImage = null;
  //  public GameObject TendencyIconObj = null;
  //  public Image TendencyIcon = null;
  // private int LayoutSizeTop_NoTendency = 15;
  // private int LayoutSizeTop_Tendency = -35;
  [SerializeField] private VerticalLayoutGroup LayoutGroup = null;
  [SerializeField] private Vector2 LeftPos= Vector2.zero;
  [SerializeField] private Vector2 RightPos= Vector2.zero;
  [SerializeField] private UI_dialogue MyUIDialogue = null;
  [SerializeField] private Button MyButton = null;
  [SerializeField] private TextMeshProUGUI MyDescription = null;
  [SerializeField] private GameObject PayHolder = null;
  public Image PayIcon = null;
  public TextMeshProUGUI PayInfo = null;
  [SerializeField] private GameObject CheckHolder = null;
  [SerializeField] private GameObject OverTendencyHolder = null;
  public Image OverTendencyIcon = null;
  [SerializeField] private Image SkillIcon_A = null;
  [SerializeField] private Image SkillIcon_B = null;
  public TextMeshProUGUI SkillInfo_left_10 = null;
  public TextMeshProUGUI SkillInfo_left_1 = null;
  public TextMeshProUGUI SkillInfo_left_center = null;
  public TextMeshProUGUI SkillInfo_right_10 = null;
  public TextMeshProUGUI SkillInfo_right_1 = null;
  public TextMeshProUGUI SkillInfo_right_center = null;
  [SerializeField] private PreviewInteractive MyPreviewInteractive = null;
  [SerializeField] private PreviewInteractive ChatPreviewInteractive = null;
  [SerializeField] private Onpointer_highlight HighlightEffect = null;
  public TendencyTypeEnum MyTendencyType = TendencyTypeEnum.None;
  public bool IsLeft = true;
  [SerializeField] private GameObject TendencyHolder = null;
  [SerializeField] private Image NextTendencyIcon = null;
  [SerializeField] private List<Image> TendencyProgressArrows= new List<Image>();
  [SerializeField] private List<CanvasGroup> TendencyProgressArrows_inside=new List<CanvasGroup>();
  public bool IsOverTendency = false;
  [SerializeField] private RectTransform TopRect = null;
  [SerializeField] private GameObject ChatPanelPrefab = null;
  [SerializeField] private GameObject ChatCount_Obj = null;
  [SerializeField] private RectTransform ChatCount_Pos = null;
  [SerializeField] private TextMeshProUGUI ChatCount_Text = null;
  [SerializeField] private int ChatCount = 0;
  private float ChatPanelWaitTime = 0.1f;
  [SerializeField] private UI_Selection OtherSelection = null;
  public void AddChatCount(string nickname, StreamingTypeEnum type)
  {
    string _chatname = (type == StreamingTypeEnum.Chzz ? "<sprite=122> " : "<sprite=123> ") + nickname;
    ChatNamePanel _newchat=Instantiate(ChatPanelPrefab,UIManager.Instance.MyCanvas).GetComponent<ChatNamePanel>();
    _newchat.Setup(_chatname, TopRect);
    ChatCount++;
    ChatCount_Text.text = ChatCount.ToString();
    ChatQueue.Enqueue(_newchat);
    if (chatqueuefield == null)
    {
      chatqueuefield = ChatQueueCoroutine();
      StartCoroutine(chatqueuefield);
    }
  }
  private Queue<ChatNamePanel> ChatQueue = new Queue<ChatNamePanel>();
  private IEnumerator chatqueuefield = null;
  private IEnumerator ChatQueueCoroutine()
  {
    var _wait = new WaitForSeconds(ChatPanelWaitTime);
    while (ChatQueue.Count > 0)
    {
      StartCoroutine(ChatQueue.Dequeue().movepanel());
      yield return _wait;
    }
    chatqueuefield = null;
  }
  public void StopAllChat()
  {
    if (chatqueuefield != null)
    {
      StopCoroutine(chatqueuefield);
    }
    while (ChatQueue.Count > 0)
    {
      ChatQueue.Dequeue().DestroyPanel();
    }
  }
  public int Index
  {
    get { return IsLeft ? 0 : 1; }
  }
  //현재 이 선택지가 가지는 설명문
  public SelectionData MySelectionData = null;
  public void AddExp(Experience exp)
  {
    MyUIDialogue.AddExp(IsLeft, exp);
  }
  public void DeActive() => StartCoroutine(UIManager.Instance.ChangeAlpha(MyGroup,0.0f,0.6f));
  public void Setup(SelectionData _data)
  {
    HighlightEffect.RemoveAllCall();
    if (GameManager.Instance.IsChzzConnect||GameManager.Instance.IsTwitchConnect)
    {
      ChatQueue.Clear();
      ChatCount = 0;
      ChatCount_Text.text = "0";
      if (!ChatCount_Obj.activeSelf)
      {
        ChatCount_Obj.SetActive(true);
      }
      RectTransform _chatrect = ChatCount_Obj.GetComponent<RectTransform>();
      _chatrect.position = ChatCount_Pos.position;
      _chatrect.anchoredPosition3D = new Vector3(_chatrect.anchoredPosition3D.x, _chatrect.anchoredPosition3D.y, 0.0f);
    }
    else
    {
      if (ChatCount_Obj.activeSelf) ChatCount_Obj.SetActive(false);
    }
    SkillIcon_A.fillAmount = 1.0f;
    SkillIcon_B.fillAmount = 1.0f;
    MySelectionData = _data;
    if (MyGroup.alpha == 0.0f)
    {
      MyGroup.alpha = 1.0f;
    }
    MyGroup.interactable = true;
    MyGroup.blocksRaycasts = true;

    if (_data.Tendencytype == TendencyTypeEnum.Body)
    {
      if (_data.Index == 0 && GameManager.Instance.MyGameData.Tendency_Body.Level == -2) IsOverTendency = true;
      else if (_data.Index == 1 && GameManager.Instance.MyGameData.Tendency_Body.Level == 2) IsOverTendency = true;
      else IsOverTendency = false;
    }
    else if (_data.Tendencytype == TendencyTypeEnum.Head)
    {
      if (_data.Index == 0 && GameManager.Instance.MyGameData.Tendency_Head.Level == -2) IsOverTendency = true;
      else if (_data.Index == 1 && GameManager.Instance.MyGameData.Tendency_Head.Level == 2) IsOverTendency = true;
      else IsOverTendency = false;
    }
    else IsOverTendency = false;

    if (IsOverTendency)
    {
      if (PayHolder.activeInHierarchy == true) PayHolder.SetActive(false);
      if (CheckHolder.activeInHierarchy == true) CheckHolder.SetActive(false);
      if (OverTendencyHolder.activeInHierarchy == false) OverTendencyHolder.SetActive(true);
      OverTendencyIcon.fillAmount = 1.0f;
    }
    else 
    switch (MySelectionData.ThisSelectionType)
    {
      case SelectionTargetType.None:
        if (PayHolder.activeInHierarchy == true) PayHolder.SetActive(false);
        if (CheckHolder.activeInHierarchy == true) CheckHolder.SetActive(false);
        if(OverTendencyHolder.activeInHierarchy == true) OverTendencyHolder.SetActive(false);
        break;
      case SelectionTargetType.Pay:
        if (PayHolder.activeInHierarchy == false) PayHolder.SetActive(true);
        if (CheckHolder.activeInHierarchy == true) CheckHolder.SetActive(false);
        if (OverTendencyHolder.activeInHierarchy == true) OverTendencyHolder.SetActive(false);
        PayIcon.fillAmount = 1.0f;
        switch (MySelectionData.SelectionPayTarget)
        {
          case StatusTypeEnum.HP:
            PayIcon.sprite = GameManager.Instance.ImageHolder.HPDecreaseIcon;
            HighlightEffect.SetInfo(HighlightEffectEnum.HP);
            break;
          case StatusTypeEnum.Sanity:
            PayIcon.sprite = GameManager.Instance.ImageHolder.SanityDecreaseIcon;
            HighlightEffect.SetInfo(HighlightEffectEnum.Sanity);
            break;
          case StatusTypeEnum.Gold:
            PayIcon.sprite = GameManager.Instance.ImageHolder.GoldDecreaseIcon;
            HighlightEffect.SetInfo(HighlightEffectEnum.Gold);
            break;
        }
        break;
      case SelectionTargetType.Check_Single:
        if (PayHolder.activeInHierarchy == true) PayHolder.SetActive(false);
        if (CheckHolder.activeInHierarchy == false) CheckHolder.SetActive(true);
        if (OverTendencyHolder.activeInHierarchy == true) OverTendencyHolder.SetActive(false);
        if (SkillIcon_B.gameObject.activeInHierarchy == true) SkillIcon_B.gameObject.SetActive(false);
        SkillIcon_A.fillAmount = 1.0f;
        SkillIcon_A.sprite = GameManager.Instance.ImageHolder.GetSkillIcon(MySelectionData.SelectionCheckSkill[0], false);
        HighlightEffect.SetInfo(new List<SkillTypeEnum> { MySelectionData.SelectionCheckSkill[0] });

        break;
      case SelectionTargetType.Check_Multy:
        if (PayHolder.activeInHierarchy == true) PayHolder.SetActive(false);
        if (CheckHolder.activeInHierarchy == false) CheckHolder.SetActive(true);
        if (OverTendencyHolder.activeInHierarchy == true) OverTendencyHolder.SetActive(false);
        if (SkillIcon_B.gameObject.activeInHierarchy.Equals(false)) SkillIcon_B.gameObject.SetActive(true);
        SkillIcon_A.fillAmount = 1.0f;
        SkillIcon_B.fillAmount = 1.0f;
        Sprite[] _sprs = new Sprite[2];
        _sprs[0] = GameManager.Instance.ImageHolder.GetSkillIcon(MySelectionData.SelectionCheckSkill[0], false);
        _sprs[1] = GameManager.Instance.ImageHolder.GetSkillIcon(MySelectionData.SelectionCheckSkill[1], false);
        SkillIcon_A.sprite = _sprs[0];
        SkillIcon_B.sprite = _sprs[1];
        HighlightEffect.SetInfo(new List<SkillTypeEnum> { MySelectionData.SelectionCheckSkill[0], MySelectionData.SelectionCheckSkill[1] });

        break;
    }//아이콘 채우기 등 이미지만 작업
    LayoutRebuilder.ForceRebuildLayoutImmediate(MyGroup.transform as RectTransform);

    UpdateValues();
    HighlightEffect.Interactive = !IsOverTendency;

    MyTendencyType = MySelectionData.Tendencytype;
    MyPreviewInteractive.MySelectionTendency = MyTendencyType;
    ChatPreviewInteractive.MySelectionTendency = MyTendencyType;
    if (MyTendencyType == TendencyTypeEnum.None)
    {
      if (TendencyHolder.activeInHierarchy == true) TendencyHolder.SetActive(false);
    }
    else
    {
      MyPreviewInteractive.MySelectionTendencyDir = IsLeft;
      ChatPreviewInteractive.MySelectionTendencyDir = IsLeft;
      Tendency _targettendency = GameManager.Instance.MyGameData.GetTendency(MyTendencyType);
      Sprite _nexttendencyicon = null;
      Sprite _tendencyicon = null;

      if (IsLeft)
      {
        HighlightEffect.SetInfo(_targettendency.Type == TendencyTypeEnum.Body ? HighlightEffectEnum.Rational : HighlightEffectEnum.Mental);
        if (_targettendency.Level < 0)
        {
          _tendencyicon = _targettendency.CurrentIcon;
        }
        else
        {
          _tendencyicon = GameManager.Instance.ImageHolder.GetTendencyIcon(MyTendencyType, -1);
        }

        if(_targettendency.Progress>=0)
        {
          if (TendencyHolder.activeInHierarchy == true) TendencyHolder.SetActive(false);
        }
        else
        {
          if (_targettendency.Level == -2)
          {
            if (TendencyHolder.activeInHierarchy == true) TendencyHolder.SetActive(false);
          }
          else
          {
            if (TendencyHolder.activeInHierarchy == false) TendencyHolder.SetActive(true);

            _nexttendencyicon = _targettendency.GetNextIcon(true);
            NextTendencyIcon.sprite = _nexttendencyicon;

            int _progress = -1 * _targettendency.Progress;
            int _maxprogress = _targettendency.CurrentProgressTargetCount;
            Sprite _arrow = GameManager.Instance.ImageHolder.Arrow_Active(_targettendency.Type, true);
          for(int i = 0; i < 3; i++)
            {
              if (i >= _maxprogress)
              {
                TendencyProgressArrows[i].gameObject.SetActive(false);
                continue;
              }

              if (TendencyProgressArrows[i].gameObject.activeInHierarchy.Equals(false)) TendencyProgressArrows[i].gameObject.SetActive(true);

              if (i < _progress)
              {
                TendencyProgressArrows[i].sprite = _arrow;
                TendencyProgressArrows_inside[i].alpha = 0.0f;
              }
              else if (i==_progress)
              {
                TendencyProgressArrows[i].sprite = _arrow;

                TendencyProgressArrows_inside[i].GetComponent<Image>().sprite = _arrow;
                StartCoroutine(arroweffect(TendencyProgressArrows_inside[i]));
              }
              else
              {
                TendencyProgressArrows[i].sprite = GameManager.Instance.ImageHolder.Arrow_Empty;
                TendencyProgressArrows_inside[i].alpha = 0.0f;
              }
            }
          }
        }
      }
      else
      {
        HighlightEffect.SetInfo(_targettendency.Type == TendencyTypeEnum.Body ? HighlightEffectEnum.Physical : HighlightEffectEnum.Material);
        if (_targettendency.Level > 0)
        {
          _tendencyicon = _targettendency.CurrentIcon;
        }
        else
        {
          _tendencyicon = GameManager.Instance.ImageHolder.GetTendencyIcon(MyTendencyType, +1);
        }

        if (_targettendency.Progress <= 0)
        {
          if (TendencyHolder.activeInHierarchy == true) TendencyHolder.SetActive(false);
        }
        else
        {
          if (_targettendency.Level == 2)
          {
            if (TendencyHolder.activeInHierarchy == true) TendencyHolder.SetActive(false);
          }
          else
          {
            if (TendencyHolder.activeInHierarchy == false) TendencyHolder.SetActive(true);

            _nexttendencyicon = _targettendency.GetNextIcon(false);
            NextTendencyIcon.sprite = _nexttendencyicon;

            int _progress =  _targettendency.Progress;
            int _maxprogress = _targettendency.CurrentProgressTargetCount;
            Sprite _arrow = GameManager.Instance.ImageHolder.Arrow_Active(_targettendency.Type, false);
            for (int i = 0; i < 3; i++)
            {
              if (i >= _maxprogress)
              {
                TendencyProgressArrows[i].gameObject.SetActive(false);
                continue;
              }

              if (TendencyProgressArrows[i].gameObject.activeInHierarchy.Equals(false)) TendencyProgressArrows[i].gameObject.SetActive(true);

              if (i < _progress)
              {
                TendencyProgressArrows[i].sprite = _arrow;
                TendencyProgressArrows_inside[i].alpha = 0.0f;
              }
              else if (i == _progress)
              {
                TendencyProgressArrows[i].sprite = _arrow;

                TendencyProgressArrows_inside[i].GetComponent<Image>().sprite = _arrow;
                StartCoroutine(arroweffect(TendencyProgressArrows_inside[i]));
              }
              else
              {
                TendencyProgressArrows[i].sprite = GameManager.Instance.ImageHolder.Arrow_Empty;
                TendencyProgressArrows_inside[i].alpha = 0.0f;
              }
            }
          }
        }

      }
    }

    MyButton.transition = Selectable.Transition.SpriteSwap;
    MyButton.spriteState = GameManager.Instance.ImageHolder.GetSelectionButtonBackground(MyTendencyType, IsLeft);
    MyImage.sprite = MyButton.spriteState.selectedSprite;
    MyDescription.text = _data.Name;
  }
  public void UpdateValues()
  {
    if (IsOverTendency)
    {
    }
    else
    {
      int _requirevalue = 0, _currentvalue = 0;
      switch (MySelectionData.ThisSelectionType)
      {
        case SelectionTargetType.None:
          break;
        case SelectionTargetType.Pay:
          _requirevalue = MyUIDialogue.GetRequireValue(IsLeft) * -1;
          switch (MySelectionData.SelectionPayTarget)
          {
            case StatusTypeEnum.HP:
              PayInfo.text = _requirevalue.ToString();
              break;
            case StatusTypeEnum.Sanity:
              PayInfo.text = _requirevalue.ToString();
              break;
            case StatusTypeEnum.Gold:
              if (GameManager.Instance.MyGameData.Gold < _requirevalue)
              {
                HighlightEffect.SetInfo(HighlightEffectEnum.Sanity);
                PayInfo.text = WNCText.PercentageColor(_requirevalue.ToString(), GameManager.Instance.MyGameData.Gold / _requirevalue * -1);
              }
              else
              {
                PayInfo.text = _requirevalue.ToString();
              }
              break;
          }
          break;
        case SelectionTargetType.Check_Single:
          if (SkillInfo_left_10.gameObject.activeSelf) SkillInfo_left_10.gameObject.SetActive(false);
          if (SkillInfo_left_1.gameObject.activeSelf) SkillInfo_left_1.gameObject.SetActive(false);
          if (!SkillInfo_left_center.gameObject.activeSelf) SkillInfo_left_center.gameObject.SetActive(true);
          if (SkillInfo_right_10.gameObject.activeSelf) SkillInfo_right_10.gameObject.SetActive(false);
          if (SkillInfo_right_1.gameObject.activeSelf) SkillInfo_right_1.gameObject.SetActive(false);
          if (!SkillInfo_right_center.gameObject.activeSelf) SkillInfo_right_center.gameObject.SetActive(true);

          _requirevalue = GameManager.Instance.MyGameData.CheckSkillSingleValue; 
          _currentvalue = MyUIDialogue.GetRequireValue(IsLeft);
          if (skillcountcoroutine == null)
          {
            skillcountcoroutine = UIManager.Instance.ChangeCount(SkillInfo_left_center, _currentvalue, _requirevalue, WNCText.PercentageColor);
            StartCoroutine(skillcountcoroutine);
          }
          else
          {
            StopCoroutine(skillcountcoroutine);
                    skillcountcoroutine = UIManager.Instance.ChangeCount(SkillInfo_left_center, _currentvalue, _requirevalue, WNCText.PercentageColor);
            StartCoroutine(skillcountcoroutine);
          }
          SkillInfo_right_center.text = _requirevalue.ToString();

          break;
        case SelectionTargetType.Check_Multy:
          if (SkillInfo_left_10.gameObject.activeSelf) SkillInfo_left_10.gameObject.SetActive(false);
          if (SkillInfo_left_1.gameObject.activeSelf) SkillInfo_left_1.gameObject.SetActive(false);
          if (!SkillInfo_left_center.gameObject.activeSelf) SkillInfo_left_center.gameObject.SetActive(true);
          if (SkillInfo_right_10.gameObject.activeSelf) SkillInfo_right_10.gameObject.SetActive(false);
          if (SkillInfo_right_1.gameObject.activeSelf) SkillInfo_right_1.gameObject.SetActive(false);
          if (!SkillInfo_right_center.gameObject.activeSelf) SkillInfo_right_center.gameObject.SetActive(true);


          _requirevalue = GameManager.Instance.MyGameData.CheckSkillMultyValue; 
          _currentvalue = MyUIDialogue.GetRequireValue(IsLeft);
          if (!gameObject.activeSelf)
          {
            SkillInfo_left_center.text= WNCText.PercentageColor(Mathf.FloorToInt(_currentvalue).ToString(), _currentvalue / _requirevalue);
          }
          else
          {
            if (skillcountcoroutine == null)
            {
              skillcountcoroutine = UIManager.Instance.ChangeCount(SkillInfo_left_center, _currentvalue, _requirevalue, WNCText.PercentageColor);
              StartCoroutine(skillcountcoroutine);
            }
            else
            {
              StopCoroutine(skillcountcoroutine);
              skillcountcoroutine = UIManager.Instance.ChangeCount(SkillInfo_left_center, _currentvalue, _requirevalue, WNCText.PercentageColor);
              StartCoroutine(skillcountcoroutine);
            }
          }
          //          SkillInfo_require.text = WNCText.PercentageColor(_requirevalue.ToString(), (float)_requirevalue / (float)_currentvalue);
          SkillInfo_right_center.text = _requirevalue.ToString();
          HighlightEffect.SetInfo(new List<SkillTypeEnum> { MySelectionData.SelectionCheckSkill[0], MySelectionData.SelectionCheckSkill[1] });

          break;
      }
    }
    LayoutRebuilder.ForceRebuildLayoutImmediate(MyGroup.transform as RectTransform);
  }
  private IEnumerator skillcountcoroutine = null;
  private IEnumerator arroweffect(CanvasGroup group)
  {
    group.alpha = 1.0f;
    float _time = 0.0f, _targettime = 1.0f;
    float _alpha = 0.0f;
    while (true)
    {
      _time = 0.0f;
      _alpha = 1.0f;
      group.alpha = _alpha;
      while (_time < _targettime)
      {
        _alpha = Mathf.Lerp(1.0f, 0.0f, _time / _targettime);
        _time += Time.deltaTime;
        group.alpha = _alpha;
        yield return null;
      }
      yield return null;
    }
  }

  public void Select()
  {
    if (UIManager.Instance.IsWorking) return;

    StopAllCoroutines();
    OtherSelection.StopAllCoroutines();
    foreach (var _group in TendencyProgressArrows_inside)
    {
      _group.alpha = 0.0f;
    }
    UIManager.Instance.AddUIQueue(select());
  }

  private IEnumerator select()
  {
    MyGroup.interactable = false;
    HighlightEffect.Interactive = false;
    UIManager.Instance.PreviewManager.ClosePreview();

    if (MyTendencyType != TendencyTypeEnum.None)
    {
      GameManager.Instance.AddTendencyCount(MyTendencyType, Index);
  //    StartCoroutine(UIManager.Instance.SetIconEffect(MyTendencyType, IsLeft, TendencyIconObj.transform as RectTransform));
    }
    switch (MySelectionData.ThisSelectionType)
    {
      case SelectionTargetType.None:
        break;
      case SelectionTargetType.Pay:
        break;
      case SelectionTargetType.Check_Single:
      //  yield return StartCoroutine(UIManager.Instance.SetIconEffect(true, MySelectionData.SelectionCheckSkill[0], SkillIcon_A.transform as RectTransform));
        break;
      case SelectionTargetType.Check_Multy:
      //  StartCoroutine(UIManager.Instance.SetIconEffect(true, MySelectionData.SelectionCheckSkill[0], SkillIcon_A.transform as RectTransform));
      //  yield return new WaitForSeconds(0.1f);
       // yield return StartCoroutine(UIManager.Instance.SetIconEffect(true, MySelectionData.SelectionCheckSkill[0], SkillIcon_B.transform as RectTransform));
        break;
    }
    MyUIDialogue.SelectSelection(this);

    yield return null;
  }

}
