using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttachInteraction : MonoBehaviour
{
  
    [SerializeField] private AnimaPersonaggio _characterAnimationaManager;

    [SerializeField] private GameObject UI;

    private bool hiddenUI = false;
    // Start is called before the first frame update
    void Start()
    {
        _characterAnimationaManager = GameObject.Find("CharacterAnimationManager").GetComponent<AnimaPersonaggio>();
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
        if (hiddenUI)
        {
            txt.text = "hide";
            ShowUI();
        }
        else
        {
            txt.text = "show";
            HideUi();
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
