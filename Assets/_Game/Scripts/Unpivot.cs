using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unpivot : MonoBehaviour
{
    private bool isCollided = false;

    [SerializeField] private GameObject brickObj;
    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == TagManager.PLAYER && !isCollided)
        {
            isCollided = true;
            other.GetComponent<Player>().RemoveBrick();
            brickObj.SetActive(true);
        }
    }
}
