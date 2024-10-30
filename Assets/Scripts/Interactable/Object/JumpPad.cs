using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            // �� ��° ������ ù ��° ������ ���� ���̸� �ٱ� ����, velocity�� y���� �ʱ�ȭ
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;

            rb.AddForce(Vector3.up * 200f, ForceMode.Impulse);
        }
    }
}
