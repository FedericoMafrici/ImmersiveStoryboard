using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachInteraction : MonoBehaviour
{
  
    [SerializeField] private AnimaPersonaggio _characterAnimationaManager;
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

   
}
