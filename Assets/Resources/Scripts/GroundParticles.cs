using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundParticles : MonoBehaviour
{
    private ParticleSystem []  particles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Vector3 position = collision.transform.position;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (!particles[i].isPlaying) 
                {
                    particles[i].gameObject.transform.position = position;
                    particles[i].Play();
                    break;
                }
            }
            Destroy(collision.gameObject);
        }
    }

    private void Awake()
    {
        particles = new ParticleSystem[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
        }
    }
}
