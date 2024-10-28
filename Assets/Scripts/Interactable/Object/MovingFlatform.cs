using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingFlatform : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _speed;

    private bool _isArrived = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isArrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endPos, _speed * Time.deltaTime);

            if (transform.position == _endPos)
                _isArrived = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPos, _speed * Time.deltaTime);

            if (transform.position == _startPos)
                _isArrived = false;
        }

        transform.eulerAngles += new Vector3(0f, 60f * Time.deltaTime, 0f);
        //transform.Translate(Vector3.left * 0.4f * Time.deltaTime);
        //transform.Translate(Vector3.up * 0.2f *  Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
