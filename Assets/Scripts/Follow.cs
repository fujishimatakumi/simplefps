using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Follow : MonoBehaviour
{

    void Update()
    {
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

}

