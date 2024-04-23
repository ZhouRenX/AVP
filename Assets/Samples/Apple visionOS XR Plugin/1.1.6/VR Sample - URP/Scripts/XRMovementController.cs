using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands.Gestures;


public class XRMovementController : MonoBehaviour
{
    public Transform xrOrigin;
    public Transform cam;
    public Transform rayOrigin;
    public Vector3 moveDir;
    public float xInput;
    public float zInput;
    public float speed;

    bool hasXInput, hasZInput = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();
        if (moveDir.magnitude > 0)
            xrOrigin.position += moveDir;
    }

    void UpdateSpeed()
    {
        moveDir = (hasXInput ? xInput : 0) * cam.right + (hasZInput ? zInput : 0) * cam.forward;
        moveDir = moveDir.normalized * speed * Time.deltaTime;
        hasXInput = false; hasZInput = false;
    }

    public void UpdateXInput(float x)
    {
        Debug.Log("UpdateXInput " + x);
        hasXInput = true;
        xInput = x;
    }

    public void UpdateZInput(float z)
    {
        hasZInput = true;
        zInput = z;
    }

    public void IndexMove(XRHandJointsUpdatedEventArgs eventArgs)
    {

        XRHand hand = eventArgs.hand;
        XRHandJoint indexTipJoint = hand.GetJoint(XRHandJointID.IndexTip);
        if (indexTipJoint.TryGetPose(out Pose pose))
        {
            float x = pose.up.y * -1f;
            Debug.Log("xAngle " + x);

            UpdateXInput(x);
            UpdateZInput(1);
        }
    }

    public void PinchMove(XRHandJointsUpdatedEventArgs eventArgs)
    {

        Debug.Log("PinchMove");

        Vector3 linearVelocity = rayOrigin.forward.normalized;
        float zAngle = Vector3.Angle(new Vector3(cam.forward.x, 0, cam.forward.z), new Vector3(linearVelocity.x, 0, linearVelocity.z));
        Debug.Log("zAngle " + zAngle);
        zAngle *= (Mathf.PI / 180);

        float xAngle = Vector3.Angle(new Vector3(cam.right.x, 0, cam.right.z), new Vector3(linearVelocity.x, 0, linearVelocity.z));
        Debug.Log("xAngle " + xAngle);
        xAngle *= (Mathf.PI / 180);

        UpdateXInput(Mathf.Cos(xAngle));
        UpdateZInput(Mathf.Cos(zAngle));



    }
}
