using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public int numberOfGameObject;
    public float radiusOfExplousion;
    public ParticleSystem particles;
    private AudioSource audio;
    private AudioClip explousionClip;
    private AudioClip hitClip;
    private AudioClip triggerClip;
    private Animator anm;
    public int live = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && numberOfGameObject == 0 && anm.GetInteger("State") == 0)
        {
            anm.SetInteger("State", 1);
            audio.transform.position = transform.position;
            audio.PlayOneShot(triggerClip);
            Invoke("MineExplousion", 0.5f);

        }
        if (collision.tag == "Bullet" && numberOfGameObject == 1)
        {
            var part = Instantiate(particles, transform.position, Quaternion.identity);
            var main = part.main;
            main.startSpeed = 1f;
            main.startColor = new Color(0.9f, 0.1f, 0.1f);
            part.Play();
            Destroy(part.gameObject, 2f);
            audio.PlayOneShot(hitClip);
            Destroy(collision.gameObject);
            live--;
        }

    }

    private void FixedUpdate()
    {
        if(live == 0) 
        {
            RedBarrelEplousion();
        }
    }

    void RedBarrelEplousion() 
    {
        var part = Instantiate(particles, transform.position, Quaternion.identity);
        var main = part.main;
        var color = part.colorOverLifetime;
        main.startSpeed = 10f;
        main.startColor = new Color(1f, 0.97f, 0.97f);
        color.enabled = true;
        particles.Play();
        audio.pitch = 1f;
        audio.transform.position = transform.position;
        audio.PlayOneShot(explousionClip);
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, radiusOfExplousion);
        foreach (Collider2D col in collider2D)
        {
            if (col.tag == "Player")
            {
                Player player = col.GetComponent<Player>();
                player.Damage(); ;
            }
            if (col.tag == "Enemy")
            {
                Enemy enemy = col.GetComponent<Enemy>();
                enemy.Damage();
            }
            if (col.tag == "Box")
            {
                Box box = col.GetComponent<Box>();
                box.DestroyBox();
            }
            if (col.tag == "RedBarrel")
            {
                Mine mine = col.GetComponent<Mine>();
                mine.RedBarrelEplousion();
            }
        }
        Destroy(part.gameObject, 2f);
        Destroy(gameObject);
    }

    void MineExplousion() 
    {
        var part = Instantiate(particles, transform.position, Quaternion.identity);
        var main = part.main;
        var color = part.colorOverLifetime;
        main.startSpeed = 5f;
        main.startLifetime = 1f;
        main.startColor = new Color(1f, 0.97f, 0.97f);
        color.enabled = true;
        particles.Play();
        audio.pitch = 1f;
        audio.transform.position = transform.position;
        audio.PlayOneShot(explousionClip);
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, radiusOfExplousion);
        foreach (Collider2D col in collider2D)
        {
            if (col.tag == "Player")
            {
                Player player = col.GetComponent<Player>();
                player.Damage(); ;
            }
            if (col.tag == "Enemy")
            {
                Enemy enemy = col.GetComponent<Enemy>();
                enemy.Damage();
            }
        }
        Destroy(part.gameObject, 2f);
        Destroy(gameObject);
    }


    private void Awake()
    {
        hitClip = Resources.Load<AudioClip>("Audio/Sounds/Hit");
        audio = GameObject.Find("SoundOfGameObjects").GetComponent<AudioSource>();
        if (numberOfGameObject == 0) anm = GetComponent<Animator>();
        explousionClip = Resources.Load<AudioClip>("Audio/Sounds/Explousion");
        triggerClip = Resources.Load<AudioClip>("Audio/Sounds/Detonation");
    }
}
