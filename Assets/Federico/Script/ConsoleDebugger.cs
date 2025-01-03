using TMPro;
using UnityEngine;

public class ConsoleDebugger : MonoBehaviour
{
    [SerializeField] public TMP_Text _text; // Database con la lista delle azioni per ogni oggetto
   
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_text !=null)
        {
            _text.text = "";
           // AddText("console pronta e funzionante\n");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearText()
    {
        _text.text = "";
    }
    public void SetText(string txt)
    {
        if (_text != null)
        {
            _text.text += "\n" + txt + "\n";
        }
        else
        {
            Debug.LogError("Testo del console debugger non inizalizzato");
        }
    }

    public void AddText(string txt)
    {
        _text.text += txt+"\n";
    }
}
