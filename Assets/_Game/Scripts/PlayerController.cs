using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MAX_IDLE_DISTANCE = 200f;

    private Vector3 mouseStartPoint;
    private Vector3 mouseEndPoint;
    private Direction curDirection;

    public Direction CurDirection { get { return curDirection; } set { curDirection = value; } }

    public enum Direction
    {
        IDLE    = 0,
        FORWARD = 1,
        BACK    = 2,
        LEFT    = 3,
        RIGHT   = 4,
    }

    public void OnInit()
    {
        curDirection = Direction.IDLE;
    }

    public void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPoint = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseEndPoint = Input.mousePosition;
            curDirection = GetDirection();
        }
    }

    private Direction GetDirection()
    {
        Direction direction = Direction.IDLE;

        if(Vector3.Distance(mouseStartPoint, mouseEndPoint) > MAX_IDLE_DISTANCE )
        {
            if (Mathf.Abs(mouseEndPoint.y - mouseStartPoint.y) < MAX_IDLE_DISTANCE )
            {
                if (mouseStartPoint.x > mouseEndPoint.x)
                {
                    direction = Direction.LEFT;
                }
                else if (mouseStartPoint.x < mouseEndPoint.x)
                {
                    direction = Direction.RIGHT;
                }
            }
            else if (Mathf.Abs(mouseEndPoint.x - mouseStartPoint.x) < MAX_IDLE_DISTANCE )
            {
                if (mouseStartPoint.y > mouseEndPoint.y)
                {
                    direction = Direction.BACK;
                }
                else if (mouseStartPoint.y < mouseEndPoint.y)
                {
                    direction = Direction.FORWARD;
                }
            }
        }

        return direction;
    }
}
