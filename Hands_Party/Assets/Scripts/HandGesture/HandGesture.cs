using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity.HandTracking;

public class HandGesture : MonoBehaviour
{
  public HandTrackingGraph handtracking;

  public List<LandmarkList> landmarks = new List<LandmarkList>();
  public List<ClassificationList> handness = new List<ClassificationList>();

  public List<Vector3> LeftHandWorldLandMarks = new List<Vector3>();
  public List<Vector3> RightHandWorldLandMarks = new List<Vector3>();

  public float NormalizedDis;

  public enum Gesture
  {
    Release,
    Grab,
    IndexUp,
    PinkyUp,
    IndexAndPinkyUp,
  }

  public Gesture LeftHandGesture;
  public Gesture RightHandGesture;

  enum Hands
  {
    Left,
    Right
  }

  float GrabThreshold = 0.04f;

  void Start()
  {
    handtracking.OnHandWorldLandmarksOutput.AddListener(GetHandWorldLandMarks);
    handtracking.OnHandednessOutput.AddListener(GetHandedness);

    GrabThreshold = 0.05f;
  }

  
  void Update()
  {
    if (handness.Count <= 0)
    {
      return;
    }

    if (landmarks.Count > 0)
    {
      for (int x = 0; x < landmarks.Count; x++)
      {
        if (handness[x].Classification[0].Label == "Left")
        {
          GetLandMarks(landmarks[x], Hands.Left);
        }
        else if (handness[x].Classification[0].Label == "Right")
        {
          GetLandMarks(landmarks[x], Hands.Right);
        }
      }
    }

    //NormalizedDis = Mathf.Clamp01((distance - MinDis) / (MaxDis - MinDis));

    LeftHandGesture = GestureRecognition(LeftHandWorldLandMarks);
    RightHandGesture = GestureRecognition(RightHandWorldLandMarks);
  }

  void GetLandMarks(LandmarkList landmarkList, Hands hands)
  {
    for (int x = 0; x < landmarkList.Landmark.Count; x++)
    {
      Vector3 coor = new Vector3(landmarkList.Landmark[x].X, landmarkList.Landmark[x].Y, landmarkList.Landmark[x].Z);
      if (hands == Hands.Left)
      {
        if (LeftHandWorldLandMarks.Count <= x)
        {
          LeftHandWorldLandMarks.Add(coor);
        }
        else
        {
          LeftHandWorldLandMarks[x] = coor;
        }
      }
      else
      {
        if (RightHandWorldLandMarks.Count <= x)
        {
          RightHandWorldLandMarks.Add(coor);
        }
        else
        {
          RightHandWorldLandMarks[x] = coor;
        }
      }
    }
  }

  Gesture GestureRecognition(List<Vector3> landMarkList)
  {
    if (landMarkList.Count < 21) return Gesture.Release;
    Gesture ges = Gesture.Release;
    /*
    Debug.Log("8 5 index : " + Vector3.Distance(landMarkList[8], landMarkList[5]));
    Debug.Log("12 9 middle : " + Vector3.Distance(landMarkList[12], landMarkList[9]));
    Debug.Log("16 13 ring : " + Vector3.Distance(landMarkList[16], landMarkList[13]));
    Debug.Log("20 17 pinky : " + Vector3.Distance(landMarkList[20], landMarkList[17]));
    */
    if (Vector3.Distance(landMarkList[8], landMarkList[5]) < GrabThreshold &&
        Vector3.Distance(landMarkList[12], landMarkList[9]) < GrabThreshold &&
        Vector3.Distance(landMarkList[16], landMarkList[13]) < GrabThreshold &&
        Vector3.Distance(landMarkList[20], landMarkList[17]) > GrabThreshold)
    {
      ges = Gesture.PinkyUp;
    }

    if (Vector3.Distance(landMarkList[8], landMarkList[5]) > GrabThreshold &&
        Vector3.Distance(landMarkList[12], landMarkList[9]) < GrabThreshold &&
        Vector3.Distance(landMarkList[16], landMarkList[13]) < GrabThreshold  &&
        Vector3.Distance(landMarkList[20], landMarkList[17]) < GrabThreshold)
    {
      ges = Gesture.IndexUp;
    }

    if (Vector3.Distance(landMarkList[8], landMarkList[5]) > GrabThreshold &&
        Vector3.Distance(landMarkList[12], landMarkList[9]) < GrabThreshold &&
        Vector3.Distance(landMarkList[16], landMarkList[13]) < GrabThreshold &&
        Vector3.Distance(landMarkList[20], landMarkList[17]) > GrabThreshold)
    {
      ges = Gesture.IndexAndPinkyUp;
    }

    if (Vector3.Distance(landMarkList[8], landMarkList[5]) < GrabThreshold    &&
        Vector3.Distance(landMarkList[12], landMarkList[9]) < GrabThreshold   &&
        Vector3.Distance(landMarkList[16], landMarkList[13]) < GrabThreshold  &&
        Vector3.Distance(landMarkList[20], landMarkList[17]) < GrabThreshold)
    {
      ges = Gesture.Grab;
    }
    return ges;
  }


  public void GetHandWorldLandMarks(List<LandmarkList> landmarkLists)
  {
    if (landmarkLists == null)
    {
      return;
    }
    landmarks = landmarkLists;
  }

  public void GetHandedness(List<ClassificationList> handednessList)
  {
    if (handednessList == null)
    {
      return;
    }
    handness = handednessList;
  }

}
