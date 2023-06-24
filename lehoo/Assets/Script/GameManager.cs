using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }

  [HideInInspector] public GameData MyGameData = null;            //게임 데이터(진행도,현재 진행 중 이벤트, 현재 맵 상태,퀘스트 등등)
  [HideInInspector] public GameJsonData MyGameJsonData = null;
  [HideInInspector] public const string GameDataName = "WNCGameData.json";
  [HideInInspector] public MapData MyMapData = null;              //맵 데이터(맵 정보만)
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
  public Dictionary<string, TextData> TextDic = new Dictionary<string, TextData>();   //각종 텍스트 딕셔터리
  public TextData NullText = null;
  private FailureData goldfaildata = null;
  public FailureData GoldFailData
  {
    get
    {
      if (goldfaildata == null)
      {
        goldfaildata = new FailureData();
        goldfaildata.Description = GetTextData("goldfail").Name;
        goldfaildata.Panelty_target = PenaltyTarget.Status;
        goldfaildata.Loss_target = StatusType.Sanity;
        goldfaildata.Illust = ImageHolder.NoGoldIllust;

      }
      return goldfaildata;
    }
  }
  public TextData GetTextData(string _id)
  {
    // Debug.Log($"{_id} ID를 가진 텍스트 데이터 {(TextDic.ContainsKey(_id)?"있음":"없음")}");
    if (!TextDic.ContainsKey(_id)) { Debug.Log($"{_id} 없음?"); return NullText; }
    return TextDic[_id];
  }
  public TextData GetTextData(EnvironmentType envir)
  {
    switch (envir)
    {
      case EnvironmentType.NULL:
        return NullText;
      case EnvironmentType.River:
        return GetTextData("river");
      case EnvironmentType.Forest:
        return GetTextData("forest");
      case EnvironmentType.Highland:
        return GetTextData("highland");
      case EnvironmentType.Mountain:
        return GetTextData("mountain");
      default:
        return GetTextData("sea");
    }
  }
  public TextData GetTextData(SkillName _skill)
  {
    string _name = "";
    switch (_skill)
    {
      case SkillName.Speech: _name = "speech"; break;
      case SkillName.Threat: _name = "threat"; break;
      case SkillName.Deception: _name = "deception"; break;
      case SkillName.Logic: _name = "logic"; break;
      case SkillName.Kombat: _name = "kombat"; break;
      case SkillName.Bow: _name = "bow"; break;
      case SkillName.Somatology: _name = "somatology"; break;
      case SkillName.Survivable: _name = "survivable"; break;
      case SkillName.Biology: _name = "biology"; ; break;
      case SkillName.Knowledge: _name = "knowledge"; break;
    }
    return GetTextData(_name);
  }
  public TextData GetTextData(ThemeType _theme)
  {
    string _name = "";
    switch (_theme)
    {
      case ThemeType.Conversation: _name = "conversation"; break;
      case ThemeType.Force: _name = "force"; break;
      case ThemeType.Wild: _name = "wild"; break;
      case ThemeType.Intelligence: _name = "intelligence"; break;
    }
    return GetTextData(_name);
  }
  public TextData GetTextData(ThemeType theme,bool isup,bool isstrong)
  {
    string _name = "";
    switch (theme)
    {
      case ThemeType.Conversation: _name = "conversation"; break;
      case ThemeType.Force: _name = "force"; break;
      case ThemeType.Wild: _name = "wild"; break;
      case ThemeType.Intelligence: _name = "intelligence"; break;
    }
    _name += isstrong ? "double" : "";
    _name += isup ? "up" : "down";
    return GetTextData(_name);
  }
  public TextData GetTextData(EffectType _effect)
  {
    switch (_effect)
    {
      case EffectType.Speech: return GetTextData("speechup");
      case EffectType.Threat: return GetTextData("threatup");
      case EffectType.Deception: return GetTextData("deceptionup");
      case EffectType.Logic: return GetTextData("logicup");
      case EffectType.Kombat: return GetTextData("kombatup");
      case EffectType.Bow: return GetTextData("bowup");
      case EffectType.Somatology: return GetTextData("somatologyup");
      case EffectType.Survivable: return GetTextData("survivableup");
      case EffectType.Biology: return GetTextData("biologyup");
      case EffectType.Knowledge: return GetTextData("knowledgeup");
      case EffectType.HPLoss: return GetTextData(StatusType.HP, false);
      case EffectType.HPGen: return GetTextData(StatusType.HP, true);
      case EffectType.SanityLoss: return GetTextData(StatusType.Sanity, false);
      case EffectType.SanityGen: return GetTextData(StatusType.Sanity, true);
      case EffectType.GoldLoss: return GetTextData(StatusType.Gold, false);
      case EffectType.GoldGen: return GetTextData(StatusType.Gold, true);
      default: return NullText;
    }
  }
  public TextData GetTextData(TendencyType tendency,int level)
  {
    string _name = tendency.Equals(TendencyType.Body) ? "tendency_body_" : "tendency_head_";
    string _level = "";
    switch (level)
    {
      case -2:_level = "m2";break;
      case -1:_level = "m1";break;
      case 0:_level = "0";break;
      case 1:_level = "p1";break;
      case 2:_level = "p2";break;
    }
    return GetTextData(_name + _level);
  }
  public TextData GetTextData(PlaceType _place)
  {
    switch (_place)
    {
      case PlaceType.Residence: return GetTextData("residence");
      case PlaceType.Marketplace: return GetTextData("marketplace");
      case PlaceType.Temple: return GetTextData("temple");
      case PlaceType.Library: return GetTextData("library");
      case PlaceType.Theater: return GetTextData("theater");
      case PlaceType.Academy: return GetTextData("academy");
    }
    return NullText;
  }
  public TextData GetPlaceEffectTextData(PlaceType place)
  {
    switch (place)
    {
      case PlaceType.Residence:
        return GetTextData("residenceeffect");
      case PlaceType.Marketplace:
        return GetTextData("marketplaceeffect");
      case PlaceType.Temple:
        return GetTextData("templeeffect");
      case PlaceType.Library:
        return GetTextData("libraryeffect");
      case PlaceType.Theater:
        return GetTextData("theatereffect");
      default:
        return GetTextData("academyeffect");
    }
  }
  public TextData GetTextData(System.Type _eventtype)
  {
    if (_eventtype == typeof(EventData)) return GetTextData("normaleventpredescription");
    else if (_eventtype == typeof(FollowEventData)) return GetTextData("followeventpredescription");
    else return GetTextData("questeventpredescription");
  }
  public TextData GetTextData(StatusType type)
  {
    switch (type)
    {
      case StatusType.HP:return GetTextData("hp");
      case StatusType.Sanity:return GetTextData("sanity");
      default:return GetTextData("gold");
    }
  }
  public TextData GetTextData(StatusType type,bool isincrease)
  {
    switch (type)
    {
      case StatusType.HP: if (isincrease) return GetTextData("hpincrease"); else return GetTextData("hpdecrease");
      case StatusType.Sanity: if (isincrease) return GetTextData("sanityincrease"); else return GetTextData("sanitydecrease");
      default: if (isincrease) return GetTextData("goldincrease"); else return GetTextData("golddecrease");
    }
  }
  /// <summary>
  /// 레벨은 1/2
  /// </summary>
  /// <param name="type"></param>
  /// <param name="isup"></param>
  /// <param name="level"></param>
  /// <returns></returns>
  public TextData GetTextData(StatusType type,bool isincrease,bool isup, int value)
  {
    string _targetname = "";
    switch (type)
    {
      case StatusType.HP:
        _targetname = "hp";break;
      case StatusType.Sanity:
        _targetname = "sanity";break;
      case StatusType.Gold:
        _targetname = "gold";break;
    }
    _targetname += isincrease ? "increase" : "decrease";
    _targetname += Mathf.Abs(value)>ConstValues.DoubleValue ? "" : "double";
    _targetname += isup ? "up" : "down";
    return GetTextData(_targetname);
  }
  public TextData GetTextData(StatusType type, bool isincrease, bool isup, bool isstrong)
  {
    string _targetname = "";
    switch (type)
    {
      case StatusType.HP:
        _targetname = "hp"; break;
      case StatusType.Sanity:
        _targetname = "sanity"; break;
      case StatusType.Gold:
        _targetname = "gold"; break;
    }
    _targetname += isincrease ? "increase" : "decrease";
    _targetname += isstrong ? "" : "double";
    _targetname += isup ? "up" : "down";
    return GetTextData(_targetname);
  }
    public void LoadData()
  {
    if (File.Exists(Application.persistentDataPath+"/"+GameDataName ))
    {
      MyGameJsonData = JsonUtility.FromJson<GameJsonData>(File.ReadAllText(Application.persistentDataPath + "/" + GameDataName));
      MyGameData = MyGameJsonData.GetGameData();
      MyMapData = MyGameJsonData.GetMapData();
    }
    //저장된 플레이어 데이터가 있으면 데이터 불러오기

    Dictionary<string, TextData> _temp = JsonConvert.DeserializeObject<Dictionary<string, TextData>>(TextData.text);
    foreach (var _data in _temp)
    {
      TextData _texttemp = _data.Value;
     if(_texttemp.Name.Contains("\\n")) _texttemp.Name=_texttemp.Name.Replace("\\n", "\n");
      if (_texttemp.Description.Contains("\\n")) _texttemp.Description = _texttemp.Description.Replace("\\n", "\n");
      if (_texttemp.SelectionDescription.Contains("\\n")) _texttemp.SelectionDescription = _texttemp.SelectionDescription.Replace("\\n", "\n");
      if (_texttemp.SelectionSubDescription.Contains("\\n")) _texttemp.SelectionSubDescription = _texttemp.SelectionSubDescription.Replace("\\n", "\n");
      if (_texttemp.FailDescription.Contains("\\n")) _texttemp.FailDescription = _texttemp.FailDescription.Replace("\\n", "\n");
      if (_texttemp.SuccessDescription.Contains("\\n")) _texttemp.SuccessDescription = _texttemp.SuccessDescription.Replace("\\n", "\n");
      if (TextDic.ContainsKey(_data.Value.ID)) { Debug.Log($"{_data.Value.ID} 겹침! 확인 필요!"); return; }
      TextDic.Add(_data.Value.ID, _data.Value);
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

    Dictionary<string, ExperienceJsonData> _expjson = new Dictionary<string, ExperienceJsonData>();
    _expjson = JsonConvert.DeserializeObject<Dictionary<string, ExperienceJsonData>>(EXPData.text);
    foreach (var _data in _expjson)
    {
      Experience _exp = _data.Value.ReturnEXPClass();
      ExpDic.Add(_data.Value.ID, _exp);
      if(_exp.ExpType.Equals(ExpTypeEnum.Mad))MadExpDic.Add(_data.Value.ID, _exp);
    }
    //경험 Json -> EXPDic

   if(MyGameData!=null)EventHolder.LoadAllEvents();

  }//각종 Json 가져와서 변환
  public void SaveData()
  {

  }//현재 데이터 저장
  public void SuccessCurrentEvent(TendencyType _tendencytype,int index)
  {
   if(MyGameData.CurrentSettlement!=null)MyGameData.CurrentSettlement.SetAvailablePlaces();
    EventHolder.RemoveEvent(MyGameData.CurrentEvent.OriginID);
    switch (_tendencytype)
    {
      case TendencyType.None:
        MyGameData.SuccessEvent_None.Add(MyGameData.CurrentEvent.OriginID); break;
      case TendencyType.Body:
        if(index.Equals(0))
          MyGameData.SuccessEvent_Rational.Add(MyGameData.CurrentEvent.OriginID); 
        else
          MyGameData.SuccessEvent_Physical.Add(MyGameData.CurrentEvent.OriginID); 
        break;
      case TendencyType.Head:
        if(index.Equals(0))
        MyGameData.SuccessEvent_Mental.Add(MyGameData.CurrentEvent.OriginID);
        else
        MyGameData.SuccessEvent_Material.Add(MyGameData.CurrentEvent.OriginID); 
        break;
    }
    MyGameData.SuccessEvent_All.Add(MyGameData.CurrentEvent.OriginID);
    MyGameData.CurrentEventSequence = EventSequence.Clear;
    if (MyGameData.CurrentSettlement != null)
    {
      MyGameData.Turn++;
      UIManager.Instance.UpdateTurnIcon();
    }
  }
  public void FailCurrentEvent(TendencyType _tendencytype, int index)
  {
    if (MyGameData.CurrentSettlement != null) MyGameData.CurrentSettlement.SetAvailablePlaces();
    switch (_tendencytype)
    {
      case TendencyType.None:
        MyGameData.FailEvent_None.Add(MyGameData.CurrentEvent.OriginID); break;
      case TendencyType.Body:
        if (index.Equals(0))
          MyGameData.FailEvent_Rational.Add(MyGameData.CurrentEvent.OriginID);
        else
          MyGameData.FailEvent_Physical.Add(MyGameData.CurrentEvent.OriginID);
        break;
      case TendencyType.Head:
        if (index.Equals(0))
          MyGameData.FailEvent_Mental.Add(MyGameData.CurrentEvent.OriginID);
        else
          MyGameData.FailEvent_Material.Add(MyGameData.CurrentEvent.OriginID);
        break;
    }
    MyGameData.FailEvent_All.Add(MyGameData.CurrentEvent.OriginID);
    MyGameData.CurrentEventSequence = EventSequence.Clear;
    if (MyGameData.CurrentSettlement != null)
    {
      MyGameData.Turn++;
      UIManager.Instance.UpdateTurnIcon();
    }
  }
  public void AddBadExp(Experience badexp)
  {
    int _targetslot = 0;
    List<int> _emptylist = new List<int>();
    for (int i = 0; i < MyGameData.ShortTermEXP.Length; i++)
      if (MyGameData.ShortTermEXP[i] == null) _emptylist.Add(i);
    //단기 슬롯에서 빈 칸 가져오기
    if (_emptylist.Count > 0)
    {
      _targetslot=Random.Range(0,_emptylist.Count);
      MyGameData.ShortTermEXP[_targetslot] = badexp;
      UIManager.Instance.UpdateExpShortTermIcon();
      return;
    } //단기 슬롯 중 빈 칸이 있다면 무작위로 악경험 삽입하고 종료

    if (MyGameData.LongTermEXP==null)
    {
      MyGameData.LongTermEXP = badexp;
      UIManager.Instance.UpdateExpLongTermIcon();
      return;
    } //단기 슬롯에 빈 칸이 없다면 장기 슬롯 중 무작위로 악경험 삽입하고 종료

    if (Random.Range(0, 100) < 75)
    {
      _targetslot = Random.Range(0, 2);
      MyGameData.ShortTermEXP[_targetslot] = badexp;
      UIManager.Instance.UpdateExpShortTermIcon();
    } //장기,단기 둘 다 꽉 차있다면 75% 확률로 단기 경험 하나 대체
    else
    {
      MyGameData.LongTermEXP = badexp;
      UIManager.Instance.UpdateExpLongTermIcon();
    } //15% 확률로 장기 경험 하나 대체
  }
  public void AddShortExp(Experience _exp, int _index)
  {
    if (UIManager.Instance.MyQuestSuggent.IsActivePanel) UIManager.Instance.MyQuestSuggent.OpenStarting();
    //퀘스트 제시 패널에서 경험 저장하는 경우라면 다음 단계로 넘어가게

    if (_exp.ExpType.Equals(ExpTypeEnum.Mad)) MyGameData.MadnessCount++;
    _exp.Duration = ConstValues.ShortTermStartTurn;
    MyGameData.ShortTermEXP[_index] = _exp;
    UIManager.Instance.UpdateExpShortTermIcon();
  }
  public void AddLongExp(Experience _exp)
  {
    if (UIManager.Instance.MyQuestSuggent.IsActivePanel) UIManager.Instance.MyQuestSuggent.OpenStarting();
    //퀘스트 제시 패널에서 경험 저장하는 경우라면 다음 단계로 넘어가게

    if (_exp.ExpType.Equals(ExpTypeEnum.Mad)) MyGameData.MadnessCount++;
    _exp.Duration = ConstValues.LongTermStartTurn;
    MyGameData.LongTermEXP = _exp;
    MyGameData.CurrentSanity -= ConstValues.LongTermChangeCost;
    UIManager.Instance.UpdateSanityText();
    UIManager.Instance.UpdateExpLongTermIcon();
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
    if (_event.GetType().Equals(typeof(QuestEventData))) MyGameData.LastQuestCount = 0;
    MyGameData.CurrentEvent = _event;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    MyGameData.RemoveEvent.Add(_event.OriginID);
    //추후 등장하지 않게
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
    MyGameData.CurrentSanity -= MyGameData.SettleSanityLoss;
    UIManager.Instance.UpdateSanityText();
    Dictionary<Settlement,int> _temp=new Dictionary<Settlement,int>();
    MyGameData.AddDiscomfort(MyGameData.CurrentSettlement);
    MyGameData.CurrentEvent = _targetevent;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    MyGameData.RemoveEvent.Add(_targetevent.OriginID);
    //추후 등장하지 않게
    UIManager.Instance.OpenDialogue();
    SaveData();
  }//제시 패널에서 이벤트를 선택한 경우
  public void SelectQuestEvent(EventDataDefulat questevent)
  {
    MyGameData.LastQuestCount = 0;
    Dictionary<Settlement, int> _temp = new Dictionary<Settlement, int>();
    MyGameData.CurrentEvent = questevent;
    MyGameData.CurrentEventSequence = EventSequence.Progress;
    //현재 이벤트 데이터에 삽입
    MyGameData.RemoveEvent.Add(questevent.OriginID);
    //추후 등장하지 않게
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
        NullText = new TextData();
        NullText.Name = "NullName";
        NullText.Description = "NullDescription";
        NullText.SelectionDescription = "NullSelection@NullSelection";
        NullText.FailDescription = "NullFail";
        NullText.SuccessDescription = "NullSuccess";
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
      CreateNewMap();

    }
    if (Input.GetKeyDown(KeyCode.F1)) MyGameData.CurrentSanity = 3;
  }

  public void StartNewGame(QuestHolder newquest)
  {
    UIManager.Instance.AddUIQueue(startnewgame(newquest));
  }
  private IEnumerator startnewgame(QuestHolder newquest)
  {
    MyGameData = new GameData();//새로운 게임 데이터 생성
    MyGameData.CurrentQuest= newquest;
    EventHolder.SetAllEvents();

    yield return StartCoroutine(createnewmap());//새 맵 만들기

    yield return StartCoroutine(UIManager.Instance.opengamescene());
    UIManager.Instance.UpdateAllUI();

    UIManager.Instance.OpenQuestDialogue();
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
        string _id = MyGameData.CurrentEvent.OriginID;
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

    yield return null;
  }
  public void CreateNewMap()
  {
    StartCoroutine(createnewmap());
  }
  private IEnumerator createnewmap()
  {
    maptext _map = FindObjectOfType<maptext>().GetComponent<maptext>();

    _map.MakePerfectMap();

    yield return new WaitUntil(()=>MyGameJsonData != null);
    MyMapData = MyGameJsonData.GetMapData();

    Settlement _startsettle = MyMapData.AllSettles[Random.Range(0, MyMapData.AllSettles.Count)];

    MyGameData.CurrentSettlement = _startsettle;
    MyGameData.AvailableSettles = MyMapData.GetCloseSettles(_startsettle, 3);
    MyGameData.CurrentPos = _startsettle.VectorPos;

    _map.MakeTilemap(MyMapData);
    UIManager.Instance.UpdateMap_SetPlayerPos(_startsettle);
    yield return null;
  }
}
