using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int hp;
    public float x;
    public float y;

    public Enemy(int _hp,float _x,float _y)
    {
        hp = _hp;
        x = _x;
        y = _y;
    }
    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    public void Damage(int damage)
    {
        hp -= damage;
        Debug.Log("Enemyの体力:" + hp);
    }
}
