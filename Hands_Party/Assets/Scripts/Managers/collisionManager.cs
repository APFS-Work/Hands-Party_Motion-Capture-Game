using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionManager : MonoBehaviour
{
  public bool Visible;

  public List<GameObject> collsionSpheres;

  private void Update()
  {
    if (Visible == true && collsionSpheres[0].GetComponent<MeshRenderer>() == false)
    {
      foreach(GameObject sphere in collsionSpheres)
      {
        sphere.GetComponent<MeshRenderer>().enabled = true;
      }
    }
  }

  public void ChangeHandCollision(bool collisionOn)
  {
    foreach (GameObject sphere in collsionSpheres)
    {
      sphere.SetActive(collisionOn);
    }
  }
}
