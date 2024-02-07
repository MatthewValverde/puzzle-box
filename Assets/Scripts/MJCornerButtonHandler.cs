using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class MJCornerButtonHandler : BPTweeningBase
{
    public int index = 0;
    public float tweenDownDuration = 0.1f;
    [HideInInspector]
    public MjGridPosition gridPosition;
    [HideInInspector]
    public MjColors colorName;
    [HideInInspector]
    public MjColors cornerColorToBe;
    [HideInInspector]
    public bool useMouseEvents = false;
    [HideInInspector]
    public float tweenDepth = 0.15f;
    Vector3 originalocalPosition;
    Coroutine tweenCoroutine;
    Coroutine holdCoroutine;
    Coroutine sendCoroutine;
    //public event Action OnCornerDown;
    private bool isTriggerCornerClickSubscribed = false;

    void Start()
    {
        rootBone = transform;
        originalocalPosition = transform.localPosition;
    }

    public void SubscribeToOnCornerDown(CornerEventHandler callback)
    {
        if (!isTriggerCornerClickSubscribed)
        {
            OnCornerDown += callback;
            isTriggerCornerClickSubscribed = true;
        }
    }

    public void SetColorTint(MjButtonColor buttonColor)
    {
        colorName = buttonColor.name;
        SetColor(buttonColor.color);
    }

    public void SetColorTint(Color buttonColor)
    {
        SetColor(buttonColor);
    }

    public void TriggerClick()
    {
        if (useMouseEvents) return;
        if (tweenCoroutine != null) return;
        tweenCoroutine = StartCoroutine(MouseDownTriggered());
        ClickItZTween(tweenDownDuration, originalocalPosition.z + tweenDepth, Ease.Linear);
    }

    void SetColor(Color color)
    {
        Material[] mat = GetComponent<Renderer>().materials;
        mat[1].SetColor("_BaseColor", color);
    }

    private void OnMouseDown()
    {
        if (!useMouseEvents) return;
        OnCornerDownHandler(index); 
        ClickItZTween(tweenDownDuration, originalocalPosition.z + tweenDepth, Ease.Linear);
        tweenCoroutine = StartCoroutine(MouseDownTriggered());
    }
    private IEnumerator MouseDownTriggered()
    {
        yield return new WaitForSeconds(tweenDownDuration);
        tweenCoroutine = null;
        ClickItZTween(tweenDownDuration, originalocalPosition.z, Ease.Linear);
    }

    public delegate void CornerEventHandler(int index);
    public event CornerEventHandler OnCornerDown;
    public void OnCornerDownHandler(int index)
    {
        OnCornerDown?.Invoke(index);
    }
}
