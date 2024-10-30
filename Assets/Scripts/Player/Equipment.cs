using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip[] _curEquips;
    public Transform[] _equipParents;

    private PlayerController _controller;
    private PlayerCondition _condition;

    private float[] _equipStats;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<PlayerController>();
        _condition = GetComponent<PlayerCondition>();
        _curEquips = new Equip[(int)EquipType.None];
        _equipStats = new float[(int)EquipType.None];
    }

    public void EquipNew(ItemData data)
    {
        UnEquip(data._equipType);
        _curEquips[(int)data._equipType] = Instantiate(data._equipPrefab, _equipParents[(int)data._equipType]).GetComponent<Equip>();
        _equipStats[(int)data._equipType] = data._stat;

        if (data._equipType == EquipType.Bag)
            CharacterManager.Instance.Player.Controller.IncreaseSpeed(_equipStats[(int)data._equipType]);
        else if (data._equipType == EquipType.Belt)
            CharacterManager.Instance.Player.Controller.IncreaseJumpPower(_equipStats[(int)data._equipType]);
        else if (data._equipType == EquipType.Weapon)
            CharacterManager.Instance.Player.Animator.SetBool("IsEquipWeapon", true);
    }

    public void UnEquip(EquipType type)
    {
        if (_curEquips[(int)type] != null)
        {
            if (type == EquipType.Bag)
                CharacterManager.Instance.Player.Controller.DecreaseSpeed(_equipStats[(int)type]);
            else if (type == EquipType.Belt)
                CharacterManager.Instance.Player.Controller.DecreaseJumpPower(_equipStats[(int)type]);
            else if (type == EquipType.Weapon)
                CharacterManager.Instance.Player.Animator.SetBool("IsEquipWeapon", false);

            Destroy(_curEquips[(int)type].gameObject);
            _curEquips[(int)type] = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && _curEquips[(int)EquipType.Weapon] != null && _controller._canLook)
        {
            _curEquips[(int)EquipType.Weapon].OnAttackInput();
        }
    }

    public void OnHit()
    {
        _curEquips[(int)EquipType.Weapon].GetComponent<EquipTool>().OnHit();
    }
}
