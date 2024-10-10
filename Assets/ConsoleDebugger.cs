using TMPro;
using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
    [SerializeField] public TMP_Text _text; // Database con la lista delle azioni per ogni oggetto
   
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddText("console pronta e funzionante\n");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string txt)
    {
        _text.text="console pronta a partire e funzionante \n"; 
        _text.text += txt + "\n";
    }

    public void AddText(string txt)
    {
        _text.text += txt+"\n";
    }
}
