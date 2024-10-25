using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData _data;

    public string GetInteractPrompt()
    {
        string str = $"{_data._displayName}\n{_data._description}";
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player._itemData = _data;
        CharacterManager.Instance.Player.AddItem?.Invoke();
        Destroy(gameObject);
    }
}
