using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
  public HandLandMarksChecking handLandMarksChecking;

  public GameObject LeftHand, RightHand;
  public GameObject LeftHandWrist, RightHandWrist;

  public GameObject LeftHandGrip, RightHandGrip;

  public float MinDis, MaxDis;
  public float NormalizedDistance;

  // Start is called before the first frame update
  void Start()
  {
    //Max Dis Land Mark Distance is : 5.289561
    //Min Dis Land Mark Distance is : 2.375426
    
  }

  // Update is called once per frame
  void Update()
  {
    /*
    if (Input.GetKeyDown(KeyCode.Space))
    {
      Debug.LogWarning("Land Mark Distance is : " + Vector3.Distance(handLandMarksChecking.LeftHandLandMarks[0], handLandMarksChecking.LeftHandLandMarks[9]));
    }
    */
  }
  //NormalizedFloat = (MiddleNum - NumOne) / (NumTwo - NumOne);
  private void LateUpdate()
  {
    if (handLandMarksChecking.LeftHandLandMarks.Count > 0)
    {
      //LeftHandWrist.transform.position = handLandMarksChecking.LeftHandIKTarget[0];
      LeftHandMovement();
      LeftWristRotate();
    }
    if (handLandMarksChecking.RightHandLandMarks.Count > 0)
    {
      RightHandMovement();
      RightWristRotate();
    }
  }

  void LeftHandMovement()
  {
    LeftHandWrist.transform.position = handLandMarksChecking.LeftHandIKTarget[0];

    //LeftHandWrist.transform.position = new Vector3(LeftHandWrist.transform.position.x, LeftHandWrist.transform.position.y, LeftHandWrist.transform.position.z + 10.0f * NormalizedDistance);
  }
  void LeftWristRotate()
  {
    Vector3 Wrist = handLandMarksChecking.LeftHandIKTarget[0];
    Vector3 middleFinger = handLandMarksChecking.LeftHandIKTarget[5];

    Vector3 MiddleToWrist = middleFinger - Wrist;

    if (MiddleToWrist == Vector3.zero)
    {
      return;
    }

    Vector3 RotateAngle = Quaternion.LookRotation(MiddleToWrist).eulerAngles;
    
    if (handLandMarksChecking.LeftHandFacingCam == false)
    {
      RotateAngle.y = 180.0f;
    }
    else
    {
      RotateAngle.y = 0;
    }
    RotateAngle += Camera.main.transform.localRotation.eulerAngles;
    LeftHandWrist.transform.rotation = Quaternion.Euler(RotateAngle);
    //LeftHandWrist.transform.rotation = Quaternion.LookRotation(MiddleToWrist);

    /*
    if (LeftHandFrontFacing == true)
    {
      LeftHandWrist.transform.rotation = Quaternion.LookRotation(MiddleToWrist);
    }
    else
    {
      LeftHandWrist.transform.rotation = Quaternion.LookRotation(MiddleToWrist) * Quaternion.Euler(0f, 0f, 180f);
    }
    */
  }

  void RightHandMovement()
  {
    RightHandWrist.transform.position = handLandMarksChecking.RightHandIKTarget[0];
  }
  void RightWristRotate()
  {
    Vector3 Wrist = handLandMarksChecking.RightHandIKTarget[0];
    Vector3 middleFinger = handLandMarksChecking.RightHandIKTarget[5];

    Vector3 MiddleToWrist = middleFinger - Wrist;

    if (MiddleToWrist == Vector3.zero)
    {
      return;
    }

    Vector3 RotateAngle = Quaternion.LookRotation(MiddleToWrist).eulerAngles;

    if (handLandMarksChecking.RightHandFacingCam == false)
    {
      RotateAngle.y = 180.0f;
    }
    else
    {
      RotateAngle.y = 0;
    }
    RotateAngle += Camera.main.transform.localRotation.eulerAngles;
    RightHandWrist.transform.rotation = Quaternion.Euler(RotateAngle);
  }

  public bool DebugMode;
  public GameObject LeftWristAngler, RightWristAngler;
  private void OnDrawGizmos()
  {
    if (DebugMode == false) return;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(LeftHandWrist.transform.position, LeftHandWrist.transform.position + LeftHandWrist.transform.forward * 10.0f);
    Gizmos.DrawLine(RightHandWrist.transform.position, RightHandWrist.transform.position + RightHandWrist.transform.forward * 10.0f);

    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(LeftHandGrip.transform.position, LeftHandGrip.transform.position + LeftHandGrip.transform.forward);
    Gizmos.DrawLine(RightHandGrip.transform.position, RightHandGrip.transform.position + RightHandGrip.transform.forward);

    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(LeftHand.transform.position, LeftHand.transform.position + LeftHand.transform.forward);
    Gizmos.DrawLine(RightHand.transform.position, RightHand.transform.position + RightHand.transform.forward);

    Gizmos.color = Color.green;
    Gizmos.DrawLine(LeftWristAngler.transform.position, LeftWristAngler.transform.position + LeftWristAngler.transform.forward);
    Gizmos.DrawLine(RightWristAngler.transform.position, RightWristAngler.transform.position + RightWristAngler.transform.forward);
  }
}
