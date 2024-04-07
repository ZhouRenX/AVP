using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PolySpatial.Samples
{
    public class DefButton : HubButton
    {
        public UnityEvent onClick;

        public override void Press()
        {
            base.Press();

            onClick.Invoke();
        }
    }
}
