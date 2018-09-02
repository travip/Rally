using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    public Image fadeCover;
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BeginFadeToGame(0.5f));
        }
	}

    IEnumerator BeginFadeToGame(float transTime)
    {
        float elapsedTime = 0.0f;
        Color c = Color.black;
        while (elapsedTime < transTime)
        {
            c.a = Mathf.Lerp(0f, 1f, (elapsedTime / transTime));
            fadeCover.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
