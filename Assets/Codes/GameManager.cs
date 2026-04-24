using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //gameplay
    bool GameOver=false;
    public static GameManager Instance { get; private set; }

    private PlayerMovement playerMovement;
    private NodeManager nodeComponent;

    public GameObject refToScore;
    //score;
    //public TextMesh textMesh;

    public int score = 0;
    bool gameStart = false;
    private int lastScore = 0;
    // heart
    private GameObject heart1;
    private GameObject heart2;
    private GameObject heart3;

    public GameObject heart1Prefab;
    public GameObject heart2Prefab; 
    public GameObject heart3Prefab;

    byte heart=3;

    public TextMesh refToCountBeforeStart;
    int startSecond = 3;
    public bool isGameRunning = false;
    // 利 加档

    bool hasExecuted;

    TextMesh textMesh;

    // 笼

    public ParticleSystem particle;
    private void Start()
    {
        particle.gameObject.SetActive(false);

        nodeComponent = GetComponent<NodeManager>();
        playerMovement = GetComponent<PlayerMovement>();

        textMesh = refToScore.GetComponent<TextMesh>();
        StartCoroutine(EnemySponding());

        // 檬扁 窍飘 积己
        UpdateHearts();
        InvokeRepeating(nameof(DecreaseTime), 1.0f, 1.0f);
    }

    void DecreaseTime()
    {
        if (startSecond > 0)
        {
            startSecond--;
            refToCountBeforeStart.text = ""+startSecond;
        }
        if(startSecond==0)
        {
            refToCountBeforeStart.text = "START!";
            startSecond--;
            StartCoroutine(DestroyTextAfterDelay(1f));
        }
    }
    IEnumerator DestroyTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(refToCountBeforeStart);
        isGameRunning= true;
        particle.gameObject.SetActive(true);
    }

    private void Update()
    {

 
        if (heart == 2 && heart3 != null)
        {
            Destroy(heart3);
            heart3 = null;
        }

        if (heart == 1 && heart2 != null)
        {
            Destroy(heart2);
            heart2 = null;
        }

        if (heart == 0 && heart1 != null)
        {
            Destroy(heart1);
            heart1 = null;
            GameOver = true;
        }

      
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
        // Heart 积己 肺流
        if (heart >= 1 && heart1 == null)
            CreateHeart(0, ref heart1, heart1Prefab);

        if (heart >= 2 && heart2 == null)
            CreateHeart(1, ref heart2, heart2Prefab);

        if (heart >= 3 && heart3 == null)
            CreateHeart(2, ref heart3, heart3Prefab);
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
        textMesh.text = "Score: " + score;
    }

    public void ScoreDown()
    {
        
        //score -=5;
        heart--;
        
        if (score < 0) score = 0;
        textMesh.text = "Score: " + score;
    }
    public void Gold()
    {
        score+=5;
        textMesh.text = "Score: " + score;
        heart++;
    }
  
    

        


}
