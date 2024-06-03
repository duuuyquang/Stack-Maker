using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject targetObj;

    [SerializeField] private Vector3 offset = new Vector3(0f, 11f, -7f);
    [SerializeField] private float speed = 1f;

    private Vector3 targetPos;

    void LateUpdate()
    {
        targetPos = targetObj.transform.position + offset;
        //transform.position = Vector3.Lerp(transform.position, targetPos, 2f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void OnInit()
    {
        targetPos = targetObj.transform.position + offset;
        transform.position = targetPos;
    }
}
