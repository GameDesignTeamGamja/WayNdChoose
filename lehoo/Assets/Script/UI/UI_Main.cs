using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class UI_Main : UI_default
{
  public CanvasGroup IllustGroup = null;
  public ImageSwapScript Illust = null;
  public static float ImageChangeTime = 6.0f;
  private WaitForSeconds ImageSwapWait = new WaitForSeconds(ImageChangeTime);
  private Sprite CurrentIllust = null;
  private IEnumerator showimage()
  {
    while (true)
    {
      Illust.Next(GameManager.Instance.ImageHolder.GetRandomMainIllust(CurrentIllust), ImageChangeTime);
      CurrentIllust = GameManager.Instance.ImageHolder.GetRandomMainIllust(CurrentIllust);
      yield return ImageSwapWait;
    }
  }
  [SerializeField] private CanvasGroup LogoGroup = null;
  [SerializeField] private Button LoadGameButton = null;
  [SerializeField] private TextMeshProUGUI LoadGameText = null;
  [SerializeField] private TextMeshProUGUI LoadInfoText = null;
  [SerializeField] private CanvasGroup LoadInfoGroup=null;
  [SerializeField] private TextMeshProUGUI NewGameText = null;
 // [SerializeField] private TextMeshProUGUI OptionText = null;
  [SerializeField] private TextMeshProUGUI QuitText = null;
  
  [SerializeField] private TextMeshProUGUI Quest_0_Text = null;
  [Space(10)]
  [SerializeField] private CanvasGroup QuestIllustGroup = null;
  [SerializeField] private Image QuestIllust = null;
  [SerializeField] private TextMeshProUGUI QuestDescription = null;
  [SerializeField] private Button StartNewGameButton = null;
  [SerializeField] private TextMeshProUGUI StartNewGameText = null;
  [SerializeField] private TextMeshProUGUI BackToMainText = null;
  [Space(10)]
  public float MainUIOpenTime = 0.4f;
  public float MainUICloseTime = 0.2f;
  private WaitForSeconds LittleWait = new WaitForSeconds(0.2f);
  private WaitForSeconds Wait = new WaitForSeconds(0.3f);
  private void Start()
  {
    NewGameText.text = GameManager.Instance.GetTextData("NEWGAME");
    LoadGameText.text = GameManager.Instance.GetTextData("LOADGAME");
 //   OptionText.text = GameManager.Instance.GetTextData("OPTION");
    QuitText.text = GameManager.Instance.GetTextData("QUITGAME");
    StartNewGameText.text = GameManager.Instance.GetTextData("STARTGAME");
    BackToMainText.text = GameManager.Instance.GetTextData("QUIT");
   Quest_0_Text.text = GameManager.Instance.EventHolder.Quest_Cult.QuestName;
    SetupMain();
  }
  public void SetupMain()
  {
    if (GameManager.Instance.GameSaveData != null)
    {

      string _turnname = "";
      switch (GameManager.Instance.GameSaveData.Turn)
      {
        case 0:
          _turnname = GameManager.Instance.GetTextData("SPRING_NAME");
          break;
        case 1:
          _turnname = GameManager.Instance.GetTextData("SUMMER_NAME");
          break;
        case 2:
          _turnname = GameManager.Instance.GetTextData("AUTUMN_NAME");
          break;
        case 3:
          _turnname = GameManager.Instance.GetTextData("WINTER_NAME");
          break;
      }



      switch ((QuestType) GameManager.Instance.GameSaveData.QuestType)
      {
        case QuestType.Cult:
          LoadInfoText.text = string.Format(GameManager.Instance.GetTextData("ProgressInfo"),
       GameManager.Instance.GameSaveData.Year,
       _turnname,
       GameManager.Instance.GameSaveData.Cult_Progress,
       GameManager.Instance.EventHolder.GetEvent(GameManager.Instance.GameSaveData.CurrentEventID).Name,
       GameManager.Instance.GameSaveData.HP,
       GameManager.Instance.GameSaveData.Sanity,
       GameManager.Instance.GameSaveData.Gold,
       GameManager.Instance.GameSaveData.Movepoint);
          break;
      }
    }
    else
    {
      LoadGameButton.gameObject.SetActive(false);
      LoadInfoText.text = "";
    }
    StartCoroutine(showimage());
    UIManager.Instance.AddUIQueue(openmain());
  }//메인 화면 텍스트 세팅
  public void StartGameDirect()
  {
    if (UIManager.Instance.IsWorking) return;
    UIManager.Instance.AddUIQueue(startgamedirect());
  }
  private IEnumerator startgamedirect()
  {
    SelectedQuest = QuestType.Cult;

    yield return StartCoroutine(closemain());

    GameManager.Instance.StartNewGame(SelectedQuest);

  }
  public void OpenScenario()//새 게임 눌러 시나리오 선택으로 넘어가는 메소드
  {
    UIManager.Instance.AddUIQueue(closemain());

    SelectQuest(0);

    UIManager.Instance.AddUIQueue(openscenario());
  }
  public void LoadGame()//불러오기 버튼 눌러 게임 시작(미완성)
  {
    if (UIManager.Instance.IsWorking) return;

    UIManager.Instance.AddUIQueue(loadgame());
  }
  private IEnumerator loadgame()
  {
    yield return StartCoroutine(closemain());

    GameManager.Instance.LoadGame();
  }
  public void ReturnToMain()
  {
    UIManager.Instance.AddUIQueue(closescenario());
    UIManager.Instance.AddUIQueue(openmain());
  }
  public void SelectQuest(int index)//시나리오 버튼 누를때
  {
    Debug.Log("scenario opened");
    QuestType _quest = (QuestType)index;
    SelectedQuest = _quest;
    QuestDescription.text = GameManager.Instance.EventHolder.GetQuest(SelectedQuest).QuestDescription;
    QuestIllust.sprite = GameManager.Instance.EventHolder.GetQuest(SelectedQuest).QuestIllust;
    StartNewGameButton.interactable = true;
  }
  private QuestType SelectedQuest = QuestType.Cult;
  public void StartNewGame()//버튼으로 새 게임 시작 버튼 누르는거
  {
    UIManager.Instance.AddUIQueue(startscenario());

    GameManager.Instance.StartNewGame(SelectedQuest);
    //게임매니저에서 데이터 생성->맵 생성->(메인->게임)전환 코루틴 실행->이후 처리
  }
  private IEnumerator startscenario()
  {
 //   StartNewGameButton.interactable = false;

    StartCoroutine(UIManager.Instance.ChangeAlpha(QuestIllustGroup, 0.0f, 3.0f));

    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("startgame").Rect, GetPanelRect("startgame").InsidePos, GetPanelRect("startgame").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questdescription").Rect, GetPanelRect("questdescription").InsidePos, GetPanelRect("questdescription").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("back").Rect, GetPanelRect("back").InsidePos, GetPanelRect("back").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    yield return StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questbuttonholder").Rect, GetPanelRect("questbuttonholder").InsidePos, GetPanelRect("questbuttonholder").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    GetPanelRect("questillust").Rect.anchoredPosition = GetPanelRect("questillust").OutisdePos;
    yield return new WaitUntil(() => (QuestIllustGroup.alpha == 0.0f));
  }
  private IEnumerator openmain()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(LogoGroup, 1.0f, MainUIOpenTime));

    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("mainillust").Rect, GetPanelRect("mainillust").OutisdePos, GetPanelRect("mainillust").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("quitgame").Rect, GetPanelRect("quitgame").OutisdePos, GetPanelRect("quitgame").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("newgame").Rect, GetPanelRect("newgame").OutisdePos, GetPanelRect("newgame").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    if (LoadInfoText.text != "")
    {
      StartCoroutine(UIManager.Instance.ChangeAlpha(LoadInfoGroup,1.0f,1.0f));
    }
  yield return  StartCoroutine(UIManager.Instance.moverect(GetPanelRect("loadgame").Rect, GetPanelRect("loadgame").OutisdePos, GetPanelRect("loadgame").InsidePos, MainUIOpenTime, true));
  }
  private IEnumerator closemain()
  {
    StartCoroutine(UIManager.Instance.ChangeAlpha(LogoGroup, 0.0f, MainUICloseTime));
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("loadgame").Rect, GetPanelRect("loadgame").InsidePos, GetPanelRect("loadgame").OutisdePos, MainUICloseTime, false));
    yield return LittleWait;

    if (LoadInfoText.text != "")
    {
      StartCoroutine(UIManager.Instance.ChangeAlpha(LoadInfoGroup, 0.0f, 1.0f));
    }

    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("newgame").Rect, GetPanelRect("newgame").InsidePos, GetPanelRect("newgame").OutisdePos, MainUICloseTime, false));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("quitgame").Rect, GetPanelRect("quitgame").InsidePos, GetPanelRect("quitgame").OutisdePos, MainUICloseTime, false));
    yield return LittleWait;
    yield return StartCoroutine(UIManager.Instance.ChangeAlpha(IllustGroup, 0.0f, MainUICloseTime));
  }
  private IEnumerator openscenario()
  {
 //   StartNewGameButton.interactable = false;
    if (QuestIllustGroup.alpha == 0.0f) QuestIllustGroup.alpha = 1.0f;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questillust").Rect, GetPanelRect("questillust").OutisdePos, GetPanelRect("questillust").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questdescription").Rect, GetPanelRect("questdescription").OutisdePos, GetPanelRect("questdescription").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questbuttonholder").Rect, GetPanelRect("questbuttonholder").OutisdePos, GetPanelRect("questbuttonholder").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("startgame").Rect, GetPanelRect("startgame").OutisdePos, GetPanelRect("startgame").InsidePos, MainUIOpenTime, true));
    yield return Wait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("back").Rect, GetPanelRect("back").OutisdePos, GetPanelRect("back").InsidePos, MainUIOpenTime, true));
  }
  private IEnumerator closescenario()
  {
//  StartNewGameButton.interactable = false;

    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("back").Rect, GetPanelRect("back").InsidePos, GetPanelRect("back").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questbuttonholder").Rect, GetPanelRect("questbuttonholder").InsidePos, GetPanelRect("questbuttonholder").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("startgame").Rect, GetPanelRect("startgame").InsidePos, GetPanelRect("startgame").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questdescription").Rect, GetPanelRect("questdescription").InsidePos, GetPanelRect("questdescription").OutisdePos, MainUIOpenTime, true));
    yield return LittleWait;
    yield return StartCoroutine(UIManager.Instance.moverect(GetPanelRect("questillust").Rect, GetPanelRect("questillust").InsidePos, GetPanelRect("questillust").OutisdePos, MainUIOpenTime, true));
    QuestIllust.sprite = GameManager.Instance.ImageHolder.Transparent;
    QuestDescription.text = "";
  }
  public void QuitGame()
  {
    Application.Quit();
  }
}
