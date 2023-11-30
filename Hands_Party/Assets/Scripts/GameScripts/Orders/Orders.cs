using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;

public class Orders : MonoBehaviour
{
  public localProperties localProps;

  public List<Food> ReadyFood = new List<Food>();
  public List<string> OrderList = new List<string>();

  public UnityEvent OnOrderFinish;

  public MeshRenderer burgerOrderRenderer;
  public List<Material> burgersMaterials;
  enum burgerMatIndex
  { 
    hamburger = 0,
    cheese = 1,
    doubleCheese = 2
  }
  public TMP_Text friesSizeText;
  public MeshRenderer drinkOrderRenderer;
  public List<Material> drinksMaterials;
  enum drinkMatIndex
  {
    cola = 0,
    fanta = 1,
    sprite = 2
  }

  void Start()
  {
    newOrder();

    OnOrderFinish.AddListener(OrderFinishLog);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      newOrder();
    }
  }

  private void CheckOrderFinish()
  {
    int finishScore = 0;
    for (int x = 0; x < OrderList.Count; x++)
    {
      foreach (Food ready in ReadyFood)
      {
        string readyFood = GetSpecific(ready);
        if (readyFood == OrderList[x])
        {
          finishScore++;
        }
      }
    }

    if (finishScore >= OrderList.Count)
    {
      finishOrder();
    }
  }

  private string GetSpecific(Food food)
  { 
    switch (food.foodType)
    {
      case Food.typeOfFood.Burger:
        return food.GetComponent<Burger>().burgerType.ToString();
      case Food.typeOfFood.Fries:
        return food.GetComponent<Fries>().size.ToString();
      case Food.typeOfFood.Drink:
        return food.GetComponent<Drink>().typeOfDrink.ToString();
      default:
        return null;
    }
  }

  private void finishOrder()
  {
    foreach (Food food in ReadyFood)
    {
      Destroy(food.gameObject);
    }
    ReadyFood = new List<Food>();
    OnOrderFinish.Invoke();
    localProps.AddScore(1);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.GetComponent<Food>() == false) return;

    Food food = other.gameObject.GetComponent<Food>();

    if (ReadyFood.Contains(food) == false)
    {
      ReadyFood.Add(food);
      //Debug.LogWarning(GetSpecific(food) + " Ready");
    }

    CheckOrderFinish();
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.GetComponent<Food>() == false) return;

    Food food = other.gameObject.GetComponent<Food>();

    if (ReadyFood.Contains(food) == true)
    {
      ReadyFood.Remove(food);
    }
  }

  void newOrder()
  {
    OrderList = new List<string>();
    OrderList.Add(((Food.BurgerType)Random.RandomRange(0, Enum.GetNames(typeof(Food.BurgerType)).Length - 2)).ToString());
    OrderList.Add(((Food.FriesSize)Random.RandomRange(0, Enum.GetNames(typeof(Food.FriesSize)).Length)).ToString());
    OrderList.Add(((Food.DrinkType)Random.RandomRange(0, Enum.GetNames(typeof(Food.DrinkType)).Length)).ToString());

    foreach (string food in OrderList)
    {
      switch (food)
      {
        case "Cheese":
          burgerOrderRenderer.material = burgersMaterials[(int)burgerMatIndex.cheese];
          break;
        case "DoubleCheese":
          burgerOrderRenderer.material = burgersMaterials[(int)burgerMatIndex.doubleCheese];
          break;
        case "Hamburger":
          burgerOrderRenderer.material = burgersMaterials[(int)burgerMatIndex.hamburger];
          break;
        case "Big":
          friesSizeText.text = "Large";
          break;
        case "Medium":
          friesSizeText.text = "Medium";
          break;
        case "Small":
          friesSizeText.text = "Small";
          break;
        case "Cola":
          drinkOrderRenderer.material = drinksMaterials[(int)drinkMatIndex.cola];
          break;
        case "Fanta":
          drinkOrderRenderer.material = drinksMaterials[(int)drinkMatIndex.fanta];
          break;
        case "Sprite":
          drinkOrderRenderer.material = drinksMaterials[(int)drinkMatIndex.sprite];
          break;
        default:
          break;
      }
    }
  }

  public void OrderFinishLog()
  {
    Debug.Log("Order " + " Complete");
  }
}
