using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractionType
{
    Door,
    Lever,
    Box,
    Button
}

[CreateAssetMenu(fileName = "Interactable", menuName = "Create Data/New Object", order = 1)]
public class InteractableData : ScriptableObject
{
    [Header("Info")]
    public string _displayName;
    public string _description;
    public InteractionType _type;
    public bool _isInteract;
}
