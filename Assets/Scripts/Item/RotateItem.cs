using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles += new Vector3(0f, 60f * Time.deltaTime, 0f);
    }
}
