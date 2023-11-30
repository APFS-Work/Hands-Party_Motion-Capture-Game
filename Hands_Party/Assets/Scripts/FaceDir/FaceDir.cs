using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity.FaceDetection;
using Mediapipe;
using Mediapipe.Unity;

public class FaceDir : MonoBehaviour
{
  public DetectionListAnnotationController faceDetectionController;

  /*
  Key points
  RightEye = 0,
  LeftEye = 1,
  NoseTip = 2,
  MouthCenter = 3,
  RightEarTragion = 4,
  LeftEarTragion = 5
  */

  public enum FacingDir
  { 
    Forward,
    Left,
    Right,
    Top,
    Bottom
  }

  public bool DebugMode;
  public List<Vector2> _faceKeyPoints = new List<Vector2>();

  public float LeftToNoseTip, RightToNoseTip;
  public float scaleOfLeftRight;

  public float TopToNose, BottomToNose;
  public float scaleOfTopDown;

  public FacingDir LeftOrRight, TopOrBottom;

  // Start is called before the first frame update
  void Start()
  {
    for (int x = 0; x < 6; x++)
    {
      _faceKeyPoints.Add(Vector2.zero);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (faceDetectionController._currentTarget == null) return;

    #region left and right dir
    LeftToNoseTip = Vector2.Distance(ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[5]), ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[2]));
    RightToNoseTip = Vector2.Distance(ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[4]), ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[2]));
    scaleOfLeftRight = LeftToNoseTip / RightToNoseTip;

    if (scaleOfLeftRight >= 1.7f) LeftOrRight = FacingDir.Right;
    else if (scaleOfLeftRight <= 0.3f) LeftOrRight = FacingDir.Left;
    else LeftOrRight = FacingDir.Forward;
    #endregion

    #region Top And Down Dir
    TopToNose = faceDetectionController._currentTarget[0].LocationData.RelativeBoundingBox.Ymin + faceDetectionController._currentTarget[0].LocationData.RelativeBoundingBox.Height - ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[2]).y;
    BottomToNose = ToVectorTwo(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[2]).y - faceDetectionController._currentTarget[0].LocationData.RelativeBoundingBox.Ymin;
    scaleOfTopDown = TopToNose / BottomToNose;
    if (scaleOfTopDown >= 1.8f) TopOrBottom = FacingDir.Top;
    else if (scaleOfTopDown <= 1f) TopOrBottom = FacingDir.Bottom;
    else TopOrBottom = FacingDir.Forward;
    #endregion

    if (DebugMode == false) return;
    for (int x = 0; x < 6; x++)
    {
      _faceKeyPoints[x] = new Vector2(faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[x].X, faceDetectionController._currentTarget[0].LocationData.RelativeKeypoints[x].Y);
    }
  }

  Vector2 ToVectorTwo(LocationData.Types.RelativeKeypoint keypoint)
  {
    Vector2 v2 = Vector2.zero;
    if (keypoint.HasX) v2.x = keypoint.X;
    if (keypoint.HasY) v2.y = keypoint.Y;
    return v2;
  }
}

