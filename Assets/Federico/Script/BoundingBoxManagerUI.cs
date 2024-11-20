using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class BoundingBoxManagerUI : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private FadeMaterial enviroment;
    [SerializeField] private TutorialSecondPart _tutorialSecondPart;
    [SerializeField] private Button  _showTutorialButton;
    private int _cardCounter = 0;
    private int _currCardCounter=0;
    private GameObject currActiveCard;
    public static EventHandler<EventArgs> OnTutorialFirstPartCompleted;
    public static EventHandler<EventArgs> OnBoundingBoxPlacement;
    public static EventHandler<EventArgs> OnBoundingBoxPlacementCompleted;
    public static EventHandler<EventArgs> e;
    public static EventHandler<EventArgs> OnSceneInizializationCompleted;
   
    [SerializeField] public GameObject leftHandButton;
    [SerializeField] public GameObject rightHandButton;
    private bool isActive = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cardCounter = Cards.Count;
        _currCardCounter = 0;
        currActiveCard = Cards[_currCardCounter];
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
        textHandMenu.text = "Hide";
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
        textHandMenu.text = "Hide";
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
            
            switch (_currCardCounter)
            {
                case 2:
                    leftHandButton.SetActive(false);
                    rightHandButton.SetActive(false);
                    break;
                case 3:
                    leftHandButton.SetActive(true);
                    rightHandButton.SetActive(true);
                    break;
                case 4:
                    leftHandButton.SetActive(false);
                    rightHandButton.SetActive(false);
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacement?.Invoke(this,EventArgs.Empty);
                    break;
                case 5:
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                    break;
                case 6:
                    Debug.Log("è possibile gestire il piano di seduta delle bounding Box");
                    ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    break;
                case 7:
                    break;
                default: 
                    break;
            }
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
                case 1:
                    leftHandButton.SetActive(false); 
                    rightHandButton.SetActive(false); 
                    break;
                case 2: 
                     leftHandButton.SetActive(false); 
                     rightHandButton.SetActive(false); 
                     break;
                 case 3: 
                     leftHandButton.SetActive(true); 
                     rightHandButton.SetActive(true); 
                     break;
                 case 4: 
                     leftHandButton.SetActive(false); 
                     rightHandButton.SetActive(false); 
                     break;
                 case 5:
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                     break;
                 case 6:
                    Debug.Log("è possibile gestire il piano di seduta delle bounding Box");
                    ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    enviroment.FadeSkybox(true);
                    break;
                 case 7:
                    Debug.Log("Non è più posisbile Gestire il piano di seduta delle bounding box, termine inizializzazione scena");
                    ControllerManager.StopBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    break;
                 case 9:
                     OnSceneInizializationCompleted?.Invoke(this,EventArgs.Empty); 
                     ControllerManager.OnObjectsSpawnable?.Invoke(this,EventArgs.Empty);
                     ControllerManager.OnPanelsSpawned?.Invoke(this,EventArgs.Empty);
                     break;
                 case 16:
                     _currCardCounter = 0;
                     currActiveCard.SetActive(false);
                     currActiveCard = Cards[_currCardCounter];
                     currActiveCard.SetActive(true);
                     HideTutorial();
                     break;
                default: 
                    break;
            }
        }
    }
}
