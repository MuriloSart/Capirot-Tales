using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BeanScript : MonoBehaviour
{
    private Vector2 initialTouchPos;
    private Vector2 finalTouchPos;
    private float moveAngle = 0;
    [SerializeField]
    private Board board;

    [SerializeField]
    private int column;
    [SerializeField]
    private int row;

    public bool matched;

    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private CircleCollider2D circleCollider;

    private void Start()
    {
        column = (int)transform.position.x;
        row = (int)transform.position.y;
        matched = false;

        board = FindObjectOfType<Board>();
    }

    private void OnMouseDown()
    {
        initialTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board = FindObjectOfType<Board>();
        

    }

    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Só efetua o movimento caso a distância seja maior do que 0.15 unidades
        if(Vector2.Distance(initialTouchPos, finalTouchPos) > 0.15f)AngleCalc();
    }

    public void AngleCalc()
    {
        moveAngle = Mathf.Atan2(finalTouchPos.y - initialTouchPos.y, finalTouchPos.x - initialTouchPos.x) * 180 / Mathf.PI;
        MoveBean();
    }


    private void Update()
    {       
        transform.position = Vector2.Lerp(transform.position, new Vector2(column, row), 8 * Time.deltaTime);
        if(matched)
        {
            sr.color = new Color(1, 1, 1, 0.2f);
            circleCollider.enabled = false;  
        }

        FindMatch();
    }

    public void MoveBean()
    {
        BeanScript bean = null;

        if (moveAngle > -45 && moveAngle <= 45 && column < board.width - 1)
        {
            //Move para direita
            bean = board.allBeans[column + 1, row].gameObject.GetComponent<BeanScript>();
        }
        else if (moveAngle < -45 && moveAngle >= -135 && row > 0)
        {
            //Move para baixo
            bean = board.allBeans[column, row - 1].gameObject.GetComponent<BeanScript>();

        }
        else if (moveAngle <= 135 && moveAngle > 45 && row < board.height - 1)
        {
            //Move para cima
            bean = board.allBeans[column, row + 1].gameObject.GetComponent<BeanScript>();
        }
        else if (moveAngle > 135 || moveAngle <= -135 && column > 0)
        {
            //Move para a esquerda
            bean = board.allBeans[column - 1, row].gameObject.GetComponent<BeanScript>();
        }

        if (bean != null && !bean.matched)
        {
        
            int targetColumn = bean.column;
            int targetRow = bean.row;

            //Troca a posição dos gameobjects na matriz do board
            board.allBeans[bean.column, bean.row] = gameObject;
            board.allBeans[column, row] = bean.gameObject;

            bean.NewPosition(new Vector2(column, row));
            NewPosition(new Vector2(targetColumn, targetRow));
            string saveName = bean.name;
            bean.name = name;
            name = saveName;

           

        }

    }

    public void NewPosition(Vector2 newPosition)
    {
        column = (int)newPosition.x;
        row = (int)newPosition.y;
    }

    public void FindMatch()
    {
        if (matched) return;

        if(column > 0 && column <  board.width - 1) {

            GameObject leftBean = board.allBeans[column - 1, row];
            GameObject rightBean = board.allBeans[column + 1, row];

            if (leftBean.tag == this.gameObject.tag && rightBean.gameObject.tag == this.tag)
            {
                matched = true;
                leftBean.GetComponent<BeanScript>().matched = true;
                rightBean.GetComponent<BeanScript>().matched = true;
            }
        }

        if (row > 0 && row < board.height - 1)
        {

            GameObject upperBean = board.allBeans[column, row + 1];
            GameObject bottomBean = board.allBeans[column, row - 1];

            if (upperBean.tag == this.gameObject.tag && bottomBean.gameObject.tag == this.tag)
            {
                matched = true;
                upperBean.GetComponent<BeanScript>().matched = true;
                bottomBean.GetComponent<BeanScript>().matched = true;
            }
        }


    }

    

}
