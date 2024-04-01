using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;

namespace PolySpatial.Samples
{
    public class OpenLogButton : HubButton
    {
        [SerializeField]
        public GameObject reporter;

        public override void Press()
        {
            base.Press();
            reporter.SendMessage("doShow");

        }
    }
}
