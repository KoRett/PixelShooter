using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController instance = null;
    int sceneIndex;
    int levelComplete;
    private bool flgDie = true;

    private Player player;
    public MainMenu mainMenu;

    private Animator[] lives;

    private Text patrons;
    private Text allPatrons;
    private Animator dieScreen;

    private AudioSource soundButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        soundButton = GetComponent<AudioSource>();

        player = FindFirstObjectByType<Player>();
        lives = new Animator[player.live];
        for (int i = 0; i < player.live; i++)
        {
            lives[i] = player.transform.GetChild(2).GetChild(0).GetChild(i).GetComponent<Animator>();
        }
        patrons = player.transform.GetChild(2).GetChild(1).GetChild(0).GetComponentInChildren<Text>();
        allPatrons = player.transform.GetChild(2).GetChild(1).GetChild(1).GetComponentInChildren<Text>();
        dieScreen = player.transform.GetChild(2).GetChild(10).GetComponent<Animator>();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        levelComplete = PlayerPrefs.GetInt("LevelComplete");
    }

    public void isEndGame()
    {
        PlayerPrefs.SetInt("LevelComplete", sceneIndex);
        Invoke("LoadMainMenu", 1f);
        PlayerPrefs.SetInt("StateMenu", 1);
    }


    public void LoadMainMenu()
    {
        SceneLoading.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Restart()
    {

        dieScreen.SetBool("Die", false);
        dieScreen.Play("Idle");
        SceneLoading.LoadScene(sceneIndex);
        Time.timeScale = 1;
    }

    void Update()
    {
        for (int i = lives.Length - 1; i >= 0; i--)
        {
            if (player.live - 1 < i)
            {
                lives[i].SetInteger("State", 1);
                if (player.live <= 0 && flgDie)
                {
                    dieScreen.SetTrigger("Die");
                    flgDie = false;
                }
            }
            else
            {
                lives[i].SetInteger("State", 0);
            }
        }

        patrons.text = System.Convert.ToString(player.patrons);
        allPatrons.text = System.Convert.ToString(player.allPatrons);
    }

    public void DownSetScaleOfButton(GameObject button)
    {
        button.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
    }

    public void UpSetScaleOfButton(GameObject button)
    {
        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnSoundButton()
    {
        soundButton.Play();
    }

}
