using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 20f;
    public Vector3 direction;
    private SpriteRenderer sprite;
    public Vector3 Direction { set { direction = value;  } }

    void Start()
    { 
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.flipX = direction.x < 0;
        Destroy(gameObject,0.75f);
        direction.Set(direction.x, UnityEngine.Random.Range(-0.07f,0.07f),direction.z);
        transform.Rotate(0, 0, (float)Math.Atan( (double)direction.y));
        
    }

    
    void FixedUpdate()
    {

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

    }

    public void DestroyBullet(float time) 
    {
        Destroy(gameObject, time);
    }
}
