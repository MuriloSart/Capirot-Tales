using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState state = GameState.move;

    public int width;
    public int height;
    public int offSet = 20;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allBeans;
    public GameObject[] beans;

    private int beanToUse;

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
                GameObject tile = Instantiate(tilePrefab, new Vector2(transform.position.x + i, transform.position.y + j + offSet), Quaternion.identity);
                tile.name = (i + "," + j);

                int maxIterations = 0;
                beanToUse = Random.Range(0, beans.Length);
                while(MatchesAt(i, j, beans[beanToUse]) && maxIterations < 100)
                {
                    maxIterations++;
                    beanToUse = Random.Range(0, beans.Length);
                }

                GameObject bean = Instantiate(beans[beanToUse], tile.transform.position, Quaternion.identity);
                bean.GetComponent<BeanScript>().column = i;
                bean.GetComponent<BeanScript>().row = j;
                bean.name = tile.name;
                allBeans[i, j] = bean;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject bean)
    {
        if(column > 1 && row > 1)
        {
            if (allBeans[column -1, row].tag == bean.tag && allBeans[column -2, row].tag == bean.tag)
                return true;
            if (allBeans[column, row - 1].tag == bean.tag && allBeans[column, row - 2].tag == bean.tag)
                return true;
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allBeans[column, row - 1].tag == bean.tag && allBeans[column, row - 2].tag == bean.tag)
                    return true; 
            }
            if (column > 1)
            {
                if (allBeans[column - 1, row].tag == bean.tag && allBeans[column - 2, row].tag == bean.tag)
                    return true;
            }
        }


        return false;
    }

    public void DelayedDestroyMatches()
    {
        Invoke(nameof(DestroyMatches), 1f);
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allBeans[column, row].GetComponent<BeanScript>().matched)
        {
            Destroy(allBeans[column, row]);
            allBeans[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allBeans[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }
    
    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allBeans[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    BeanScript script = allBeans[i, j].GetComponent<BeanScript>();
                    script.row -= nullCount;
                    script.previousRow -= nullCount;
                    allBeans[script.column, script.row] = script.gameObject;
                    allBeans[i, j] = null;
                   

                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allBeans[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int beanToUse = Random.Range(0, beans.Length);
                    GameObject piece = Instantiate(beans[beanToUse], tempPosition, Quaternion.identity);
                    allBeans[i, j] = piece;
                    piece.GetComponent<BeanScript>().column = i;
                    piece.GetComponent<BeanScript>().row = j;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                return allBeans[i, j]?.GetComponent<BeanScript>()?.matched ?? true;
            }
        }

        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        state = GameState.move;
    }
}
