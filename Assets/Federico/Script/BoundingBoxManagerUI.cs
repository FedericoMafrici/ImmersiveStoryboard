using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoundingBoxManagerUI : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;
    private int _cardCounter = 0;
    private int _currCardCounter=0;
    private GameObject currActiveCard;
    
    public static EventHandler<EventArgs> OnBoundingBoxPlacementCompleted;

    public static EventHandler<EventArgs> OnSceneInizializationCompleted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cardCounter = Cards.Count;
        _currCardCounter = 0;
        currActiveCard = Cards[_currCardCounter];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                case 2:
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                break;
                case 3:
                    Debug.Log("Start Plane Edit chiamata");
                    ControllerManager.OnBoundingBoxPlaneEdit.Invoke(this,EventArgs.Empty);
                    break;
                case 4:
                    Debug.Log("Ho chiamato l'evento di cambio stato simulazione");
                    OnSceneInizializationCompleted.Invoke(this,EventArgs.Empty); 
                    currActiveCard.SetActive(false);
                    _currCardCounter = 0;
                    _tutorial.SetActive(false);
                    break;
                default: 
                    break;
            }
        }
    }
}
