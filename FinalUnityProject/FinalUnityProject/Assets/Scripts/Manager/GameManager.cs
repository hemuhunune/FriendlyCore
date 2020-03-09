using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Photon.MonoBehaviour
{

    [SerializeField] GameObject _playerPrefab = default;
    [SerializeField] GameObject _touchObjectPrefab = default;
    [SerializeField] GameObject _enemyObjectPrefab = default;
    [SerializeField] GameObject playerStart = default;

    public bool leftButtonTouch = false;
    public bool rightButtonTouch = false;
    public bool jumpButtonTouch = false;

    public GameObject lastTouchObject = default;
    public int friendly = 110;
    private float _gameTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.PLAYER))
        {
            InitPhotonPlayerObject();
            Debug.Log("プレイヤー生成");
        }
        else if(GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.TOUCH))
        {
            InitPhotonTouchObject();
            Debug.Log("タッチオブジェクト生成");
            InitPhotonEnemyObject();
            Debug.Log("敵オブジェクト生成");
        }
        GlobalGameManager._instance.gamemanager = this;
    }

    private void Update()
    {
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.PLAYER))
        {
            _gameTime = Time.time;
        }
    }

    public void FriendlyUpdate(int num)
    {
        friendly += num;
        if(friendly < 0)
        {
            friendly = 0;
        }
    }

    /// <summary>
    /// プレイヤー生成
    /// </summary>
    private void InitPhotonPlayerObject()
    {
        GameObject obj = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity, 0);

        obj.transform.localPosition = playerStart.transform.localPosition;

    }
    /// <summary>
    /// タッチオブジェクト生成
    /// </summary>
    private void InitPhotonTouchObject()
    {
        PhotonNetwork.Instantiate(_touchObjectPrefab.name, Vector3.zero, Quaternion.identity, 0);
       
    }
    /// <summary>
    /// 敵オブジェクト生成
    /// </summary>
    private void InitPhotonEnemyObject()
    {
        PhotonNetwork.Instantiate(_enemyObjectPrefab.name, Vector3.zero, Quaternion.identity, 0);
    }

    /// <summary>
    /// 左ボタンを押した瞬間
    /// </summary>
    public void LeftButtonTouchDown()
    {
        leftButtonTouch = true;
    }
    /// <summary>
    /// 左ボタンを離した瞬間
    /// </summary>
    public void LeftButtonTouchUp()
    {
        leftButtonTouch = false;
    }
    /// <summary>
    /// 右ボタンを押した瞬間
    /// </summary>
    public void RightButtonTouchDown()
    {
        rightButtonTouch = true;
    }
    /// <summary>
    /// 右ボタンを離した瞬間
    /// </summary>
    public void RightButtonTouchUp()
    {
        rightButtonTouch = false;
    }
    /// <summary>
    /// ジャンプボタンを押した瞬間
    /// </summary>
    public void JumpButtonTouchDown()
    {
        jumpButtonTouch = true;
    }
    /// <summary>
    /// ジャンプボタンを離した時
    /// </summary>
    public void JumpButtonTouchUp()
    {
        //if (_inputVertical > 0.0f)
        //{
        //  _inputVertical *= 0.5f;
        //}
        jumpButtonTouch = false;
        Debug.Log("ジャンプボタン離したよ");
    }
    /// <summary>
    /// 最初から
    /// </summary>
    public void SceneReset()
    {
        GlobalGameManager._instance.gameRestartCount += 1;
        PhotonNetwork.DestroyAll();
        PhotonNetwork.isMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("SceneGame");
    }
    /// <summary>
    /// リザルトシーンへ遷移
    /// </summary>
    public void SceneChangeResult()
    {
        GlobalGameManager._instance.clearTime = _gameTime;
        if(friendly > 100)
        {
            friendly = 100;
        }
        GlobalGameManager._instance.friendly = friendly;
        
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.PLAYER))
        {
            GlobalGameManager._instance.serverManager.SetJsonFromWww();
            Debug.Log("シーン遷移");
            PhotonNetwork.LoadLevel("SceneResult");
        }
    }
}
