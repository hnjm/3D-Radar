using UnityEngine;
using UnityEngine.UI;

public class Tracker : MonoBehaviour {

  #region Private Variables
  // 3D Icon Prefab
  [SerializeField] private GameObject icon3DPrefab;

  // Incremented id for all tracked objects
  private static int id = 0;

  // Unique identifer for this tracked object
  private int thisId;
  #endregion

  #region Properties
  // ID accessor
  public int ID { get { return thisId; } }
  #endregion

  #region Private Methods
  // Start is called before the first frame update
  private void Start() {
    if (RadarManager.Instance != null) {
      RadarManager.Instance.Register(this);
      InstantiateTrackedIcon();
    }
  }

  // Create a 3D icon for this tracked object
  private void InstantiateTrackedIcon() {
    Radar3D radar = Radar3D.Instance;
    if (radar != null) {
      GameObject newIcon3D = Instantiate(icon3DPrefab, radar.transform);
      gameObject.name = "Traffic Vehicle " + id.ToString();
      newIcon3D.name = "Traffic Vehicle Icon " + id.ToString();
      newIcon3D.GetComponentInChildren<Text>().text = id.ToString();
      radar.Add3DIcon(id, newIcon3D);
      thisId = id;
      id++;
    }
  }
  #endregion
}