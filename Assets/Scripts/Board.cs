using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allBeans;
    public GameObject[] beans;

    public bool beanMoving = false;
    void Start()
    {
        width = 5;
        height = 8;

        allTiles = new BackgroundTile[width, height];  
        allBeans = new GameObject[width, height];
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector2(transform.position.x + i, transform.position.y + j), Quaternion.identity);
                tile.name = (i + "," + j);
                GameObject bean = Instantiate(beans[Random.Range(0, beans.Length)], tile.transform.position, Quaternion.identity);
                bean.name = tile.name;
                allBeans[i, j] = bean;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
           
    }



}
