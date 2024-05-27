using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public int live = 3;

    [SerializeField]
    public float speed = 5f;

    [SerializeField]
    public float jumpForce = 10f;

    [SerializeField]
    public float distance = 5f;

    [SerializeField]
    public float timeToDestroyBullet = 4f;

    private ParticleSystem particlesBlood;
    private ParticleSystem particlesShoot;

    private bool distanceChek = true;
    private Vector3 pos;
    public bool canRun = true;

    public AudioSource shootSound;
    public AudioSource stepSound;
    private AudioClip getHit;

    [SerializeField]
    private ScoreCounter scoreCounter = null;

    //Дальность поиска игрока и остановка перед ним
    [SerializeField]
    private float range = 15f;
    private float rangeStop = 2.5f;
    private float rangeJump = 0.3f;

    public float rangeShoot = 8f;
    private bool canBullet = true;
    public float reloadTime = 4f;
    private float timeOfReload;
    private float chekPlayerTime = 0f;
    private Bullet bullet;

    private Material matBlink;
    private Material matDefault;

    private Vector3 direction;

    private bool isGrounded = false;
    public LayerMask whatIsGrounded;

    private GameObject player;

    //Raycast длдя поиска игрока и для распознования препядствий для прыжка
    [SerializeField]
    private bool isNeedSeePlayer = true;
    private RaycastHit2D hit;
    private RaycastHit2D hitShoot;
    private RaycastHit2D hitJump;
    private Vector3 hitPosOriginal;
    private Vector3 hitPosPlayer;
    private Vector3 hitJumpPosOriginal;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Damage();
            onTriggerPlayer();
            Destroy(collision.gameObject);
        }

    }

    private void Awake()
    {
        timeOfReload = reloadTime;
        sprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anm = GetComponent<Animator>();
        player = FindFirstObjectByType<Player>().gameObject;
        pos = transform.position;
        matDefault = sprite.material;
        matBlink = Resources.Load<Material>("Materials/MaterialBlink");
        getHit = Resources.Load<AudioClip>("Audio/Sounds/Hit");
        bullet = Resources.Load<Bullet>("Prefabs/Bullet");
        particlesShoot = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        particlesBlood = transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();

        scoreCounter = FindFirstObjectByType<ScoreCounter>();
    }


    private void FixedUpdate()
    {
        if (live <= 0)
        {
            if (scoreCounter != null) scoreCounter.addScore();

            Destroy(gameObject);
        }

        //Изменение расположений raycast-ов относительно куда смотрит бот
        if (sprite.flipX)
        {
            hitJumpPosOriginal.Set(transform.position.x - 1.5f, transform.position.y + 0.7f, transform.position.z);
        }
        else
        {
            hitJumpPosOriginal.Set(transform.position.x + 1.5f, transform.position.y + 0.7f, transform.position.z);
        }

        ChekGround();
        if (!isGrounded) anm.Play("Jump");
    }

    void Update()
    {
        //Отсчёт времени между выстрелами
        if (!canBullet) timeOfReload -= Time.deltaTime;
        if (timeOfReload <= 0)
        {
            canBullet = true;
            timeOfReload = reloadTime;
        }

        MoveHits();

        //Условие при котормо бот среагирует на игрока
        if (((Math.Abs(transform.position.x - player.transform.position.x) < range && (hit.transform.tag == "Player" || !isNeedSeePlayer)) || Vector2.Distance(transform.position, player.transform.position) < rangeStop) && player.GetComponent<Player>().live > 0)
        {
            chekPlayerTime = 5f;
            onTriggerPlayer();
        }
        else
        {
            chekPlayerTime -= Time.deltaTime;
            if (chekPlayerTime > 0) onTriggerPlayer();
            else WithoutTriggerPlayer();
        }

    }

    private void onTriggerPlayer()
    {
        //Получение направление бота к игроку
        if (transform.position.x > player.transform.position.x) direction = -transform.right;
        else direction = transform.right;

        if (hitShoot.transform != null && hitShoot.transform.gameObject == player && canBullet && Vector2.Distance(hitShoot.transform.position, transform.position) < rangeShoot) Shoot();
        
        if (canRun)
        {
            if (Math.Abs(transform.position.x - player.transform.position.x) > rangeStop)
            {
                if (hitJump && hitJump.transform.tag != "Player") Jump();
                Run();
            }
            else
            {
                if (((!sprite.flipX && player.transform.position.x < transform.position.x) || (sprite.flipX && player.transform.position.x > transform.position.x)) && player.transform.position.x != transform.position.x)
                {
                    if (hitJump && hitJump.transform.tag != "Player") Jump();
                    Run();
                }
                else
                {
                    if (isGrounded)
                    {
                        anm.SetInteger("State", 0);
                        anm.Play("Idle");
                        rb.velocity = new Vector2(0f, rb.velocity.y);
                    }
                }
            }
        }
        else
        {
            sprite.flipX = direction.x < 0;
        }
    }

    private void WithoutTriggerPlayer()
    {
        //Что будет делать бот если не видит игрока
        if (distance != 0)
        {
            if (transform.position.x > pos.x) distanceChek = true;

            else if (transform.position.x < pos.x - distance) distanceChek = false;

            if (hitJump && hitJump.transform.tag != "Player") Jump();

            if (pos.x - distance <= transform.position.x && distanceChek)
            {
                direction = -transform.right;
                Run();
            }
            else distanceChek = false;

            if (transform.position.x <= pos.x && !distanceChek)
            {
                direction = transform.right;
                Run();
            }
            else distanceChek = true;
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (isGrounded)
            {
                anm.SetInteger("State", 0);
                anm.Play("Idle");
            }
        }
    }

    private void MoveHits()
    {
        if (sprite.flipX)
        {
            hitPosOriginal.Set(transform.position.x - 1.5f, transform.position.y + 1.4f, transform.position.z);
            hitJump = Physics2D.Raycast(hitJumpPosOriginal, -transform.right, rangeJump);
            hitShoot = Physics2D.Raycast(hitPosOriginal, -transform.right);
        }
        else
        {
            hitPosOriginal.Set(transform.position.x + 1.5f, transform.position.y + 1.4f, transform.position.z);
            hitJump = Physics2D.Raycast(hitJumpPosOriginal, transform.right, rangeJump);
            hitShoot = Physics2D.Raycast(hitPosOriginal, transform.right);
        }
        hitPosPlayer = player.transform.position - hitPosOriginal;
        hitPosPlayer.Set(hitPosPlayer.x, hitPosPlayer.y + 1.3f, hitPosPlayer.z);
        hit = Physics2D.Raycast(hitPosOriginal, hitPosPlayer);
    }

    void Run()
    {
        rb.velocity = new Vector2(speed * direction.x, rb.velocity.y);
        sprite.flipX = direction.x < 0;
        if (isGrounded)
        {
            anm.SetInteger("State", 1);
            anm.Play("Run");
            if (!stepSound.isPlaying)
            {
                stepSound.volume = 0.07f;
                stepSound.PlayOneShot(stepSound.clip);
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    void Shoot()
    {
        Vector3 position = transform.position; position.y += 1.2f;
        if (sprite.flipX)
        {
            position.x -= 1.4f;
            particlesShoot.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(180f, Vector3.up));
        }
        else
        {
            position.x += 1.2f;
            particlesShoot.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(0f, Vector3.up));
        }
        position.y += UnityEngine.Random.Range(-0.1f, 0.1f);
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1f : 1f);
        newBullet.DestroyBullet(timeToDestroyBullet);
        shootSound.pitch = UnityEngine.Random.Range(0.7f, 0.9f);
        shootSound.PlayOneShot(shootSound.clip);
        particlesShoot.Play();
        canBullet = false;
    }

    //Проверка персонажа на нахождение на земле с помощью поиска колайдера земли 
    void ChekGround()

    {
        isGrounded = Physics2D.OverlapBox(transform.position, new Vector2(0.6f, 0.3f), 0, whatIsGrounded);
        anm.SetBool("IsGrounded", isGrounded);
        anm.SetFloat("vSpeed", rb.velocity.y);
    }

    public void Damage()
    {
        live--;
        particlesBlood.Play();
        stepSound.volume = 1f;
        stepSound.PlayOneShot(getHit);
        sprite.material = matBlink;
        Invoke("ResetMaterial", 0.05f);
    }
    private void ResetMaterial()
    {
        sprite.material = matDefault;
    }
}
