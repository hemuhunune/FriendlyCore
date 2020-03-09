using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GlobalGameManager._instance.CheckUserId((int)GlobalGameManager.PLAYER_TYPE.TOUCH))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
