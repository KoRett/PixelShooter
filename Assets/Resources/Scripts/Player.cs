using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public int live = 3;

    [SerializeField]
    public float speed = 5f;

    [SerializeField]
    public float jumpForce = 10f;

    [SerializeField]
    public int patrons = 30;
    [SerializeField]
    public int allPatrons = 120;

    [SerializeField]
    public float timeToDestroyBullet = 4f;

    [SerializeField]
    private GameObject pauseMenu;
    private bool pause;

    private ParticleSystem particlesBlood;
    private ParticleSystem particlesShoot1;
    private ParticleSystem particlesShoot2;
    [SerializeField]
    private AudioSource shootSound;
    [SerializeField]
    private AudioSource stepSound;
    private AudioClip jump;
    private AudioClip getHit;
    private AudioClip reloadClip;

    [SerializeField]
    private float reloadTime = 0.26f;
    private float timeOfReload;
    private bool canBullet = true;
    private bool canReload = true;
    private bool pressedShoot = false;
    private Bullet bullet;

    private Material matBlink;
    private Material matDefault;
    private bool chekMat = true;
    private int countChekMat = 3;
    private float timeChekMat = 0.15f;

    private float direction;

    private bool isStair = false;
    private bool isGrounded = false;
    public LayerMask whatIsGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anm;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            Damage();
        }
        if (collision.tag == "Heal")
        {
            if (live < 6)
            {
                live++;
                Destroy(collision.gameObject);
            }
        }
        if (collision.tag == "Patrons")
        {
            if (allPatrons != 180)
            {
                if (allPatrons < 150)
                {
                    allPatrons += 30;
                    Destroy(collision.gameObject);
                }
                else
                {
                    allPatrons = 180;
                    Destroy(collision.gameObject);
                }
            }
        }
        if (collision.tag == "Stair")
        {
            isStair = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Stair")
        {
            isStair = false;
        }
    }

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anm = GetComponent<Animator>();
        matDefault = sprite.material;
        matBlink = Resources.Load<Material>("Materials/MaterialBlink");
        jump = Resources.Load<AudioClip>("Audio/Sounds/Jump");
        getHit = Resources.Load<AudioClip>("Audio/Sounds/Hit");
        reloadClip = Resources.Load<AudioClip>("Audio/Sounds/Reload");
        bullet = Resources.Load<Bullet>("Prefabs/Bullet");
        particlesShoot1 = transform.GetChild(5).GetChild(0).GetComponent<ParticleSystem>();
        particlesShoot2 = transform.GetChild(5).GetChild(1).GetComponent<ParticleSystem>();
        particlesBlood = transform.GetChild(5).GetChild(2).GetComponent<ParticleSystem>();
        timeOfReload = reloadTime;
    }

    void Update()
    {
        if (live > 0)
        {
            ChekGround();
            JumpAnimation();

            //Отсчет времени между выстрелами
            if (!canBullet) timeOfReload -= Time.deltaTime;
            if (timeOfReload <= 0 && patrons > 0)
            {
                canBullet = true;
                timeOfReload = reloadTime;
            }
            /*if (Input.GetButton("Horizontal")) Run();
            if (Input.GetButtonUp("Horizontal"))
            {
                direction = 0;
                anm.SetInteger("State", 0);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            if (Input.GetKey(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.Mouse0)) OnButtonDown(0);
            if (Input.GetKeyUp(KeyCode.Mouse0)) OnButtonUp(0);
            if (Input.GetKeyDown(KeyCode.R)) OnButtonDown(1);*/
            if (pressedShoot) Shoot();
            if (direction != 0) Run();
        }
    }

    public void OnButtonDown(int i)
    {
        switch (i)
        {
            case 0:
                pressedShoot = true;
                break;
            case 1:
                if (allPatrons != 0 && canReload)
                {
                    canReload = false;
                    shootSound.PlayOneShot(reloadClip);
                    canBullet = false;
                    timeOfReload = 0.85f;
                    Invoke("Reload", 0.8f);
                }
                break;
            case 2:
                direction = 1f;
                anm.SetInteger("State", 1);
                break;
            case 3:
                direction = -1f;
                anm.SetInteger("State", 1);
                break;
            case 4:
                if (!pause)
                {
                    pause = true;
                    pauseMenu.SetActive(true);
                    Time.timeScale = 0;
                }
                else
                {
                    pause = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1;
                }
                break;

        }
    }
    public void OnButtonUp(int i)
    {
        switch (i)
        {
            case 0:
                pressedShoot = false;
                break;
            case 1:
                direction = 0;
                rb.velocity = new Vector2(0f, rb.velocity.y);
                anm.SetInteger("State", 0);
                break;
        }
    }

    private void Run()
    {
        anm.SetInteger("State", 1);
        if (Input.GetAxis("Horizontal") < 0) direction = -1;
        if (Input.GetAxis("Horizontal") > 0) direction = 1;
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        sprite.flipX = direction < 0;
        if (isGrounded)
        {
            anm.Play("Run");
            if (!stepSound.isPlaying)
            {
                stepSound.PlayOneShot(stepSound.clip, 0.04f);
            }
        }

    }

    private void Jump()
    {
        if (isGrounded)
        {
            stepSound.PlayOneShot(jump, 1f);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anm.SetBool("IsGrounded", false);
        }
        if (isStair)
        {
            rb.velocity = new Vector2(rb.velocity.x, 8f);
        }
    }

    public void Shoot()
    {
        if (canBullet)
        {
            Vector3 position = transform.position; position.y += 1.2f;
            if (sprite.flipX)
            {
                position.x -= 1.4f;
                particlesShoot1.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(180f, Vector3.up));
                particlesShoot2.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(180f, Vector3.up));
            }
            else
            {
                position.x += 1.2f;
                particlesShoot1.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(0f, Vector3.up));
                particlesShoot2.transform.SetPositionAndRotation(position, Quaternion.AngleAxis(0f, Vector3.up));
            }
            position.y += UnityEngine.Random.Range(-0.1f, 0.1f);
            Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
            newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1f : 1f);
            newBullet.DestroyBullet(timeToDestroyBullet);
            shootSound.pitch = UnityEngine.Random.Range(0.7f, 0.9f);
            shootSound.PlayOneShot(shootSound.clip);
            if (particlesShoot1.isPlaying)
            {
                particlesShoot2.startSpeed = rb.velocity.x + 1; 
                particlesShoot2.Play();
            }
            else
            {
                particlesShoot1.startSpeed = rb.velocity.x + 1;
                particlesShoot1.Play();
            }
            canBullet = false;
            patrons--;
        }
    }

    public void Reload()
    {
        if (allPatrons > 30)
        {
            allPatrons -= 30 - patrons;
            patrons = 30;
        }
        else
        {
            int i = 30 - patrons;
            if (i > allPatrons)
            {

                patrons += allPatrons;
                allPatrons = 0;
            }
            else
            {
                patrons += i;
                allPatrons -= i;
            }
        }
        canReload = true;
    }

    public void Damage()
    {
        live--;
        stepSound.PlayOneShot(getHit, 0.2f);
        particlesBlood.Play();
        sprite.material = matBlink;
        Invoke("ResetMaterial", timeChekMat);
    }

    private void ResetMaterial()
    {
        if (chekMat)
        {
            sprite.material = matDefault;
            countChekMat--;
            chekMat = false;
            if (countChekMat > 0) Invoke("ResetMaterial", timeChekMat);
            else
            {
                timeChekMat = 0.15f;
                countChekMat = 3;
            }
        }
        else
        {
            timeChekMat /= 1.5f;
            sprite.material = matBlink;
            chekMat = true;
            Invoke("ResetMaterial", timeChekMat);
        }
    }
    //Проверка персонажа на нахождение на земле с помощью поиска колайдера земли 
    private void ChekGround()

    {
        isGrounded = Physics2D.OverlapBox(transform.position, new Vector2(0.85f, 0.3f), 0, whatIsGrounded);
        anm.SetBool("IsGrounded", isGrounded);
        anm.SetFloat("vSpeed", rb.velocity.y);
    }

    private void JumpAnimation()
    {
        if (!isGrounded)
        {
            if (rb.velocity.y == 0f) rb.velocity = new Vector2(0f, rb.velocity.y);
            anm.Play("Jump");
        }
        else
        {
            if (direction == 0) anm.Play("Idle");
        }
    }
}
