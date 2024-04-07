using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System.Linq;



#if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    Transform m_TargetTransform;
    [SerializeField]
    InputActionReference m_PrimaryTouch;
    [SerializeField]
    Transform m_CameraTransform;

    [SerializeField]
    GameObject m_HeadPoseReticle;

    [SerializeField]
    public UnityEvent onPinch;

#if UNITY_INCLUDE_ARFOUNDATION
    GameObject m_SpawnedHeadPoseReticle;
    RaycastHit m_HitInfo;

    public ARPlane curPlane;


    void Start()
    {
        m_SpawnedHeadPoseReticle = Instantiate(m_HeadPoseReticle, Vector3.zero, Quaternion.identity);
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        m_PrimaryTouch.action.Enable();
    }

    void Update()
    {
        var YPosition = 0f;
        if (Physics.Raycast(new Ray(m_CameraTransform.position, m_CameraTransform.forward), out m_HitInfo))
        {
            if (m_HitInfo.transform.TryGetComponent(out ARPlane plane))
            {
                if (plane != null)
                {
                    curPlane = plane;
                    YPosition = m_HitInfo.point.y;
                }


                m_SpawnedHeadPoseReticle.transform.SetPositionAndRotation(m_HitInfo.point, Quaternion.FromToRotation(m_SpawnedHeadPoseReticle.transform.up, m_HitInfo.normal));
            }
        }


        var activeTouches = Touch.activeTouches;
        var primaryTouchData = m_PrimaryTouch.action.ReadValue<SpatialPointerState>();

        if (activeTouches.Count > 0)
        {
            var primaryTouchPhase = activeTouches[0].phase;
            if (primaryTouchPhase == TouchPhase.Began || primaryTouchPhase == TouchPhase.Moved)
            {
                // keep target position aligned with ground
                var worldPosition = primaryTouchData.interactionPosition;
                var targetPosition = new Vector3(worldPosition.x, YPosition, worldPosition.z);

                if (Physics.Raycast(new Ray(m_CameraTransform.position, targetPosition - m_CameraTransform.position), out m_HitInfo))
                {
                    if (m_HitInfo.transform.TryGetComponent(out ARPlane plane))
                    {
                        if (MathF.Abs(m_HitInfo.normal.y - 1f) < 0.1f)
                        {
                            m_TargetTransform.position = m_HitInfo.point;
                            onPinch.Invoke();
                        }

                    }
                }


            }
        }

    }

#endif
}
