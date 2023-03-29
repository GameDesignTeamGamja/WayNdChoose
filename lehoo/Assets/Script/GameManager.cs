using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }

  [HideInInspector] public CharacterData MyCharacterData = null;  //캐릭터 데이터(체력,정신력,돈,스킬레벨,특성,경험,성향)
  private const string CharacterDataName = "CharacterData.json";
  [HideInInspector] public GameData MyGameData = null;            //게임 데이터(진행도,현재 진행 중 이벤트, 현재 맵 상태,퀘스트 등등)
  private const string GameDataName = "GameData.json";
  [HideInInspector] public MapData MyMapData = null;              //맵 데이터(맵 정보만)
  private MapSaveData MyMapSaveData = null;
  private const string MapDataName = "MapData.json";

  [SerializeField] private ImageHolder ImageHolder = null;             //이벤트,경험,특성,정착지 일러스트 홀더

  [SerializeField] private TextAsset EventData = null;  //이벤트 Json
  [SerializeField] private TextAsset EXPData = null;    //경험 Json
  [SerializeField] private TextAsset TraitData = null;  //특성 Json
  public EventHolder EventHolder = new EventHolder();                               //이벤트 저장할 홀더
  public Dictionary<string, Experience> ExpDic = new Dictionary<string, Experience>();  //경험 딕셔너리
  public Dictionary<string, Trait> TraitsDic = new Dictionary<string, Trait>();         //특성 딕셔너리

  public void LoadData()
  {
    Dictionary<string, EventJsonData> _eventjson = new Dictionary<string, EventJsonData>();
    _eventjson = JsonConvert.DeserializeObject<Dictionary<string, EventJsonData>>(EventData.text);
    foreach (var _data in _eventjson) EventHolder.AddData(_data.Key, _data.Value);
    //이벤트 Json -> EventHolder

    Dictionary<string,ExperienceJsonData> _expjson = new Dictionary<string,ExperienceJsonData>();
    _expjson = JsonConvert.DeserializeObject<Dictionary<string, ExperienceJsonData>>(EXPData.text);
    foreach(var _data in _expjson) 
    { 
      Experience _exp = _data.Value.ReturnEXPClass();
      ExpDic.Add(_data.Key, _exp);
    }
    //경험 Json -> EXPDic

    Dictionary<string,TraitJsonData> _traitjson = new Dictionary<string,TraitJsonData>();
    _traitjson = JsonConvert.DeserializeObject<Dictionary<string,TraitJsonData>>(TraitData.text);
    foreach(var _data in _traitjson)
    {
      Trait _trait=_data.Value.ReturnTraitClass();
      TraitsDic.Add(_data.Key, _trait);
    }
    //특성 Json -> TraitDic
    string _datapath = Application.persistentDataPath + CharacterDataName;

    //일단 데이터 불러오기는 나중에 만들것
    MyCharacterData = new CharacterData();
    MyGameData = new GameData();


  }//각종 Json 가져와서 변환

  private void Awake()
  {
    if(instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
      LoadData();
    }
    else Destroy(gameObject);

  }
  public void LoadGameScene()
  {
    StartCoroutine(loadscene("GameScene"));
  }
  private IEnumerator loadscene(string scenename)
  {
    AsyncOperation _oper = SceneManager.LoadSceneAsync(scenename);
    _oper.allowSceneActivation = true;

    yield return new WaitUntil(()=> _oper.isDone==true);
    CreateNewMap();
    yield return null;
  }
  public void CreateNewMap()
  {
    maptext _map = FindObjectOfType<maptext>().GetComponent<maptext>();

    MyMapSaveData = _map.MakeMap();
    MyMapData = MyMapSaveData.ConvertToMapData();

    Settlement _startsettle = MyMapData.AllSettles[Random.Range(0, MyMapData.AllSettles.Count)];

    MyGameData.CurrentSettle = _startsettle;
    MyGameData.PlayerPos = _startsettle.VectorPos();

    MyGameData.AvailableSettle = MyMapData.GetCloseSettles(_startsettle, 3);
    foreach (Settlement _settle in MyGameData.AvailableSettle) _settle.IsOpen = true;

    _map.MakeTilemap(MyMapSaveData,MyMapData);


    UIManager.Instance.UpdateMap_PlayerPos(_startsettle);
    UIManager.Instance.SetStartDialogue();
  }
}
