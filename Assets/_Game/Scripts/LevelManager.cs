using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    private int curLevel = 0;

    public const int MAX_LEVEL = 3;

    public int CurLevel { get { return curLevel; } set { curLevel = value; } }

    private GameObject curLevelObj;

    void Awake()
    {
        instance = this;
    }

    public void LoadMapByLevel(int level)
    {
        if (curLevelObj != null)
        {
            Destroy(curLevelObj.gameObject);
        }
        CurLevel = level;
        Object temp = Resources.Load("Level/Level" + level);
        if (temp == null)
        {
            CurLevel = 1;
            temp = Resources.Load("Level/Level" + CurLevel);
        }

        curLevelObj = (GameObject)Instantiate(temp, Vector3.zero, Quaternion.identity);
        curLevelObj.transform.parent = transform;
    }
}
