using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainManager : MonoBehaviour
{

    private enum BUTTON_TYPE
    {
        CREATE_ROOM = 0,
        JOIN_ROOM,
        JOIN_RANDOM_ROOM,
    }


    [SerializeField] private Button[] buttons = default;
    [SerializeField] private Text roomName = null;
    [SerializeField] private InputField _inputField = default;
    // Start is called before the first frame update
    void Start()
    {
        GlobalGameManager._instance.mainManager = this;

        buttons[(int)BUTTON_TYPE.CREATE_ROOM].onClick.AddListener(OnCreateRoomButton);
        buttons[(int)BUTTON_TYPE.JOIN_ROOM].onClick.AddListener(OnJoinRoomButton);
        buttons[(int)BUTTON_TYPE.JOIN_RANDOM_ROOM].onClick.AddListener(OnJoinRandomRoomButton);

    }

    public void OnCreateRoomButton()
    {
        GlobalGameManager._instance.photonManager.CreateRoom(roomName.text);

    }
    public void OnJoinRoomButton()
    {
        GlobalGameManager._instance.photonManager.JoinRoom(roomName.text);

    }
    public void OnJoinRandomRoomButton()
    {
        GlobalGameManager._instance.photonManager.JoinRandomRoom();

    }
    public void OnInputFieldEnd()
    {
        roomName.text = _inputField.text;
    }
}
