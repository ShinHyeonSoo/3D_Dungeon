using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LauncherType
{
    Timer,
    InputKey
}

public class PlatformLauncher : MonoBehaviour
{
    [SerializeField] private LauncherType _type;
    [SerializeField] private float _launchDelay = 3f;

    private Rigidbody _playerRb;

    private bool _isTimeOnPlatform = false;
    private float _timeOnPlatform = 0f;

    private void Start()
    {
        _playerRb = CharacterManager.Instance.Player.gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(_isTimeOnPlatform)
        {
            switch (_type)
            {
                case LauncherType.Timer:
                    {
                        _timeOnPlatform += Time.deltaTime;

                        if (_timeOnPlatform > _launchDelay)
                        {
                            Launch();
                            _timeOnPlatform = 0f;
                        }
                    }
                    break;
                case LauncherType.InputKey:
                    {
                        if(Input.GetKeyDown(KeyCode.E))
                        {
                            Launch();
                        }
                    }
                    break;
            }
        }
    }

    private void Launch()
    {
        _playerRb.velocity = Vector3.zero;

        Vector3 dir = _playerRb.gameObject.transform.forward;
        _playerRb.AddForce(dir * 400f, ForceMode.Impulse);
        _playerRb.AddForce(Vector3.up * 200f, ForceMode.Impulse);

        CharacterManager.Instance.Player.Controller.IsFlying = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTimeOnPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTimeOnPlatform = false;
        }
    }
}
