using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    public GameObject[] Fruits;
    public GameObject GridBackground;

    public static int GridRows = 5;
    public static int GridCols = 5;

    public static bool HasNullCells = true;

    public GameObject[,] FruitsGrid = new GameObject[GridRows, GridCols];

    void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int row = 0; row < FruitsGrid.GetLength(0); row++)
        {
            for (int col = 0; col < FruitsGrid.GetLength(1); col++)
            {
                Instantiate(GridBackground, new Vector3(row, col, 1), Quaternion.identity, transform);
                var fruit = Instantiate(Fruits[Random.Range(0, Fruits.Length)], new Vector3(row, col, 0), Quaternion.identity, transform);
                FruitsGrid[row, col] = fruit;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                var matchingObjectsList = new List<CustomGameObject>();
                var matchingObjects = CheckForAdjacentObjects(hit.collider.gameObject, matchingObjectsList);
                if (matchingObjects.Count > 1)
                {
                    foreach (var item in matchingObjects)
                    {
                        if (item.GameObject != null)
                        {
                            FruitsGrid[(int)item.GameObject.transform.position.x, (int)item.GameObject.transform.position.y] = null;
                            Destroy(item.GameObject);
                        }
                    }

                    while (HasNullCells)
                    {
                        CheckForNullCellsInGrid();
                    }
                    HasNullCells = true;

                    SpawnNewTiles();

                    var isReadyToShuffle = IsReadyToShuffle(FruitsGrid);
                    if (isReadyToShuffle)
                    {
                        ShuffleGrid();
                    }
                }
            }
        }
    }

    private void ShuffleGrid()
    {
        for (int row = 0; row < FruitsGrid.GetLength(0); row++)
        {
            for (int col = FruitsGrid.GetLength(1) - 1; col >= 0; col--)
            {
                Destroy(FruitsGrid[row, col]);
                FruitsGrid[row, col] = null;
            }
        }

        InitializeGrid();
    }

    private bool IsReadyToShuffle(GameObject[,] fruitsGrid)
    {
        for (int row = 0; row < FruitsGrid.GetLength(0); row++)
        {
            for (int col = FruitsGrid.GetLength(1) - 1; col >= 0; col--)
            {
                var obj = fruitsGrid[row, col];

                obj.GetComponent<Collider2D>().enabled = false;

                RaycastHit2D downHit = Physics2D.Raycast(obj.transform.position, -Vector2.up);

                RaycastHit2D upHit = Physics2D.Raycast(obj.transform.position, Vector2.up);

                RaycastHit2D rightHit = Physics2D.Raycast(obj.transform.position, Vector2.right);

                RaycastHit2D leftHit = Physics2D.Raycast(obj.transform.position, -Vector2.right);

                obj.GetComponent<Collider2D>().enabled = true;

                if (downHit.collider != null && obj.tag == downHit.collider.tag && downHit.collider.gameObject != obj)
                {
                    return false;
                }
                else if (upHit.collider != null && obj.tag == upHit.collider.tag && upHit.collider.gameObject != obj)
                {
                    return false;
                }
                else if (rightHit.collider != null && obj.tag == rightHit.collider.tag && rightHit.collider.gameObject != obj)
                {
                    return false;
                }
                else if (leftHit.collider != null && obj.tag == leftHit.collider.tag && leftHit.collider.gameObject != obj)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void SpawnNewTiles()
    {
        for (int row = 0; row < FruitsGrid.GetLength(0); row++)
        {
            for (int col = FruitsGrid.GetLength(1) - 1; col > 0; col--)
            {
                if (FruitsGrid[row, col] == null)
                {
                    Instantiate(GridBackground, new Vector3(row, col, 1), Quaternion.identity, transform);
                    var fruit = Instantiate(Fruits[Random.Range(0, Fruits.Length)], new Vector3(row, col, 0), Quaternion.identity, transform);
                    FruitsGrid[row, col] = fruit;
                }
            }
        }
    }

    private void CheckForNullCellsInGrid()
    {
        bool hasNullCells = false;
        for (int row = 0; row < FruitsGrid.GetLength(0); row++)
        {
            for (int col = FruitsGrid.GetLength(1) - 1; col > 0; col--)
            {
                try
                {
                    if (col > 0 && FruitsGrid[row, col - 1] == null && FruitsGrid[row, col] != null)
                    {
                        var fruit = FruitsGrid[row, col];
                        FruitsGrid[row, col].transform.position = new Vector2(row, col - 1);
                        FruitsGrid[row, col] = null;
                        FruitsGrid[row, col - 1] = fruit;
                        hasNullCells = true;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log($"Error on X:{row} Y:{col}");
                }
            }
        }
        HasNullCells = hasNullCells;
    }

    private List<CustomGameObject> CheckForAdjacentObjects(GameObject obj, List<CustomGameObject> matchingObjectsList)
    {
        var go = new CustomGameObject
        {
            GameObject = obj,
            IsChecked = true
        };

        go.GameObject.GetComponent<Collider2D>().enabled = false;

        RaycastHit2D downHit = Physics2D.Raycast(obj.transform.position, -Vector2.up);

        RaycastHit2D upHit = Physics2D.Raycast(obj.transform.position, Vector2.up);

        RaycastHit2D rightHit = Physics2D.Raycast(obj.transform.position, Vector2.right);

        RaycastHit2D leftHit = Physics2D.Raycast(obj.transform.position, -Vector2.right);

        obj.GetComponent<Collider2D>().enabled = true;

        if (!matchingObjectsList.Contains(go))
        {
            matchingObjectsList.Add(go);
        }

        if (downHit.collider != null && obj.tag == downHit.collider.tag && downHit.collider.gameObject != obj && !matchingObjectsList.Any(x => x.GameObject == downHit.collider.gameObject))
        {
            matchingObjectsList.Add(new CustomGameObject { GameObject = downHit.collider.gameObject, IsChecked = false });
        }
        if (upHit.collider != null && obj.tag == upHit.collider.tag && upHit.collider.gameObject != obj && !matchingObjectsList.Any(x => x.GameObject == upHit.collider.gameObject))
        {
            matchingObjectsList.Add(new CustomGameObject { GameObject = upHit.collider.gameObject, IsChecked = false });
        }
        if (rightHit.collider != null && obj.tag == rightHit.collider.tag && rightHit.collider.gameObject != obj && !matchingObjectsList.Any(x => x.GameObject == rightHit.collider.gameObject))
        {
            matchingObjectsList.Add(new CustomGameObject { GameObject = rightHit.collider.gameObject, IsChecked = false });
        }
        if (leftHit.collider != null && obj.tag == leftHit.collider.tag && leftHit.collider.gameObject != obj && !matchingObjectsList.Any(x => x.GameObject == leftHit.collider.gameObject))
        {
            matchingObjectsList.Add(new CustomGameObject { GameObject = leftHit.collider.gameObject, IsChecked = false });
        }

        foreach (var item in matchingObjectsList.Where(x => !x.IsChecked).ToList())
        {
            matchingObjectsList.Remove(item);
            CheckForAdjacentObjects(item.GameObject, matchingObjectsList);
        }

        return matchingObjectsList.ToList();
    }
}

public class CustomGameObject
{
    public GameObject GameObject { get; set; }

    public bool IsChecked { get; set; }
}
