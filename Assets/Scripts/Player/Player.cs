using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerController _controller;
    private PlayerCondition _condition;
    private Equipment _equipment;

    private Animator _animator;

    public ItemData _itemData;
    public Action AddItem;

    public Transform _dropPosition;

    public PlayerController Controller { get { return _controller; } }
    public PlayerCondition Condition { get { return _condition; } }
    public Equipment Equipment { get { return _equipment; } }
    public Animator Animator { get { return _animator; } }

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        _controller = GetComponent<PlayerController>();
        _condition = GetComponent<PlayerCondition>();
        _equipment = GetComponent<Equipment>();

        _animator = GetComponent<Animator>();
    }
}
