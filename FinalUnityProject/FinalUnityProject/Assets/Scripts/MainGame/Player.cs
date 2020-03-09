using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private const int DAMAGE_TIME = 2;
    private const int MAX_HP = 10;
    //点滅の間隔
    [SerializeField] private float _ｆlashInterval = 0.3f;

    [SerializeField] private Slider _hpSlider = default;
    //HP
    private int _hp = MAX_HP;
    
    //Rendererのインスタンス化
    private Renderer _renderer = default;
    //PlayerMoveの取得
    private PlayerMove _playerMove = default;
    //ダメージを喰らったかどうかのフラグ
    private bool _damageFlag = false;
    //落下したかどうかのフラグ
    private bool _fallFlag = false;
    //点滅しているかどうかのフラグ
    private bool _flashFlag = false;
    //ダメージを喰らってからのタイムをカウント
    private float _damageCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _playerMove = gameObject.GetComponent<PlayerMove>();
        _hpSlider.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(_flashFlag == true)
        {
            PlayerFlash();
        }
        else
        {
            _renderer.enabled = true;
        }
    }

    private void PlayerDamage()
    {
        HpUpdate(1);
       // _damageFlag = false;
        _flashFlag = true;
        Debug.Log("当たったよ");
    }

    private void PlayerFall()
    {
        if(_flashFlag == false)
        {
            HpUpdate(1);
        }
        transform.position = _playerMove._RespawnPos;
        _flashFlag = true;
        _fallFlag = false;
    }

    private void PlayerFlash()
    {
        StartCoroutine("VisibleCoroutine");
        _damageCount += Time.deltaTime;
        if (_damageCount > DAMAGE_TIME)
        {
            _renderer.enabled = true;
            _damageFlag = false;
            _flashFlag = false;
            _damageCount = 0;
        }
    }

    private void HpUpdate(int damage)
    {
        _hp -= damage;
        _hpSlider.value = (float)_hp / (float)MAX_HP;
        Debug.Log(_hp / MAX_HP);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Damage") && _damageFlag == false)
        {
            PlayerDamage();
            _damageFlag = true;
            string rootLayer = LayerMask.LayerToName(collision.transform.parent.gameObject.layer);
            
            if (rootLayer == "TouchGravityObj"
                || rootLayer == "TouchNonGravityObj"
                || rootLayer == "TouchNonMove")
            {
                GlobalGameManager._instance.gamemanager.FriendlyUpdate(-10);
            }
           
        }
        if (LayerMask.LayerToName(collision.gameObject.layer) == "DeathLayer" && _fallFlag == false)
        {
            _fallFlag = true;
            if(_fallFlag == true)
            {
                PlayerFall();
            }
        }
    }
    /// <summary>
    /// プレイヤーの体力が0になった時にゲームをリスタートする処理
    /// </summary>
    private void DestroyPlayer()
    {
        GlobalGameManager._instance.gamemanager.SceneReset();
    }

    IEnumerator VisibleCoroutine()
    {
        while (_flashFlag == true)
        {
            _renderer.enabled = !_renderer.enabled;

            yield return new WaitForSeconds(_ｆlashInterval);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //送信処理
            stream.SendNext(_hp);
            stream.SendNext(_flashFlag);
            if (_hp == 0)
            {
                DestroyPlayer();
            }
        }
        else
        {
            //受信処理
            int hp = (int)stream.ReceiveNext();
            bool flash = (bool)stream.ReceiveNext();
            if(hp != _hp)
            {
                _hp = hp;
                _hpSlider.value = (float)_hp / (float)MAX_HP;
            }
            if(_flashFlag != flash)
            {
                _flashFlag = flash;
            }
        }
    }
}
