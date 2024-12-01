using System;
using UnityEngine;
using UnityEngine.UI;

public class ValueVisualizer : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    [SerializeField] private Text _text;
    private float _prevValue;
    public static EventHandler<ActionTimeChangedEventArgs> onActionTimeChanged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = _slider.value.ToString();
        if (_slider.value != _prevValue)
        {
            onActionTimeChanged.Invoke(this,new ActionTimeChangedEventArgs( _slider.value));
            _prevValue = _slider.value;
        }

       
    }
    
}

public class ActionTimeChangedEventArgs : EventArgs
{
    public float NewValue { get; }

    public ActionTimeChangedEventArgs(float newValue)
    {
        NewValue = newValue;
    }
}