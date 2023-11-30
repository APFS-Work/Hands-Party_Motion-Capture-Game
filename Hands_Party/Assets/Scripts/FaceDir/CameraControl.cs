using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
  public FaceDir faceDir;

  public GameObject Menu;

  public Camera cam = null;

  public float TopDownSensitivity = 5.0f;
  public float LeftRightSensitivity = 5.0f;

  public bool locked;

  Vector3 rotateAngle;

  // Start is called before the first frame update
  void Start()
  {
    locked = true;
    if (cam == null) cam = Camera.main;
    TopDownSensitivity /= 10f;
    LeftRightSensitivity /= 10f;
  }

  // Update is called once per frame
  void Update()
  {
    if (locked) return;
    float angle;
    angle = cam.transform.localRotation.eulerAngles.y;
    if (faceDir.LeftOrRight == FaceDir.FacingDir.Left)
    {
      angle -= LeftRightSensitivity;
      angle = ClampCameraY(-45, 20, angle);
      cam.transform.localRotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, angle, 0);
      return;
    }
    else if (faceDir.LeftOrRight == FaceDir.FacingDir.Right)
    {
      angle += LeftRightSensitivity;
      angle = ClampCameraY(-45, 20, angle);
      cam.transform.localRotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, angle, 0);
      return;
    }
    
    angle = cam.transform.localRotation.eulerAngles.x;
    NormalizeAngle(angle);
    if (faceDir.TopOrBottom == FaceDir.FacingDir.Top)
    {
      angle -= TopDownSensitivity;
    }
    else if (faceDir.TopOrBottom == FaceDir.FacingDir.Bottom)
    {
      angle += TopDownSensitivity;
    }

    ClampCamera(-10f, 25f, angle);
  }

  float NormalizeAngle(float a)
  {
    return a - 180f * Mathf.Floor((a + 180f) / 180f);
  }

  public void LockCamera()
  {
    locked = true;
  }

  public void LockCamera(Vector3 Angle)
  {
    locked = true;
    cam.transform.localRotation = Quaternion.Euler(Angle);
  }

  public void ClampCamera(float minX, float maxX, float angle)
  {
    if (angle > 180f)
    {
      angle -= 360f;
    }
    angle = Mathf.Clamp(angle, minX, maxX);
    Vector3 currentAngle = new Vector3(angle, cam.transform.localRotation.eulerAngles.y, cam.transform.localRotation.eulerAngles.z);
    cam.transform.localRotation = Quaternion.Euler(currentAngle);
  }

  public float ClampCameraY(float minY, float maxY, float a)
  {
    if (a > 180f)
    {
      a -= 360f;
    }
    float an = Mathf.Clamp(a, minY, maxY);
    return an;
  }
}
