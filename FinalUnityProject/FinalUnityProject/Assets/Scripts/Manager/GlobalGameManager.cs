using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public enum PLAYER_TYPE
    {
        PLAYER = 1,
        TOUCH,
    }

    public static GlobalGameManager _instance = null;

    [System.NonSerialized]
    public PhotonManager photonManager = null;

    [System.NonSerialized]
    public MainManager mainManager = null;

    [System.NonSerialized]
    public  GameManager gamemanager = null;

    [System.NonSerialized]
    public TouchManager touchManager = null;

    [System.NonSerialized]
    public CameraManager cameraManager = null;

    [System.NonSerialized]
    public ServerManager serverManager = null;

    [System.NonSerialized]
    public int UserId = 0;

    [System.NonSerialized]
    public int gameRestartCount = 0;

    [System.NonSerialized]
    public float clearTime = 0f;

    [System.NonSerialized]
    public int friendly = 110;

    [System.NonSerialized]
    public string teamName = "無名のチーム";

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Application.targetFrameRate = 60; //60FPSに設定  
        DontDestroyOnLoad(gameObject);
    }

    public bool CheckUserId(int id)
    {
        bool flag = false;
        if (_instance.UserId == id)
        {
            flag = true;
        }
        return flag;
    }

}
