using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReStartBotton : MonoBehaviour
{
    public TextMesh refToScore;
    private void Update()
    {
        int score = PlayerPrefs.GetInt("Score", 0);

        refToScore.text = "" + score;
    }

    // Start is called before the first frame update
    public void OnMouseDown()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
