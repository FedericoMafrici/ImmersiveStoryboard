using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class TutorialSecondPart : MonoBehaviour
{

    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;

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

            switch (_currCardCounter)
            {
                default: 
                    break;
            }
        }
    }
}
