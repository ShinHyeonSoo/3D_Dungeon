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

    Condition Health { get { return _uiCondition._health; } }
    Condition Stamina { get { return _uiCondition._stamina; } }

    public event Action OnTakeDamage;

    void Update()
    {
        RecoveryStamina();

        if (Health._curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Die()
    {
        Debug.Log("ав╬З╢ы.");
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
