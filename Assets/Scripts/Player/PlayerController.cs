using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float _moveSpeed;
    public float _jumpPower;
    public float _runSpeedRate = 2f;
    public LayerMask _groundLayerMask;
    private Vector2 _curMovementInput;
    private float _runStaminaLimit = 20f;
    private bool _isDoubleJump = false;
    private bool _isFlying = false;
    private int _jumpCount;
    private bool _isClimbing;

    [Header("Look")]
    public Transform _cameraContainer;
    public float _lookSensitivity;
    private Vector2 _mouseDelta;
    public bool _canLook = true;

    public event Action Inventory;
    public Action Interaction;
    private Rigidbody _rigidbody;

    [SerializeField] Vector3 TestVelocity;
    private float _lastCheckTime;
    private float _checkRate = 0.1f;

    public bool IsDoubleJump { get { return _isDoubleJump; } set { _isDoubleJump = value; } }
    public bool IsFlying { get { return _isFlying; } set {_isFlying = value; } }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        DetectWall();
    }

    private void LateUpdate()
    {
        if (_canLook)
            CameraLook();
    }

    private void FixedUpdate()
    {
        // 플랫폼 발사기 이용중엔 비활성화
        if(!_isFlying)
            Move();
    }

    private void DetectWall()
    {
        if (Time.time - _lastCheckTime > _checkRate)
        {
            _lastCheckTime = Time.time;
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.forward);
            //Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            //Debug.DrawLine(ray.origin, ray.direction, Color.red);

            if (Physics.Raycast(ray, out hit, 0.3f, 1 << LayerMask.NameToLayer("Wall")))
            {
                _isClimbing = true;
                _rigidbody.useGravity = false;
                CharacterManager.Instance.Player.Animator.SetBool("IsClimbing", true);
            }
            else
            {
                _isClimbing = false;
                _rigidbody.useGravity = true;
                CharacterManager.Instance.Player.Animator.SetBool("IsClimbing", false);
            }
        }
    }

    private void Move()
    {
        if (_isClimbing)
        {
            Climbing();
            return;
        }

        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= _moveSpeed;
        dir.y = _rigidbody.velocity.y;


        _rigidbody.velocity = dir;

        TestVelocity = _rigidbody.velocity;
    }

    private void Climbing()
    {
        Vector3 dir = transform.up * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= _moveSpeed * 0.5f;

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
            PlayMoveAnim(_curMovementInput);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
            PlayMoveAnim(_curMovementInput);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started/* && IsGrounded()*/)
        {
            if (IsGrounded())
            {
                // 두 번째 점프도 첫 번째 점프와 같은 높이를 뛰기 위해, velocity의 y값을 초기화
                Vector3 velocity = _rigidbody.velocity;
                velocity.y = 0;
                _rigidbody.velocity = velocity;

                _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
                CharacterManager.Instance.Player.Animator.SetBool("Jump", true);
                ++_jumpCount;
            }
            else if(_isClimbing)
            {
                Vector3 dir = transform.forward * -1f;
                _rigidbody.AddForce(dir * 10f, ForceMode.VelocityChange);
                _isClimbing = false;
                _rigidbody.useGravity = true;
                CharacterManager.Instance.Player.Animator.SetBool("IsClimbing", false);
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && HasStemina())
        {
            _moveSpeed *= _runSpeedRate;
            CharacterManager.Instance.Player.Condition._isRun = true;
            CharacterManager.Instance.Player.Animator.SetBool("Run", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // 조건분기, 아이템 먹었을때 달리기와 아닐때 달리기 속도 되돌리기 처리
            if (CharacterManager.Instance.Player.Condition._isRun)
            {
                RestoreSpeed(_runSpeedRate);
                CharacterManager.Instance.Player.Condition._isRun = false;
                CharacterManager.Instance.Player.Animator.SetBool("Run", false);
            }
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

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Interaction?.Invoke();
            Interaction = null;
        }
    }

    bool HasStemina()
    {
        return CharacterManager.Instance.Player.Condition._uiCondition._stamina._curValue > _runStaminaLimit;
    }

    bool IsGrounded()
    {
        // 점프 상태 = 카운트 1 이면 아래 검사를 무시 (true 처리)
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

    void PlayMoveAnim(Vector2 dir)
    {
        if(dir == Vector2.zero)
        {
            CharacterManager.Instance.Player.Animator.SetBool("Move", false);
        }
        else
        {
            CharacterManager.Instance.Player.Animator.SetBool("Move", true);
            CharacterManager.Instance.Player.Animator.SetFloat("MoveX", dir.x);
            CharacterManager.Instance.Player.Animator.SetFloat("MoveY", dir.y);
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        _canLook = !toggle;
    }
    public void RestoreSpeed(float rate)
    {
        _moveSpeed /= rate;
    }

    public void IncreaseSpeed(float speed)
    {
        _moveSpeed += speed;
    }

    public void DecreaseSpeed(float speed)
    {
        _moveSpeed -= speed;
    }

    public void IncreaseJumpPower(float power)
    {
        _jumpPower += power;
    }

    public void DecreaseJumpPower(float power)
    {
        _jumpPower -= power;
    }
}
