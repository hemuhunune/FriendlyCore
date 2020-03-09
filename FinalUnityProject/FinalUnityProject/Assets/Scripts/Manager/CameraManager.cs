using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject player = default;
    private Vector3 pos = Vector3.zero;
    [SerializeField,Range(1f,5f)]
    private float speed = 1f;
    // Start is called before the first frame update
    private void Awake()
    {
        GlobalGameManager._instance.cameraManager = this;
    }
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player)
        {
            pos = player.transform.position;
            pos.x += 3;
            pos.y = pos.y / 5;
            pos.z = -10;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
        }
    }
}
