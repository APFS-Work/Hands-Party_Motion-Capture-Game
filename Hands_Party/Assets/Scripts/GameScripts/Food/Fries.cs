using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fries : Food
{
  public FriesSize size;
  public bool fill = false;
  public GameObject fries;

  private void Awake()
  {
    foodType = typeOfFood.Fries;
  }

  // Start is called before the first frame update
  void Start()
  {
    switch (size)
    {
      case FriesSize.Big:
        break;
      case FriesSize.Medium:
        gameObject.transform.localScale -= new Vector3(0.2f, 0.2f, 0f);
        break;
      case FriesSize.Small:
        gameObject.transform.localScale -= new Vector3(0.4f, 0.4f, 0f);
        break;
    }
    fries.active = fill;
  }

  // Update is called once per frame
  void Update()
  {
    if (fries.active != fill)
    {
      fries.active = fill;
    }
  }
}

