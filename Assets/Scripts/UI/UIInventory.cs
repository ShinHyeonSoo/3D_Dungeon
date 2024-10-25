using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TextType
{
    ItemName,
    ItemDescription,
    StatName,
    StatValue,
}

public enum BtnType
{
    Use,
    Equip,
    UnEquip,
    Drop,
}

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] _slots;
    public Transform _slotPanel;
    private Transform _dropPosition;
    private GameObject _inventoryWindow;

    [Header("Select Item")]
    public GameObject _selectedTextBundle;
    public GameObject _buttonBundle;
    private TextMeshProUGUI[] _selectedTexts;
    private GameObject[] _buttons;

    private PlayerController _controller;
    private PlayerCondition _condition;

    private ItemData _selectedItem;
    private int _selectedItemIdx;

    int _curEquipIdx;

    private void Awake()
    {
        _inventoryWindow = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        _selectedTexts = _selectedTextBundle.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        _buttons = _buttonBundle.gameObject.GetComponentsInChildren<Button>().Select(t => t.gameObject).ToArray();
        // Select(t => t.gameObject).ToArray() Button ������Ʈ�� gameObject�� ������ �ش� ������Ʈ�� ���� ���� ������Ʈ�� ������ �迭�� ��ȯ


        _controller = CharacterManager.Instance.Player.Controller;
        _condition = CharacterManager.Instance.Player.Condition;
        _dropPosition = CharacterManager.Instance.Player._dropPosition;

        _controller.Inventory += Toggle;
        CharacterManager.Instance.Player.AddItem += AddItem;

        _inventoryWindow.SetActive(false);
        _slots = new ItemSlot[_slotPanel.childCount];

        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i] = _slotPanel.GetChild(i).GetComponent<ItemSlot>();
            _slots[i]._index = i;
            _slots[i]._inventory = this;
        }

        ClearSelectedItemWindow();
        UpdateUI();
    }

    private void ClearSelectedItemWindow()
    {
        _selectedTexts[(int)TextType.ItemName].text = string.Empty;
        _selectedTexts[(int)TextType.ItemDescription].text = string.Empty;
        _selectedTexts[(int)TextType.StatName].text = string.Empty;
        _selectedTexts[(int)TextType.StatValue].text = string.Empty;

        _buttons[(int)BtnType.Use].SetActive(false);
        _buttons[(int)BtnType.Equip].SetActive(false);
        _buttons[(int)BtnType.UnEquip].SetActive(false);
        _buttons[(int)BtnType.Drop].SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            _inventoryWindow.SetActive(false);
        }
        else
        {
            _inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return _inventoryWindow.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player._itemData;

        // �������� �ߺ� �������� canStack
        if (data._canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot._quantity++;
                UpdateUI();
                CharacterManager.Instance.Player._itemData = null;
                return;
            }
        }

        // ����ִ� ���� �����´�
        ItemSlot emptySlot = GetEmptySlot();

        // �ִٸ�
        if (emptySlot != null)
        {
            emptySlot._item = data;
            emptySlot._quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player._itemData = null;
            return;
        }

        // ���ٸ�
        ThrowItem(data);
        CharacterManager.Instance.Player._itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i]._item != null)
                _slots[i].Setting();
            else
                _slots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i]._item == data &&
                _slots[i]._quantity < data._maxStackAmount)
            {
                return _slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i]._item == null)
            {
                return _slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data._dropPrefab, _dropPosition.position,
            Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (_slots[index]._item == null) return;

        _selectedItem = _slots[index]._item;
        _selectedItemIdx = index;

        _selectedTexts[(int)TextType.ItemName].text = _selectedItem._displayName;
        _selectedTexts[(int)TextType.ItemDescription].text = _selectedItem._description;

        _selectedTexts[(int)TextType.StatName].text = string.Empty;
        _selectedTexts[(int)TextType.StatValue].text = string.Empty;

        for (int i = 0; i < _selectedItem._consumables.Length; ++i)
        {
            _selectedTexts[(int)TextType.StatName].text += _selectedItem._consumables[i]._type.ToString() + "\n";
            _selectedTexts[(int)TextType.StatValue].text += _selectedItem._consumables[i]._value.ToString() + "\n";
        }

        _buttons[(int)BtnType.Use].SetActive(_selectedItem._type == ItemType.Consumable);
        _buttons[(int)BtnType.Equip].SetActive(_selectedItem._type == ItemType.Equipable && !_slots[index]._equipped);
        _buttons[(int)BtnType.UnEquip].SetActive(_selectedItem._type == ItemType.Equipable && _slots[index]._equipped);
        _buttons[(int)BtnType.Drop].SetActive(true);
    }

    public void OnUseButton()
    {
        if (_selectedItem._type == ItemType.Consumable)
        {
            for (int i = 0; i < _selectedItem._consumables.Length; ++i)
            {
                switch (_selectedItem._consumables[i]._type)
                {
                    case ConsumableType.Health:
                        _condition.Heal(_selectedItem._consumables[i]._value);
                        break;
                    case ConsumableType.Stamina:
                        _condition.Drink(_selectedItem._consumables[i]._value);
                        break;
                    case ConsumableType.SpeedBoost:
                        _condition.SpeedBoost(_selectedItem._consumables[i]._value, 
                            _selectedItem._consumables[i]._duration,
                            _selectedItem._consumables[i]._type);
                        break;
                    case ConsumableType.DoubleJump:
                        _condition.DoubleJump(_selectedItem._consumables[i]._duration, 
                            _selectedItem._consumables[i]._type);
                        break;
                    case ConsumableType.Invincibility:
                        // TODO : ����
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(_selectedItem);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        _slots[_selectedItemIdx]._quantity--;

        if (_slots[_selectedItemIdx]._quantity <= 0)
        {
            _selectedItem = null;
            _slots[_selectedItemIdx]._item = null;
            _selectedItemIdx = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    //public void OnEquipButton()
    //{
    //    if (_slots[_curEquipIdx]._equipped)
    //    {
    //        UnEquip(_curEquipIdx);
    //    }

    //    _slots[_selectedItemIdx]._equipped = true;
    //    _curEquipIdx = _selectedItemIdx;
    //    CharacterManager.Instance.Player._equipment.EquipNew(_selectedItem);
    //    UpdateUI();

    //    SelectItem(_selectedItemIdx);
    //}

    //public void UnEquipButton()
    //{
    //    UnEquip(_selectedItemIdx);
    //}

    //private void UnEquip(int index)
    //{
    //    _slots[index]._equipped = false;
    //    CharacterManager.Instance.Player._equipment.UnEquip();
    //    UpdateUI();

    //    if (_selectedItemIdx == index)
    //    {
    //        SelectItem(_selectedItemIdx);
    //    }
    //}
}