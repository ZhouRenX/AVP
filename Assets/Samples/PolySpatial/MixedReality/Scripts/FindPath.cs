using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


#if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

public class FindPath : MonoBehaviour
{
    [SerializeField]
    Transform m_CameraTransform;

    [SerializeField]
    GameObject m_PathPoint;

#if UNITY_INCLUDE_ARFOUNDATION
    RaycastHit m_HitInfo;

    public float rotateSpeed;
    Vector3 curDir;
    Vector3 targetDir;

    void Start()
    {
    }

    void Update()
    {
        if (Vector3.Angle(curDir, targetDir) > 1f)
        {
            if (Physics.Raycast(new Ray(m_CameraTransform.position, curDir), out m_HitInfo))
            {
                if (m_HitInfo.transform.TryGetComponent(out ARPlane plane))
                {
                    if (MathF.Abs(plane.normal.y - 1f) < 0.01f)
                    {
                        GameObject instance = Instantiate(m_PathPoint, m_HitInfo.point, Quaternion.FromToRotation(m_PathPoint.transform.up, m_HitInfo.normal));
                        Destroy(instance, 3f);
                    }
                }
            }

            float step = rotateSpeed * Time.deltaTime;
            curDir = Vector3.RotateTowards(curDir, targetDir, step, 0);
        }
    }

    public void GetPath()
    {
        curDir = m_CameraTransform.forward;
        targetDir = Vector3.down;

    }
#endif
}
