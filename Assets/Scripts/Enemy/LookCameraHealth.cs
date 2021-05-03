using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCameraHealth : MonoBehaviour
{
    void Update()
    {
        this.transform.LookAt(Camera.main.transform);
    }
}
