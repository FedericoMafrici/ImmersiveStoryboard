using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class BoundingBoxManagerUI : MonoBehaviour
{

    [SerializeField] private SimulationManager _simulationManager;
    
    [SerializeField] private List<GameObject> Cards;
    [SerializeField] private TextMeshProUGUI textHandMenu;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private FadeMaterial enviroment;
    [SerializeField] private TutorialSecondPart _tutorialSecondPart;
    [SerializeField] private GameObject hideButton;
    [SerializeField] private Button  _showTutorialButton;
    private int _cardCounter = 0;
    private int _currCardCounter=0;
    private GameObject currActiveCard;
    public static EventHandler<EventArgs> OnTutorialFirstPartCompleted;
    public static EventHandler<EventArgs> OnBoundingBoxPlacement;
    public static EventHandler<EventArgs> OnBoundingBoxPlacementCompleted;
    public static EventHandler<EventArgs> OnBoundingBoxAllowLabel;
    public static EventHandler<EventArgs> e;
    public static EventHandler<EventArgs> OnSceneInizializationCompleted;
    public static EventHandler<EventArgs> OnShowScriptPanel;    
    [SerializeField] public GameObject leftHandButton;
    [SerializeField] public GameObject rightHandButton;
    private bool isActive = true;
    private bool firstTimeHandlingPlane = true;
    public bool isLeftHanded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cardCounter = Cards.Count;
        _currCardCounter = 0;
        currActiveCard = Cards[_currCardCounter];
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
        textHandMenu.text = "Hide";
        _simulationManager = FindObjectOfType<SimulationManager>();
        hideButton.SetActive(false);
        BoundingBoxInteractionManager.onPlaneSpawned += HandleTutorialAfterObjectWithPlaneSpawned;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void HandleTutorialAfterObjectWithPlaneSpawned(object ender, EventArgs e)
    {
        if (firstTimeHandlingPlane)
        {
            ShowTutorial();
            MoveToNextPanel();
            firstTimeHandlingPlane = false;
        }
        else
        {
            BoundingBoxInteractionManager.onPlaneSpawned -= HandleTutorialAfterObjectWithPlaneSpawned;
            
        }
    }
    public void SetLeftHanded()
    {
        isLeftHanded = true;
    }

    public void SetRightHanded()
    {
        isLeftHanded = false;
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
            TextMeshProUGUI text;
            switch (_currCardCounter)
            {
                case 1:
                    leftHandButton.SetActive(false);
                    rightHandButton.SetActive(false);
                    break;
                case 2:
                    leftHandButton.SetActive(true);
                    rightHandButton.SetActive(true);
                    break;
                case 3:
                    leftHandButton.SetActive(false);
                    rightHandButton.SetActive(false);
                    break;
                case 4:
                    leftHandButton.SetActive(false);
                    rightHandButton.SetActive(false);
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacement?.Invoke(this,EventArgs.Empty);
                    break;
                case 5: 
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text != null && isLeftHanded)
                    {
                        text.text = text.text.Replace("Y/B", "Y");
                        text.text = text.text.Replace("X/A", "X");
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "A");
                        text.text = text.text.Replace("Y/B", "B");
                    }
                    Debug.Log("Non è più possibile spostare le bounding Box");
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                    break;
                case 6:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text != null && isLeftHanded)
                    {
                        text.text = text.text.Replace("X/A", "X");
                        text.text = text.text.Replace("Y/B", "Y");
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "A");
                        text.text = text.text.Replace("Y/B", "B");
                    }
                    Debug.Log("è possibile gestire il piano di seduta delle bounding Box");
                    ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    break;
                case 7:
                    break;
                case 10:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "X");
                        text.text=text.text.Replace("Y/B", "Y");
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "A");
                        text.text=text.text.Replace("Y/B", "B");
                    }
                  
                    break;
                case 12:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("B/X", "B");
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("B/X", "X");
                       
                    }
                    break;
                case 13:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("B/Y", "B");
                        
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("B/Y", "Y");
                       
                    }
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

            TextMeshProUGUI text;
            switch (_currCardCounter)
            {
                case 0:
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                    break;
                case 1:
                    OnBoundingBoxPlacementCompleted?.Invoke(this,EventArgs.Empty);
                    hideButton.SetActive(true);
                    leftHandButton.SetActive(false); 
                    rightHandButton.SetActive(false); 
                    break;
                case 2: 
                     leftHandButton.SetActive(true); 
                     rightHandButton.SetActive(true); 
                     break;
                 case 3: 
                     leftHandButton.SetActive(false); 
                     rightHandButton.SetActive(false); 
                     break;
                 case 4: 
                     leftHandButton.SetActive(false); 
                     rightHandButton.SetActive(false); 
                     break;
                 case 5: 
                    Debug.Log("Ora è possibile etichettare le bounding box");
                    OnBoundingBoxAllowLabel?.Invoke(this,EventArgs.Empty);
                    break;
                 case 6:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text != null && isLeftHanded)
                    {
                        text.text = text.text.Replace("Y/B", "Y");
                        text.text = text.text.Replace("X/A", "X");
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text = text.text.Replace("Y/B", "B");
                        text.text=text.text.Replace("X/A", "A");
                        
                    }
                     Debug.Log("è possibile gestire il piano di seduta delle bounding Box");
                    ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
                    enviroment.FadeSkybox(true);
                    _simulationManager.changePasstroughToggle();
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
                 case 10:
                     text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                     if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "X");
                        text.text=text.text.Replace("Y/B", "Y");
                        text.text=text.text.Replace("X/B", "X");
                    }
                     else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("X/A", "A");
                        text.text=text.text.Replace("Y/B", "B");
                        text.text=text.text.Replace("X/B", "A");
                    }
                    break;
                 case 11:
                    Debug.Log("QUI VIENE MOSTRATO IL PANNELLO  SCRIPT");
                    OnShowScriptPanel?.Invoke(this,EventArgs.Empty);
                    break;
                 case 12:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("B/X", "B");
                        
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("B/X", "X");
                       
                    }
                    break;
                case 13:
                    text = currActiveCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (text!=null &&isLeftHanded)
                    {
                        text.text=text.text.Replace("B/Y", "B");
                        
                    }
                    else if( text!=null && !isLeftHanded)
                    {
                        text.text=text.text.Replace("B/Y", "Y");
                       
                    }
                    break;
                 case 22:
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
