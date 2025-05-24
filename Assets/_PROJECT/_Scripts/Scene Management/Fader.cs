using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;

public class Fader : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float fadeTimer;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void FadeInOrOut(bool _fadeInOrOut)
    {
        StartCoroutine(FadeRoutine(_fadeInOrOut));
    }

    public IEnumerator FadeRoutine(bool _fadeInOrOut)
    {
        yield return image.DOFade(_fadeInOrOut ? 1 : 0, fadeTimer).WaitForCompletion();

    }
}
