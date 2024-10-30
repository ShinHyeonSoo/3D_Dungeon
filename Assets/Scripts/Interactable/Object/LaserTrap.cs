using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] GameObject _trapObj;
    [SerializeField] GameObject _warningPanel;

    private LayerMask _layer;
    private Ray[] _rays;

    private float _rayDistance = 2f;
    private float _lastCheckTime;
    private float _checkRate = 0.1f;
    private float _coroutineDelay = 0.3f;

    private bool _isDetect = false;

    // Start is called before the first frame update
    void Start()
    {
        _layer = 1 << CharacterManager.Instance.Player.gameObject.layer;

        _rays = new Ray[3];

        for(int i = 0; i < _rays.Length; ++i)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2.6f, transform.position.z - 0.7f + (i * 0.7f));
            _rays[i] = new Ray(pos, Vector3.down);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDetect && Time.time - _lastCheckTime > _checkRate)
        {
            _lastCheckTime = Time.time;

            if (RayHitCheck(_rays))
            {
                StartCoroutine(CoroutineDetect(_coroutineDelay));
            }
        }
    }

    private bool RayHitCheck(Ray[] rays)
    {
        RaycastHit hit;
        
        for (int i = 0; i < rays.Length; ++i)
        {
            if (Physics.Raycast(rays[i], out hit, _rayDistance, _layer))
            {
                _isDetect = true;
                return true;
            }

            Debug.DrawRay(rays[i].origin, rays[i].direction * _rayDistance, Color.red);
        }

        return false;
    }

    IEnumerator CoroutineDetect(float delay)
    {
        _trapObj.SetActive(true);
        _warningPanel.SetActive(true);

        yield return new WaitForSeconds(delay);

        _trapObj.GetComponentInChildren<Rigidbody>().isKinematic = false;
        _warningPanel.SetActive(false);
    }
}
