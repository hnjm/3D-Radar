using UnityEngine;

public class Radar3DIcon : MonoBehaviour {

  #region Private Variables
  // 3D icon variables
  private SpriteRenderer vehicleIcon;
  private LineRenderer vehicleHeight;
  private Color color;
  private bool blinking = false;
  #endregion

  #region Private Methods
  // Start is called before the first frame update
  private void Start() {
    vehicleIcon = GetComponentInChildren<SpriteRenderer>();
    vehicleHeight = GetComponentInChildren<LineRenderer>();
    color = Color.grey;
  }

  // Simulate a blinking effect by enabling/disabling the sprite renderer
  private void Blink() {
    blinking = true;
    vehicleIcon.enabled = !vehicleIcon.enabled;
  }
  #endregion

  #region Public Methods
  // Update the 3D icon's position and color
  public void UpdatePosition(Vector3 updatedLocation, Color newColor, bool above) {
    color = newColor;

    // Determine whether this icon should be blinking or not
    if (color == Color.red && !blinking) {
      InvokeRepeating("Blink", 0f, 0.25f);
    } else if (color == Color.grey && blinking) {
      CancelInvoke();
      vehicleIcon.enabled = true;
      blinking = false;
    }

    // Adjust the sorting order of each icon based on its altitude difference to the ownship
    if (above) {
      vehicleIcon.sortingOrder = 2;
      vehicleHeight.sortingOrder = 2;
    } else {
      vehicleIcon.sortingOrder = 0;
      vehicleHeight.sortingOrder = 0;
    }

    // Update the icon's position on the 3D radar plane
    vehicleIcon.gameObject.transform.localPosition = updatedLocation;

    // Update the line renderer to show the tracked object's altitude is comparison to the 3D radar
    vehicleHeight.SetPosition(0, updatedLocation);
    vehicleHeight.SetPosition(1, updatedLocation + new Vector3(0f, -updatedLocation.y, 0f));

    // Set the icon and line renderer's color
    vehicleIcon.color = color;
    vehicleHeight.material.SetColor("_Color", color);
  }
  #endregion
}