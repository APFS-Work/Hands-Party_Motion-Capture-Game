using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PhyButton : MonoBehaviour
{
  [SerializeField]
  private float threshold = 0.1f;
  [SerializeField]
  private float deadZone = 0.025f;

  private bool isPressed;
  private Vector3 startPos;
  private ConfigurableJoint joint;

  public UnityEvent OnPressed, OnReleased;

  public bool isReady;
  public TMP_Text readyText;

  public Material red, green;
  public MeshRenderer clickerRenderer;

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

    readyText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - readyText.transform.position) * Quaternion.Euler(0f, 180f, 0f);
    if (isReady && clickerRenderer.material != green)
    {
      clickerRenderer.material = green;
      readyText.color = Color.green;
      readyText.text = "Ready";
    }
    if (!isReady && clickerRenderer.material != red)
    {
      clickerRenderer.material = red;
      readyText.color = Color.red;
      readyText.text = "Not Ready";
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
    isReady = !isReady;
    OnReleased.Invoke();
    //Debug.Log("Released");
  }
}
