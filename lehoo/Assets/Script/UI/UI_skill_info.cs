using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_skill_info : UI_default//스크립트 이름은 Skill인데 Theme 표시하는 기능임
{
  [SerializeField] private Image TouchBlock = null;
  [SerializeField] private TextMeshProUGUI SkillName;
  [SerializeField] private Image SkillIllust = null;
  [SerializeField] private TextMeshProUGUI SkillDescription = null;
  [SerializeField] private TextMeshProUGUI SkillLevel = null;
  [SerializeField] private Image SkillIcon = null;
  [SerializeField] private CanvasGroup BackButton = null;

  private int CurrentThemeIndex = -1;
  public void OpenUI(int _index)
  {
    if (UIManager.Instance.IsWorking) return;
    if (IsOpen && CurrentThemeIndex.Equals(_index)) { CloseUI(); return; }
    DefaultGroup.alpha = 1.0f;
    DefaultGroup.interactable = true;
    DefaultGroup.blocksRaycasts = true;
    BackButton.interactable = true;
    BackButton.blocksRaycasts = true;

    IsOpen = true;
    TouchBlock.enabled = true;
    SkillType _skilltype = (SkillType)_index;
    Sprite _icon = GameManager.Instance.ImageHolder.GetSkillIcon(_skilltype);
    Sprite _illust = GameManager.Instance.ImageHolder.GetSkillIllust(_skilltype);
    string _name = GameManager.Instance.GetTextData(_skilltype, 0), _desscription = GameManager.Instance.GetTextData(_skilltype, 3);

    SkillIcon.sprite = _icon;
    SkillName.text = _name;
    SkillLevel.text = GameManager.Instance.MyGameData.GetSkill(_skilltype).Level.ToString();
    SkillIllust.sprite= _illust;
    SkillDescription.text = _desscription;

    if (CurrentThemeIndex.Equals(-1))
    {
      UIManager.Instance.AddUIQueue(UIManager.Instance.OpenUI(GetPanelRect("myrect").Rect, GetPanelRect("myrect").OutisdePos, GetPanelRect("myrect").InsidePos, UIManager.Instance.LargePanelMoveTime,true));
    }//닫혀 있던 상태에서 처음으로 열었을때면 UI 열기 이펙트
    else
    {
    }//열린 상태에서 다른 테마 아이콘 클릭한거면 내용물만 바꾸기
    CurrentThemeIndex = _index;
  }
  public override void CloseUI()
  {
    UIManager.Instance.CurrentTopUI = null;
    IsOpen = false;
    BackButton.interactable = false;
    BackButton.blocksRaycasts = false;
    TouchBlock.enabled = false;
    StartCoroutine(UIManager.Instance.ChangeAlpha(DefaultGroup, 0.0f, 0.1f, false));
    StartCoroutine(UIManager.Instance.CloseUI(GetPanelRect("myrect").Rect, GetPanelRect("myrect").InsidePos, GetPanelRect("myrect").OutisdePos, UIManager.Instance.LargePanelMoveTime, false));
    UIManager.Instance.CurrentTopUI = null;
    CurrentThemeIndex = -1;
  }
}
