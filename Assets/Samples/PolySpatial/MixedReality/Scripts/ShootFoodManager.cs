using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands.Gestures;
#endif
public class ShootFoodManager : MonoBehaviour
{
    public GameObject foodPrefab;
    public void ShootFood(XRHandJointsUpdatedEventArgs eventArgs)
    {
        XRHand hand = eventArgs.hand;
        XRHandJoint indexTipJoint = hand.GetJoint(XRHandJointID.IndexTip);

        if (indexTipJoint.TryGetPose(out Pose pose))
        {
            GameObject instance = Instantiate(foodPrefab, pose.position,pose.rotation);
            instance.GetComponent<Rigidbody>().AddForce(pose.forward*5f,ForceMode.Impulse);
            
        
        }
    }
}
