using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileButtonScript : MonoBehaviour
{
  public Button Button = null;
  public UI_map MapUI = null;
  public TileData TileData = null;
  public Image BottomImage = null;
  public Image TopImage = null;
  public Image LandmarkImage = null;
  public void Clicked()
  {
    if (UIManager.Instance.IsWorking) return;
    if (TileData.Coordinate == GameManager.Instance.MyGameData.Coordinate) return;

    MapUI.SelectTile(TileData);
  }

}
