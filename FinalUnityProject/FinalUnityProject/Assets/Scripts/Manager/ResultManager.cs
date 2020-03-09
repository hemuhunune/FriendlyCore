using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : Photon.MonoBehaviour
{
    [SerializeField] private GameObject _serverManagerPrefab = default;
    private void Awake()
    {
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.PLAYER))
        {
            PhotonNetwork.Instantiate(_serverManagerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }
    }
}
