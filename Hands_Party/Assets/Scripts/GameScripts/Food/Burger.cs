using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burger : Food
{
  public enum burgerIngredients
  { 
    TopBun,
    BottomBun,
    Meat,
    Lettuce,
    Cheese
  }

  public burgerIngredients ingredients;
  public BurgerType burgerType = BurgerType.Unfinish;

  List<burgerIngredients> burgerFormation = new List<burgerIngredients>();
  float height = 0;
  float totalHeight = 0;
  BoxCollider gainCollider;

  private void Awake()
  {
    foodType = typeOfFood.Burger;
  }

  // Start is called before the first frame update
  void Start()
  {
    Collider collider;
    if (gameObject.GetComponent<MeshCollider>())
    {
      collider = gameObject.GetComponent<MeshCollider>();
    }
    else
    {
      collider = gameObject.GetComponent<Collider>();
    }

    foreach (BoxCollider box in gameObject.GetComponents<BoxCollider>())
    {
      if (box.isTrigger == true)
      {
        gainCollider = box;
      }
    }

    height = collider.bounds.size.y;
    totalHeight = height / 2;

    burgerType = BurgerType.Unfinish;

    burgerFormation.Add(ingredients);
  }

  // Update is called once per frame
  void Update()
  {
    /*
    if (fixLocation != Vector3.zero)
    {
      gameObject.transform.localPosition = fixLocation;
    }
    */
    if (ingredients != burgerIngredients.BottomBun || burgerType == BurgerType.None) return;
    if (burgerFormation[burgerFormation.Count - 1] == burgerIngredients.TopBun)
    {
      Classify();
    }
  }

  void Classify()
  {
    if (burgerFormation.Count <= 3)
    {
      burgerType = BurgerType.None;
      return;
    }

    //Hamburger meat lettuce
    if (burgerFormation[1] == burgerIngredients.Meat && burgerFormation[2] == burgerIngredients.Lettuce && burgerFormation[3] == burgerIngredients.TopBun)
    {
      burgerType = BurgerType.Hamburger;
      return;
    }

    //Cheese burger meat cheese
    if (burgerFormation[1] == burgerIngredients.Meat && burgerFormation[2] == burgerIngredients.Cheese && burgerFormation[3] == burgerIngredients.TopBun)
    {
      burgerType = BurgerType.Cheese;
      return;
    }

    if (burgerFormation.Count < 6)
    {
      burgerType = BurgerType.None;
      return;
    }

    //Double cheese burger meat cheese meat cheese
    if (burgerFormation[1] == burgerIngredients.Meat && burgerFormation[2] == burgerIngredients.Cheese
      && burgerFormation[3] == burgerIngredients.Meat && burgerFormation[4] == burgerIngredients.Cheese && burgerFormation[5] == burgerIngredients.TopBun)
    {
      burgerType = BurgerType.DoubleCheese;
      return;
    }

    burgerType = BurgerType.None;
  }

  //Vector3 fixLocation = Vector3.zero;

  private void OnTriggerEnter(Collider other)
  {
    if (ingredients != burgerIngredients.BottomBun || burgerType != BurgerType.Unfinish) return;
    if (other.GetComponent<Burger>())
    {
      Burger burger = other.GetComponent<Burger>();
      if (burger.ingredients == burgerIngredients.BottomBun) return;

      //Debug.LogWarning("I am " + ingredients + " and it is " + burger.ingredients);

      Destroy(other.GetComponent<Rigidbody>());
      other.transform.parent = gameObject.transform;
      other.transform.localRotation = Quaternion.identity;
      other.transform.localPosition = new Vector3(0, totalHeight + (burger.height / 2), 0);

      //burger.fixLocation = new Vector3(0, totalHeight + (burger.height / 2), 0);

      if (gainCollider != null)
      {
        gainCollider.size += new Vector3(0, burger.height, 0);
        gainCollider.center += new Vector3(0, burger.height / 2, 0);
      }

      totalHeight += burger.height;
      burgerFormation.Add(burger.ingredients);
      Destroy(burger);
    }
  }
}
