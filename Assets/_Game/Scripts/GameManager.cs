using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private bool isWin = false;
    private bool isLose = false;

    [SerializeField] private Player player;
    [SerializeField] private CameraFollower cameraFollower;

    [SerializeField] private GameObject winPanelObj;
    [SerializeField] private GameObject losePanelObj;
    [SerializeField] private GameObject endPanelObj;

    [SerializeField] private TMP_Text textStack;
    [SerializeField] private TMP_Text textLevel;

    public bool IsWin { get { return isWin; } set { isWin = value; } }
    public bool IsLose { get { return isLose; } set { isLose = value; } }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OnInitNextLevel();
    }

    private void OnInit()
    {
        IsWin = false;
        IsLose = false;
    }

    private void HidePanel()
    {
        winPanelObj.SetActive(false);
        losePanelObj.SetActive(false);
        endPanelObj.SetActive(false);
    }

    public void OnInitNextLevel()
    {
        if(LevelManager.Instance.CurLevel + 1 > LevelManager.MAX_LEVEL)
        {
            HidePanel();
            ShowEndPanel();
        } 
        else
        {
            player.OnInit();
            LevelManager.Instance.LoadMapByLevel(++LevelManager.Instance.CurLevel);
            UpdateTextStack();
            UpdateTextLevel();
            HidePanel();
            cameraFollower.OnInit();
            Invoke(nameof(OnInit), 0.1f);
        }
    }

    public void OnInitCurrentLevel()
    {
        player.OnInit();
        LevelManager.Instance.LoadMapByLevel(LevelManager.Instance.CurLevel);
        UpdateTextStack();
        UpdateTextLevel();
        HidePanel();
        cameraFollower.OnInit();
        Invoke(nameof(OnInit), 0.5f);
    }

    public void OnInitLevel(int level)
    {
        player.OnInit();
        LevelManager.Instance.LoadMapByLevel(0);
        UpdateTextStack();
        UpdateTextLevel();
        HidePanel();
        cameraFollower.OnInit();
        Invoke(nameof(OnInit), 0.5f);
    }

    public bool CheckGameOver()
    {
        return IsWin || IsLose;
    }

    public void UpdateTextLevel()
    {
        textLevel.text = "Level " + LevelManager.Instance.CurLevel;
    }

    public void UpdateTextStack()
    {
        textStack.text = "Stack: " + player.BrickStack.Count;
    }

    public void ShowWinPanel()
    {
        winPanelObj.SetActive(true);
    }

    public void ShowLosePanel()
    {
        losePanelObj.SetActive(true);
    }

    public void ShowEndPanel()
    {
        endPanelObj.SetActive(true);
    }    
}
