using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillsEffect : MonoBehaviour
{
  public Vector3 targetPosition;

  public bool isThunder;

  // Start is called before the first frame update
  void Start()
  {
    //targetPosition = new Vector3(0, 0, 30f);
  }

  // Update is called once per frame
  void Update()
  {
    if (isThunder == false)
    {
      if (transform.position.z < targetPosition.z)
      {
        transform.Translate(Vector3.forward * Time.deltaTime * 60);
      }
      else
      {
        Destroy(gameObject);
      }
    }
    else
    {
      if ((transform.localScale.z * 5) < Vector3.Distance(targetPosition, transform.position))
      {
        transform.localScale += new Vector3(0, 0, .1f);
      }
      else
      {
        Destroy(gameObject);
      }
    }
  }
}
