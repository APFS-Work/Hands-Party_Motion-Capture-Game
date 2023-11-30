musing Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLandMarksChecking : MonoBehaviour
{
  public MultiHandLandmarkListAnnotationController handAnnoController;
  #region Hand Land Marks
  public HandGesture handGesture;


  public List<Vector3> LeftHandLandMarks = new List<Vector3>();
  public List<Vector3> RightHandLandMarks = new List<Vector3>();

  public List<GameObject> LeftHandLandMarkBalls = new List<GameObject>();
  public List<GameObject> RightHandLandMarkBalls = new List<GameObject>();

  public float ZAmplifier = 100.0f;
  public float DistanceScaler = 0.7f;

  public bool LeftHandFacingCam = false;
  public bool RightHandFacingCam = false;
  #endregion

  #region IK for hand model
  public bool UpdateDistance = false;
  public List<float> HandDis = new List<float>();

  public List<float> LeftHandDis = new List<float>();
  public List<Vector3> LeftHandRoatate = new List<Vector3>();
  public List<Vector3> LeftHandIKTarget = new List<Vector3>();
  public List<GameObject> LeftHandIKs = new List<GameObject>();

  public List<float> RightHandDis = new List<float>();
  public List<Vector3> RightHandRoatate = new List<Vector3>();
  public List<Vector3> RightHandIKTarget = new List<Vector3>();
  public List<GameObject> RightHandIKs = new List<GameObject>();

  public Camera ScreenCam;

  public bool DebugMode;

  public float MinDis, MaxDis;
  public float NormalizedDistance;
  public float HandLength;

  public float Distance;
  #endregion

  bool LeftHandFrontFacing, RightHandFrontFacing;

  List<GameObject> LeftHandCubes = new List<GameObject>();
  List<GameObject> RightHandCubes = new List<GameObject>();

  GameObject LeftHandRenderCubes;
  GameObject RightHandRenderCubes;

  // Start is called before the first frame update
  void Start()
  {
    LeftHandRenderCubes = new GameObject();
    RightHandRenderCubes = new GameObject();
    LeftHandRenderCubes.name = "LeftHandCubes";
    RightHandRenderCubes.name = "RightHandCubes";

    LeftHandIKTarget.Add(Vector3.zero);
    RightHandIKTarget.Add(Vector3.zero);
    for (int x = 0; x < 10; x++)
    {
      HandDis.Add(0);

      LeftHandDis.Add(0);
      LeftHandRoatate.Add(Vector3.zero);
      LeftHandIKTarget.Add(Vector3.zero);

      RightHandDis.Add(0);
      RightHandRoatate.Add(Vector3.zero);
      RightHandIKTarget.Add(Vector3.zero);
    }

    ZAmplifier = 100.0f;

    DistanceScaler = 0.6f;
    //0: 2.580673 1: 1.660503 2: 2.638373 3: 2.286866 4: 2.800474 5: 2.530092 6: 2.957748 7: 2.359103 8: 3.082688 9: 2.047228
    #region SetIKDistance
    if (UpdateDistance == false)
    {
      HandDis[0] = 2.580673f * DistanceScaler;
      HandDis[1] = 1.660503f * DistanceScaler;
      HandDis[2] = 2.638373f * DistanceScaler;
      HandDis[3] = 2.286866f * DistanceScaler;
      HandDis[4] = 2.800474f * DistanceScaler;
      HandDis[5] = 2.530092f * DistanceScaler;
      HandDis[6] = 2.957748f * DistanceScaler;
      HandDis[7] = 2.359103f * DistanceScaler;
      HandDis[8] = 3.082688f * DistanceScaler;
      HandDis[9] = 2.047228f * DistanceScaler;
    }
    #endregion

    //Set Min and Max Distance
    //Mark Distance is : 0.340671
    //Mark Distance is : 0.1040879
    MaxDis = 0.340671f;
    MinDis = 0.1040879f;
    MaxDis = 0.4f;
    MinDis = 0.12f;

  }

  // Update is called once per frame
  private void Update()
  {
    if (TextureSource.connected == false)
    { 
      return;
    } 
    #region TestCode
    /*
    if (handAnnoController.multi_hand_landmarks != null)
    {         
      //Debug.LogWarning(handAnnoController.multi_hand_landmarks[0].Landmark.Count);
      for (int x = 0; x < 21; x++)
      {
        Vector3 v3 = new Vector3(1.0f - handAnnoController.multi_hand_landmarks[0].Landmark[x].X, 1.0f - handAnnoController.multi_hand_landmarks[0].Landmark[x].Y, -3 * handAnnoController.multi_hand_landmarks[0].Landmark[x].Z);
        
        if (HandOneLandMarks.Count < 21)
        {
          HandOneLandMarks.Add(v3);
        }
        else
        {
          HandOneLandMarks[x] = v3;
        }
        
      }
    }
    */
    #endregion
    if (handAnnoController.Handeness != null)
    {
      //Debug.LogWarning(handAnnoController.Handeness[0].Classification[0].Label);
      for (int x = 0; x < handAnnoController.Handeness.Count; x++)
      {
        if (handAnnoController.Handeness[x].Classification[0].Label == "Left")
        {
          UpdateLandMarks(Hand.Left, x);
        }
        else
        {
          UpdateLandMarks(Hand.Right, x);
        }
      }

      GetDis();
      GetDir();

      DetermineFacing();

      UpdateIKTarget();
      MoveIKTarget();
    }
  }
  //Update Hand Land Marks Vector3 List
  void UpdateLandMarks(Hand hand,int index)
  {
    Vector3 wristMark = new Vector3(handAnnoController.multi_hand_landmarks[index].Landmark[0].X, handAnnoController.multi_hand_landmarks[index].Landmark[0].Y, 0);
    Vector3 middleMark = new Vector3(handAnnoController.multi_hand_landmarks[index].Landmark[9].X, handAnnoController.multi_hand_landmarks[index].Landmark[9].Y, 0);
    float dis = Vector3.Distance(wristMark, middleMark);
    Distance = dis;
    NormalizedDistance = Mathf.Clamp01((dis - MinDis) / (MaxDis - MinDis));
    NormalizedDistance = (float)((int)(NormalizedDistance * 100)) / 100;
    /*
    //Debug.LogWarning(handAnnoController.multi_hand_landmarks[0].Landmark.Count);

    Debug.Log(handAnnoController.multi_hand_landmarks[index].Landmark[0].Z);

    wristMark.z *= UnityEngine.Screen.width * ZAmplifier;
    */
    for (int x = 0; x < 21; x++)
    {
      //Vector3 v3 = new Vector3(handAnnoController.multi_hand_landmarks[index].Landmark[x].X, 1.0f - handAnnoController.multi_hand_landmarks[index].Landmark[x].Y, -handAnnoController.multi_hand_landmarks[index].Landmark[x].Z);
      Vector3 v3 = new Vector3(handAnnoController.multi_hand_landmarks[index].Landmark[x].X, 1.0f - handAnnoController.multi_hand_landmarks[index].Landmark[x].Y, 0);
      if (hand == Hand.Left)
      {
        v3.z = handGesture.LeftHandWorldLandMarks[index].z;
      }
      else
      {
        v3.z = handGesture.RightHandWorldLandMarks[index].z;
      }

      /*
      if (x != 0)
      {    
        v3.z += wristMark.z;
      }
      else
      {
        v3.z = wristMark.z;
      }
      */

      //v3.z *= 100f;

      Vector3 v = new Vector3(v3.x * UnityEngine.Screen.width, v3.y * UnityEngine.Screen.height, v3.z);

      v.z = 0.0f;
      //.5f
      v += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * NormalizedDistance);
      Vector3 sv = ScreenCam.ScreenToWorldPoint(v);
      
      if (hand == Hand.Left)
      {
        if (LeftHandLandMarks.Count < 21)
        {
          //LeftHandLandMarks.Add(v);
          LeftHandLandMarks.Add(sv);
        }
        else
        {
          //LeftHandLandMarks[x] = v;
          LeftHandLandMarks[x] = sv;
        }
      }

      if (hand == Hand.Right)
      {
        if (RightHandLandMarks.Count < 21)
        {
          //RightHandLandMarks.Add(v);
          RightHandLandMarks.Add(sv);
        }
        else
        {
          //RightHandLandMarks[x] = v;
          RightHandLandMarks[x] = sv;
        }
      }
    }

  }

  void GetDis()
  {

    if (UpdateDistance == false)
    { 
      return;
    }
    //Thumb
    HandDis[0] = Vector3.Distance(LeftHandLandMarks[0], LeftHandLandMarks[2]);
    HandDis[1] = Vector3.Distance(LeftHandLandMarks[2], LeftHandLandMarks[4]);
    //Index
    HandDis[2] = Vector3.Distance(LeftHandLandMarks[0], LeftHandLandMarks[5]);
    HandDis[3] = Vector3.Distance(LeftHandLandMarks[8], LeftHandLandMarks[5]);
    //Middle
    HandDis[4] = Vector3.Distance(LeftHandLandMarks[0], LeftHandLandMarks[9]);
    HandDis[5] = Vector3.Distance(LeftHandLandMarks[9], LeftHandLandMarks[12]);
    //Ring
    HandDis[6] = Vector3.Distance(LeftHandLandMarks[0], LeftHandLandMarks[13]);
    HandDis[7] = Vector3.Distance(LeftHandLandMarks[13], LeftHandLandMarks[16]);
    //Pinky
    HandDis[8] = Vector3.Distance(LeftHandLandMarks[0], LeftHandLandMarks[17]);
    HandDis[9] = Vector3.Distance(LeftHandLandMarks[17], LeftHandLandMarks[20]);
    if (Input.GetKeyDown(KeyCode.Space))
    {
      Debug.LogWarning(" 0: " + HandDis[0] +
        " 1: " + HandDis[1] +
        " 2: " + HandDis[2] +
        " 3: " + HandDis[3] +
        " 4: " + HandDis[4] +
        " 5: " + HandDis[5] +
        " 6: " + HandDis[6] +
        " 7: " + HandDis[7] +
        " 8: " + HandDis[8] +
        " 9: " + HandDis[9] );
    }
  }

  void GetDir()
  {
    #region LeftHand
    if (LeftHandLandMarks.Count > 0)
    { 
      //Thumb
      LeftHandRoatate[0] = LeftHandLandMarks[2] - LeftHandLandMarks[0];
      if (Vector3.Distance(LeftHandLandMarks[2], LeftHandLandMarks[0]) > 0)
      { 
        LeftHandDis[0] = HandDis[0] / Vector3.Distance(LeftHandLandMarks[2], LeftHandLandMarks[0]);
      }

      LeftHandRoatate[1] = LeftHandLandMarks[4] - LeftHandLandMarks[2];
      if (Vector3.Distance(LeftHandLandMarks[4], LeftHandLandMarks[2]) > 0)
      {
        LeftHandDis[1] = HandDis[1] / Vector3.Distance(LeftHandLandMarks[4], LeftHandLandMarks[2]);
      }
      //Index
      LeftHandRoatate[2] = LeftHandLandMarks[5] - LeftHandLandMarks[0];
      if (Vector3.Distance(LeftHandLandMarks[5], LeftHandLandMarks[0]) > 0)
      { 
        LeftHandDis[2] = HandDis[2] / Vector3.Distance(LeftHandLandMarks[5], LeftHandLandMarks[0]);
      }

      LeftHandRoatate[3] = LeftHandLandMarks[8] - LeftHandLandMarks[5];
      if (Vector3.Distance(LeftHandLandMarks[8], LeftHandLandMarks[5]) > 0)
      { 
        LeftHandDis[3] = HandDis[3] / Vector3.Distance(LeftHandLandMarks[8], LeftHandLandMarks[5]);
      }
      //Middle
      LeftHandRoatate[4] = LeftHandLandMarks[9] - LeftHandLandMarks[0];
      if (Vector3.Distance(LeftHandLandMarks[9], LeftHandLandMarks[0]) > 0)
      {
        LeftHandDis[4] = HandDis[4] / Vector3.Distance(LeftHandLandMarks[9], LeftHandLandMarks[0]);
      }
      LeftHandRoatate[5] = LeftHandLandMarks[12] - LeftHandLandMarks[9];
      if (Vector3.Distance(LeftHandLandMarks[12], LeftHandLandMarks[9]) > 0)
      { 
        LeftHandDis[5] = HandDis[5] / Vector3.Distance(LeftHandLandMarks[12], LeftHandLandMarks[9]);
      }
      //Ring
      LeftHandRoatate[6] = LeftHandLandMarks[13] - LeftHandLandMarks[0];
      if (Vector3.Distance(LeftHandLandMarks[13], LeftHandLandMarks[0]) > 0)
      {
        LeftHandDis[6] = HandDis[6] / Vector3.Distance(LeftHandLandMarks[13], LeftHandLandMarks[0]);
      }
      LeftHandRoatate[7] = LeftHandLandMarks[16] - LeftHandLandMarks[13];
      if (Vector3.Distance(LeftHandLandMarks[16], LeftHandLandMarks[13]) > 0)
      {
        LeftHandDis[7] = HandDis[7] / Vector3.Distance(LeftHandLandMarks[16], LeftHandLandMarks[13]);
      }
      //Pinky
      LeftHandRoatate[8] = LeftHandLandMarks[17] - LeftHandLandMarks[0];
      if (Vector3.Distance(LeftHandLandMarks[17], LeftHandLandMarks[0]) > 0)
      {
        LeftHandDis[8] = HandDis[8] / Vector3.Distance(LeftHandLandMarks[17], LeftHandLandMarks[0]);
      }
      LeftHandRoatate[9] = LeftHandLandMarks[20] - LeftHandLandMarks[17];
      if (Vector3.Distance(LeftHandLandMarks[20], LeftHandLandMarks[17]) > 0)
      {
        LeftHandDis[9] = HandDis[9] / Vector3.Distance(LeftHandLandMarks[20], LeftHandLandMarks[17]);
      }
    }
    #endregion

    #region RightHand
    if (RightHandLandMarks.Count > 0)
      { 
      //Thumb
      RightHandRoatate[0] = RightHandLandMarks[2] - RightHandLandMarks[0];
      if (Vector3.Distance(RightHandLandMarks[2], RightHandLandMarks[0]) > 0)
      {
        RightHandDis[0] = HandDis[0] / Vector3.Distance(RightHandLandMarks[2], RightHandLandMarks[0]);
      }
      RightHandRoatate[1] = RightHandLandMarks[4] - RightHandLandMarks[2];
      if (Vector3.Distance(RightHandLandMarks[4], RightHandLandMarks[2]) > 0)
      { 
        RightHandDis[1] = HandDis[1] / Vector3.Distance(RightHandLandMarks[4], RightHandLandMarks[2]);
      }
      //Index
      RightHandRoatate[2] = RightHandLandMarks[5] - RightHandLandMarks[0];
      if (Vector3.Distance(RightHandLandMarks[5], RightHandLandMarks[0]) > 0)
      {
        RightHandDis[2] = HandDis[2] / Vector3.Distance(RightHandLandMarks[5], RightHandLandMarks[0]);
      }
      RightHandRoatate[3] = RightHandLandMarks[8] - RightHandLandMarks[5];
      if (Vector3.Distance(RightHandLandMarks[8], RightHandLandMarks[5]) > 0)
      {
        RightHandDis[3] = HandDis[3] / Vector3.Distance(RightHandLandMarks[8], RightHandLandMarks[5]);
      }
      //Middle
      RightHandRoatate[4] = RightHandLandMarks[9] - RightHandLandMarks[0];
      if (Vector3.Distance(RightHandLandMarks[9], RightHandLandMarks[0]) > 0)
      {
        RightHandDis[4] = HandDis[4] / Vector3.Distance(RightHandLandMarks[9], RightHandLandMarks[0]);
      }
      RightHandRoatate[5] = RightHandLandMarks[12] - RightHandLandMarks[9];
      if (Vector3.Distance(RightHandLandMarks[12], RightHandLandMarks[9]) > 0)
      {
        RightHandDis[5] = HandDis[5] / Vector3.Distance(RightHandLandMarks[12], RightHandLandMarks[9]);
      }
      //Ring
      RightHandRoatate[6] = RightHandLandMarks[13] - RightHandLandMarks[0];
      if (Vector3.Distance(RightHandLandMarks[13], RightHandLandMarks[0]) > 0)
      {
        RightHandDis[6] = HandDis[6] / Vector3.Distance(RightHandLandMarks[13], RightHandLandMarks[0]);
      }
      RightHandRoatate[7] = RightHandLandMarks[16] - RightHandLandMarks[13];
      if (Vector3.Distance(RightHandLandMarks[16], RightHandLandMarks[13]) > 0)
      {
        RightHandDis[7] = HandDis[7] / Vector3.Distance(RightHandLandMarks[16], RightHandLandMarks[13]);
      }
      //Pinky
      RightHandRoatate[8] = RightHandLandMarks[17] - RightHandLandMarks[0];
      if (Vector3.Distance(RightHandLandMarks[17], RightHandLandMarks[0]) > 0)
      {
        RightHandDis[8] = HandDis[8] / Vector3.Distance(RightHandLandMarks[17], RightHandLandMarks[0]);
      }
      RightHandRoatate[9] = RightHandLandMarks[20] - RightHandLandMarks[17];
      if (Vector3.Distance(RightHandLandMarks[20], RightHandLandMarks[17]) > 0)
      {
        RightHandDis[9] = HandDis[9] / Vector3.Distance(RightHandLandMarks[20], RightHandLandMarks[17]);
      }
    }
    #endregion
  }

  void DetermineFacing()
  {
    bool facingCamera = true;
    //facingCamera = LeftHandLandMarks[0].x > RightHandLandMarks[0].x;

    if (facingCamera)
    {
      bool upSideDown;

      if (LeftHandLandMarks.Count > 0)
      { 
        upSideDown = LeftHandLandMarks[9].y < LeftHandLandMarks[0].y;
        if (upSideDown)
        {
          LeftHandFacingCam = LeftHandLandMarks[5].x < LeftHandLandMarks[17].x;
        }
        else
        {
          LeftHandFacingCam = LeftHandLandMarks[5].x > LeftHandLandMarks[17].x;
        }
      }

      if (RightHandLandMarks.Count > 0)
      {
        upSideDown = RightHandLandMarks[9].y < RightHandLandMarks[0].y;
        if (upSideDown)
        {
          RightHandFacingCam = RightHandLandMarks[5].x > RightHandLandMarks[17].x;
        }
        else
        {
          RightHandFacingCam = RightHandLandMarks[5].x < RightHandLandMarks[17].x;
        }
      }
    }
  }

  void UpdateIKTarget()
  {
    //0 1 2 3 4 5 6  7  8  9  10
    //0 2 4 5 8 9 12 13 16 17 20
    #region LeftHand
    if (LeftHandLandMarks.Count > 0)
    { 
      LeftHandIKTarget[0] = LeftHandLandMarks[0];
      //Thumb
      LeftHandIKTarget[1] = LeftHandIKTarget[0] + LeftHandRoatate[0] * LeftHandDis[0];
      LeftHandIKTarget[2] = LeftHandIKTarget[1] + LeftHandRoatate[1] * LeftHandDis[1];
      //Index
      LeftHandIKTarget[3] = LeftHandIKTarget[0] + LeftHandRoatate[2] * LeftHandDis[2];
      LeftHandIKTarget[4] = LeftHandIKTarget[3] + LeftHandRoatate[3] * LeftHandDis[3];
      //Middle
      LeftHandIKTarget[5] = LeftHandIKTarget[0] + LeftHandRoatate[4] * LeftHandDis[4];
      LeftHandIKTarget[6] = LeftHandIKTarget[5] + LeftHandRoatate[5] * LeftHandDis[5];
      //Ring
      LeftHandIKTarget[7] = LeftHandIKTarget[0] + LeftHandRoatate[6] * LeftHandDis[6];
      LeftHandIKTarget[8] = LeftHandIKTarget[7] + LeftHandRoatate[7] * LeftHandDis[7];
      //Pinky
      LeftHandIKTarget[9] = LeftHandIKTarget[0] + LeftHandRoatate[8] * LeftHandDis[8];
      LeftHandIKTarget[10] = LeftHandIKTarget[9] + LeftHandRoatate[9] * LeftHandDis[9];
    }
    #endregion

    #region RightHand
    if (RightHandLandMarks.Count > 0)
    {
      RightHandIKTarget[0] = RightHandLandMarks[0];
      //Thumb
      RightHandIKTarget[1] = RightHandIKTarget[0] + RightHandRoatate[0] * RightHandDis[0];
      RightHandIKTarget[2] = RightHandIKTarget[1] + RightHandRoatate[1] * RightHandDis[1];
      //Index
      RightHandIKTarget[3] = RightHandIKTarget[0] + RightHandRoatate[2] * RightHandDis[2];
      RightHandIKTarget[4] = RightHandIKTarget[3] + RightHandRoatate[3] * RightHandDis[3];
      //Middle
      RightHandIKTarget[5] = RightHandIKTarget[0] + RightHandRoatate[4] * RightHandDis[4];
      RightHandIKTarget[6] = RightHandIKTarget[5] + RightHandRoatate[5] * RightHandDis[5];
      //Ring
      RightHandIKTarget[7] = RightHandIKTarget[0] + RightHandRoatate[6] * RightHandDis[6];
      RightHandIKTarget[8] = RightHandIKTarget[7] + RightHandRoatate[7] * RightHandDis[7];
      //Pinky
      RightHandIKTarget[9] = RightHandIKTarget[0] + RightHandRoatate[8] * RightHandDis[8];
      RightHandIKTarget[10] = RightHandIKTarget[9] + RightHandRoatate[9] * RightHandDis[9];
    }
    #endregion
  }
  void MoveIKTarget()
  {
    if (LeftHandIKs.Count < LeftHandIKTarget.Count || RightHandIKs.Count < RightHandIKTarget.Count)
    {
      return;
    }
    for (int x = 0; x < LeftHandIKTarget.Count; x++)
    {
      LeftHandIKs[x].transform.position = LeftHandIKTarget[x];
      /*
      float Scaler = 0.5f;
      switch (x)
      {
        case 2:
          LeftHandIKs[x].transform.position += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * .5f);
          break;
        case 4:
          LeftHandIKs[x].transform.position += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * .5f);
          break;
        case 6:
          LeftHandIKs[x].transform.position += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * .5f);
          break;
        case 8:
          LeftHandIKs[x].transform.position += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * .5f);
          break;
        case 10:
          LeftHandIKs[x].transform.position += new Vector3(0, 0, (ScreenCam.transform.forward - ScreenCam.transform.position).z * .5f);
          break;
        default:
          break;
      }
      */
    }
    for (int x = 0; x < RightHandIKTarget.Count; x++)
    {
      RightHandIKs[x].transform.position = RightHandIKTarget[x];
    }
  }
  

  private void LateUpdate()
  {
    if (DebugMode == false) return;
    MoveSphere();
    LeftHandRender();
    RightHandRender();
  }

  void MoveSphere()
  {
    /*
    if (HandOneLandMarks.Count < 21) return;
    for (int x = 0; x < 21; x++)
    {
      LandMarkBalls[x].transform.position = new Vector3(HandOneLandMarks[x].x, HandOneLandMarks[x].y, HandOneLandMarks[x].z);
    }
    */
    if (LeftHandLandMarks.Count > 0)
    {
      for (int x = 0; x < 21; x++)
      {
        LeftHandLandMarkBalls[x].transform.position = LeftHandLandMarks[x];
      }
    }

    if (RightHandLandMarks.Count > 0)
    {
      for (int x = 0; x < 21; x++)
      {
        RightHandLandMarkBalls[x].transform.position = RightHandLandMarks[x];
      }
    }
  }

  GameObject CreateCube(Hand hand)
  {
    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    if (hand == Hand.Left)
    {
      cube.transform.parent = LeftHandRenderCubes.transform;
    }
    else
    {
      cube.transform.parent = RightHandRenderCubes.transform;
    }
    return cube;
  }

  void SetCubePos(Vector3 p1, Vector3 p2, GameObject cube)
  {
    cube.transform.position = (p1 + p2) / 2;
    Vector3 dir = p2 - p1;
    cube.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    float dis = Vector3.Distance(p1, p2);
    cube.transform.localScale = new Vector3(cube.transform.localScale.x, cube.transform.localScale.y, dis);
  }

  void LeftHandRender()
  {
    if (LeftHandLandMarks.Count == 0) return;

    for (int x = 0; x < 21; x++)
    {
      if (LeftHandCubes.Count <= x)
      {
        LeftHandCubes.Add(CreateCube(Hand.Left));
      }
      else
      {
        if (x != 0 && x != 5 && x != 9 && x != 13 && x != 17)
        {
          SetCubePos(LeftHandLandMarks[x], LeftHandLandMarks[x - 1], LeftHandCubes[x]);
        }
        else
        {
          SetCubePos(LeftHandLandMarks[17], LeftHandLandMarks[0], LeftHandCubes[0]);
          SetCubePos(LeftHandLandMarks[5], LeftHandLandMarks[0], LeftHandCubes[5]);
          SetCubePos(LeftHandLandMarks[9], LeftHandLandMarks[5], LeftHandCubes[9]);
          SetCubePos(LeftHandLandMarks[13], LeftHandLandMarks[9], LeftHandCubes[13]);
          SetCubePos(LeftHandLandMarks[17], LeftHandLandMarks[13], LeftHandCubes[17]);
        }
      }

    }
  }

  void RightHandRender()
  {
    if (RightHandLandMarks.Count == 0) return;

    for (int x = 0; x < 21; x++)
    {
      if (RightHandCubes.Count <= x)
      {
        RightHandCubes.Add(CreateCube(Hand.Right));
        RightHandCubes[x].transform.parent = RightHandRenderCubes.transform;
      }
      else
      {
        if (x != 0 && x != 5 && x != 9 && x != 13 && x != 17)
        {
          SetCubePos(RightHandLandMarks[x], RightHandLandMarks[x - 1], RightHandCubes[x]);
        }
        else
        {
          SetCubePos(RightHandLandMarks[17], RightHandLandMarks[0], RightHandCubes[0]);
          SetCubePos(RightHandLandMarks[5], RightHandLandMarks[0], RightHandCubes[5]);
          SetCubePos(RightHandLandMarks[9], RightHandLandMarks[5], RightHandCubes[9]);
          SetCubePos(RightHandLandMarks[13], RightHandLandMarks[9], RightHandCubes[13]);
          SetCubePos(RightHandLandMarks[17], RightHandLandMarks[13], RightHandCubes[17]);
        }
      }

    }
  }
}

public enum Hand
{ 
  Left,
  Right
}
