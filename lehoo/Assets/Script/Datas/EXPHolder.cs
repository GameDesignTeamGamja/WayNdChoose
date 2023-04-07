using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EXPType {Conversation,Forece,Survive,Intelligence
    , Speech, Threat, Deception, logic, Martialarts, Bow, Somatology, Survivable, Biology, Knowledge,
  HPLoss, HPRegen,
  SNLoss, SNGen,
  MoneyLoss, MoneyGen }

public class Experience
{
  public string ID = "";
  public bool GoodExp = false;
  public string Name = "";
  public string Description = "";
  public EXPType Type;
  public int Info;
  public EXPAcquireData AcquireData=null;
}
public class EXPAcquireData
{
    public int Duration = 0;//남은 턴
    public int Year = 0;    //획득 년도
    public int Season = 0;  //획득 턴(계절)
    public string Place = "";//어디서 얻었는지
  public string EventID = "";//무슨 이벤트에서 얻었는지
}
public class ExperienceJsonData
{
  public string ID = "";
  public int GoodOrBad;
  public string Name = "";
  public string Description = "";
  public int Type;    //0~9 : 기술들  10~   체력,정신력,돈 등
  public int Info;
  public Experience ReturnEXPClass()
  {
    Experience _exp = new Experience();
    _exp.ID= ID;
    _exp.GoodExp = GoodOrBad == 0 ? false : true;
    _exp.Name = Name;
    _exp.Description = Description;
    _exp.Type = (EXPType)Type;
    _exp.Info = Info;
    return _exp;
  }
}

