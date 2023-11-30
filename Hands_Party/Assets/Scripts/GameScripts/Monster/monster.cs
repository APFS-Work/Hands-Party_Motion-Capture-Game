using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class monster : MonoBehaviour
{
  public Animator anime;
  public bool dead;

  public bool left;
  public bool run = false;
  public float speed;

  public float DestroyLine;

  public MeshRenderer gestureSign;
  enum gestureSignIndex
  { 
    Left_Index,
    Left_Pinky,
    Left_IndexAndPinky,
    Right_Index,
    Right_Pinky,
    Right_IndexAndPinky
  }
  public List<Material> gestureSignMats;
  public HandGesture.Gesture gesture = HandGesture.Gesture.Release;

  // Start is called before the first frame update
  void Start()
  {
    randomGesture();
    speed = Random.Range(1,3);
    if (run)
    {
      speed = 4;
    }
    DestroyLine = -30f;
    anime = gameObject.GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (dead == true)
    {
      return;
    }
    transform.Translate(Vector3.forward * Time.deltaTime * speed * 10);
    if (transform.position.z < DestroyLine)
    {
      Destroy(gameObject);
    }
  }

  public void die()
  {
    dead = true;
    anime.SetTrigger("die");
    Destroy(gameObject, 3f);
  }

  void randomGesture()
  {
    int randomGesture = Random.Range(2, 5);
 
    gesture = (HandGesture.Gesture)randomGesture;

    if (left)
    {
      switch (gesture)
      {
        case HandGesture.Gesture.IndexUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Left_Index];
          break;
        case HandGesture.Gesture.PinkyUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Left_Pinky];
          break;
        case HandGesture.Gesture.IndexAndPinkyUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Left_IndexAndPinky];
          break;
      }
    }
    else
    {
      switch (gesture)
      {
        case HandGesture.Gesture.IndexUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Right_Index];
          break;
        case HandGesture.Gesture.PinkyUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Right_Pinky];
          break;
        case HandGesture.Gesture.IndexAndPinkyUp:
          gestureSign.material = gestureSignMats[(int)gestureSignIndex.Right_IndexAndPinky];
          break;
      }
    }
  }
}
