using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : MonoBehaviour
{
    public ParticleSystem particles;
    private GameObject heal;
    private GameObject patrons;
    public int hasItem;
    private AudioSource audio;
    private AudioClip hitClip;
    private int live = 2;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            var part = Instantiate(particles, transform.position, Quaternion.identity);
            var main = part.main;
            main.startColor = new Color(0.33f, 0.22f, 0.09f);
            particles.Play();
            Destroy(part.gameObject, 2f);
            live--;
            Destroy(collision.gameObject);
           
        }

    }

    private void FixedUpdate()
    {
        if (live == 0) DestroyBox();
    }

    private void Awake()
    {
        audio = GameObject.Find("SoundOfGameObjects").GetComponent<AudioSource>();
        hitClip = Resources.Load<AudioClip>("Audio/Sounds/Hit");
        heal = Resources.Load<GameObject>("Prefabs/Heal");
        patrons = Resources.Load<GameObject>("Prefabs/Patrons");
    }

    public void DestroyBox() 
    {
        var part = Instantiate(particles, transform.position, Quaternion.identity);
        var main = part.main;
        main.startColor = new Color(0.33f, 0.22f, 0.09f);
        particles.Play();
        Destroy(part.gameObject, 2f);
        audio.transform.position = transform.position;
        audio.pitch = Random.Range(0.8f, 1f);
        audio.PlayOneShot(hitClip);
        if (hasItem == 1) Instantiate(heal, transform.position, Quaternion.identity);
        if (hasItem == 2) Instantiate(patrons, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
