using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkFillArea : MonoBehaviour
{
  public Food.DrinkType drinkType;

  public GameObject drinkFillLineRenderer;

  bool isfilling;
  public drinkBtn drinkBtn;

  Drink drink = null;

  // Start is called before the first frame update
  void Start()
  {
    drinkBtn.OnPressed.AddListener(filling);
    drinkBtn.OnReleased.AddListener(NotFilling);
    drinkFillLineRenderer.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {
    if (isfilling == false)
    {
      return;
    }

    fillDrink();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (drink != null || other.GetComponent<Drink>() == false) return;
    drink = other.GetComponent<Drink>();
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject == drink.gameObject)
    {
      drink = null;
    }
  }

  void fillDrink()
  {
    if (drink != null && drink.fill < 1f)
    {
      drink.fill += Time.deltaTime / 5f;
      if (drink.fill > 0.5f) return;
      if (drink.typeOfDrink != drinkType)
      {
        drink.typeOfDrink = drinkType;
        drink.ChangeDrink();
      }
    }
  }

  public void filling()
  {
    isfilling = true;
    drinkFillLineRenderer.SetActive(true);
  }

  public void NotFilling()
  {
    isfilling = false;
    drinkFillLineRenderer.SetActive(false);
  }
}
