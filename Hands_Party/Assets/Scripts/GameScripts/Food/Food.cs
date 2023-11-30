using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
  public typeOfFood foodType;

  public enum typeOfFood
  {
    Burger,
    Fries,
    Drink
  }
    public enum BurgerType
    {
      Hamburger,
      Cheese,
      DoubleCheese,
      None = -1,
      Unfinish = -2
    }

    public enum DrinkType
    {
      Cola,
      Sprite,
      Fanta
    }

    public enum FriesSize
    {
      Big,
      Medium,
      Small
    }
  public enum Recipe
  {


  }
}

