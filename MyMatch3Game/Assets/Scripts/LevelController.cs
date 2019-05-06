using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public GameObject levelObject;
    private List<GameObject> Clones;
    private GameObject CurrentLevelObject;

    public GameObject GameHero;
    void Start()
    {
        Clones = new List<GameObject>();
        CurrentLevelObject = Instantiate(levelObject as GameObject, new Vector3(-1, 4.25f), Quaternion.identity, transform);
        Clones.Add(CurrentLevelObject);
    }

    void Update()
    {
        CheckIfEdgeIsReached(CurrentLevelObject);
    }

    private void CheckIfEdgeIsReached(GameObject clone)
    {
        var xSize = clone.GetComponent<Collider2D>().bounds.size.x;
        var offset = Math.Round(clone.transform.position.x + xSize - 5);
        if (Math.Round(GameHero.transform.position.x) == offset)
        {
            CurrentLevelObject = Instantiate(clone, new Vector3(xSize - 1.91f, clone.transform.position.y, clone.transform.position.z), Quaternion.identity, transform);
            Clones.Add(CurrentLevelObject);
        }
    }
}
