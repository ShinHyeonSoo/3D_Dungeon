using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float _moveSpeed;
    public float _jumpPower;
    public LayerMask _groundLayerMask;
    private Vector2 _curMovementInput;
    private float _originMoveSpeed;
    private float _runSpeedRate = 2f;
    private float _runStaminaLimit = 20f;
    private bool _isDoubleJump = false;
    private int _jumpCount;

    [Header("Look")]
    public Transform _cameraContainer;
    public float _lookSensitivity;
    private Vector2 _mouseDelta;
    public bool _canLook = true;

    public event Action Inventory;
    private Rigidbody _rigidbody;

    public bool IsDoubleJump { get { return _isDoubleJump; } set { _isDoubleJump = value; } }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _originMoveSpeed = _moveSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (_canLook)
            CameraLook();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= _moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        transform.eulerAngles += new Vector3(0, _mouseDelta.x * _lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            // 두 번째 점프도 첫 번째 점프와 같은 높이를 뛰기 위해, velocity의 y값을 초기화
            Vector3 velocity = _rigidbody.velocity;
            velocity.y = 0;
            _rigidbody.velocity = velocity;

            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
            ++_jumpCount;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && HasStemina())
        {
            _moveSpeed *= _runSpeedRate;
            CharacterManager.Instance.Player.Condition._isRun = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // TODO : 조건분기, 아이템 먹었을때 달리기와 아닐때 달리기 속도 되돌리기 처리
            RestoreSpeed();
            CharacterManager.Instance.Player.Condition._isRun = false;
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Inventory?.Invoke();
            ToggleCursor();
        }
    }

    public void RestoreSpeed()
    {
        _moveSpeed = _originMoveSpeed;
    }

    bool HasStemina()
    {
        return CharacterManager.Instance.Player.Condition._uiCondition._stamina._curValue > _runStaminaLimit;
    }

    bool IsGrounded()
    {
        // TODO : 점프 상태 = 카운트 1 이면 아래 검사를 무시 (true 처리)
        if (_isDoubleJump && _jumpCount == 1) return true;

        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; ++i)
        {
            if (Physics.Raycast(rays[i], 0.1f, _groundLayerMask))
            {
                _jumpCount = 0;
                return true;
            }
        }

        return false;
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        _canLook = !toggle;
    }
}
