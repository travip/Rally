using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnImage : MonoBehaviour
{
    public const float SHRINK_TIME = 0.2f;
    public Image image;
    public Vector3 startSize;


    public void Start()
    {
        StartCoroutine(ShrinkIn(SHRINK_TIME));
    }

    private IEnumerator ShrinkIn(float shrinkTime)
    {
        float elapsedTime = 0f;
        while(elapsedTime < shrinkTime)
        {
            transform.localScale = Vector3.Lerp(startSize, Vector3.one, (elapsedTime / shrinkTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void RemoveThisItem()
    {
        StartCoroutine(FadeOut(SHRINK_TIME));
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            Color c = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0, (elapsedTime / fadeTime)));
            image.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
