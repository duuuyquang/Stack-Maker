using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{

    [SerializeField] private GameObject chestCloseObj;
    [SerializeField] private GameObject chestOpenObj;
    [SerializeField] private GameObject fireworkObj;
    public Transform winBoxTransform;

    private GameManager gameManager;

    public void SetFinishAnimation()
    {
        foreach (Transform obj in fireworkObj.transform)
        {
            obj.GetComponent<ParticleSystem>().Play();
        }
        chestCloseObj.SetActive(false);
        chestOpenObj.SetActive(true);
        GameManager.Instance.ShowWinPanel();
    }
}
