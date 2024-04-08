using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands.Gestures;
#endif

public enum EHandActionState
{
    normal,
    start,
}
[Serializable]
public class DynamicHandAction
{
    public XRHandTrackingEvents handTrackingEvents;

    public XRHandShape startHandShape;
    public XRHandShape endHandShape;

    public float minimumStartHoldTime = 0.2f;
    public float maxDetectionTime = 0.5f;

    public UnityEvent startedEvent;
    public UnityEvent startedStopEvent;
    public UnityEvent endedEvent;
    public UnityEvent endedStopEvent;

    EHandActionState state = EHandActionState.normal;
    bool m_WasDetected;

    float m_HoldStartTime;
    float m_DetectionStartTime;

    public void Init()
    {
        handTrackingEvents.jointsUpdated.AddListener(CheckHand);
    }
    public void Remove()
    {
        handTrackingEvents.jointsUpdated.RemoveListener(CheckHand);
    }

    public void CheckHand(XRHandJointsUpdatedEventArgs eventArgs)
    {
        Debug.Log("CheckHand");
        bool detected = false;
        switch (state)
        {
            case EHandActionState.normal:
                {
                    detected = startHandShape != null && startHandShape.CheckConditions(eventArgs);
                    if (!m_WasDetected && detected)
                    {
                        m_HoldStartTime = Time.timeSinceLevelLoad;
                    }
                    m_WasDetected = detected;
                    if (detected)
                    {
                        var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
                        if (holdTimer > minimumStartHoldTime)
                        {
                            m_WasDetected = false;
                            startedEvent?.Invoke();
                            state = EHandActionState.start;
                            m_DetectionStartTime = Time.timeSinceLevelLoad;
                        }
                    }
                    else
                    {
                        startedStopEvent?.Invoke();
                    }
                    break;
                }
            case EHandActionState.start:
                {
                    if (Time.timeSinceLevelLoad - m_DetectionStartTime < maxDetectionTime)
                    {
                        detected = endHandShape != null && endHandShape.CheckConditions(eventArgs);
                        if (detected)
                        {
                            endedEvent?.Invoke();
                            state = EHandActionState.normal;
                        }
                    }
                    else
                    {
                        endedStopEvent?.Invoke();
                    }
                    break;
                }
            default:
                break;
        }

    }
}

public class HandGestureDetect : MonoBehaviour
{
    public DynamicHandAction[] dynamicHandActions;
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (var ht in dynamicHandActions)
        {
            ht.Init();
        }
    }
    void OnDisable()
    {
        foreach (var ht in dynamicHandActions)
        {
            ht.Remove();
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
