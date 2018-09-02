using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    public Image fadeCover;

    // Turning UI
    [SerializeField]
    private Transform playerTurnInputList;
    [SerializeField]
    private TurnImage turnImage;
    [SerializeField]
    private Sprite rightBend, rightTurn, rightSquare, rightAcute;
    [SerializeField]
    private Sprite leftBend, leftTurn, leftSquare, leftAcute;
    [SerializeField]
    private Sprite straight;

    private readonly Queue<TurnImage> turnImageQueue = new Queue<TurnImage>();

    public bool started = false;
    public bool GameOverScreen = false;

    // Timer
    public float time;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI restartText;

    // Use this for initialization
    void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (started)
        {
            time += Time.deltaTime;
            UpdateTimer();
        }

    }

    public void FadeIn()
    {
        StartCoroutine(StartFadeIn(0.5f));
    }

    public void RestartGame()
    {
        StartCoroutine(StartFadeOut(0.5f));
    }

    private IEnumerator StartFadeOut(float transTime)
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
        SceneManager.LoadScene("Game");
    }

    private IEnumerator StartFadeIn(float transTime)
    {
        float elapsedTime = 0.0f;
        Color c = Color.black;
        while (elapsedTime < transTime)
        {
            c.a = Mathf.Lerp(1f, 0f, (elapsedTime / transTime));
            fadeCover.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Player.Instance.BeginCountdown();
    }

    public IEnumerator BeginCountdown()
    {
        GameOverScreen = false;
        restartText.gameObject.SetActive(false);
        mainText.text = "Ready";
        yield return new WaitForSeconds(2);
        mainText.text = "3";
        yield return new WaitForSeconds(1);
        mainText.text = "2";
        yield return new WaitForSeconds(1);
        mainText.text = "1";
        yield return new WaitForSeconds(1);
        mainText.text = "GO!";
        Player.Instance.StartGame();
        yield return new WaitForSeconds(1);
        mainText.gameObject.SetActive(false);
        started = true;
    }

    public void DisplayGameOver()
    {
        mainText.text = "CRASHED!";
        mainText.gameObject.SetActive(true);
        started = false;
        restartText.gameObject.SetActive(true);
        GameOverScreen = true;
    }

    private void UpdateTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timerText.text = string.Format("{0:D2}:{1:D2}.{2:D1}", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds/100));
    }

    public void SetMisses(int misses)
    {
        missText.text = misses.ToString(); ;
    }

    public void DequeueAction()
    {
        turnImageQueue.Dequeue().RemoveThisItem();
    }

    public void AddTurnImageToList(Player.ActionType actionType)
    {
        if(turnImageQueue.Count == 5)
        {
            Destroy(turnImageQueue.Dequeue().gameObject);
        }
        TurnImage newTurn = Instantiate(turnImage, playerTurnInputList);
        newTurn.transform.SetAsFirstSibling();
        switch(actionType)
        {
            case Player.ActionType.Straight:
                newTurn.image.sprite = straight;
                break;
            //case Player.ActionType.Slow:
            //    newTurn.image.sprite = slow;
            //    break;
            case Player.ActionType.RightBend:
                newTurn.image.sprite = rightBend;
                break;
            case Player.ActionType.RightTurn:
                newTurn.image.sprite = rightTurn;
                break;
            case Player.ActionType.RightSquare:
                newTurn.image.sprite = rightSquare;
                break;
            case Player.ActionType.RightAcute:
                newTurn.image.sprite = rightAcute;
                break;
            case Player.ActionType.LeftBend:
                newTurn.image.sprite = leftBend;
                break;
            case Player.ActionType.LeftTurn:
                newTurn.image.sprite = leftTurn;
                break;
            case Player.ActionType.LeftSquare:
                newTurn.image.sprite = leftSquare;
                break;
            case Player.ActionType.LeftAcute:
                newTurn.image.sprite = leftAcute;
                break;
        }
        turnImageQueue.Enqueue(newTurn);
    }
}
