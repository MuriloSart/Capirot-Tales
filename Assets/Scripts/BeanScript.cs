using JetBrains.Annotations;
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

    public int column;
    public int row;

    public bool matched;

    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private CircleCollider2D circleCollider;

    private BeanScript otherBean;
    [SerializeField]
    public int previousColumn;
    [SerializeField]
    public int previousRow;

    IEnumerator matchAfterMove = null;

    bool lastMatch = false;

    private void Start()
    {
        matched = false;
        board = FindObjectOfType<Board>();
        name = (int)transform.position.x + "," + (int)transform.position.y;

    }

    private void Update()
    {

        float distanceToTargetPos = Vector2.Distance(transform.position, new Vector2(column, row));


        if (distanceToTargetPos > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(column, row), 8 * Time.deltaTime);
        }


        if (matched)
        {
            sr.color = new Color(1, 1, 1, 0.2f);
            circleCollider.enabled = false;
        }

        FindMatch();
    }

    private void OnMouseDown()
    {
        if (board.state == GameState.move)
        {
            initialTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.state == GameState.move)
        { 
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Só efetua o movimento caso a distância seja maior do que 0.15 unidades
            if(Vector2.Distance(initialTouchPos, finalTouchPos) > 0.15f && matchAfterMove == null && !board.beanMoving) 
                AngleCalc();
        }
        else
            board.state = GameState.move;
    }

    public void AngleCalc()
    {
        moveAngle = Mathf.Atan2(finalTouchPos.y - initialTouchPos.y, finalTouchPos.x - initialTouchPos.x) * 180 / Mathf.PI;
        if(Vector2.Distance(transform.position, new Vector2(column, row)) < 0.15f) 
            MoveBean();
        board.state = GameState.wait;
    }




    public void MoveBean() //Mexendo o Bean ao de acordo com o angulo que o mouse arrasta
    {

        BeanScript bean = null;

        

        if (moveAngle > -45 && moveAngle <= 45 && column < board.width - 1)
        {
            //Move para direita
            bean = board.allBeans[column + 1, row].gameObject.GetComponent<BeanScript>();
            previousColumn = column;
            previousRow = row;
        }
        else if (moveAngle < -45 && moveAngle >= -135 && row > 0)
        {
            //Move para baixo
            bean = board.allBeans[column, row - 1].gameObject.GetComponent<BeanScript>();
            previousColumn = column;
            previousRow = row;

        }
        else if (moveAngle <= 135 && moveAngle > 45 && row < board.height - 1)
        {
            //Move para cima
            bean = board.allBeans[column, row + 1].gameObject.GetComponent<BeanScript>();
            previousColumn = column;
            previousRow = row;
        }
        else if (moveAngle > 135 || moveAngle <= -135 && column > 0)
        {
            //Move para a esquerda
            bean = board.allBeans[column - 1, row].gameObject.GetComponent<BeanScript>();
            previousColumn = column;
            previousRow = row;
        }

        if (bean != null && !bean.matched)
        {
            board.beanMoving = true;
            otherBean = bean;

            int otherColumn = otherBean.column;
            int otherRow = otherBean.row;

            otherBean.MoveToPosition(this);

            MoveToPosition(otherColumn, otherRow);


            if (matchAfterMove == null)
            {
                matchAfterMove = CheckMatchAfterMove();
                StartCoroutine(matchAfterMove);
            }

        }

    }

    IEnumerator CheckMatchAfterMove()
    {
        yield return new WaitForSeconds(.5f);

        if(otherBean != null)
        {

            if (!otherBean.matched && !matched)
            {

                board.allBeans[otherBean.column, otherBean.row] = gameObject;
                board.allBeans[column, row] = otherBean.gameObject;

                otherBean.row = row;
                otherBean.column = column;
                row = previousRow;
                column = previousColumn;

                yield return new WaitForSeconds(.5f);
                board.state = GameState.move;
            }
            else
            {
                previousColumn = column;
                previousRow = row;
                otherBean.previousColumn = otherBean.column;
                otherBean.previousRow = otherBean.row;

                board.DestroyMatches();
            }
        }
        else
        {
            previousColumn = column;
            previousRow = row;
            otherBean.previousColumn = otherBean.column;
            otherBean.previousRow = otherBean.row;
        }
        otherBean = null;
        matchAfterMove = null;
        board.beanMoving = false;
    }


    public void MoveToPosition(BeanScript target)
    {

        name = target.column + "," + target.row;

        column = target.column;
        row = target.row;

        board.allBeans[target.column, target.row] = gameObject;

    }


    public void MoveToPosition(int column, int row)
    {
        name = column + "," + row;

        this.column = column;
        this.row = row;

        board.allBeans[column, row] = gameObject;
    }



    public void FindMatch()
    {
        if (matched && lastMatch == true) return;

        if(column > 0 && column <  board.width - 1) {

            GameObject leftBean = board.allBeans[column - 1, row];
            GameObject rightBean = board.allBeans[column + 1, row];

            if(leftBean != null && rightBean != null) 
            {
                if (leftBean.tag == this.gameObject.tag && rightBean.gameObject.tag == this.tag)
                {
                    matched = true;
                    leftBean.GetComponent<BeanScript>().matched = true;
                    rightBean.GetComponent<BeanScript>().matched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {

            GameObject upperBean = board.allBeans[column, row + 1];
            GameObject bottomBean = board.allBeans[column, row - 1];

            if (upperBean != null && bottomBean != null)
            {
                if (upperBean.tag == this.gameObject.tag && bottomBean.gameObject.tag == this.tag)
                {
                    matched = true;
                    upperBean.GetComponent<BeanScript>().matched = true;
                    bottomBean.GetComponent<BeanScript>().matched = true;
                }
            }  
        }

        if (matched)
        {
            lastMatch = true;
            board.beanMoving = false;
            board.DelayedDestroyMatches();
        }

    }

}
