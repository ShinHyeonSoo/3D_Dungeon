using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class Interaction : MonoBehaviour
{
    public float _checkRate = 0.05f;
    private float _lastCheckTime;

    public float _maxCheckDistance;
    public LayerMask _layerMask;

    public GameObject _curInteractObj;
    private IInteractable _curInteractable;

    public TextMeshProUGUI _promptText;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Time.time - _lastCheckTime > _checkRate)
        {
            _lastCheckTime = Time.time;
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward);
            //Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            
            //Debug.DrawLine(ray.origin, ray.direction, Color.red);

            if (Physics.Raycast(ray, out hit, _maxCheckDistance, _layerMask))
            {
                if (hit.collider.gameObject != _curInteractObj)
                {
                    _curInteractObj = hit.collider.gameObject;
                    _curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                _curInteractObj = null;
                _curInteractable = null;
                _promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        _promptText.gameObject.SetActive(true);
        _promptText.text = _curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _curInteractable != null)
        {
            _curInteractable.OnInteract();
            _curInteractObj = null;
            _curInteractable = null;
            _promptText.gameObject.SetActive(false);
        }
    }
}
