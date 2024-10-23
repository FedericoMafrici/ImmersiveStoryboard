using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoundingBoxManagerUI : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private FadeMaterial enviroment;
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
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                break;
                case 3:
                    Debug.Log("è possibile gestire il piano di seduta delle bounding Box");
                    ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    break;
                case 4:
                    Debug.Log("Non è più posisbile Gestire il piano di seduta delle bounding box, termine inizializzazione scena");
                    ControllerManager.StopBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    OnSceneInizializationCompleted?.Invoke(this,EventArgs.Empty); 
                    enviroment.FadeSkybox(false);
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
