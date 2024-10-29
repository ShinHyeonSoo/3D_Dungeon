using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGround : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.Controller.IsFlying = false;
            CharacterManager.Instance.Player.Animator.SetBool("Jump", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.Controller.IsFlying = false;
            CharacterManager.Instance.Player.Animator.SetBool("Jump", false);
        }
    }
}
