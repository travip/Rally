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

    public Transform PlayerBox;
    public Transform DriverBox;
    public TextMeshProUGUI driverMsg;
    public TextMeshProUGUI playerMsg;

    public float MsgTime = 1.5f;
    private bool msgActive;

    public List<string> successMsgs = new List<string>();
    public List<string> missMsgs = new List<string>();
    public List<string> crashMsgs = new List<string>();

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
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayDriverMsg(crashMsgs[0]));
    }

    public void DisplayMissMessage()
    {
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayDriverMsg(missMsgs[0]));
    }

    public void DisplaySuccessMessage()
    {
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayDriverMsg(successMsgs[0]));
    }

    public IEnumerator DisplayPlayerMessage(string val)
    {
        playerMsg.text = val;
        PlayerBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(MsgTime);
        PlayerBox.gameObject.SetActive(false);
        msgActive = false;
    }

    public IEnumerator DisplayDriverMsg(string val)
    {
        playerMsg.text = val;
        DriverBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(MsgTime);
        DriverBox.gameObject.SetActive(false);
        msgActive = false;
    }
}
