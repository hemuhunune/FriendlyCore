using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMove : Photon.MonoBehaviour
{
    //移動速度
    [SerializeField] private float _moveSpeed = 10.0f;
    //ジャンプ速度
    [SerializeField] private float _jumpSpeed = 10.0f;

    private const float LEFT_POINT = -0.094f * 3;
    private const float RIGHT_POINT = 0.093f * 3;
    private const float DOWN_POINT = -0.171f * 3;
    private const float UP_POINT = 0.245f * 3;

    private const float MIN_FRICTION = 1.5f;
    private const float MAX_FRICTION = 100.0f;
    private const float MAX_HORIZONTAL = 1.0f;
    //最大落下速度
    private const float MAX_FALL_SPEED = -8.0f;

    //transform.positon
    private Vector2 _pos = Vector2.zero;

    public Vector2 _RespawnPos = Vector2.zero;
    //移動ベクトル
    private Vector2 _move = Vector2.zero;
    //移動方向のフラグ
    private bool _leftMoveFlag = false;
    private bool _rightMoveFlag = false;
    //ジャンプブタンが押されたか否か
    private bool _isJumpButtonTouch = false;
    //横移動値
    private float _inputHorizontal = 0.0f;
    //縦移動値
    private float _inputVertical = 0.0f;
    //摩擦
    private float _friction = 2.0f;
    
    //ジャンプ可能フラグ
    private bool _jumpFlag = false;
    //ジャンプボタンを離して落下しているか
    private bool _fallFlag = false;
    //重力
    private float _gravity = 0.0f;
    //rigidbodyのインスタンス化
    private new Rigidbody2D rigidbody = default;
    //PhotonViewのインスタンス化
    private new PhotonView photonView = default;
    //PhotonTransuformViewのインスタンス化
    private PhotonTransformView photonTransformView = default;
    //Animetorのインスタンス化
    private Animator _animator = default;
    // Start is called before the first frame update
    void Start()
    {
        photonTransformView = GetComponent<PhotonTransformView>();
        rigidbody = GetComponent<Rigidbody2D>();
        GlobalGameManager._instance.cameraManager.player = gameObject;
        photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _pos = transform.position;
        
    }
    void FixedUpdate()
    {
        
        Move();
    }

    private void SetAnimation()
    {
        if(_leftMoveFlag == true)
        {
            _animator.SetBool("run", true);
            transform.localScale = new Vector2(-3, 3);
        }
        if(_rightMoveFlag == true)
        {
            _animator.SetBool("run", true);
            transform.localScale = new Vector2(3, 3);
        }
        if (_leftMoveFlag == false && _rightMoveFlag == false)
        {
            _animator.SetBool("run", false);
        }

        if(_jumpFlag == false)
        {
            if (_inputVertical > 0)
            {
                _animator.SetBool("jump", true);
            }
            else
            {
                _animator.SetBool("jump", false);
                _animator.SetBool("fall", true);
            }
        }
    }

    /// <summary>
    /// 移動関連
    /// 何も入力していない時に摩擦は2.0fに設定
    /// </summary>
    void Move()
    {
        rigidbody.velocity = Vector2.zero;
        //ボタン入力
        ButtonTouchFlagSet();
        if (GlobalGameManager._instance.UserId == (int)GlobalGameManager.PLAYER_TYPE.PLAYER)
        {
            //左
            if (_leftMoveFlag == true)
            { 
                LeftMove();
            }
            //右
            if (_rightMoveFlag == true)
            {
                RightMove();
            }

            //摩擦
            if (_leftMoveFlag == false && _rightMoveFlag == false)
            {
                _friction = 2.0f;
            }
            //縦移動
            JumpMove();

            //横移動値に摩擦を加算
            _inputHorizontal -= Mathf.Lerp(0.0f, _inputHorizontal, Time.deltaTime * _moveSpeed / _friction);

            //ブロックとの判定を取得してHorizontalの値を修正
            CollisionBlocks();
            //横移動と縦移動の値をセット
            _move = new Vector2(_inputHorizontal * _moveSpeed, _inputVertical);

            rigidbody.velocity = _move;

            SetAnimation();

        }
        photonTransformView.SetSynchronizedValues(speed: rigidbody.velocity, turnSpeed: 0);
    }
    /// <summaset
    /// 左方向への移動値と摩擦設定
    /// </summary>
    private void LeftMove()
    {
        _inputHorizontal -= Time.deltaTime;

        if (_inputHorizontal < -MAX_HORIZONTAL)
        {
            _inputHorizontal = -MAX_HORIZONTAL;
        }
        if (_inputHorizontal > 0.0f)
        {
            _friction = MIN_FRICTION;
        }
        else
        {
            _friction = MAX_FRICTION;
        }
    }

    /// <summary>
    /// 右方向への移動値と摩擦設定
    /// </summary>
    private void RightMove()
    {
        _inputHorizontal += Time.deltaTime;
        if (_inputHorizontal > MAX_HORIZONTAL)
        {
            _inputHorizontal = MAX_HORIZONTAL;
        }

        if (_inputHorizontal < 0.0f)
        {
            _friction = MIN_FRICTION;
        }
        else
        {
            _friction = MAX_FRICTION;
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void JumpMove()
    {
        //落下中に地面をチェック
        if (_inputVertical <= 0.0f)
        {
            _jumpFlag = FloorCollisonIsChecked();
        }
        //重力
        if (_jumpFlag == false)
        {
            _gravity += 0.03f;
            _inputVertical -= _gravity;
            if(_inputVertical < MAX_FALL_SPEED)
            {
                _inputVertical = MAX_FALL_SPEED;
            }
        }
        //着地時
        else
        {
            _gravity = 0.0f;
            //一瞬で0にすると少し浮いてしまうので徐々に
            _inputVertical -= Mathf.Lerp(0.0f, _inputVertical, Time.deltaTime * 5);
            _fallFlag = false;
            _animator.SetBool("fall", false);
        }
    }

    /// <summary>
    /// ジャンプ初速
    /// </summary>
    private void JumpSpeedAdd()
    {
        _inputVertical = _jumpSpeed;
    }

    //床に接地しているかチェック
    private bool FloorCollisonIsChecked()
    {
        bool FloorCheck = false;

        if (CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT, _pos.y + DOWN_POINT), Vector2.down, 0.2f, "Block") == true
            || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT, _pos.y + DOWN_POINT), Vector2.down, 0.2f, "Block") == true)
        {
            FloorCheck = true;
            _RespawnPos = _pos;
        }
        if (CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT, _pos.y + DOWN_POINT), Vector2.down, 0.2f, "TouchNonGravityObj") == true
            || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT, _pos.y + DOWN_POINT), Vector2.down, 0.2f, "TouchNonGravityObj") == true)
        {
            FloorCheck = true;
        }
        return FloorCheck;
    }

    /// <summary>
    /// GlobalGameManagerからボタン入力を受け取る
    /// </summary>
    private void ButtonTouchFlagSet()
    {
        _leftMoveFlag = GlobalGameManager._instance.gamemanager.leftButtonTouch;
        _rightMoveFlag = GlobalGameManager._instance.gamemanager.rightButtonTouch;
        _isJumpButtonTouch = GlobalGameManager._instance.gamemanager.jumpButtonTouch;

        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.PLAYER))
        {
            if (_jumpFlag == true && _isJumpButtonTouch == true)
            {
                _jumpFlag = false;
                JumpSpeedAdd();
            }
            if (_fallFlag == false && _isJumpButtonTouch == false)
            {
                if (_inputVertical > 0.0f)
                {
                    _inputVertical *= 0.5f;
                }
                _fallFlag = true;
            }
        }
        
    }

    /// <summary>
    /// レイを飛ばしてオブジェクトを取得
    /// </summary>
    /// <param name="checkVectorPosition"></param>レイ発生座標
    /// <param name="checkVectorDirection"></param>レイを飛ばす方向
    /// <param name="checkLength"></param>レイの長さ
    /// <param name="LayerName"></param>取得するオブジェクトのレイヤーネーム
    /// <returns>hitflag</returns>レイがヒットしたか
    private bool CollisonLayerCheck(Vector2 checkVectorPosition, Vector2 checkVectorDirection, float checkLength, string LayerName)
    {
        int LayerObject = LayerMask.GetMask(LayerName);
        bool hitFlag = false;
        Ray2D ray = new Ray2D(checkVectorPosition, checkVectorDirection);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, checkLength, LayerObject);
        
        if (hit)
        {
            hitFlag = true;
        }
        Debug.DrawRay(ray.origin, ray.direction * checkLength, Color.green, 1 / 60, false);

        return hitFlag;

    }

    /// <summary>
    /// ブロックとの衝突判定
    /// </summary>
    private void CollisionBlocks()
    {
        //右
        if (CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT,_pos.y + UP_POINT) , Vector2.right, 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT,_pos.y + DOWN_POINT) , Vector2.right , 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT,_pos.y + UP_POINT) , Vector2.right , 0.2f, "TouchNonGravityObj") == true
         || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT,_pos.y + DOWN_POINT) , Vector2.right , 0.2f, "TouchNonGravityObj") == true)
        {
            if (_inputHorizontal > 0.0f)
            {
                _inputHorizontal = 0.0f;
            }
        }
        //左
        if (CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT,_pos.y + UP_POINT) , Vector2.left , 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT,_pos.y + DOWN_POINT) , Vector2.left , 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT,_pos.y + UP_POINT) , Vector2.left , 0.2f, "TouchNonGravityObj") == true
         || CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT,_pos.y + DOWN_POINT) , Vector2.left , 0.2f, "TouchNonGravityObj") == true)
        {
            if (_inputHorizontal < 0.0f)
            {
                _inputHorizontal = 0.0f;
            }
        }
        //上
        if (CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT, _pos.y + UP_POINT) ,  Vector2.up, 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT, _pos.y + UP_POINT) , Vector2.up, 0.2f, "Block") == true
         || CollisonLayerCheck(new Vector2(_pos.x + LEFT_POINT, _pos.y + UP_POINT) , Vector2.up, 0.2f, "TouchNonGravityObj") == true
         || CollisonLayerCheck(new Vector2(_pos.x + RIGHT_POINT, _pos.y + UP_POINT) , Vector2.up, 0.2f, "TouchNonGravityObj") == true)
        {
            if (_inputVertical > 0.0f)
            {
                _inputVertical = -2.0f;
            }
        }
    }

    /// <summary>
    /// データ送信
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }
}
