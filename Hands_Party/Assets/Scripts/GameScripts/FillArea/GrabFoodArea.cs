using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabFoodArea : MonoBehaviour
{
  public GameObject instantiateFoodPrefab;

  public GameObject tempObj;

  public List<GrabThings> grabThings = new List<GrabThings>();

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    if (grabThings.Count <= 0)
    {
      return;
    }

    GetThings();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.GetComponent<GrabThings>() == false)
    {
      return;
    }
    GrabThings grabTh = other.gameObject.GetComponent<GrabThings>();

    if (grabThings.Contains(grabTh) == false)
    {
      grabThings.Add(grabTh);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.GetComponent<GrabThings>() == false)
    {
      return;
    }
    GrabThings grabTh = other.gameObject.GetComponent<GrabThings>();

    if (grabThings.Contains(grabTh) == true)
    {
      grabThings.Remove(grabTh);
    }
  }

  void GetThings()
  {
    foreach (GrabThings grabThs in grabThings)
    {
      if (grabThs.grab == true && grabThs.selectedObject == null)
      {
        grabThs.selectedObject = Instantiate(instantiateFoodPrefab, tempObj.transform);
      }
    }
  }
}
