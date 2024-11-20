using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttachInteraction : MonoBehaviour
{
  
    [SerializeField] private AnimaPersonaggio _characterAnimationaManager;
    [SerializeField] private CenterUIPanel centerUIPanel;
    [SerializeField] private GameObject UI;

    private bool hiddenUI = false;
    // Start is called before the first frame update
    void Start()
    {
        _characterAnimationaManager = GameObject.Find("CharacterAnimationManager").GetComponent<AnimaPersonaggio>();
        UI = GameObject.Find("UI");
        if (UI == null)
        {
            Debug.LogError("errore nell'ottenimento della risorsa UI attachinteraction menu hand setup");
        }
    }

   public void onValueChanged(string value)
    {
        Text actionText = this.GetComponentInChildren<Text>();
        if (actionText == null)
        {
            Debug.LogError("errore componente text della ui non trovato");
        }
        _characterAnimationaManager.ActionClick(actionText.text);
    }

    public void HandleUi(TextMeshProUGUI txt)
    {
        Debug.Log("oggetto UI acquisito"+UI.name);
        if (hiddenUI)
        {
            txt.text = "hide";
            centerUIPanel.ShowPanels();
            Debug.Log("UI stato attuale (dopo Hide): " + UI.activeSelf);
            Debug.Log("visualizzo la ui");
            hiddenUI = false;
        }
        else
        {
            txt.text = "show";
            centerUIPanel.HidePanels();
            Debug.Log("UI stato attuale (dopo show): " + UI.activeSelf);
            Debug.Log("nascondo la ui");
            hiddenUI = true;
        }
    }
    public void HideUi()
    {
        UI.SetActive(false);
        hiddenUI = true;
    }

    public void ShowUI()
    {
     UI.SetActive(true);
     hiddenUI = false;
     
    }
    

}
