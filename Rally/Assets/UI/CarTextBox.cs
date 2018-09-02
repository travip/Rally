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

    public Conversation Debug1;
    public Conversation debug2;

    float FadeTime = 0.2f;

    public float MsgTime = 2f;
    private bool msgActive;
    private bool isConversing = false;

    public List<Conversation[]> startMsgs = new List<Conversation[]>();
    public List<Conversation[]> successMsgs = new List<Conversation[]>();
    public List<Conversation[]> missMsgs = new List<Conversation[]>();
    public List<Conversation[]> crashMsgs = new List<Conversation[]>();

    public List<Conversation[]> leftTurnMsgs = new List<Conversation[]>();
    public List<Conversation[]> leftSquareMsgs = new List<Conversation[]>();
    public List<Conversation[]> leftAcuteMsgs = new List<Conversation[]>();

    public List<Conversation[]> rightTurnMsgs = new List<Conversation[]>();
    public List<Conversation[]> rightSquareMsgs = new List<Conversation[]>();
    public List<Conversation[]> rightAcuteMsgs = new List<Conversation[]>();

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

    public void DisplayCrashMessage()
    {
        if(!isConversing)
            StartCoroutine(Converse(crashMsgs[0]));
    }

    public void DisplayMissMessage()
    {
        if (!isConversing)
            StartCoroutine(Converse(missMsgs[0]));
    }

    public void DisplaySuccessMessage()
    {
        if (!isConversing)
            StartCoroutine(Converse(successMsgs[0]));
    }

    public void TESTCONVERSE()
    {
        StartCoroutine(Converse( new Conversation[] { Debug1, debug2 }));
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
