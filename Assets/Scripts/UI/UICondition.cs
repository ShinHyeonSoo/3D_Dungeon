using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition _health;
    public Condition _stamina;

    void Start()
    {
        CharacterManager.Instance.Player.Condition._uiCondition = this;
    }
}
