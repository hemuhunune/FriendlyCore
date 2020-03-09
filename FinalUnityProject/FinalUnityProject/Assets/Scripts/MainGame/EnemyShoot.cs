using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    private float _speed = 0.2f;
    [SerializeField] private int direction = -1;
    private Vector2 _pos = Vector2.zero;

    private const float SHOOT_LIFE = 0.5f;
    private float _shootCount = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        _pos = transform.position;
        _pos.x += _speed * direction;
        transform.position = _pos;

        Life();
    }

    private void Life()
    {
        _shootCount += Time.deltaTime;
        if (_shootCount > SHOOT_LIFE)
        {
            _shootCount = 0;
            gameObject.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //送信処理
            stream.SendNext(_pos);
        }
        else
        {
            //受信処理
            Vector2 pos = (Vector2)stream.ReceiveNext();
            if(pos != _pos)
            {
                _pos = pos;
            }
        }
    }
}
