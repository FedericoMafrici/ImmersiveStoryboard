using UnityEngine;
using UnityEngine.UI;

public class ValueVisualizer : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    [SerializeField] private Text _text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = _slider.value.ToString();
    }
}
