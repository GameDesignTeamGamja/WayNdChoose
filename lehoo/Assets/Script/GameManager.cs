using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public enum GameOverTypeEnum { HP,Sanity}
public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }

  [HideInInspector] public GameData MyGameData = null;            //게임 데이터(진행도,현재 진행 중 이벤트, 현재 맵 상태,퀘스트 등등)
  [HideInInspector] public GameJsonData GameJsonData = null;
  [HideInInspector] public const string GameDataName = "WNCGameData.json";
  [HideInInspector] public ProgressData MyProgressData = new ProgressData();

  public ImageHolder ImageHolder = null;             //이벤트,경험,특성,정착지 일러스트 홀더

  [SerializeField] private TextAsset NormalEventData = null;  //이벤트 Json
  [SerializeField] private TextAsset FollowEventData = null;  //연계 이벤트 Json
  [SerializeField] private TextAsset QuestEventData = null;   //퀘스트 이벤트 Json
  [SerializeField] private TextAsset EXPData = null;    //경험 Json
  [SerializeField] private TextAsset TextData = null;
  public EventHolder EventHolder = new EventHolder();                               //이벤트 저장할 홀더
  public Dictionary<string, Experience> ExpDic = new Dictionary<string, Experience>();  //경험 딕셔너리
  public Dictionary<string, Experience> MadExpDic = new Dictionary<string, Experience>();
  public Dictionary<string,string> TextDic=new Dictionary<string, string>();
  public const string NullText = "no text exist";
  private GoldFailData goldfaildata = null;
  public GoldFailData GoldFailData
  {
    get
    {
      if (goldfaildata == null)
      {
        goldfaildata = new GoldFailData();
        goldfaildata.Description = GetTextData("GOLDFAIL_TEXT");
        goldfaildata.Panelty_target = PenaltyTarget.Status;
        goldfaildata.Loss_target = StatusType.Sanity;
        goldfaildata.Illust = ImageHolder.NoGoldIllust;

      }
      return goldfaildata;
    }
  }
  public string GetTextData(string _id)
  {
    // Debug.Log($"{_id} ID를 가진 텍스트 데이터 {(TextDic.ContainsKey(_id)?"있음":"없음")}");
    if (!TextDic.ContainsKey(_id)) { Debug.Log($"{_id} 없음?"); return NullText; }
    return TextDic[_id];
  }
  /// <summary>
  /// texttype : 이름/설명
  /// </summary>
  /// <param name="envir"></param>
  /// <param name="texttype"></param>
  /// <returns></returns>
  public string GetTextData(EnvironmentType envir,int texttype)
  {
    string _str = "";
    switch (envir)
    {
      case EnvironmentType.NULL:
        return NullText;
      case EnvironmentType.River:
        _str = "RIVER";
        break;
      case EnvironmentType.Forest:
        _str = "FOREST";
        break;
      case EnvironmentType.Highland:
        _str = "HIGHLAND";
        break;
      case EnvironmentType.Mountain:
        _str = "MOUNTAIN";
        break;
      default:
        _str = "SEA";
        break;
    }
    _str += "_";
    if (texttype.Equals(0)) _str += "NAME";
    else _str += "DESCRIPTION";
    return GetTextData(_str);
  }
  /// <summary>
  /// 이름(아이콘 없음),이름(아이콘 있음),아이콘,설명,간략설명,증가,크게 증가,감소,크게 감소
  /// </summary>
  /// <param name="_theme"></param>
  /// <param name="texttype"></param>
  /// <returns></returns>
  public string GetTextData(SkillType skilltype,int texttype)
  {
    string _name = "";
    switch (skilltype)
    {
      case SkillType.Conversation: _name = "CONVERSATION"; break;
      case SkillType.Force: _name = "FORCE"; break;
      case SkillType.Wild: _name = "WILD"; break;
      case SkillType.Intelligence: _name = "INTELLIGENCE"; break;
    }
    _name += "_";
    switch (texttype)
    {
      case 0:
        _name += "NAME_NOICON";
        break;
      case 1:
        _name += "NAME_ICON";
        break;
      case 2:
        _name += "ICON";
        break;
      case 3:
        _name += "DESCRIPTION";
        break;
      case 4:
        _name += "SUBDESCRIPTION";
        break;
      case 5:
        _name += "UP_NORMAL";
        break;
      case 6:
        _name += "UP_HIGH";
        break;
      case 7:
        _name += "DOWN_NORMAL";
        break;
      case 8:
        _name += "DOWN_HIGH";
        break;
    }
    return GetTextData(_name);
  }
  public string GetTextData(SkillType skilltype, bool isup,bool isstrong,bool isicon)
  {
    string _name = "";
    switch (skilltype)
    {
      case SkillType.Conversation: _name = "CONVERSATION"; break;
      case SkillType.Force: _name = "FORCE"; break;
      case SkillType.Wild: _name = "WILD"; break;
      case SkillType.Intelligence: _name = "INTELLIGENCE"; break;
    }
    _name += isup ? "_UP" : "_DOWN";
    _name += isstrong ? "_HIGH" : "_NORMAL";
    _name += isicon ? "_ICON" : "";
    return GetTextData(_name);
  }
  public string GetTextData(EffectType _effect,bool isicon)
  {
    switch (_effect)
    {
      case EffectType.Conversation: return GetTextData(SkillType.Conversation,true,false,isicon);
      case EffectType.Force: return GetTextData(SkillType.Force , true, false, isicon);
      case EffectType.Wild: return GetTextData(SkillType.Wild, true, false, isicon);
      case EffectType.Intelligence: return GetTextData(SkillType.Intelligence, true, false, isicon);
      case EffectType.HPLoss: return GetTextData(StatusType.HP,false,isicon?2:1);
      case EffectType.HPGen: return GetTextData(StatusType.HP, true, isicon?2:1);
      case EffectType.SanityLoss: return GetTextData(StatusType.Sanity, false, isicon?2:1);
      case EffectType.SanityGen: return GetTextData(StatusType.Sanity, true, isicon?2:1);
      case EffectType.GoldLoss: return GetTextData(StatusType.Gold, false, isicon?2:1);
      case EffectType.GoldGen: return GetTextData(StatusType.Gold, true, isicon?2:1);
      default: return NullText;
    }
  }
  /// <summary>
  /// 이름 설명 간략설명 아이콘
  /// </summary>
  /// <param name="tendency"></param>
  /// <param name="level"></param>
  /// <returns></returns>
  public string GetTextData(TendencyType tendency,int level,int texttype)
  {
    string _name = tendency.Equals(TendencyType.Body) ? "TENDENCY_BODY" : "TENDENCY_HEAD";
    string _level = "";
    switch (level)
    {
      case -2:_level = "M2";break;
      case -1:_level = "M1";break;
      case 1:_level = "P1";break;
      case 2:_level = "P2";break;
    }
    string _type = "";
    switch (texttype)
    {
      case 0:_type = "NAME";break;
      case 1: _type = "DESCRIPTIION"; break;
      case 2: _type = "SUBDESCRIPTION"; break;
      case 3: _type = "ICON"; break;
    }
    return GetTextData(_name+"_" + _level+"_"+_type);
  }
  /// <summary>
  /// 이름 설명 아이콘 효과 효과설명
  /// </summary>
  /// <param name="_place"></param>
  /// <param name="texttype"></param>
  /// <returns></returns>
  public string GetTextData(PlaceType _place,int texttype)
  {
    string _str = "PLACE_";
    switch (_place)
    {
      case PlaceType.Residence: _str += "RESIDENCE";break;
      case PlaceType.Marketplace: _str += "MARKETPLACE"; break;
      case PlaceType.Temple: _str += "TEMPLE"; break;
      case PlaceType.Library: _str += "LIBRARY"; break;
      case PlaceType.Theater: _str += "THEATER"; break;
      case PlaceType.Academy: _str += "ACADEMY"; break;
    }
    _str += "_";
    switch (texttype)
    {
      case 0: _str += "NAME";break;
      case 1: _str += "DESCRIPTION";break;
      case 2: _str += "ICON";break;
      case 3: _str += "EFFECT_NAME";break;
      case 4: _str += "EFFECT_DESCRIPTION";break;
    }

    return GetTextData(_str);
  }
  /// <summary>
  /// 이름(X) 이름(O) 아이콘 설명 간략설명|회복(X) 회복(O) 회복아이콘|소모(X) 소모(O) 소모아이콘|회복증가(X) 회복증가(O) 회복아이콘|소모증가(X) 소모증가(O) 소모아이콘 
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public string GetTextData(StatusType type,int texttype)
  {
    string _str = "";
    switch (type)
    {
      case StatusType.HP: _str = "HP";break;
      case StatusType.Sanity:_str = "SANITY";break;
      case StatusType.Gold: _str = "GOLD";break;
    }
    _str += "_";
    switch (texttype)
    {
      case 0:_str += "NAME_NOICON";break;
      case 1: _str += "NAME_ICON"; break;
      case 2: _str += "ICON"; break;
      case 3: _str += "DESCRIPTION"; break;
      case 4: _str += "SUBDESCRIPTION"; break;
      case 5: _str += "RESTORE_NAME_NOICON"; break;
      case 6: _str += "RESTORE_NAME_ICON"; break;
      case 7: _str += "RESTORE_ICON"; break;
      case 8: _str += "PAY_NAME_NOICON"; break;
      case 9: _str += "PAY_NAME_ICON"; break;
      case 10: _str += "PAY_ICON"; break;
      case 11: _str += "INCREASE_NAME_NOICON"; break;
      case 12: _str += "INCREASE_NAME_ICON"; break;
      case 13: _str += "INCREASE_ICON"; break;
      case 14: _str += "DECREASE_NAME_NOICON"; break;
      case 15: _str += "DECREASE_NAME_ICON"; break;
      case 16: _str += "DECREASE_ICON"; break;
    }
    return GetTextData(_str);
  }
  /// <summary>
  /// 아이콘X 아이콘O 아이콘
  /// </summary>
  /// <param name="type"></param>
  /// <param name="isincrease"></param>
  /// <param name="icontype"></param>
  /// <returns></returns>
  public string GetTextData(StatusType type,bool isincrease,int icontype)
  {
    int _typecode = isincrease ? 11 : 14;
    _typecode += icontype;
    return GetTextData(type, _typecode);
  }
    public void LoadData()
  {
    if (File.Exists(Application.persistentDataPath+"/"+GameDataName ))
    {
      GameJsonData = JsonUtility.FromJson<GameJsonData>(File.ReadAllText(Application.persistentDataPath + "/" + GameDataName));
      MyGameData = GameJsonData.GetGameData();
    }
    //저장된 플레이어 데이터가 있으면 데이터 불러오기

    Dictionary<string, TextData> _temp = JsonConvert.DeserializeObject<Dictionary<string, TextData>>(TextData.text);
    foreach (var _data in _temp)
    {
      string _texttemp = _data.Value.TEXT;
      if (_texttemp.Contains("\\n")) _texttemp = _texttemp.Replace("\\n", "\n");
      if (TextDic.ContainsKey(_data.Key)) { Debug.Log($"{_data.Key} 겹침! 확인 필요!"); return; }
      TextDic.Add(_data.Value.ID, _data.Value.TEXT);
    }

    Dictionary<string, EventJsonData> _eventjson = new Dictionary<string, EventJsonData>();
    _eventjson = JsonConvert.DeserializeObject<Dictionary<string, EventJsonData>>(NormalEventData.text);
    foreach (var _data in _eventjson) EventHolder.ConvertData_Normal(_data.Value);
    //이벤트 Json -> EventHolder

    Dictionary<string, FollowEventJsonData> _followeventjson = new Dictionary<string, FollowEventJsonData>();
    _followeventjson = JsonConvert.DeserializeObject<Dictionary<string, FollowEventJsonData>>(FollowEventData.text);
    foreach (var _data in _followeventjson) EventHolder.ConvertData_Follow(_data.Value);
    //연계 이벤트 Json -> EventHolder

    Dictionary<string, QuestEventDataJson> _questeventjson = new Dictionary<string, QuestEventDataJson>();
    _questeventjson = JsonConvert.DeserializeObject<Dictionary<string, QuestEventDataJson>>(QuestEventData.text);
    foreach (var _data in _questeventjson) EventHolder.ConvertData_Quest(_data.Value);
    //퀘스트 Json -> EventHolder
    if (EventHolder.Quest_Wolf == null)EventHolder.Quest_Wolf = new QuestHolder_Wolf();



    Dictionary<string, ExperienceJsonData> _expjson = new Dictionary<string, ExperienceJsonData>();
    _expjson = JsonConvert.DeserializeObject<Dictionary<string, ExperienceJsonData>>(EXPData.text);
    foreach (var _data in _expjson)
    {
      Experience _exp = _data.Value.ReturnEXPClass();
      ExpDic.Add(_data.Value.ID, _exp);
      if(_exp.ExpType.Equals(ExpTypeEnum.Mad))MadExpDic.Add(_data.Value.ID, _exp);
    }
    //경험 Json -> EXPDic

  }//각종 Json 가져와서 변환
  public void SaveData()
  {

  }//현재 데이터 저장
  public void SuccessCurrentEvent(TendencyType _tendencytype,int index)
  {
    int _tendencyindex = 0;
    switch (_tendencytype)
    {
      case TendencyType.None:
        _tendencyindex = 0;
        break;
      case TendencyType.Body:
        if (index.Equals(0)) _tendencyindex = 1;
        else _tendencyindex = 2;
        break;
      case TendencyType.Head:
        if (index.Equals(0)) _tendencyindex = 3;
        else _tendencyindex = 4;
        break;
    }
    EventHolder.RemoveEvent(MyGameData.CurrentEvent,true,_tendencyindex);

    MyGameData.CurrentEventSequence = EventSequence.Clear;
    if (MyGameData.CurrentSettlement != null)
    {
      MyGameData.Turn++;
      UIManager.Instance.UpdateTurnIcon();
    }
  }
  public void FailCurrentEvent(TendencyType _tendencytype, int index)
  {
    int _tendencyindex = 0;
    switch (_tendencytype)
    {
      case TendencyType.None:
        _tendencyindex = 0;
        break;
      case TendencyType.Body:
        if (index.Equals(0)) _tendencyindex = 1;
        else _tendencyindex = 2;
        break;
      case TendencyType.Head:
        if (index.Equals(0)) _tendencyindex = 3;
        else _tendencyindex = 4;
        break;
    }
    EventHolder.RemoveEvent(MyGameData.CurrentEvent, false, _tendencyindex);

    MyGameData.CurrentEventSequence = EventSequence.Clear;
    if (MyGameData.CurrentSettlement != null)
    {
      MyGameData.Turn++;
      UIManager.Instance.UpdateTurnIcon();
    }
  }
  public void AddExp_Long(Experience exp)
  {
    if (exp.ExpType == ExpTypeEnum.Mad)
    {
      MyGameData.MaxSanity -= ConstValues.SanityLoseByMadnessExp;
      MyGameData.CurrentSanity = MyGameData.MaxSanity;
      UIManager.Instance.UpdateSanityText();
      UIManager.Instance.MyMadPanel.CloseUI();
    }
    MyGameData.LongTermEXP = exp;
    UIManager.Instance.UpdateExpLongTermIcon();
  }
  public void AddExp_Short(Experience exp,int index)
  {
    if (exp.ExpType == ExpTypeEnum.Mad)
    {
      MyGameData.MaxSanity -= ConstValues.SanityLoseByMadnessExp;
      MyGameData.CurrentSanity = MyGameData.MaxSanity;
      UIManager.Instance.UpdateSanityText();
      UIManager.Instance.MyMadPanel.CloseUI();
    }
    MyGameData.ShortTermEXP[index] = exp;
    UIManager.Instance.UpdateExpShortTermIcon();
  }
  public void ShiftShortExp(Experience _exp, int _index)
  {
    _exp.Duration = ConstValues.ShortTermStartTurn;
    Experience _target = MyGameData.ShortTermEXP[_index];
    MyGameData.ShortTermEXP[_index] = _exp;
    UIManager.Instance.UpdateExpShortTermIcon();
  }
  public void ShiftLongExp(Experience _exp)
  {
    _exp.Duration = ConstValues.LongTermStartTurn;
    Experience _target = MyGameData.LongTermEXP;
    MyGameData.LongTermEXP= _exp;
    MyGameData.CurrentSanity -= ConstValues.LongTermChangeCost;
    UIManager.Instance.UpdateExpLongTermIcon();
  }
  public void SetOuterEvent(EventDataDefulat _event)
  {
    MyGameData.CurrentEvent = _event;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    UIManager.Instance.OpenDialogue();
    //다이어로그 열기
  }//야외 이동을 통해 이벤트를 받은 경우
  public void SetSettlementPlace()
  {
    
    UIManager.Instance.OpenSuggestUI();
    //제시 UI 열기
    SaveData();
  }//정착지의 장소 세팅
  public void SelectEvent(EventDataDefulat _targetevent)
  {
    MyGameData.CurrentEvent = _targetevent;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    UIManager.Instance.OpenDialogue();
    SaveData();
  }//제시 패널에서 이벤트를 선택한 경우
  public void SelectQuestEvent(EventDataDefulat questevent)
  {
    Dictionary<Settlement, int> _temp = new Dictionary<Settlement, int>();
    MyGameData.CurrentEvent = questevent;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    UIManager.Instance.OpenDialogue();
    SaveData();
  }
  public void AddTendencyCount(TendencyType _tendencytype,int index)
  {
    switch (_tendencytype)
    {
      case TendencyType.Body:
        MyGameData.Tendency_Body.AddCount(index.Equals(0) ? false : true);
        break;
      case TendencyType.Head:
        MyGameData.Tendency_Head.AddCount(index.Equals(0) ? false : true);
        break;
    }
  }
  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
      LoadData();
      //  DebugAllEvents();
    }
    else Destroy(gameObject);

  }
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Backspace))
    {
      MyGameData = new GameData();
      CreateNewMap();

    }
    if (Input.GetKeyDown(KeyCode.F1)) MyGameData.CurrentSanity = 3;
  }
  public void GameOver(GameOverTypeEnum gameovertype)
  {
    UIManager.Instance.GameOver(gameovertype);
  }
  public void StartNewGame(QuestType newquest)
  {
    UIManager.Instance.AddUIQueue(startnewgame(newquest));
  }
  private IEnumerator startnewgame(QuestType newquest)
  {
    MyGameData = new GameData();//새로운 게임 데이터 생성
    MyGameData.CurrentQuest= newquest;

    yield return StartCoroutine(createnewmap());//새 맵 만들기

    Settlement _randomsettle=MyGameData.MyMapData.AllSettles[Random.Range(0,MyGameData.MyMapData.AllSettles.Count)];
    MyGameData.CurrentSettlement= _randomsettle;
    MyGameData.Coordinate=_randomsettle.Tiles[Random.Range(0,_randomsettle.Tiles.Count)].Coordinate;

    yield return StartCoroutine(UIManager.Instance.opengamescene());
    UIManager.Instance.UpdateAllUI();
    UIManager.Instance.UpdateMap_SetPlayerPos(MyGameData.Coordinate);
    switch (MyGameData.CurrentQuest)
    {
      case QuestType.Wolf:UIManager.Instance.QuestUI_Wolf.OpenUI((QuestHolder_Wolf)MyGameData.CurrentQuestData); break;
    }
  }
  /// <summary>
  /// 저장된 데이터로 게임 시작
  /// </summary>
  public void LoadGame()
  {
    UIManager.Instance.AddUIQueue(loadgame());
  }
  private IEnumerator loadgame()
  {
    //게임 데이터는 이미 불러온 데이터 사용

    UIManager.Instance.CreateMap();
    UIManager.Instance.UpdateMap_SetPlayerPos();
    yield return StartCoroutine(UIManager.Instance.opengamescene());
    UIManager.Instance.UpdateAllUI();

    /*
    if (MyGameData.CurrentEvent == null)
    {
      UIManager.Instance.OpenSuggestUI();
    }
    else
    {
      if (MyGameData.CurrentEventSequence.Equals(EventSequence.Progress))
      {
        UIManager.Instance.OpenDialogue();
        //이벤트 있을 때, 진행 단계일 경우 이름, 일러스트, 설명, 선택지 세팅하고 이벤트 패널 열기
      }
      else
      {
        string _id = MyGameData.CurrentEvent.ID;
        SuccessData _success = null;
        if (MyGameData.SuccessEvent_None.Contains(_id)) _success = MyGameData.CurrentEvent.SuccessDatas[0];
        else if(MyGameData.SuccessEvent_Rational.Contains(_id))_success = MyGameData.CurrentEvent.SuccessDatas[0];
        else if(MyGameData.SuccessEvent_Mental.Contains(_id)) _success = MyGameData.CurrentEvent.SuccessDatas[0];
        else if(MyGameData.SuccessEvent_Physical.Contains(_id)) _success = MyGameData.CurrentEvent.SuccessDatas[1];
        else _success=MyGameData.CurrentEvent.SuccessDatas[1];
        if (_success != null) { UIManager.Instance.OpenSuccessDialogue(_success); yield break; }

        FailureData _fail = null;
        if (MyGameData.FailEvent_None.Contains(_id)) _fail = MyGameData.CurrentEvent.FailureDatas[0];
        else if (MyGameData.FailEvent_Rational.Contains(_id)) _fail = MyGameData.CurrentEvent.FailureDatas[0];
        else if (MyGameData.FailEvent_Mental.Contains(_id)) _fail = MyGameData.CurrentEvent.FailureDatas[0];
        else if (MyGameData.FailEvent_Physical.Contains(_id)) _fail = MyGameData.CurrentEvent.FailureDatas[1];
        else _fail = MyGameData.CurrentEvent.FailureDatas[1];
        if (_fail != null) { UIManager.Instance.OpenFailDialogue(_fail); yield break; }
        

        //이벤트 있을 때, 완료 단계일 경우 완료 리스트에서 현재 이벤트 찾고 완료 결과에 따라 설명, 보상 세팅 열고 이벤트 패널 열기
      }
    }
    */
    yield return null;
  }
  public void CreateNewMap() => StartCoroutine(createnewmap());

  private IEnumerator createnewmap()
  {
    maptext _map = FindObjectOfType<maptext>().GetComponent<maptext>();

    _map.MakePerfectMap();

    yield return new WaitUntil(()=>MyGameData.MyMapData != null);

    Settlement _startsettle = MyGameData.MyMapData.AllSettles[Random.Range(0, MyGameData.MyMapData.AllSettles.Count)];

    MyGameData.CurrentSettlement = _startsettle;
    MyGameData.Coordinate = _startsettle.Coordinate;

    _map.MakeTilemap();
    UIManager.Instance.UpdateMap_SetPlayerPos(_startsettle.Tiles[Random.Range(0,_startsettle.Tiles.Count)].Coordinate);
    yield return null;
  }
}
public class TextData
{
 public string ID = "", TEXT = "", ETC = "";
}
