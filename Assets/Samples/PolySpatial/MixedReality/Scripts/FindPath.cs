using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    Transform m_PinchTargetTransform;
    [SerializeField]
    Transform m_PetTransform;

#if UNITY_INCLUDE_ARFOUNDATION
    RaycastHit m_HitInfo;

    public float raycastMoveSpeed;
    Vector3 curDir;
    Vector3 targetDir;

    List<Vector3> path = new List<Vector3>();

    public float m_FindPathInterval = 0.1f;
    float m_TimeOfLastConditionCheck;

    void Start()
    {
    }

    void Update()
    {
    }
    void TryGetPath()
    {
        if (Time.timeSinceLevelLoad > m_TimeOfLastConditionCheck + m_FindPathInterval)
        {
            m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
            StopCoroutine("GetPath");
            StartCoroutine("GetPath");
        }
    }
    IEnumerator GetPath()
    {
        path = new List<Vector3>();
        while (Vector3.Distance(curDir, targetDir) > .05f)
        {
            if (Physics.Raycast(new Ray(m_CameraTransform.position, curDir - m_CameraTransform.position), out m_HitInfo))
            {
                if (m_HitInfo.transform.TryGetComponent(out ARPlane plane))
                {
                    if (Vector3.Angle(m_HitInfo.normal,Vector3.up) < 10f)
                    {
                        path.Add(m_HitInfo.point);
                        GameObject instance = Instantiate(m_PathPoint, m_HitInfo.point, Quaternion.FromToRotation(m_PathPoint.transform.up, m_HitInfo.normal));
                        Destroy(instance, 3f);
                    }
                }
            }

            float step = raycastMoveSpeed * Time.deltaTime;
            curDir = Vector3.MoveTowards(curDir, targetDir, step);

            yield return null;
        }
        GetComponent<PetMovementManager>().GetPath(path);
    }

    public void GetPathToPinchPoint()
    {
        curDir = m_PetTransform.position;
        targetDir = m_PinchTargetTransform.position;
        TryGetPath();
    }

    public void GetPathToCamera()
    {
        curDir = m_PetTransform.position;
        targetDir = new Vector3(m_CameraTransform.position.x, m_PetTransform.position.y, m_CameraTransform.position.z);
        TryGetPath();
    }
#endif
}
