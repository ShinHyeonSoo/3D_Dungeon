using System;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition _uiCondition;
    public bool _isRun;

    private float _runStamina = 0.1f;
    private float _efficacyStat;
    private float _efficacyRate;
    private float _runRate;
    private float _lastCheckTime;

    private bool _isEfficacy;
    private bool _isInvincibility;

    private ConsumableType _consumableType;

    Condition Health { get { return _uiCondition._health; } }
    Condition Stamina { get { return _uiCondition._stamina; } }

    private void Start()
    {
        _runRate = CharacterManager.Instance.Player.Controller._runSpeedRate;
    }

    void Update()
    {
        RecoveryStamina();

        if(_isEfficacy)
            CheckEfficacy();

        if (Health._curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Drink(float amount)
    {
        Stamina.Add(amount);
    }

    public void SpeedBoost(float amount, float duration, ConsumableType type)
    {
        _efficacyStat = amount;
        CharacterManager.Instance.Player.Controller._moveSpeed *= _efficacyStat;
        _efficacyRate = duration;
        _consumableType = type;
        _lastCheckTime = Time.time;
        _isEfficacy = true;
    }

    public void DoubleJump(float duration, ConsumableType type)
    {
        CharacterManager.Instance.Player.Controller.IsDoubleJump = true;
        _efficacyRate = duration;
        _consumableType = type;
        _lastCheckTime = Time.time;
        _isEfficacy = true;
    }

    public void Invincibility(float duration, ConsumableType type)
    {
        _isInvincibility = true;
        _efficacyRate = duration;
        _consumableType = type;
        _lastCheckTime = Time.time;
        _isEfficacy = true;
    }

    private void CheckEfficacy()
    {
        if(Time.time - _lastCheckTime > _efficacyRate)
        {
            switch(_consumableType)
            {
                case ConsumableType.SpeedBoost:
                    // TODO : 아이템 먹었을 때와, 달리기 중일때 구분해서 속도 되돌리기
                    CharacterManager.Instance.Player.Controller.RestoreSpeed(_efficacyStat);
                    _efficacyStat = 0f;
                    break;
                case ConsumableType.DoubleJump:
                    CharacterManager.Instance.Player.Controller.IsDoubleJump = false;
                    break;
                case ConsumableType.Invincibility:
                    _isInvincibility = false;
                    break;
            }
            _consumableType = ConsumableType.None;
            _isEfficacy = false;
        }
    }

    public void Die()
    {
        Debug.Log("죽었다.");
    }
    
    public void RecoveryStamina()
    {
        if (!_isRun)
            Stamina.Add(Stamina._passiveValue * Time.deltaTime);
        else
            Run();
    }

    public void TakePhysicalDamage(int damage)
    {
        // TODO : 무적 상태일 때 여기서 리턴
        if (_isInvincibility) return;

        Health.Subtract(damage);
        // TODO : Hit 애니메이션 재생

    }

    public bool UseStamina(float amount)
    {
        if (Stamina._curValue - amount < 0f)
        {
            return false;
        }

        Stamina.Subtract(amount);
        return true;
    }

    public void Run()
    {
        if (Stamina._curValue <= 0f)
        {
            CharacterManager.Instance.Player.Controller.RestoreSpeed(_runRate);
            CharacterManager.Instance.Player.Animator.SetBool("Run", false);
            _isRun = false;
            return;
        }

        Stamina.Subtract(_runStamina);
    }
}
