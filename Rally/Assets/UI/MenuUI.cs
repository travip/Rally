using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    public Image fadeCover;
    public bool isInstructionsOpen = false;
    public int instructionsPage = 0;

    public GameObject instructions;
    public GameObject instructionsPageOne;
    public GameObject instructionsPageTwo;

    // Update is called once per frame
    void Update ()
    {
        if (isInstructionsOpen)
        {
            if (Input.anyKeyDown)
            {
                NextPage();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInstructions();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(BeginFadeToGame(0.5f));
            }
        }
	}

    public void OpenInstructions()
    {
        isInstructionsOpen = true;
        instructionsPage = 1;
        instructions.gameObject.SetActive(true);
        instructionsPageOne.gameObject.SetActive(true);
    }

    public void NextPage()
    {
        if (instructionsPage == 1)
        {
            instructionsPageOne.gameObject.SetActive(false);
            instructionsPageTwo.gameObject.SetActive(true);
            instructionsPage++;
        }
        else
        {
            instructionsPageTwo.gameObject.SetActive(false);
            instructions.gameObject.SetActive(false);
            instructionsPage = 0;
            isInstructionsOpen = false;
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
