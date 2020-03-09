using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonTochObject : Photon.MonoBehaviour
{
    private new Rigidbody2D rigidbody = default;
    public Vector2 pos = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if(GlobalGameManager._instance.UserId != (int)GlobalGameManager.PLAYER_TYPE.TOUCH)
        {
            rigidbody.isKinematic = true;
        }
        if(LayerMask.LayerToName(gameObject.layer) == "TouchNonGravityObj")
        {
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos = transform.position;
        if(rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            rigidbody.velocity = Vector2.zero;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(pos);
        }
        else
        {
            pos = (Vector2)stream.ReceiveNext();
        }
    }
}
