using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //gameplay
    bool GameOver=false;
    public static GameManager Instance { get; private set; }

    private PlayerMovement playerMovement;
    private NodeManager nodeComponent;

    public GameObject refToScore;
    //score;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI countBeforeStartText;


    bool gameStart = false;
    private int lastScore = 0;
    // heart
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;

    byte heart=3;

    private int startSecond = 3;
    public bool isGameRunning = false;


    bool hasExecuted;
    public ParticleSystem particle;

    //Getter
    private int score = 0;
    public int Score => score;


    private void Start()
    {
        particle.gameObject.SetActive(false);

        nodeComponent = GetComponent<NodeManager>();
        playerMovement = GetComponent<PlayerMovement>();

        scoreText = refToScore.GetComponent<TextMeshProUGUI>();
        StartCoroutine(EnemySponding());

  
        UpdateHearts();
        countBeforeStartText.text = startSecond.ToString();
        InvokeRepeating(nameof(CountDown), 1f, 1f);
    }

    void CountDown()
    {
        if (startSecond > 0)
        {
            startSecond--;
            countBeforeStartText.text = startSecond.ToString();
        }
        else if (startSecond == 0)
        {
            countBeforeStartText.text = "START!";
            startSecond = -1; // prevent repeat
            StartCoroutine(DestroyTextAfterDelay(1f));
        }
    }
    IEnumerator DestroyTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        countBeforeStartText.gameObject.SetActive(false);
        isGameRunning = true;
        particle.gameObject.SetActive(true);
    }

    private void Update()
    {

      
        UpdateHearts();


        if (heart > 3) heart = 3;



        if (!hasExecuted && score % 10 == 0 && score / 10 >= 1)
        {
            nodeComponent.speedA += 0.1f;
            hasExecuted = true;
        }

        if (score % 10 != 0)
        {
            hasExecuted = false;
        }

        if (heart <= 0)
        {
            SceneManager.LoadScene("EndGame");
        }

        if (GameOver)
        {
            PlayerPrefs.SetInt("Score", score); 
            PlayerPrefs.Save(); 
        }
    }

    private void UpdateHearts()
    {
        heart1.enabled = heart >= 1;
        heart2.enabled = heart >= 2;
        heart3.enabled = heart >= 3;
    }

    private void CreateHeart(int index, ref GameObject heart, GameObject prefab)
    {
        Vector3[] positions = {
        new Vector3(-8, -3.4f, 0),  
        new Vector3(-7.3f, -3.46f, 0), 
        new Vector3(-6.59f, -3.46f, 0) 
    };

        heart = Instantiate(prefab);
        heart.transform.position = positions[index]; 
        heart.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
    }

    
    private void GameStart()
    {
        if (isGameRunning)
        {
            nodeComponent.GStart();
        }
        
    }

    IEnumerator EnemySponding()
    {
        while (true)
        {
            nodeComponent.EnemyRespone();
            yield return new WaitForSeconds(1f);
        }
    }
    public void ScoreUp()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    public void ScoreDown()
    {
        if (heart > 0)
            heart--;

        UpdateHearts();

        if (heart <= 0)
        {
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();
            SceneManager.LoadScene("EndGame");
        }
    }
    public void Gold()
    {
        score += 5;

        if (heart < 3)
            heart++;

        scoreText.text = "Score: " + score;
        UpdateHearts();
    }






}
