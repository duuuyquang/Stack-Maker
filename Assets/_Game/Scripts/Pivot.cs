using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    [SerializeField] private GameObject brickObj;

    private bool isCollided = false;

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == TagManager.PLAYER && !isCollided)
        {
            isCollided = true;
            brickObj.SetActive(false);
            other.GetComponent<Player>().CollectBrick();
        }
    }
}
