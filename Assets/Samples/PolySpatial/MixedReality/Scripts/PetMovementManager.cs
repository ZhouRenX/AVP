using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMovementManager : MonoBehaviour
{
    const float k_DeadZoneDistance = 0.04f;
    const float k_MaxDistance = 0.5f;

    [SerializeField]
    Animator m_PetAnimator;

    [SerializeField]
    Transform m_PetTransform;
    [SerializeField]
    float moveSpeed = 2f;


    public List<Vector3> path;
    public int pathStep = 0;

    void Update()
    {
        if (path.Count > 0)
            if (pathStep < path.Count)
            {
                Debug.Log("Move");
                Move();
            }
            else
            {
                StopMove();
            }
    }

    void Move()
    {
        var distance = Vector3.Distance(m_PetTransform.position, path[pathStep]);
        Debug.Log("distance " + distance);
        if (distance >= k_DeadZoneDistance)
        {
            var position = path[pathStep];
            m_PetTransform.position = Vector3.MoveTowards(m_PetTransform.position, position, Time.deltaTime * moveSpeed);

            m_PetTransform.LookAt(position);

            Vector3 deltaOffset = path[pathStep] - m_PetTransform.position;

            var normalizedSpeedXZ = new Vector3(deltaOffset.x, 0, deltaOffset.z) / k_MaxDistance;
            var normalizedSpeedY = new Vector3(0, deltaOffset.y, 0) / k_MaxDistance;

            m_PetAnimator.SetFloat("Speed", normalizedSpeedXZ.magnitude);
            m_PetAnimator.SetFloat("Angle", normalizedSpeedY.magnitude);
        }
        else
        {
            pathStep++;
        }
    }
    void StopMove()
    {
        m_PetAnimator.SetFloat("Speed", 0);
    }

    public void GetPath(List<Vector3> path)
    {
        this.path = path;
        pathStep = 0;
    }
}
