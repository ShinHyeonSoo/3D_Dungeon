using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float _attackRate;
    public float _attackDistance;
    public float _useStamina;
    private bool _isAttacking;

    [Header("Combat")]
    public bool _doesDealDamage;
    public int _damage;

    public override void OnAttackInput()
    {
        if (!_isAttacking)
        {
            if (CharacterManager.Instance.Player.Condition.UseStamina(_useStamina))
            {
                _isAttacking = true;
                Invoke("OnCanAttack", _attackRate);
            }
        }
    }

    private void OnCanAttack()
    {
        _isAttacking = false;
    }

    public void OnHit()
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.forward);
        //Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _attackDistance))
        {
            if (_doesDealDamage && hit.collider.TryGetComponent(out IDamageable idamageable))
            {
                idamageable.TakePhysicalDamage(_damage);
            }
        }
    }
}
