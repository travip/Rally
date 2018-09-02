using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarTextBox : MonoBehaviour
{

    public Car playerCar;
    public Camera mainCamera;

    Vector3 textPos;
    public Transform MsgBox;

    public Image PlayerBox;
    public Image PlayerInnerBox;
    public Image DriverBox;
    public Image DriverInnerBox;
    public TextMeshProUGUI driverMsg;
    public TextMeshProUGUI playerMsg;

    public Image PlayerArrowBox;
    public Image PlayerInnerArrowBox;
    public Image PlayerArrow;

    public List<Sprite> arrowSprites;

    public Conversation Debug1;
    public Conversation debug2;

    float FadeTime = 0.2f;

    public float MsgTime = 2f;
    private bool msgActive;
    private bool isConversing = false;

    public Conversation[] startMsgs1;
    public Conversation[] startMsgs2;
    public Conversation[] startMsgs3;

    public Conversation[] successMsgs1;
    public Conversation[] successMsgs2;
    public Conversation[] successMsgs3;

    public Conversation[] missMsgs1;
    public Conversation[] missMsgs2;
    public Conversation[] missMsgs3;

    public Conversation[] crashMsgs1;
    public Conversation[] crashMsgs2;
    public Conversation[] crashMsgs3;

    [System.Serializable]
    public struct Conversation
    {
        public string msg;
        public bool isPlayer;
    }

    void Update ()
    {
        if (msgActive)
        {
            textPos = mainCamera.WorldToScreenPoint(playerCar.TextBoxAnchor.position);
            MsgBox.position = textPos;
        }
    }

    public void DisplayStartMessage()
    {
        if (!isConversing)
            StartCoroutine(Converse(startMsgs1));
    }

    public void DisplayCrashMessage()
    {
        if(!isConversing)
            StartCoroutine(Converse(crashMsgs1));
    }

    public void DisplayMissMessage()
    {
        if (!isConversing)
            StartCoroutine(Converse(missMsgs2));
    }

    public void DisplaySuccessMessage()
    {
        if (!isConversing)
            StartCoroutine(Converse(successMsgs1));
    }

    public void TESTACTION()
    {
        StartCoroutine(DisplayAction( Player.ActionType.LeftTurn, true));
    }

    public IEnumerator DisplayAction(Player.ActionType action, bool correct)
    {
        isConversing = true;
        PlayerArrow.sprite = arrowSprites[(int)action];
        if (correct)
            PlayerArrow.color = Color.green;
        else
            PlayerArrow.color = Color.red;

        PlayerArrowBox.gameObject.SetActive(true);
        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color arrowColor = PlayerArrow.color;
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            arrowColor.a = Mathf.Lerp(0f, 1f, (elapsedTime / FadeTime));
            PlayerArrow.color = arrowColor;
            PlayerArrowBox.color = black;
            PlayerInnerArrowBox.color = white;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(MsgTime);
        if (!correct)
            StartCoroutine(DisplayDriverMsg("That wasn't right.."));

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color arrowColor = PlayerArrow.color;
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            arrowColor.a = Mathf.Lerp(1f, 0f, (elapsedTime / FadeTime));
            PlayerArrow.color = arrowColor;
            PlayerArrowBox.color = black;
            PlayerInnerArrowBox.color = white;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PlayerArrowBox.gameObject.SetActive(false);
        isConversing = false;
    }

    public IEnumerator Converse(Conversation[] msgs)
    {
        isConversing = true;
        for(int i = 0; i < msgs.Length; i++)
        {
            if (msgs[i].isPlayer)
                StartCoroutine(DisplayPlayerMessage(msgs[i].msg));
            else
                StartCoroutine(DisplayDriverMsg(msgs[i].msg));
            yield return new WaitForSeconds(MsgTime);
        }
        isConversing = false;
    }

    public IEnumerator DisplayPlayerMessage(string val)
    {
        playerMsg.text = val;
        PlayerBox.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color txt = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, (elapsedTime / FadeTime)));
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            PlayerBox.color = black;
            PlayerInnerBox.color = white;
            playerMsg.color = txt;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(MsgTime);

        elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color txt = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, (elapsedTime / FadeTime)));
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            PlayerBox.color = black;
            PlayerInnerBox.color = white;
            playerMsg.color = txt;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PlayerBox.gameObject.SetActive(false);
        
    }

    public IEnumerator DisplayDriverMsg(string val)
    {
        driverMsg.text = val;
        DriverBox.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color txt = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, (elapsedTime / FadeTime)));
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 0.5f, (elapsedTime / FadeTime)));
            DriverBox.color = black;
            DriverInnerBox.color = white;
            driverMsg.color = txt;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(MsgTime);

        elapsedTime = 0f;
        while (elapsedTime < FadeTime)
        {
            Color txt = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, (elapsedTime / FadeTime)));
            Color black = new Color(0f, 0f, 0f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            Color white = new Color(1f, 1f, 1f, Mathf.Lerp(0.5f, 0f, (elapsedTime / FadeTime)));
            DriverBox.color = black;
            DriverInnerBox.color = white;
            driverMsg.color = txt;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DriverBox.gameObject.SetActive(false);
        msgActive = false;
    }

}
