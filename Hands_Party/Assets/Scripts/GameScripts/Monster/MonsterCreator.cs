using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterCreator : MonoBehaviour
{
  public localProperties localProperties;

  public HandGesture handGesture;
  public HandGesture.Gesture leftHandGesture;
  public HandGesture.Gesture rightHandGesture;

  public bool gameRunning;

  public GameObject tempMonster;

  public Transform LeftSideSpawnPoint;
  public Transform RightSideSpawnPoint;

  public List<GameObject> monsterPrefabs;

  public List<monster> leftSideMonster = new List<monster>();
  public List<monster> rightSideMonster = new List<monster>();

  public Transform LeftSideEffectPoint;
  public Transform RightSideEffectPoint;
  enum effects
  { 
    beam,
    fire,
    thunder
  }
  public List<GameObject> effectsPrefabs;

  bool leftSideSpawning;
  bool rightSideSpawning;

  public int score = 0;
  public TMP_Text scoreText;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (gameRunning == false)
    {
      return;
    }

    if (leftHandGesture != handGesture.LeftHandGesture)
    {
      leftHandGesture = handGesture.LeftHandGesture;
      for (int x = 0; x < leftSideMonster.Count; x++)
      {
        if (leftSideMonster[x] == null)
        {
          leftSideMonster.RemoveAt(x);
        }
      }
      checkKill(true);
    }

    if (rightHandGesture != handGesture.RightHandGesture)
    {
      rightHandGesture = handGesture.RightHandGesture;
      for (int x = 0; x < rightSideMonster.Count; x++)
      {
        if (rightSideMonster[x] == null)
        {
          rightSideMonster.RemoveAt(x);
        }
      }
      checkKill(false);
    }
    

    if (leftSideSpawning == false)
    {
      StartCoroutine(RandomTimeSpawn(true));
      leftSideSpawning = true;
    }

    if (rightSideSpawning == false)
    {
      StartCoroutine(RandomTimeSpawn(false));
      rightSideSpawning = true;
    }
  }

  void randomSpawnMonster(bool left)
  {
    GameObject spawnMonster;
    int random = Random.Range(0, monsterPrefabs.Count);
    spawnMonster = Instantiate(monsterPrefabs[random], tempMonster.transform);
    monster mons = spawnMonster.GetComponent<monster>();
    mons.left = left;
    if (random == 2)
    {
      mons.run = true;
    }
    if (left)
    {
      for (int x = 0; x < leftSideMonster.Count; x++)
      {
        if (leftSideMonster[x] == null)
        {
          leftSideMonster.RemoveAt(x);
        }
      }
      spawnMonster.transform.position = LeftSideSpawnPoint.position;
      leftSideMonster.Add(mons);
    }
    else
    {
      for (int x = 0; x < rightSideMonster.Count; x++)
      {
        if (rightSideMonster[x] == null)
        {
          rightSideMonster.RemoveAt(x);
        }
      }
      spawnMonster.transform.position = RightSideSpawnPoint.position;
      rightSideMonster.Add(mons);
    }
  }


  IEnumerator RandomTimeSpawn(bool left)
  {
    float time = Random.Range(3f, 5f);
    yield return new WaitForSecondsRealtime(time);
    randomSpawnMonster(left);
    if (left)
    {
      leftSideSpawning = false;
    }
    else
    {
      rightSideSpawning = false;
    }
  }

  void checkKill(bool isLeft)
  {
    List<float> disOfMonster = new List<float>();
    if (isLeft)
    {
      if (leftSideMonster.Count <= 0)
      {
        return;
      }
      foreach (monster mons in leftSideMonster)
      {
        if (mons.dead == false)
        {
          disOfMonster.Add(Vector3.Distance(mons.gameObject.transform.position, Camera.main.transform.position));
        }
        else
        {
          disOfMonster.Add(9999f);
          //leftSideMonster.Remove(mons);
        }
      }
      int cloestMonsIndex = disOfMonster.IndexOf(Mathf.Min(disOfMonster.ToArray()));
      monster cloestMonster = leftSideMonster[cloestMonsIndex];
      if (cloestMonster.gesture == leftHandGesture)
      {
        cloestMonster.die();
        skillEffect(cloestMonster.gesture, cloestMonster.transform.position, true);
        score++;
        scoreText.text = "Score: " + score.ToString();
      }
    }
    else
    {
      if (rightSideMonster.Count <= 0)
      {
        return;
      }
      foreach (monster mons in rightSideMonster)
      {
        if (mons.dead == false)
        {
          disOfMonster.Add(Vector3.Distance(mons.gameObject.transform.position, Camera.main.transform.position));
        }
        else
        {
          disOfMonster.Add(9999f);
          //rightSideMonster.Remove(mons);
        }
      }
      int cloestMonsIndex = disOfMonster.IndexOf(Mathf.Min(disOfMonster.ToArray()));
      monster cloestMonster = rightSideMonster[cloestMonsIndex];
      if (cloestMonster.gesture == rightHandGesture)
      {
        cloestMonster.die();
        skillEffect(cloestMonster.gesture, cloestMonster.transform.position, false);
        score++;
        scoreText.text = "Score: " + score.ToString();
      }
    }
  }

  void skillEffect(HandGesture.Gesture ges, Vector3 targetPos, bool isLeft)
  {
    int effectIndex = 0;
    switch (ges)
    {
      case HandGesture.Gesture.IndexUp:
        effectIndex = (int)effects.beam;
        break;
      case HandGesture.Gesture.PinkyUp:
        effectIndex = (int)effects.fire;
        break;
      case HandGesture.Gesture.IndexAndPinkyUp:
        effectIndex = (int)effects.thunder;
        break;
    }
    GameObject createdEffect;
    switch (effectIndex)
    {
      case (int)effects.beam:
        createdEffect = Instantiate(effectsPrefabs[(int)effects.beam], tempMonster.transform);
        createdEffect.GetComponent<skillsEffect>().targetPosition = targetPos;
        break;
      case (int)effects.fire:
        createdEffect = Instantiate(effectsPrefabs[(int)effects.fire], tempMonster.transform);
        createdEffect.GetComponent<skillsEffect>().targetPosition = targetPos;
        break;
      case (int)effects.thunder:
        createdEffect = Instantiate(effectsPrefabs[(int)effects.thunder], tempMonster.transform);
        createdEffect.GetComponent<skillsEffect>().targetPosition = targetPos;
        break;
      default:
        createdEffect = Instantiate(effectsPrefabs[(int)effects.beam], tempMonster.transform);
        createdEffect.GetComponent<skillsEffect>().targetPosition = targetPos;
        break;
    }

    if (isLeft)
    {
      createdEffect.transform.position = LeftSideEffectPoint.transform.position;
    }
    else
    {
      createdEffect.transform.position = RightSideEffectPoint.transform.position;
    }
  }

  public void SendScore()
  {
    localProperties.AddScore(score);
  }
}
