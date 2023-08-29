using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_QuestWolf : UI_default
{
  [Space(10)]
  public bool Skip = false;
  [SerializeField] private float UIMoveInTime = 0.7f;
  [SerializeField] private float UIMoveOutTime = 0.4f;//Rect 움직이는거
  [SerializeField] private float FadeInTime = 1.0f;
  [SerializeField] private float FadeOutTime = 0.4f;  //이미지,텍스트 투명도
  [Space(5)]
  [SerializeField] private Image Prologue_IllustImage = null;
  [SerializeField] private CanvasGroup Prologue_IllustGroup = null;
  [SerializeField] private TextMeshProUGUI Prologue_Description = null;
  [SerializeField] private CanvasGroup Prologue_DescriptionGroup = null;
  [SerializeField] private CanvasGroup Prologue_ButtonHolderGroup = null;
  [SerializeField] private Button Prologue_Button_A = null;
  [SerializeField] private TextMeshProUGUI Prologue_ButtonText_A = null;
  [SerializeField] private Button Prologue_Button_B = null;
  [SerializeField] private TextMeshProUGUI Prologue_ButtonText_B = null;
  public int CurrentPrologueIndex = 0;
  [Space(5)]
  [SerializeField] private Image Searching_IllustImage = null;
  [SerializeField] private TextMeshProUGUI Searching_Description = null;
  [SerializeField] private CanvasGroup Searching_RewardButton_Group = null;
  [SerializeField] private Button Searching_RewardButton = null;
  [SerializeField] private TextMeshProUGUI Searching_ButtonText = null;
  [SerializeField] private Button Searching_NextButton = null;
  [SerializeField] private TextMeshProUGUI Searching_NextButtonText = null;
  [Space(5)]
  [SerializeField] private TextMeshProUGUI Wanted_Description = null;
  [SerializeField] private TextMeshProUGUI Wanted_Cult_Description = null;
  [SerializeField] private TextMeshProUGUI Wanted_Wolf_Description = null;
  private QuestHolder_Wolf QuestHolder = null;
  public void OpenUI_Prologue(QuestHolder_Wolf wolf)
  {
    if(DefaultRect.anchoredPosition!=Vector2.zero)DefaultRect.anchoredPosition = Vector2.zero;
    CurrentPrologueIndex = 0;
    QuestHolder = wolf;
    IsOpen = true;
    UIManager.Instance.AddUIQueue(openui_prologue());
  }
  private IEnumerator openui_prologue()
  {
    Prologue_IllustImage.sprite = QuestHolder.Prologue_0_Illust;
    Prologue_IllustGroup.alpha = 0.0f;
    Prologue_Description.text = QuestHolder.Prologue_0_Description;
    Prologue_DescriptionGroup.alpha = 0.0f;
    Prologue_ButtonHolderGroup.alpha = 0.0f;
    Prologue_Button_A.gameObject.SetActive(true);
    Prologue_ButtonText_A.text = QuestHolder.Prologue_0_Selection;
    Prologue_Button_B.gameObject.SetActive(false);
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(Prologue_ButtonHolderGroup.GetComponent<RectTransform>());
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetPanelRect("description_start").Rect); 
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("illust_start").Rect, GetPanelRect("illust_start").OutisdePos, GetPanelRect("illust_start").InsidePos, UIMoveInTime, UIManager.Instance.UIPanelOpenCurve));
    yield return new WaitForSeconds(0.1f);
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("description_start").Rect, GetPanelRect("description_start").OutisdePos, GetPanelRect("description_start").InsidePos, UIMoveInTime, UIManager.Instance.UIPanelOpenCurve));
    yield return new WaitForSeconds(0.5f);
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 1.0f, FadeInTime, false));
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 1.0f, FadeInTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 1.0f, FadeInTime, false));
    Prologue_Button_A.onClick.RemoveAllListeners();
    Prologue_Button_A.onClick.AddListener(Next);
    Prologue_ButtonHolderGroup.interactable = true;
    Prologue_ButtonHolderGroup.blocksRaycasts = true;
    DefaultGroup.interactable = true;
    DefaultGroup.blocksRaycasts = true;
    yield return null;
  }
  public  void CloseUI_Prologue() => UIManager.Instance.AddUIQueue(closeui_prologue());
  private IEnumerator closeui_prologue()
  {
    DefaultGroup.interactable = false;
    CurrentPrologueIndex = 0;
    IsOpen = false;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("illust_start").Rect, GetPanelRect("illust_start").InsidePos, GetPanelRect("illust_start").OutisdePos, UIMoveOutTime, UIManager.Instance.UIPanelCLoseCurve));
    yield return new WaitForSeconds(0.1f);
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("description_start").Rect, GetPanelRect("description_start").InsidePos, GetPanelRect("description_start").OutisdePos, UIMoveOutTime, UIManager.Instance.UIPanelCLoseCurve));
    yield return new WaitForSeconds(2.0f);
    Prologue_ButtonText_A.gameObject.SetActive(true);
  }

  public void SelectTendency(int index)
  {
    Prologue_ButtonHolderGroup.interactable = false;
    switch (CurrentPrologueIndex)
    {
      case 1:
        if (index == 0)
        {
          GameManager.Instance.MyGameData.Skill_Conversation.LevelByDefault += 1;
          GameManager.Instance.MyGameData.Tendency_Body.Level = -1;
          UIManager.Instance.AddUIQueue(setaftertendencypanel(QuestHolder.Prologue_Tendency_0_Selection_0_Illust, QuestHolder.Prologue_Tendency_0_Selection_0_Description, QuestHolder.Prologue_Tendency_0_Selection_0_Selection));
        }
        else
        {
          GameManager.Instance.MyGameData.Skill_Force.LevelByDefault += 1;
          GameManager.Instance.MyGameData.Tendency_Body.Level = +1;
          UIManager.Instance.AddUIQueue(setaftertendencypanel(QuestHolder.Prologue_Tendency_0_Selection_1_Illust, QuestHolder.Prologue_Tendency_0_Selection_1_Description, QuestHolder.Prologue_Tendency_0_Selection_1_Selection));
        }
        break;//(정신적+대화)선택 , (육체적+무력)선택

      case 2:
        if (index == 0)
        {
          GameManager.Instance.MyGameData.Skill_Wild.LevelByDefault += 1;
          GameManager.Instance.MyGameData.Tendency_Head.Level = -1;
          UIManager.Instance.AddUIQueue(setaftertendencypanel(QuestHolder.Prologue_Tendency_1_Selection_0_Illust, QuestHolder.Prologue_Tendency_1_Selection_0_Description, QuestHolder.Prologue_Tendency_1_Selection_0_Selection));
        }
        else
        {
          GameManager.Instance.MyGameData.Skill_Intelligence.LevelByDefault += 1;
          GameManager.Instance.MyGameData.Tendency_Head.Level = +1;
          UIManager.Instance.AddUIQueue(setaftertendencypanel(QuestHolder.Prologue_Tendency_1_Selection_1_Illust, QuestHolder.Prologue_Tendency_1_Selection_1_Description, QuestHolder.Prologue_Tendency_1_Selection_1_Selection));
        }
        break;//(감정적+자연)선택 , (물질적+지성)선택
    }
  }
  private IEnumerator setaftertendencypanel(Sprite illust,string description,string selection)
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 0.0f, FadeOutTime, false));
    yield return new WaitForSeconds(FadeOutTime);
    Prologue_IllustImage.sprite = illust;
    Prologue_Description.text = description;
    Prologue_ButtonText_A.text = selection;
    Prologue_Button_A.onClick.RemoveAllListeners();
    Prologue_Button_A.onClick.AddListener(() => Next());
    Prologue_Button_B.gameObject.SetActive(false);
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(Prologue_ButtonHolderGroup.GetComponent<RectTransform>());
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetPanelRect("description").Rect);
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 1.0f, FadeInTime, false));
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 1.0f, FadeInTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 1.0f, FadeInTime, false));

    Prologue_ButtonHolderGroup.interactable = true;

    yield return new WaitForSeconds(0.4f);
  }
  public void Next()
  {
    if (Skip)
    {
      UIManager.Instance.AddUIQueue(setprologue_3());
      return;
    }
    CurrentPrologueIndex++;
    Prologue_ButtonHolderGroup.interactable = false;
    switch (CurrentPrologueIndex)
    {
      case 1:
        UIManager.Instance.AddUIQueue(setprologue_1());
        break;
      case 2:
        UIManager.Instance.AddUIQueue(setprologue_2());
        break;
      case 3:
        UIManager.Instance.AddUIQueue(setprologue_3());
        break;
    }
  }
  private IEnumerator setprologue_1()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 0.0f, FadeOutTime, false));
    yield return new WaitForSeconds(FadeOutTime);
    Prologue_Button_B.gameObject.SetActive(true);
    Prologue_IllustImage.sprite = QuestHolder.Prologue_Tendency_0_Illust;
    Prologue_Description.text = QuestHolder.Prologue_Tendency_0_Description;
    Prologue_ButtonText_A.text = QuestHolder.Prologue_Tendency_0_Selection_0;
    Prologue_ButtonText_B.text = QuestHolder.Prologue_Tendency_0_Selection_1;
    Prologue_Button_A.onClick.RemoveAllListeners();
    Prologue_Button_A.onClick.AddListener(()=>SelectTendency(0));
    Prologue_Button_B.onClick.AddListener(()=>SelectTendency(1));
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(Prologue_ButtonHolderGroup.GetComponent<RectTransform>());
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetPanelRect("description").Rect);
    yield return new WaitForEndOfFrame();
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 1.0f, FadeInTime, false));
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 1.0f, FadeInTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 1.0f, FadeInTime, false));

    Prologue_ButtonHolderGroup.interactable = true;

    yield return new WaitForSeconds(0.4f);
  }//정신적,육체적 선택지 세팅
  private IEnumerator setprologue_2()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 0.0f, FadeOutTime, false));
    yield return new WaitForSeconds(FadeOutTime);
    Prologue_Button_B.gameObject.SetActive(true);
    Prologue_IllustImage.sprite = QuestHolder.Prologue_Tendency_1_Illust;
    Prologue_Description.text = QuestHolder.Prologue_Tendency_1_Description;
    Prologue_ButtonText_A.text = QuestHolder.Prologue_Tendency_1_Selection_0;
    Prologue_ButtonText_B.text = QuestHolder.Prologue_Tendency_1_Selection_1;
    Prologue_Button_A.onClick.RemoveAllListeners();
    Prologue_Button_A.onClick.AddListener(() => SelectTendency(0));
    Prologue_Button_B.onClick.AddListener(() => SelectTendency(1));
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(Prologue_ButtonHolderGroup.GetComponent<RectTransform>());
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetPanelRect("description").Rect);
    yield return new WaitForEndOfFrame();
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 1.0f, FadeInTime, false));
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 1.0f, FadeInTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 1.0f, FadeInTime, false));

    Prologue_ButtonHolderGroup.interactable = true;

    yield return new WaitForSeconds(0.4f);
  }//감정적,물질적 선택지 세팅
  private IEnumerator setprologue_3()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 0.0f, FadeOutTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 0.0f, FadeOutTime, false));
    yield return new WaitForSeconds(FadeOutTime);
    Prologue_Button_A.gameObject.SetActive(false);
    Prologue_Button_B.gameObject.SetActive(false);

    Prologue_IllustImage.sprite = QuestHolder.Prologue_Tendency_Last_Illust;
    Prologue_Description.text = QuestHolder.Prologue_Last_Description;

    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(Prologue_ButtonHolderGroup.GetComponent<RectTransform>());
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetPanelRect("description").Rect);

    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_IllustGroup, 1.0f, FadeInTime, false));
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_DescriptionGroup, 1.0f, FadeInTime, false));
    StartCoroutine(UIManager.Instance.ChangeAlpha(Prologue_ButtonHolderGroup, 1.0f, FadeInTime, false));

    MoveRectForButton(0);
    UIManager.Instance.MapButton.Open(1, this);

  }//지도 여는 상황 세팅

  public void OpenUI_Searching(int index)
  {
    if (DefaultRect.anchoredPosition != Vector2.zero) DefaultRect.anchoredPosition = Vector2.zero;
    IsOpen = true;

    UIManager.Instance.AddUIQueue(openui_searching(index));
  }
  private IEnumerator openui_searching(int index)
  {
    GameManager.Instance.MyGameData.Quest_Wolf_Progress++;
    QuestEventData_Wolf _data = QuestHolder.SearchingEvents[index];
    Searching_IllustImage.sprite = _data.Illust;
    Searching_Description.text = _data.Description;
    Searching_ButtonText.text = $"{GameManager.Instance.GetTextData(StatusType.Sanity, 2)} {WNCText.GetSanityColor(ConstValues.Quest_Wolf_Searching_Sanityrewardvalue)}";
    if (Searching_RewardButton_Group.alpha == 0.0f)
    {
      Searching_RewardButton_Group.alpha = 1.0f;
      Searching_RewardButton_Group.interactable = true;
      Searching_RewardButton_Group.blocksRaycasts = true;
      Searching_RewardButton.interactable = true;
    }

    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("illust_searching").Rect, GetPanelRect("illust_searching").OutisdePos, GetPanelRect("illust_searching").InsidePos, UIMoveInTime, UIManager.Instance.UIPanelOpenCurve));
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("description_searching").Rect, GetPanelRect("description_searching").OutisdePos, GetPanelRect("description_searching").InsidePos, UIMoveInTime, UIManager.Instance.UIPanelOpenCurve));

    if (GameManager.Instance.MyGameData.Quest_Wolf_Progress == 3)
    {
      StartCoroutine(UIManager.Instance.moverect(GetPanelRect("nextbutton_searching").Rect, GetPanelRect("nextbutton_searching").OutisdePos, GetPanelRect("nextbutton_searching").InsidePos, UIMoveInTime, UIManager.Instance.UIPanelOpenCurve));
      if (Searching_NextButton.interactable == false) Searching_NextButton.interactable = true;
      Searching_NextButtonText.text = GameManager.Instance.GetTextData("Quest_Sidepanel_Searching_Finish");
    }
    else
    {
      UIManager.Instance.SettleButton.Open(1, this);
    }
    UIManager.Instance.WolfSidePanel.UpdateSearchingPanel();

    yield return null;
  }
  public void SearchingSanityReward()
  {
    GameManager.Instance.MyGameData.CurrentSanity += ConstValues.Quest_Wolf_Searching_Sanityrewardvalue;
    UIManager.Instance.UpdateSanityText();
    Searching_RewardButton.interactable = false;
    StartCoroutine(UIManager.Instance.ChangeAlpha(Searching_RewardButton_Group, 0.0f, 0.8f, false));
  }
  public void NextToChoose()
  {

  }
}
