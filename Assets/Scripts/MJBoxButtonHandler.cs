using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MJBoxButtonHandler : BPTweeningBase
{
    public int index = 0;
    public float changeColorDelay = 0.25f;
    public float tweenDownDuration = 0.3f;
    [HideInInspector]
    public MjGridPosition gridPosition;
    [HideInInspector]
    public MjColors colorName;
    [HideInInspector]
    public bool useMouseEvents = false;
    [HideInInspector]
    public float tweenDepth = 0.72f;
    Vector3 originalocalPosition;
    Coroutine tweenCoroutine;
    Coroutine holdCoroutine;
    Coroutine sendCoroutine;
    bool isTriggerButtonDownSubscribed = false;
    bool isTriggerButtonHoldSubscribed = false;

    void Start()
    {
        rootBone = transform;
        originalocalPosition = transform.localPosition;

        Renderer renderer = GetComponent<Renderer>();
        Material newMaterial = new Material(renderer.sharedMaterial);
        renderer.material = newMaterial;
    }

    public void SubscribeToOnButtonDown(ButtonEventHandler callback)
    {
        if (!isTriggerButtonDownSubscribed)
        {
            OnButtonDown += callback;
            isTriggerButtonDownSubscribed = true;
        }
    }

    public void SubscribeToOnButtonHold(ButtonEventHandler callback)
    {
        if (!isTriggerButtonHoldSubscribed)
        {
            OnButtonHold += callback;
            isTriggerButtonHoldSubscribed = true;
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
        OnButtonDownHandler(gridPosition);
    }

    void SetColor(Color color)
    {
        Material mat = GetComponent<Renderer>().material;
        if (mat.HasProperty("_RampColor2"))
        {
            mat.SetColor("_RampColor2", color);
        }
        else
        {
            mat.SetColor("_BaseColor", color);
        }
    }

    private void OnMouseDown()
    {
        if (!useMouseEvents) return;
        OnButtonDownHandler(gridPosition);
        ClickItZTween(tweenDownDuration, originalocalPosition.z + tweenDepth, Ease.Linear);
        holdCoroutine = StartCoroutine(HoldCheck());
    }

    private void OnMouseUp()
    {
        if (!useMouseEvents) return;
        if (holdCoroutine != null) StopCoroutine(holdCoroutine);
        OnButtonUpHandler(gridPosition);
        ClickItZTween(tweenDownDuration, originalocalPosition.z, Ease.Linear);
    }
    private IEnumerator MouseDownTriggered()
    {
        yield return new WaitForSeconds(tweenDownDuration);
        tweenCoroutine = null;
        ClickItZTween(tweenDownDuration, originalocalPosition.z, Ease.Linear);
    }
    private IEnumerator SendPosition()
    {
        yield return new WaitForSeconds(changeColorDelay);
        sendCoroutine = null;
        OnButtonDownHandler(gridPosition);
    }
    private IEnumerator HoldCheck()
    {
        yield return new WaitForSeconds(1f);
        holdCoroutine = null;
        OnButtonHoldHandler(gridPosition);
    }

    public delegate void ButtonEventHandler(MjGridPosition position);
    public event ButtonEventHandler OnButtonDown;
    public event ButtonEventHandler OnButtonHold;
    public event ButtonEventHandler OnButtonUp;
    public void OnButtonDownHandler(MjGridPosition position)
    {
        OnButtonDown?.Invoke(position);
    }
    public void OnButtonHoldHandler(MjGridPosition position)
    {
        OnButtonHold?.Invoke(position);
    }
    public void OnButtonUpHandler(MjGridPosition position)
    {
        OnButtonUp?.Invoke(position);
    }
}

