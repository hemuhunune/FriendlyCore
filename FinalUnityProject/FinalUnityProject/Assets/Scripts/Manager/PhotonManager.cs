using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ルーム情報
/// </summary>
public class photonRoomInfo
{
	public string roomName;
	public string roomOwnerID;

	public photonRoomInfo()
	{
		roomName = "";
		roomOwnerID = "";
	}
}


public class PhotonManager : Photon.MonoBehaviour
{
    //ゲームのバージョン
	private const string PHOTON_GAME_VER = "v1.0";
    //ルーム名
	private const string GAMEROOM_NAME = "myroom01";
    //ルーム人数
	private const int GAMEROOM_LIMIT = 2;
	//自分でルームを作成したかどうか
	[HideInInspector] public bool isRoomMake = false;

	[SerializeField] private Text PhotonText = default;

	//ルームのリスト
	public List<photonRoomInfo> roomList = new List<photonRoomInfo>();

	void Start()
	{
        //インスタンス化
		GlobalGameManager._instance.photonManager = this;

		ConnectPhoton();
		Debug.Log("ConnectToPhoton");
	}

	public void ConnectPhoton()
	{
		//LoadLevel() ではなく PhotonNetwork.LoadLevel() を利用する事を許可する。
		//...これを利用することで、mastercliant が loadlevel を行うとたのクライアントもシーン遷移する。
		PhotonNetwork.automaticallySyncScene = true;

		//photonへの接続を行う。
		PhotonNetwork.ConnectUsingSettings(PHOTON_GAME_VER);

		//1秒間に送信するパケット数を修正 これを増やすと同期精度が上がるが通信負荷が増加する。
		PhotonNetwork.sendRate = 60; //def:15
		PhotonNetwork.sendRateOnSerialize = 60; //def:15

		PhotonText.text = "Photonに接続しました";
	}

