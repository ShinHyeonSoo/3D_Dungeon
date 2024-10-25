﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public InteractableData _data;

    private void Awake()
    {
        _data._isInteract = false;
    }

    public string GetInteractPrompt()
    {
        return !_data._isInteract ? $"{_data._displayName}\n{_data._description}" : string.Empty;
    }

    public void OnInteract()
    {
        if (!_data._isInteract)
        {
            // TODO : 문, 박스, 레버, 스위치 등 애니메이션 및 사운드 재생

            _data._isInteract = true;
            Debug.Log("Interactable Object");
        }
    }
}