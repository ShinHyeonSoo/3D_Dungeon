using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatformLauncher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;

            Vector3 dir = other.transform.forward;
            rb.AddForce(dir * 400f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 200f, ForceMode.Impulse);

            CharacterManager.Instance.Player.Controller.IsFlying = true;
        }
    }
}
