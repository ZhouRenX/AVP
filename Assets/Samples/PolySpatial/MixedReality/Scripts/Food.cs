using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisonEnter(Collision col)
    {
        if (col.gameObject.tag == "Pet")
        {
            Destroy(gameObject);
        }
    }
}
