using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum FadeState2 { FadeIn = 0, FadeOut, FadeInOut, FadeLoop }
public class Fadeinout2 : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10f)]
    private float fadeTime;
    private Image image;
    private FadeState2 fadeState2;

    private void Awake()
    {
        image = GetComponent<Image>();

        OnFade(FadeState2.FadeLoop);
    }

    //switch문을 통해 Fade-in,Fade-out,Fade-inout,Fade반복을 호출시킬 수 있게 만듬

    public void OnFade(FadeState2 state)
    {
        fadeState2 = state;

        switch (fadeState2)
        {
            case FadeState2.FadeIn:
                StartCoroutine(Fade(1, 0));
                break;
            case FadeState2.FadeOut:
                StartCoroutine(Fade(0, 1));
                break;
            case FadeState2.FadeInOut:
            case FadeState2.FadeLoop:

                StartCoroutine(FadeInOut());
                break;
        }
    }
    //fade in out 의 투명도를 0~1 사이 숫자를 통해 조금 덜 진하게 옅게를 조절한다.
    private IEnumerator FadeInOut()
    {
        while (true)
        {

            yield return StartCoroutine(Fade(0.7f, 0.2f));


            yield return StartCoroutine(Fade(0.2f, 0.7f));

            if (fadeState2 == FadeState2.FadeInOut)
            {
                break;
            }

        }
    }

    void Start()
    {

    }

    // 이 함수로 image의 투명도 시작점과 끝점을 입력해 조작한다.
    private IEnumerator Fade(float start, float end)
    {
        float currenTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            currenTime += Time.deltaTime;

            percent = currenTime / fadeTime;

            Color color = image.color;
            color.a = Mathf.Lerp(start, end, percent);
            image.color = color;


            yield return null;
        }

    }
}
