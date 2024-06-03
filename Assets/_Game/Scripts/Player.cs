using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const int   UNIT_BASE = 1;
    public const float UNIT_DISTANCE_BETWEEN_BRICKS = 0.3f;

    public const float CLEAR_EACH_BRICK_RATE = 0.03f;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private GameObject brickObj;

    [SerializeField] private Transform avatarTransform;
    [SerializeField] private Transform brickStackTransform;

    [SerializeField] private Animator playerAnimator;

    [SerializeField] private float speed = 10f;

    private bool isMoving = false;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 targetPos;

    private const string ANIM_NAME_DEFAULT = "default";
    private const string ANIM_NAME_WIN = "isWin";
    private string curAnimName = ANIM_NAME_DEFAULT;

    private GameManager gameManager => GameManager.Instance;

    private List<GameObject> brickStacks = new List<GameObject>();
    public List<GameObject> BrickStack { get { return brickStacks; } set { brickStacks = value; } }
    public Transform AvatarTransform { get { return avatarTransform; } }
    public Transform BrickStackTransform { get { return brickStackTransform; } }

    void Update()
    {
        if(!gameManager.CheckGameOver())
        {
            ProcessMoving();
        }
    }

    public void OnInit()
    {
        brickStacks.Clear();
        transform.position = startPosition;
        targetPos = startPosition;
        playerController.OnInit();
        SetAnimation(ANIM_NAME_DEFAULT);
    }

    protected void SetAnimation(string animName)
    {
        if (curAnimName != animName)
        {
            playerAnimator.ResetTrigger(animName);
            playerAnimator.Rebind();
            curAnimName = animName;
            playerAnimator.SetTrigger(curAnimName);
        }
    }

    private void ProcessMoving()
    {
        if (!isMoving)
        {
            playerController.GetInput();
            targetPos = GetTargetPositionByDirection(playerController.CurDirection);
            if (targetPos != transform.position)
            {
                isMoving = true;
                TurnAvatarByDirection(playerController.CurDirection);
            }
        }
        else
        {
            Move(targetPos);
        }
    }

    private void Move(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            isMoving = false;
        }
    }

    private void TurnAvatarByDirection(PlayerController.Direction direction)
    {
        switch (direction)
        {
            case PlayerController.Direction.IDLE:
                avatarTransform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case PlayerController.Direction.FORWARD:
                avatarTransform.eulerAngles = new Vector3(0, -125, 0);
                break;
            case PlayerController.Direction.BACK:
                avatarTransform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case PlayerController.Direction.LEFT:
                avatarTransform.eulerAngles = new Vector3(0, 125, 0);
                break;
            case PlayerController.Direction.RIGHT:
                avatarTransform.eulerAngles = new Vector3(0, -45, 0);
                break;
        }
    }

    private Vector3 GetTargetPositionByDirection(PlayerController.Direction direction)
    {
        Vector3 finalTargetPos = transform.position;
        Vector3 nextPos = Vector3.zero;

        switch (direction)
        {
            case PlayerController.Direction.IDLE:
                break;
            case PlayerController.Direction.FORWARD:
                nextPos = new Vector3(0, 0, UNIT_BASE);
                break;
            case PlayerController.Direction.BACK:
                nextPos = new Vector3(0, 0, -UNIT_BASE);
                break;
            case PlayerController.Direction.LEFT:
                nextPos = new Vector3( -UNIT_BASE, 0, 0);
                break;
            case PlayerController.Direction.RIGHT:
                nextPos = new Vector3(UNIT_BASE, 0, 0);
                break;
        }

        RayCastTillLastPos(nextPos, ref finalTargetPos);

        return finalTargetPos;
    }

    private void RayCastTillLastPos(Vector3 nextPos, ref Vector3 finalTargetPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(finalTargetPos, nextPos, out hit, UNIT_BASE))
        {
            if ( hit.collider != null && IsValidTag(hit.collider))
            {
                finalTargetPos += nextPos;
                RayCastTillLastPos(nextPos, ref finalTargetPos);
            }
        }
    }

    private bool IsValidTag(Collider collider)
    {
        return  collider.tag == TagManager.PIVOT    ||
                collider.tag == TagManager.UNPIVOT  || 
                collider.tag == TagManager.FINISH;
    }

    public void CollectBrick()
    {
        GameObject tempObj = Instantiate(brickObj, Vector3.zero, brickObj.transform.rotation);
        tempObj.transform.SetParent(BrickStackTransform);
        tempObj.transform.localPosition = new Vector3(0, brickStacks.Count * UNIT_DISTANCE_BETWEEN_BRICKS, 0);
        brickStacks.Add(tempObj);

        avatarTransform.Translate(0, UNIT_DISTANCE_BETWEEN_BRICKS, 0);

        GameManager.Instance.UpdateTextStack();
    }

    public void RemoveBrick()
    {
        if(brickStacks.Count > 0)
        {
            //GameObject tempObj = BrickStackObj.transform.GetChild(brickStacks.Count - 1).gameObject;
            GameObject tempObj = brickStacks[brickStacks.Count - 1];
            brickStacks.Remove(tempObj);
            Destroy(tempObj);

            avatarTransform.Translate(0, -UNIT_DISTANCE_BETWEEN_BRICKS, 0);

            GameManager.Instance.UpdateTextStack();
        }
        else
        {
            GameManager.Instance.IsLose = true;
            GameManager.Instance.ShowLosePanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TagManager.FINISH)
        {
            FinishPoint tempObj = other.GetComponent<FinishPoint>();
            StartCoroutine(FinishMove(tempObj));
        }
    }

    IEnumerator FinishMove(FinishPoint finishObj)
    {
        GameManager.Instance.IsWin = true;
        Vector3 targetPos = new Vector3(finishObj.winBoxTransform.position.x, transform.position.y, finishObj.winBoxTransform.position.z);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(.1f);

        for (int i = brickStacks.Count - 1; i >= 0; i--)
        {
            Destroy(BrickStackTransform.GetChild(i).gameObject);
            AvatarTransform.Translate(0, -UNIT_DISTANCE_BETWEEN_BRICKS, 0);
            yield return new WaitForSeconds(CLEAR_EACH_BRICK_RATE);
        }

        brickStacks.Clear();
        SetAnimation(ANIM_NAME_WIN);

        yield return new WaitForSeconds(.5f);

        finishObj.SetFinishAnimation();
    }
}
