using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabThings : MonoBehaviour
{
  public HandGesture handGesture;
  public bool LeftHand;

  public List<GameObject> pickableObjects;

  public GameObject selectedObject;
  public bool grab;

  string tag = "Pickable";

  // Start is called before the first frame update
  void Start()
  {
    tag = "Pickable";
    pickableObjects = new List<GameObject>();
  }

  // Update is called once per frame
  void Update()
  {
    if (LeftHand)
    {
      if (handGesture.LeftHandGesture == HandGesture.Gesture.Grab && selectedObject == null)
      {
        selectObject();
        grab = true;
      }
      else if (handGesture.LeftHandGesture == HandGesture.Gesture.Release && selectedObject != null)
      {
        if (selectedObject != null)
        {
          selectedObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        selectedObject = null;
        grab = false;
      }
    }
    else
    {
      if (handGesture.RightHandGesture == HandGesture.Gesture.Grab && selectedObject == null)
      {
        selectObject();
        grab = true;
      }
      else if (handGesture.RightHandGesture == HandGesture.Gesture.Release && selectedObject != null)
      {
        if (selectedObject != null)
        { 
          selectedObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        selectedObject = null;
        grab = false;
      }
    }

    if (grab == true && selectedObject != null)
    {
      selectedObject.transform.position = transform.position;
      selectedObject.transform.rotation = Quaternion.identity;
    }
  }

  void selectObject()
  {
    if (pickableObjects.Count <= 0) return;
    List<float> dis = new List<float>();
    for (int x = 0; x < pickableObjects.Count; x++)
    {
      dis.Add(Vector3.Distance(gameObject.transform.position, pickableObjects[x].transform.position));
    }
    int index = dis.IndexOf(Mathf.Min(dis.ToArray()));
    selectedObject = pickableObjects[index];
    selectedObject.transform.rotation = Quaternion.identity;
    selectedObject.GetComponent<Rigidbody>().isKinematic = true;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag != tag) return;

    if (pickableObjects.Contains(other.gameObject) == false)
    {
      if (other.gameObject.GetComponent<Rigidbody>())
      { 
        pickableObjects.Add(other.gameObject);
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.tag != tag) return;
    if (pickableObjects.Contains(other.gameObject))
    {
      pickableObjects.Remove(other.gameObject);
      other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
  }
}
