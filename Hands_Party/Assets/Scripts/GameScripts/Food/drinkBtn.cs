using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class drinkBtn : MonoBehaviour
{
  [SerializeField]
  private float threshold = 0.1f;
  [SerializeField]
  private float deadZone = 0.025f;

  private bool isPressed;
  private Vector3 startPos;
  private ConfigurableJoint joint;

  public UnityEvent OnPressed, OnReleased;

  // Start is called before the first frame update
  void Start()
  {
    startPos = transform.localPosition;
    joint = GetComponent<ConfigurableJoint>();
  }

  // Update is called once per frame
  void Update()
  {
    if (!isPressed && GetValue() + threshold >= 1)
    {
      Pressed();
    }
    if (isPressed && GetValue() - threshold <= 0)
    {
      Released();
    }
  }

  private float GetValue()
  {
    float value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
    if (Mathf.Abs(value) < deadZone)
    {
      value = 0;
    }
    return Mathf.Clamp(value, -1f, 1f);
  }

  private void Pressed()
  {
    isPressed = true;
    OnPressed.Invoke();
    //Debug.Log("Pressed");
  }

  private void Released()
  {
    isPressed = false;
    OnReleased.Invoke();
    //Debug.Log("Released");
  }
}
