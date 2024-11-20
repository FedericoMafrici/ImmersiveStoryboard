using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSecondPart : MonoBehaviour
{

    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private Button ShowButton;
    private int _cardCounter = 0;
    private int _currCardCounter = 0;
    private GameObject currActiveCard;

    private bool isActive=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    void Awake()
    {
        _cardCounter = Cards.Count;
        _currCardCounter = 0;
        currActiveCard = Cards[_currCardCounter];
        BoundingBoxManagerUI.OnSceneInizializationCompleted += ActivateTutorial;
        this.gameObject.SetActive(false);
    }

    public void ActivateTutorial(object sender,EventArgs e)
    {
        this.gameObject.SetActive(true);
        isActive = true;
        currActiveCard.SetActive(true);
        //ShowButton.onClick.RemoveAllListeners();
        //ShowButton.onClick.AddListener(HandleTutorial);
        //ShowTutorial();
    }
    // Update is called once per frame
    public void HideTutorial()
    {
        _tutorial.SetActive(false);
        textHandMenu.text = "Show";
    }
    public void ShowTutorial()
    {
        _tutorial.SetActive(true);
    }
    
    public void HandleTutorial()
    {
        if (textHandMenu.text == "Show")
        {
            textHandMenu.text = "Hide";
            ShowTutorial();
        }
        else
        {
            textHandMenu.text = "Show";
            HideTutorial();
            
        }
    }

    public void MoveToPreviousPanel()
    {
        if (_currCardCounter > 0)
        {
            currActiveCard.SetActive(false);
            _currCardCounter--;
                currActiveCard = Cards[_currCardCounter];
                currActiveCard.SetActive(true);
        }
        
    }
    public void MoveToNextPanel()
    {
        if (_currCardCounter < _cardCounter)
        {
            currActiveCard.SetActive(false);
            _currCardCounter++;
            if (_currCardCounter < _cardCounter)
            {
                currActiveCard = Cards[_currCardCounter];
                currActiveCard.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }

            switch (_currCardCounter)
            {
                case 2:
                    Debug.Log("ora gli oggetti possono spawnare");
                    ControllerManager.OnObjectsSpawnable?.Invoke(this,EventArgs.Empty);
                    break;
            }
        }
    }
}