	//---------------------------------------------
	// photon callback
	/// <summary>
	/// event:photonに接続した
	/// </summary>
	public void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhoton");
	}
	/// <summary>
	/// event:photonが切断した
	/// </summary>
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
	}
	/// <summary>
	/// event:接続失敗
	/// </summary>
	public void OnConnectionFail()
	{
		Debug.Log("OnConnectionFail");
	}
	/// <summary>
	/// event:photon接続失敗
	/// </summary>
	/// <param name="parameters">Parameters.</param>
	public void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton");
	}
	/// <summary>
	/// event:ロビー入室
	/// </summary>
	public void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby");
	}
	/// <summary>
	/// event:ロビー退室
	/// </summary>
	public void OnLeftLobby()
	{
		Debug.Log("OnLeftLobby");
	}
	/// <summary>
	/// Raises the connected to master event.
	/// autoJoinLobby が true 時には OnJoinedLobby が代わりに呼ばれる。
	/// </summary>
	public void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster");
	}
	/// <summary>
	/// event:ルームリストが更新された
	/// </summary>
	public void OnReceivedRoomListUpdate()
	{
		Debug.Log("OnReceivedRoomListUpdate");
	}
	/// <summary>
	/// event:ルーム作成
	/// </summary>
	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		Debug.Log(string.Format("Name:{0}", PhotonNetwork.room.Name));
	}
	/// <summary>
	/// event:ルーム作成失敗
	/// </summary>
	public void OnPhotonCreateRoomFailed()
	{
		Debug.Log("OnPhotonCreateRoomFailed");
	}
	/// <summary>
	/// event:ルーム入室
	/// </summary>
	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
		Debug.Log(string.Format("Name:{0}", PhotonNetwork.room.Name));
	}
	/// <summary>
	/// event:ルーム入室失敗
	/// </summary>
	/// <param name="cause">Cause.</param>
	public void OnPhotonJoinRoomFailed(object[] cause)
	{
		Debug.Log("OnPhotonJoinRoomFailed");
	}
	/// <summary>
	/// event:ランダム入室失敗
	/// </summary>
	public void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed");
	}
	/// <summary>
	/// event:ルーム退室コールバック
	/// </summary>
	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom");
	}
	/// <summary>
	/// event:誰かプレイヤーが接続された
	/// </summary>
	/// <param name="player">Player.</param>
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected");
	}
	/// <summary>
	/// event:誰かプレイヤーの接続が切れた
	/// </summary>
	/// <param name="player">Player.</param>
	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected");
	}
	/// <summary>
	/// event:マスタークライアントが切り替わった
	/// </summary>
	/// <param name="player">Player.</param>
	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched");
	}

	/// <summary>
	/// UnityのGameウィンドウに表示させる
	/// </summary>
	void OnGUI()
	{
		// Photonのステータスをラベルで表示させています
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
    //ルームクリエイト
	public void CreateRoom(string roomName = GAMEROOM_NAME)
	{
		Debug.Log(string.Format("CreateRoom({0})", roomName));

		//UserIdを1に設定
		string userId = "001";
		GlobalGameManager._instance.UserId = 1;
		//カスタムプロパティ
		ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
		customProp.Add("roomName", roomName);
		customProp.Add("roomOwnerId", userId);

		PhotonNetwork.SetPlayerCustomProperties(customProp);

		//カスタムプロパティをルームのオプションに設定
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.CustomRoomProperties = customProp;
		roomOptions.CustomRoomPropertiesForLobby = new string[] { "roomName", "roomNameID" };

		roomOptions.MaxPlayers = (byte)GAMEROOM_LIMIT;

		roomOptions.IsOpen = true;//入室を許可するか否か
		roomOptions.IsVisible = true;//ロビーから見えるようにする

		PhotonNetwork.CreateRoom(roomName, roomOptions, null);

		isRoomMake = true;
		StartCoroutine(WaitOtherPlayer(roomName));

		PhotonText.text = string.Format("ルームを作りました\nチーム名:{0}", roomName);
	}
    /// <summary>
    /// プレイヤーの参加待ち
    /// </summary>
    /// <returns></returns>
	public IEnumerator WaitOtherPlayer(string roomName = GAMEROOM_NAME)
	{
		bool isWaiting = true;
		Debug.Log(string.Format("Waiting start Limit : {0}", GAMEROOM_LIMIT));
		while (isWaiting == true)
		{
			if (PhotonNetwork.room != null &&
				PhotonNetwork.room.PlayerCount == GAMEROOM_LIMIT)
			{
				isWaiting = false;
			}
			yield return null;
		}

		Debug.Log("waiting finish!");
		SceneChangeGame(roomName);//ゲームシーンに遷移

	}
    /// <summary>
    /// ランダムなルームに入る
    /// </summary>
	public void JoinRandomRoom()
	{
		Debug.Log("JoinRandomRoom");
        //UserIdを2に設定
		GlobalGameManager._instance.UserId = 2;
		PhotonNetwork.JoinRandomRoom();

		PhotonText.text = "JoinRandomRoom";

	}
    /// <summary>
    /// 指定した名前のルームに入る
    /// </summary>
    /// <param name="roomName"></param>
	public void JoinRoom(string roomName = GAMEROOM_NAME)
	{
		Debug.Log(string.Format("JoinRoom({0})", roomName));
		//UserIdを2に設定
		GlobalGameManager._instance.UserId = 2;
		isRoomMake = false;
		PhotonNetwork.JoinRoom(roomName);

		PhotonText.text = string.Format("JoinRoom({0})", roomName);
	}
    /// <summary>
    /// ゲームシーンへ遷移
    /// </summary>
	public void SceneChangeGame(string roomName = GAMEROOM_NAME)
	{
		Debug.Log("SceneChangeGame");
		if (!PhotonNetwork.isMasterClient)
		{
			Debug.Log("Error:this function masterClient only");
			return;
		}
		GlobalGameManager._instance.teamName = roomName;
		PhotonNetwork.LoadLevel("SceneGame");
	}
    
}