using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;

    void Start()
    {
        width = 5;
        height = 8;

        allTiles = new BackgroundTile[width, height];  
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Instantiate(tilePrefab, new Vector2(transform.position.x + i, transform.position.y + j), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
