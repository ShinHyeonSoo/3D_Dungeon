using System;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition _uiCondition;
    public bool _isRun = false;
    private float _runStamina = 0.1f;
    private float _efficacyRate;
    private float _lastCheckTime;
    private bool _isEfficacy = false;

    Condition Health { get { return _uiCondition._health; } }
    Condition Stamina { get { return _uiCondition._stamina; } }

    public event Action OnTakeDamage;

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

    public void SpeedBoost(float amount, float duration)
    {
        CharacterManager.Instance.Player.Controller._moveSpeed *= amount;
        _efficacyRate = duration;
        _lastCheckTime = Time.time;
        _isEfficacy = true;
    }

    private void CheckEfficacy()
    {
        if(Time.time - _lastCheckTime > _efficacyRate)
        {
            CharacterManager.Instance.Player.Controller.RestoreSpeed();
            _isEfficacy = false;
        }
    }

    public void Die()
    {
        Debug.Log("�׾���.");
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
        Health.Subtract(damage);
        OnTakeDamage?.Invoke();
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
            CharacterManager.Instance.Player.Controller.RestoreSpeed();
            _isRun = false;
            return;
        }

        Stamina.Subtract(_runStamina);
    }
}
