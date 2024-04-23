using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands.Gestures;


[Serializable]
public class DynamicHandAction
{
    public XRHandTrackingEvents handTrackingEvents;

    public XRHandShape handShape;

    public float minimumStartHoldTime = 0.2f;

    public UnityEvent<XRHandJointsUpdatedEventArgs> performEvent;
    public UnityEvent<XRHandJointsUpdatedEventArgs> endEvent;

    bool m_WasDetected;
    bool m_WasPerformed = false;

    float m_HoldStartTime;

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
        bool detected = false;

        detected = handShape != null && handShape.CheckConditions(eventArgs);
        if (!m_WasDetected && detected)
        {
            m_HoldStartTime = Time.timeSinceLevelLoad;
        }
        else if (m_WasPerformed && !detected)
        {
            m_WasPerformed = false;
            endEvent?.Invoke(eventArgs);
        }
        m_WasDetected = detected;
        if (detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
            if (holdTimer > minimumStartHoldTime)
            {
                m_WasPerformed = true;
                Debug.Log("perform");
                performEvent?.Invoke(eventArgs);
            }
        }

    }
}

public class VRHandDetection : MonoBehaviour
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
