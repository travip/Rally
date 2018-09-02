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
    public Image MsgBox;
    public TextMeshProUGUI msg;

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
            MsgBox.transform.position = textPos;
        }
    }

    public void DisplayCrashMessage()
    {
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(crashMsgs[0]));
    }

    public void DisplayMissMessage()
    {
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(missMsgs[0]));
    }

    public void DisplaySuccessMessage()
    {
        msgActive = true;
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(successMsgs[0]));
    }

    public IEnumerator DisplayMessage(string val)
    {
        msg.text = val;
        MsgBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(MsgTime);
        MsgBox.gameObject.SetActive(false);
        msgActive = false;
    }
}
