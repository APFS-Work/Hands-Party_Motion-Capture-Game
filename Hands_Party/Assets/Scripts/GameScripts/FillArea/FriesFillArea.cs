using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriesFillArea : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.GetComponent<Fries>() == false) return;

    other.GetComponent<Fries>().fill = true;
  }
}
