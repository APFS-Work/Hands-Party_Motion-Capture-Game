using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : Food
{
  public int pourThreshold = 45;
  public Transform origin = null;
  public GameObject streamPrefab = null;
  public GameObject drink;

  public DrinkType typeOfDrink;

  public List<Color> ColaColor;
  public List<Color> SpriteColor;
  public List<Color> FantaColor;

  private bool isPouring = false;
  private DrinkStream currentStream = null;
  public float fill;
  Material drinkMat;
  Color pourColor;

  private void Awake()
  {
    foodType = typeOfFood.Drink;
  }

  // Start is called before the first frame update
  void Start()
  {
    fill = 0f;
    drinkMat = drink.GetComponent<MeshRenderer>().material;
    ChangeDrink();
  }

  // Update is called once per frame
  void Update()
  {
    bool pourCheck = CalculatePourAngle() < pourThreshold;

    if (isPouring != pourCheck)
    {
      isPouring = pourCheck;

      if (isPouring)
      {
        StartPour();
      }
      else
      {
        StopPour();
      }
    }

    if (currentStream != null && fill <= 0)
    {
      StopPour();
    }

    if (currentStream == null && fill > 0 && isPouring)
    {
      StartPour();
    }

    if (isPouring)
    {
      if (fill > 0f)
      {
        fill -= Time.deltaTime / 3f;
      }
    }

    drinkMat.SetFloat("_Fill", fill);
  }

  public void ChangeDrink()
  {
    switch (typeOfDrink)
    {
      case DrinkType.Cola:
        drinkMat.SetColor("_TopColor", ColaColor[0]);
        drinkMat.SetColor("_SideColor", ColaColor[1]);
        pourColor = ColaColor[0];
        break;
      case DrinkType.Sprite:
        drinkMat.SetColor("_TopColor", SpriteColor[0]);
        drinkMat.SetColor("_SideColor", SpriteColor[1]);
        pourColor = SpriteColor[0];
        break;
      case DrinkType.Fanta:
        drinkMat.SetColor("_TopColor", FantaColor[0]);
        drinkMat.SetColor("_SideColor", FantaColor[1]);
        pourColor = FantaColor[0];
        break;
      default:
        break;
    }
  }

  private void StartPour()
  {
    currentStream = CreateStream();
    currentStream.Begin();
  }

  private void StopPour()
  {
    if (currentStream == null) return;
    Destroy(currentStream.gameObject);
    //Debug.LogWarning("End");
  }

  private float CalculatePourAngle()
  {
    return transform.up.y * Mathf.Rad2Deg;
  }

  private DrinkStream CreateStream()
  {
    GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
    streamObject.GetComponent<LineRenderer>().startColor = pourColor;
    return streamObject.GetComponent<DrinkStream>();
  }
}


