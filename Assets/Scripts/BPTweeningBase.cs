using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class BPTweeningBase : MonoBehaviour
{
    public Transform rootBone;
    int currentPathLength = 0;
    int curveStepsCompleted = 0;

    public void ClickItZTweenSeq(float duration, float zTo, float zFrom)
    {
        if (rootBone != null)
        {
            Sequence clickSequence = DOTween.Sequence();
            clickSequence.Append(rootBone.DOLocalMoveZ(zTo, duration / 2).SetEase(Ease.InSine));
            clickSequence.Append(rootBone.DOLocalMoveZ(zFrom, duration / 2).SetEase(Ease.OutSine));
            clickSequence.Play();
        }
        else
        {
            Debug.LogError("Root Bone not assigned in the Inspector!");
        }
    }

    public void ClickItZTween(float duration, float zTo, Ease ease = Ease.Linear)
    {
        if (rootBone != null)
        {
            rootBone.DOLocalMoveZ(zTo, duration).SetEase(ease);
        }
        else
        {
            Debug.LogError("Root Bone not assigned in the Inspector!");
        }
    }

    public void SimplePath(TweenBasePath seq)
    {
        if (rootBone != null)
        {
            Sequence rotateSequence = DOTween.Sequence();
            currentPathLength = seq.path.Length;
            curveStepsCompleted = 0;
            Vector3[] tempPath = new Vector3[seq.path.Length];
            float durationPerSegment = seq.duration / seq.path.Length;
            for (int i = 0; i < seq.path.Length; i++)
            {
                Vector3 tempVec = new Vector3(seq.path[i].localPosition.x, seq.path[i].localPosition.y, seq.path[i].localPosition.z);
                tempPath[i] = tempVec;
                if (i >= seq.path.Length - 1) continue;
                Quaternion nextRot = seq.path[i + 1].localRotation;
                rotateSequence.Append(rootBone.DOLocalRotateQuaternion(nextRot, durationPerSegment).SetEase(Ease.Linear).OnComplete(PathUpdate));
            }

            Vector3[] yourPoints = tempPath;
            List<Vector3> smoothCurve = BPCurveGenerator.GenerateSmoothCurve(yourPoints, 5);
            Vector3[] smoothPath = new Vector3[smoothCurve.Count - 3];
            for (int j = 0; j < smoothCurve.Count - 3; j++)
            {
                smoothPath[j] = smoothCurve[j];
            }
            smoothPath[smoothPath.Length - 1] = tempPath[tempPath.Length - 1];
            Ease ease = GetTweenEase(seq.easeInOut);
            rootBone.DOLocalPath(smoothPath, seq.duration).SetEase(ease);
            rotateSequence.OnStepComplete(PathFinished);
            rotateSequence.Play();
        }
        else
        {
            Debug.LogError("Root Bone not assigned in the Inspector!");
        }
    }

    public void CurvePathIt(TweenBasePath seq)
    {
        if (rootBone != null)
        {
            Sequence mySequence = DOTween.Sequence();
            currentPathLength = seq.path.Length;
            curveStepsCompleted = 0;
            // Calculate total distance
            float totalDistance = 0f;
            for (int i = 0; i < seq.path.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(seq.path[i].localPosition, seq.path[i + 1].localPosition);
            }
            // Calculate and assign time per segment based on distance if adjustDuration marked true
            float durationPerSegment = 0;
            for (int i = 0; i < seq.path.Length - 1; i++)
            {
                float segmentDistance = Vector3.Distance(seq.path[i].localPosition, seq.path[i + 1].localPosition);
                //if (seq.adjustDurationToDistance) durationPerSegment = seq.duration * (segmentDistance / totalDistance);
                durationPerSegment = seq.duration / seq.path.Length;

                Vector3 nextPos = seq.path[i + 1].localPosition;
                Quaternion nextRot = seq.path[i + 1].localRotation;

                Ease easeIn = Ease.Linear;
                Ease easeOut = Ease.Linear;
                mySequence.Append(rootBone.DOLocalMove(nextPos, durationPerSegment).SetEase(easeIn).OnComplete(PathUpdate));
                mySequence.Join(rootBone.DOLocalRotateQuaternion(nextRot, durationPerSegment).SetEase(easeIn));
            }

            mySequence.OnStepComplete(PathFinished);
            mySequence.Play();
        }
        else
        {
            Debug.LogError("Root Bone not assigned in the Inspector!");
        }
    }

    public void PathUpdate()
    {
        curveStepsCompleted++;
        if (curveStepsCompleted == currentPathLength - 2)
        {
            OnReadyToLandHandler();
        }
    }

    public void PathFinished()
    {
        OnPathFinishedHandler();
    }

    public delegate void PathEventHandler();

    public event PathEventHandler OnReadyToLand;

    public void OnReadyToLandHandler()
    {
        OnReadyToLand?.Invoke();
    }

    public event PathEventHandler OnPathFinished;

    public void OnPathFinishedHandler()
    {
        OnPathFinished?.Invoke();
    }

    public Ease GetTweenEase(DOTEase ease)
    {
        switch (ease)
        {
            case DOTEase.Linear:
                return Ease.Linear;
            case DOTEase.InOutCubic:
                return Ease.InOutCubic;
            case DOTEase.InOutQuad:
                return Ease.InOutQuad;
            case DOTEase.InOutQuart:
                return Ease.InOutQuart;
            case DOTEase.InOutSine:
                return Ease.InOutSine;
            case DOTEase.InOutQuint:
                return Ease.InOutQuint;
            case DOTEase.InOutCirc:
                return Ease.InOutCirc;
            case DOTEase.InOutExpo:
                return Ease.InOutExpo;
            case DOTEase.InOutFlash:
                return Ease.InOutFlash;
            default: return Ease.Linear;
        }
    }
}
public enum DOTEase
{
    Linear,
    InOutCubic,
    InOutQuad,
    InOutQuart,
    InOutSine,
    InOutQuint,
    InOutCirc,
    InOutExpo,
    InOutFlash
}

[Serializable]
public class TweenBasePath
{
    public string trigger;
    public float duration;
    public DOTEase easeInOut;
    public Transform[] path;
}
