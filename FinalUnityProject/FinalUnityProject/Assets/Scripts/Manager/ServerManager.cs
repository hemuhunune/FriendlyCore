using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Text
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ServerManager : Photon.MonoBehaviour
{
    [SerializeField] private Text _displayField = default;
    [SerializeField] private Scrollbar _scrollbar = default;
    private bool _isGetData = false;
    
    private void Start()
    {
        _displayField.text = "wait...";
        GetJsonFromWww();
        GlobalGameManager._instance.serverManager = this;
    }

    /// <summary>
    /// Raises the click get json from www event.
    /// </summary>
    public void OnClickGetMessagesApi()
    {
        _displayField.text = "wait...";
        GetJsonFromWww();
    }

    /// <summary>
    /// Raises the click get json from www event.
    /// </summary>
    public void OnClickSetMessageApi()
    {
        _displayField.text = "wait...";
        SetJsonFromWww();
    }

    private IEnumerator WaitGetData()
    {
        bool isLoop = true;
        while(isLoop == false)
        {
            if(_isGetData == true)
            {
                isLoop = false;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    /// <summary>
    /// Gets the json from www.
    /// </summary>
    private void GetJsonFromWww()
    {
        // APIが設置してあるURLパス
        const string url = "http://localhost/FinalProject/finalgame/getGameData/";

        // Wwwを利用して json データ取得をリクエストする
        StartCoroutine(GetGameData(url, CallbackWwwSuccess, CallbackWwwFailed));
    }

    /// <summary>
    /// Callbacks the www success.
    /// </summary>
    /// <param name="response">Response.</param>
    private void CallbackWwwSuccess(string response)
    {
        // json データ取得が成功したのでデシリアライズして整形し画面に表示する
        List<GameData> messageList = GameDataModel.DeserializeFromJson(response);

        string sStrOutput = "";
        int count = 0;
        //出力
        foreach (GameData message in messageList)
        {
            sStrOutput += string.Format("{0}位 チーム名:「{1}」 仲良し度:{2}%  クリアタイム:{3}秒 \n\n"
                ,count + 1
                ,messageList[count].Name
                ,messageList[count].Friendly
                ,messageList[count].ClearTime);
            count++;
        }
        _displayField.text = sStrOutput;
        _scrollbar.value = 1.0f;
    }

    /// <summary>
    /// Callbacks the www failed.
    /// </summary>
    private void CallbackWwwFailed()
    {
        // jsonデータ取得に失敗した
        _displayField.text = "Www Failed";
    }

    /// <summary>
    /// Callbacks the API success.
    /// </summary>
    /// <param name="response">Response.</param>
    private void CallbackApiSuccess()
    {
        _displayField.text = "データ登録中";
        GetJsonFromWww();
    }

    /// <summary>
    /// Downloads the json.
    /// </summary>
    /// <returns>The json.</returns>
    /// <param name="url">S tgt UR.</param>
    /// <param name="cbkSuccess">Cbk success.</param>
    /// <param name="cbkFailed">Cbk failed.</param>
    private IEnumerator GetGameData(string url, Action<string> cbkSuccess = null, Action cbkFailed = null)
    {
        // WWWを利用してリクエストを送る
        var webRequest = UnityWebRequest.Get(url);

        //タイムアウトの指定
        webRequest.timeout = 5;

        // WWWレスポンス待ち
        yield return webRequest.SendWebRequest();

        if (webRequest.error != null)
        {
            //レスポンスエラーの場合
            Debug.LogError(webRequest.error);
            if (null != cbkFailed)
            {
                cbkFailed();
            }
        }
        else if (webRequest.isDone)
        {
            // リクエスト成功の場合
            Debug.Log($"Success:{webRequest.downloadHandler.text}");
            if (null != cbkSuccess)
            {
                cbkSuccess(webRequest.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Response the check for time out WWW.
    /// </summary>
    /// <returns>The check for time out WWW.</returns>
    /// <param name="webRequest">Www.</param>
    /// <param name="timeout">Timeout.</param>
    private IEnumerator ResponseCheckForTimeOutWWW(UnityWebRequest webRequest, float timeout)
    {
        float requestTime = Time.time;

        while (!webRequest.isDone)
        {
            if (Time.time - requestTime < timeout)
            {
                yield return null;
            }
            else
            {
                Debug.LogWarning("TimeOut"); //タイムアウト
                break;
            }
        }

        yield return null;
    }

    
    public void SetJsonFromWww()
    {
        // APIが設置してあるURLパス
        string sTgtURL = "http://localhost:80/FinalProject/finalgame/setGameData/";

        string name = GlobalGameManager._instance.teamName;
        int clearTime = (int)(GlobalGameManager._instance.clearTime);
        int friendly = GlobalGameManager._instance.friendly;
        StartCoroutine(SetGameData(sTgtURL, name, clearTime, friendly, CallbackApiSuccess, CallbackWwwFailed));
    }
    //リクエストを送信
    private IEnumerator SetGameData(string url, string name, int clearTime , int friendly, Action cbkSuccess = null, Action cbkFailed = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("cleartime", clearTime);
        form.AddField("friendly", friendly);

        // WWWを利用してリクエストを送る
        var webRequest = UnityWebRequest.Post(url, form);

        //タイムアウトの指定
        webRequest.timeout = 5;

        // WWWレスポンス待ち
        yield return webRequest.SendWebRequest();

        if (webRequest.error != null)
        {
            //レスポンスエラーの場合
            Debug.LogError(webRequest.error);
            if (null != cbkFailed)
            {
                cbkFailed();
            }
        }
        else if (webRequest.isDone)
        {

            // リクエスト成功の場合
            Debug.Log($"Success:{webRequest.downloadHandler.text}");
            if (null != cbkSuccess)
            {
                cbkSuccess();
            }
        }
    }
}
