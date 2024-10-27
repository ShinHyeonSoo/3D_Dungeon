using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            // 두 번째 점프도 첫 번째 점프와 같은 높이를 뛰기 위해, velocity의 y값을 초기화
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;

            rb.AddForce(Vector3.up * 200f, ForceMode.Impulse);
        }
    }
}
