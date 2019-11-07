using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar3D : MonoBehaviour {

  #region Private Variables
  // Cached radar manager instance
  private RadarManager radarManager;

  // The current max visible range ( outer ring distance ) of the radar
  private float range = 1609f;

  // Constant range variables
  private readonly float maxRange = 8045f;
  private readonly float quarterMile = 402.25f;
  private readonly float oneMile = 1609f;

  // Cached color for the 3D icon and its line renderer
  private Color color;

  // The tracked position in relation to the radar position
  private Vector3 trackedRelativePosition;

  // The vector3 used to updated the 3D icon's position
  private Vector3 updatedIconPosition;

  // Ring distance markers
  [SerializeField] private Text firstRing;
  [SerializeField] private Text secondRing;
  [SerializeField] private Text thirdRing;
  [SerializeField] private Text fourthRing;

  // Cached reference of the list of tracked objects
  private List<Tracker> trackedObjects = new List<Tracker>();

  // Dictionary containing 3D radar icons and their corresponding ids ( from the tracked object )
  private Dictionary<int, GameObject> radarIcons = new Dictionary<int, GameObject>();

  // Singleton instance
  private static Radar3D instance;
  #endregion

  #region Properties
  // Singleton accessor
  public static Radar3D Instance { get { return instance; } }
  #endregion

  #region Private Methods
  // Singleton pattern
  private void Awake() {
    if (instance != null) {
      Destroy(gameObject);
    } else {
      instance = this;
    }
  }

  // Decrease the current zoom level
  private void DecreaseZoom() {
    if (range >= maxRange) {
      return;
    }
    range += 10.25f;
    UpdateText();
  }

  // Increase the current zoom level
  private void IncreaseZoom() {
    if (range <= quarterMile) {
      return;
    }
    range -= 10.25f;
    UpdateText();
  }

  // Update the text marking the distance of the rings ( limit to 2 decimal places )
  private void UpdateText() {
    firstRing.text = ((range / 4f) / oneMile).ToString("F2");
    secondRing.text = (((range / 4f) * 2) / oneMile).ToString("F2");
    thirdRing.text = (((range / 4f) * 3) / oneMile).ToString("F2");
    fourthRing.text = ((range / oneMile).ToString("F2"));
  }

  // Update is called once per frame
  private void Update() {
    // Decrease the current zoom level ( I )
    if (Input.GetKey(KeyCode.I)) {
      IncreaseZoom();
    }

    // Increase the current zoom level ( O )
    if (Input.GetKey(KeyCode.O)) {
      DecreaseZoom();
    }

    // Obtain all tracked objects in the scene
    if (radarManager != null) {
      trackedObjects = radarManager.GetTrackedObjects();

      // Update the 3D icon for each tracked object
      for (int i = 0; i < trackedObjects.Count; i++) {
        Set3DIcon(trackedObjects[i]);
      }
    }
  }

  // Start is called before the first frame update
  private void Start() {
    radarManager = RadarManager.Instance;
  }

  // Determine if a point lies within a circle with a radius of 1
  private bool IsPointInCircle(float x, float y) {
    return (Mathf.Sqrt((x * x) + (y * y)) <= 1);
  }

  // Set the position and color of a tracked object's 3D icon. The icon will be disabled visually if it is outside of a given range
  private void Set3DIcon(Tracker tracked) {
    if (tracked != null) {
      // Attempt to retreive the icon object for the specified id
      radarIcons.TryGetValue(tracked.ID, out GameObject icon);

      // Only perform calculation if a valid icon object exists for the specified id
      if (icon != null) {
        // Convert the tracked object's transform from world space to local space
        trackedRelativePosition = tracked.gameObject.transform.InverseTransformPoint(transform.position);

        // Obtain the distance from the ownship to the tracked object
        float distanceToTarget = trackedRelativePosition.magnitude;

        // Calculate the factor to multiply the positon vector by
        float factor = -(distanceToTarget / range);

        // Set a vector 3 of values ranging from -1 to 1 based on the current viable range and position of the tracked object
        updatedIconPosition = trackedRelativePosition.normalized * (factor);

        // Only update the icon if its current location is within a given radius otherwise deactivate the icon
        if (IsPointInCircle(Mathf.Abs(updatedIconPosition.x), Mathf.Abs(updatedIconPosition.z))) {
          if (!icon.activeSelf) {
            icon.SetActive(true);
          }

          // Signal a warning if the tracked object is within a quarter mile of the ownship
          if (distanceToTarget <= quarterMile) {
            color = Color.red;
          } else {
            color = Color.grey;
          }

          // Update the y position separately
          updatedIconPosition.y = tracked.gameObject.transform.position.y / oneMile;

          // Update the position and color of the 3D icon
          icon.GetComponent<Radar3DIcon>().UpdatePosition(updatedIconPosition, color, (updatedIconPosition.y > 0));
        } else {
          icon.SetActive(false);
        }
      }
    }
  }
  #endregion

  #region Public Methods
  // Add a 3D icon to the dictionary of 3D icons
  public void Add3DIcon(int id, GameObject icon) {
    radarIcons.Add(id, icon);
  }
  #endregion
}