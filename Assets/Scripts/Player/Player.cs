using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerController _controller;
    private PlayerCondition _condition;

    public ItemData _itemData;
    public Action AddItem;

    public PlayerController Controller { get { return _controller; } }
    public PlayerCondition Condition { get { return _condition; } }

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        _controller = GetComponent<PlayerController>();
        _condition = GetComponent<PlayerCondition>();
    }
}
