using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LineMoveEnemy : Photon.MonoBehaviour
{
    // Start is called before the first frame update
    private Enemy _enemy = default;
    [SerializeField] private GameObject _shoot = default;

    [SerializeField] private Slider _hpSlider = default;
    private const int MAX_HP = 5;
    public int _hp = MAX_HP;

    private const float ATTACK_INTERVAL = 3f;
    private float _attackCount = 0;

    public bool _shootFlag = false;

    public bool _enemyDeath = false;

    public bool _isHit = false;
    void Start()
    {
        _enemy = new Enemy(_hp, transform.position.x, transform.position.y);
        _hpSlider.value = 1;
        _hpSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _attackCount += Time.deltaTime;

        if(_attackCount > ATTACK_INTERVAL && _shootFlag == false)
        {
            _shootFlag = true;
                   
        }

        if(_shootFlag == true)
        {
            Attack();
            _attackCount = 0;
        }
    }

    public void TouchDamage(int damage)
    {
        _enemy.Damage(damage);
       
        _hp = _enemy.hp;
        _hpSlider.gameObject.SetActive(true);
        _hpSlider.value = (float)_hp / (float)MAX_HP;
        _isHit = true;

    }

    private void Attack()
    {
        _shoot.transform.position = transform.position;
        _shoot.SetActive(true);
        _shootFlag = false;
    }

    private void SendDestroy(int hp)
    {
        if(hp <= 0)
        {
            _enemy.Destroy(gameObject);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //送信処理
            stream.SendNext(_hp);
            stream.SendNext(_shootFlag);
            stream.SendNext(_isHit);
            _isHit = false;
            SendDestroy(_hp);
        }
        else
        {
            //受信処理
            int hp = (int)stream.ReceiveNext();
            _shootFlag = (bool)stream.ReceiveNext();
            bool hit = (bool)stream.ReceiveNext();
            if (hit == true)
            {
                _hpSlider.gameObject.SetActive(true);
                _hpSlider.value = (float)hp / (float)MAX_HP;
            }
            SendDestroy(hp);
        }
    }
}
