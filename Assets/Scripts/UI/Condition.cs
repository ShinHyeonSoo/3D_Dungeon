using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float _curValue;
    public float _startValue;
    public float _maxValue;
    public float _passiveValue;
    public Image _uiBar;

    // Start is called before the first frame update
    void Start()
    {
        _curValue = _startValue;
    }

    // Update is called once per frame
    void Update()
    {
        _uiBar.fillAmount = GetPercentage();
    }

    private float GetPercentage()
    {
        return _curValue / _maxValue;
    }

    public void Add(float value)
    {
        _curValue = Mathf.Min(_curValue + value, _maxValue);
    }

    public void Subtract(float value)
    {
        _curValue = Mathf.Max(_curValue - value, 0);
    }
}
