using System.Collections.Generic;
using UnityEngine;

public class RadarManager : MonoBehaviour {

  #region Private Variables
  // Singleton instance
  private static RadarManager instance;

  // List of tracked objects
  private List<Tracker> trackers = new List<Tracker>();
  #endregion

  #region Properties
  // Singleton accessor
  public static RadarManager Instance { get { return instance; } }
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
  #endregion

  #region Public Methods
  // Return the list of tracked objects
  public List<Tracker> GetTrackedObjects() {
    return trackers;
  }

  // Add a new tracked object to the list of tracked objects
  public void Register(Tracker newTracker) {
    trackers.Add(newTracker);
  }

  // Remove a tracked object from the list of tracked objects
  public void DeRegister(Tracker trackerToRemove) {
    trackers.Remove(trackerToRemove);
  }
  #endregion
}