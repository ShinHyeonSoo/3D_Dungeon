using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPos : MonoBehaviour
{
    private Vector3 _resetPos;

    // Start is called before the first frame update
    void Start()
    {
        _resetPos = CharacterManager.Instance.Player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.transform.position = _resetPos;
        }
    }
}
