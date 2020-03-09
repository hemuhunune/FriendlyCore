using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TouchManager : Photon.MonoBehaviour
{
    [System.NonSerialized]
    public GameObject _touchObject = default;
    private GameObject _fairyObject = default;
    private bool _touchMoveFlag = false;
    [SerializeField] GameObject _touchPointPrefab = default;

    private GameObject _touchPoint = default;
    private Vector3 _initTouchPointVec = new Vector3(0.0f, 0.0f, -100.0f);
    private Rigidbody2D _rb = default;

    [SerializeField] private GameObject _fairyPrefab = default;

    private GameObject _player = default;
    private bool _isTouch = false;
    private Vector2 _pos = Vector2.zero;
    private Vector2 _startTouchPos = Vector2.zero;
    private Vector2 _differenceTouchPos = Vector2.zero;

    private float _enemyDamageTime = 30f;
    private float _currentTime = 0f;
    // Start is called before the first frame update

    void Start()
    {
        if (GlobalGameManager._instance.UserId == (int)GlobalGameManager.PLAYER_TYPE.TOUCH)
        {
            _touchPoint = PhotonNetwork.Instantiate(_touchPointPrefab.name, Vector3.zero, Quaternion.identity, 0);
            _touchPoint.transform.position = _initTouchPointVec;
            _fairyObject = PhotonNetwork.Instantiate(_fairyPrefab.name, transform.position, Quaternion.identity, 0);
            _pos = _fairyObject.transform.position;
            Debug.Log("タッチポイント生成");
        }
        GlobalGameManager._instance.touchManager = this;
    }

    // Update is called once per frame

    private void Update()
    {
        if (GlobalGameManager._instance.UserId == (int)GlobalGameManager.PLAYER_TYPE.TOUCH)
        {
            ObjectToTouch();
            EnemyAttack();
            FairyMove();
        }
    }

    void FixedUpdate()
    {
        if(_touchMoveFlag == true)
        {
            TouchObjectMove(_touchObject, _rb);
        }
        
    }

    private void EnemyAttack()
    {
        if (_touchObject)
        {
            if (LayerMask.LayerToName(_touchObject.gameObject.layer) == "TouchNonMove")
            {
                if (_touchObject.gameObject.CompareTag("Enemy") == true)
                {
                    _currentTime++;
                    if (_currentTime > _enemyDamageTime)
                    {
                        _touchObject.gameObject.SendMessage("TouchDamage", 1);
                        _currentTime = 0;
                    }
                }
            }
        }
    }

    private void FairyMove()
    {
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.TOUCH))
        {
            if (_player)
            {
                Vector2 purPos = Vector2.zero;
                if (_isTouch == false)
                {
                    Vector2 movePos = new Vector2(-1f * Mathf.Sign(_player.transform.localScale.x), 1);
                    purPos = (Vector2)_player.transform.position + movePos;
                    _fairyObject.transform.localScale = new Vector3(_player.transform.localScale.x * 2 / 3, _fairyObject.transform.localScale.y, _fairyObject.transform.localScale.z);
                }
                else
                {
                    if (_touchObject)
                    {
                        purPos = new Vector2(_touchObject.transform.position.x - 1f, _touchObject.transform.position.y);
                        _fairyObject.transform.localScale = new Vector3(2f, _fairyObject.transform.localScale.y, _fairyObject.transform.localScale.z);
                    }
                    
                }
                _pos += (purPos - _pos) / 15;

                _fairyObject.transform.position = _pos;
            }
            else
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }

        }
    }
    /// <summary>
    /// スライド移動処理
    /// </summary>
    /// <param name="touchObj"></param>タッチしたオブジェクト
    /// <param name="rb"></param>タッチしたオブジェクトのrigidbody
    private void TouchObjectMove(GameObject touchObj , Rigidbody2D rb)
    {
        Vector3 nowTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (touchObj != default)
        {
            if (LayerMask.LayerToName(touchObj.gameObject.layer) != "TouchNonMove")
            {
                Vector2 distanceToObject = ((Vector2)nowTouchPos - _differenceTouchPos) - (Vector2)touchObj.transform.position;
                rb.velocity = distanceToObject * 10.0f;
            }
        }
    }

    private void TouchObjectTouchDown(RaycastHit2D hit)
    {
        _touchObject = hit.transform.gameObject;
        Debug.Log(_touchObject);
        GlobalGameManager._instance.gamemanager.lastTouchObject = _touchObject;
        _startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _differenceTouchPos = _startTouchPos - (Vector2)_touchObject.transform.position;

        Debug.Log(_touchObject.name);
        _rb = _touchObject.GetComponent<Rigidbody2D>();

        _touchMoveFlag = true;
        if (LayerMask.LayerToName(_touchObject.gameObject.layer) == "TouchNonGravityObj")
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

       
    }
    /// <summary>
    /// 指を離した時
    /// </summary>
    private void TouchObjectTouchUp()
    {
        _touchPoint.transform.position = _initTouchPointVec;
        if (_touchObject)
        {
            //重力オブジェクトの場合
            if (LayerMask.LayerToName(_touchObject.gameObject.layer) == "TouchGravityObj")
            {
                _rb.velocity /= 2;
            }
            //非重力オブジェクトの場合
            else if (LayerMask.LayerToName(_touchObject.gameObject.layer) == "TouchNonGravityObj")
            {

                _rb.velocity = Vector2.zero;
                _rb.bodyType = RigidbodyType2D.Kinematic;
            }
            
            //Init
            _touchObject = default;
            _touchMoveFlag = false;
            _isTouch = false;
            _currentTime = 0;
            _rb = default;
        }
    }

    /// <summary>
    /// タッチしたオブジェクトを取得
    /// </summary>
    private void ObjectToTouch()
    {
        if(0 < Input.touchCount)
        {
            //タッチした瞬間
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                int LayerObject = LayerMask.GetMask(new string[] { "TouchGravityObj", "TouchNonGravityObj", "TouchNonMove" });
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit2d = Physics2D.Raycast(ray.origin, ray.direction, 10, LayerObject);
                if (hit2d)
                {
                    TouchObjectTouchDown(hit2d);
                    _isTouch = true;
                }
            }

            //移動中
            if(touch.phase == TouchPhase.Moved)
            {
                Vector3 nowTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
                _touchPoint.transform.position = new Vector3(nowTouchPos.x, nowTouchPos.y, 100);
            }
            //離した時
            if(touch.phase == TouchPhase.Ended)
            {
                TouchObjectTouchUp();
            }
        }
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("タッチした");
            int LayerObject = LayerMask.GetMask(new string[] { "TouchGravityObj", "TouchNonGravityObj", "TouchNonMove" });
            Debug.Log(LayerObject);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(ray.origin, ray.direction, 10, LayerObject);
            if (hit2d)
            {
                TouchObjectTouchDown(hit2d);
                _isTouch = true;
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 nowTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _touchPoint.transform.position = new Vector3(nowTouchPos.x, nowTouchPos.y, 100);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("指を離した");
            TouchObjectTouchUp();
        }
        */
    }
}
