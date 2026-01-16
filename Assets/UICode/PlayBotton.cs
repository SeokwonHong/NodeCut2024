using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayBotton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnMouseDown()
    {
        SceneManager.LoadScene("HowToPlay");
    }
}
