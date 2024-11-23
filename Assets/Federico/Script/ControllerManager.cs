using System;
using System.Collections;
using System.Collections.Generic;
using Trev3d.Quest.ScreenCapture;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using InputDevice = UnityEngine.InputSystem.InputDevice;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] public ButtonController controlli;
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] public SimulationManager _SimulationManager;
    [SerializeField] public ConsoleDebugger _ConsoleDebugger;
    [SerializeField] public PersistentAnchorManager persistentAnchorManager;
    [SerializeField] public XRRayInteractor leftControllerRay;
    [SerializeField] public XRRayInteractor rightControllerRay;
    [SerializeField] public XRRayInteractor activeControllerRay;
    [SerializeField] public ARAnchorManager anchorManager;
    [SerializeField] public ScreenshotManager screenshotManager;
    [SerializeField] public CenterUIPanel centerUIPanel;
    public GameObject leftXRController;
    public GameObject rightXRController;
    [SerializeField] public RawImage _screenshot;
    [SerializeField] public Camera _mainCamera;
    // andrà lanciato un evento per aggiuntere gli oggetti o l'oggetto alla lista di eventi da ascoltare 
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor firstInteractor; // Il primo interattore (controller o mano)
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor secondInteractor; // Il secondo interattore (controller o mano)
    private float initialDistance; // Distanza iniziale tra i due controller/mano
    private Vector3 initialScale; // Scala iniziale dell'oggetto
    private Vector3 currentScale; // Scala corrente durante la manipolazione
    private bool possibleInteraction = false;
    public GameObject currSelectedObject;
    
    public static EventHandler<EventArgs> OnObjectPlaced;
    public static EventHandler<EventArgs> OnObjectsSpawnable; //da invocare
    public static EventHandler<EventArgs> OnBoundingBoxPlaneEdit;
    public static EventHandler<EventArgs> StopBoundingBoxPlaneEdit;
    public static EventHandler<EventArgs> OnPanelsSpawned;
    public static EventHandler<EventArgs> OnBoundingBoxFounded;
    // bool per gestire lo stato del controller manager
    public bool objectCanSpawn = false;
    public bool canRotatePlane = true;

    public bool allowPanelsRecentering = false;
    // manipolazione piano interno alla bounding box 
    public bool isMovingPlane=false;
    private float initialControllerY;
    public Vector3 initialPlaneLocalPosition;
    public Vector3 boundingBoxSize;
    public Transform planeTransform;
    public int offset = 0;
    [SerializeField] public GameObject woman;
    private BoundingBoxInteractionManager boundingBoxSelected;

    public bool debuggingTool = false;
    public bool isFirstTimeSettingUpController = true;
    
    //eventi
    private void OnDisable()
    {
        if (controlli != null)
        {
            controlli.Left.Disable();
            controlli.Right.Disable();
            controlli.Keyboard.Disable();
            
        }
        
        OnObjectsSpawnable -= AllowObjectSpawn;
        OnBoundingBoxPlaneEdit -= AllowEditPlane;
        StopBoundingBoxPlaneEdit -= DenyEditPlane;
        OnBoundingBoxFounded -= AllowRecenteringOfPanels;
    }

    private void OnDestroy()
    {
        if (controlli != null)
        {
            controlli.Left.Disable();
            controlli.Right.Disable();
            controlli.Keyboard.Disable();
        }
        OnObjectsSpawnable -= AllowObjectSpawn;
        OnBoundingBoxPlaneEdit -= AllowEditPlane;
        StopBoundingBoxPlaneEdit -= DenyEditPlane;
        OnBoundingBoxFounded -= AllowRecenteringOfPanels;
    }
    void Start()
    {
        DeactivateLaser();
        OnObjectsSpawnable += AllowObjectSpawn;
        OnBoundingBoxPlaneEdit += AllowEditPlane;
        StopBoundingBoxPlaneEdit += DenyEditPlane;
        OnPanelsSpawned += AllowRecenteringOfPanels;
       
        SetUpForLeftHanded();
        if (debuggingTool)
        {
            OnObjectsSpawnable?.Invoke(this,EventArgs.Empty);
            OnBoundingBoxFounded?.Invoke(this,EventArgs.Empty);
        }
    }

   

    public void SetUpForLeftHanded()
    {
        if (isFirstTimeSettingUpController)
        {
            Debug.Log("Configuro i comandi per i mancini");
            controlli = new ButtonController();
            controlli.Left.Enable();
            controlli.Right.Enable();
            controlli.Left.Y.performed += ctx => Ypressed(ctx);
            controlli.Left.X.performed += ctx => X(ctx);
            controlli.Right.A.performed += ctx => Apressed(ctx);
            controlli.Right.B.performed += ctx => Bpressed(ctx);

            controlli.Left.Grip.performed += ctx => OnGripHold(ctx);
            controlli.Left.Grip.canceled += ctx => OnGripRelease(ctx);

            controlli.Keyboard.Enable();
            controlli.Keyboard.Keyboard.performed += ctx => KeyboardPressed(ctx);

            controlli.Left.Analog.performed += ctx => AnalogTouched(ctx);
            activeControllerRay = leftControllerRay;
            DeactivateLaser();
            isFirstTimeSettingUpController = false;
        }
        else
        {
            Debug.Log("controller gia configurato");
            return;
        }
    }

    public void SetUpForRightHanded()
    {
        if (isFirstTimeSettingUpController)
        {
            Debug.Log("Configuro i comandi per destri");
            controlli = new ButtonController();
            controlli.Left.Enable();
            controlli.Right.Enable();
            controlli.Left.Y.performed += ctx => Bpressed(ctx);
            //   controlli.Left.Y.canceled += ctx => Yreleased(ctx);
            controlli.Left.X.performed += ctx => Apressed(ctx);
            controlli.Right.A.performed += ctx => Ypressed(ctx);
            controlli.Right.B.performed += ctx => X(ctx);

            controlli.Right.Grip.performed += ctx => OnGripHold(ctx);
            controlli.Right.Grip.canceled += ctx => OnGripRelease(ctx);

            controlli.Keyboard.Enable();
            controlli.Keyboard.Keyboard.performed += ctx => KeyboardPressed(ctx);

            controlli.Right.Analog.performed += ctx => AnalogTouched(ctx);
            activeControllerRay = rightControllerRay;
            DeactivateLaser();
            isFirstTimeSettingUpController = false;
        }
        else
        {
            Debug.Log("controller gia configurato");
        }
    }
    private void AllowEditPlane(object sender,EventArgs e)
    {
        Debug.Log("ora puoi ruotare il piano");
        canRotatePlane = true;
    }

    private void DenyEditPlane(object sender,EventArgs e)
    {
        canRotatePlane = false;
    }
    private void AllowObjectSpawn(object sender, EventArgs e)
    {
        objectCanSpawn = true;
    }
    
    private void DenyObjectSpawn(object sender, EventArgs e)
    {
        objectCanSpawn = false;
    }
    private void AllowRecenteringOfPanels(object sender, EventArgs e)
    {
        allowPanelsRecentering = true;
    }
    
    public void KeyboardPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("A from keyboard pressed");
        
//        woman.transform.rotation = Quaternion.Euler(new Vector3(-0, 90+offset, 0));
        offset += 90;
        
    }

    private bool analogIsTilted = false; // Variabile di stato per monitorare l'inclinazione

    public void AnalogTouched(InputAction.CallbackContext ctx)
    {
        Vector2 analogValue = ctx.action.ReadValue<Vector2>();
        
        Debug.Log("Analog Rilevato");
      //  _ConsoleDebugger.SetText("hai premuto l'analog, valori rilevati: " + analogValue.x + " " + analogValue.y);
        // Controlla se l'analogico è inclinato a destra e l'azione non è stata ancora eseguita
        if (analogValue.x > 0.5f && !analogIsTilted)
        {
            _ConsoleDebugger.SetText("hai premuto l'analog a destra, valori rilevati: " + analogValue.x + " " + analogValue.y);

            Debug.Log("Hai premuto l'analog verso destra bravo!");
            if (boundingBoxSelected != null)
            {
                Debug.Log("hai selezionato una bounding box ?" + boundingBoxSelected.gameObject.name);
                _ConsoleDebugger.SetText(_ConsoleDebugger.gameObject.name);
            }

            if (boundingBoxSelected != null && boundingBoxSelected.planeRotation)
            {
                boundingBoxSelected.RotatePlane(1);
            }
            analogIsTilted = true; // Segna l'analogico come inclinato
        }
        // Controlla se l'analogico è inclinato a sinistra e l'azione non è stata ancora eseguita
        else if (analogValue.x < -0.5f && !analogIsTilted)
        {
            Debug.Log("Hai premuto l'analog verso sinistra bravissimo!");
            _ConsoleDebugger.SetText("hai premuto l'analog a sinistra , valori rilevati: " + analogValue.x + " " + analogValue.y);
            _ConsoleDebugger.SetText(_ConsoleDebugger.gameObject.name);
            if (boundingBoxSelected != null && boundingBoxSelected.planeRotation)
            {
                boundingBoxSelected.RotatePlane(-1);
            }
            analogIsTilted = true; // Segna l'analogico come inclinato
        }
        // Resetta lo stato se l'analogico è tornato al centro
        else if (Mathf.Abs(analogValue.x) < 0.5f)
        {
            analogIsTilted = false;
        }
    }
    
    public void Ypressed(InputAction.CallbackContext ctx)
    {

        if (possibleInteraction)
        {
            DeactivateLaser();
        }
        else
        {
            ActivateLaser();
        }
    }
     void X(InputAction.CallbackContext ctx)
                      {
                        
                          if(leftControllerRay == null)
                          {
                              Debug.LogError("controller sinistro non trovato");
                          }
                          if(activeControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo) && _SimulationManager.status == 0 )
                          {
                              if (hitInfo.transform.gameObject.layer==9)
                              {
                                 var anchorManager= hitInfo.transform.gameObject.GetComponent<CharacterAnchorManager>();
                                 if (anchorManager != null)
                                 {
                                     anchorManager.DetachFromAnchor();
                                 }
                                 else
                                 {
                                     _ConsoleDebugger.SetText("Componente anchor manager non reperito oggetto:"+hitInfo.transform.gameObject.name);
                                 }
                                    if(hitInfo.transform.gameObject.CompareTag("Player"))
                                    {
                                        var interaction=hitInfo.transform.gameObject.GetComponentInChildren<InteractionManagerAddOn>();
                                        if (interaction != null)
                                        {
                                            interaction.DestroyObject();
                                        }
                                        else
                                        {
                                            Debug.LogError("non ho trovato il componente interactionManager dal personaggio");
                                        }
                                    }
                                    else
                                    {
                                        var interaction = hitInfo.transform.gameObject.GetComponent<InteractionManagerAddOn>();
                                        if (interaction != null)
                                        {
                                            interaction.DestroyObject();
                                        }
                                        else
                                        {
                                            Debug.LogError("non ho trovato il componente interactionManager dal personaggio");
                                        }
                                    }
                              }
                              else
                              {
                               if(objectCanSpawn && objectToSpawn!=null && !hitInfo.transform.CompareTag("BoundingBox") && !hitInfo.transform.CompareTag("Object"))
                               {   SpawnObject(); }
                               else if(canRotatePlane || hitInfo.transform.CompareTag("Object") || hitInfo.transform.CompareTag("BoundingBox"))
                               {
                                   boundingBoxSelected=hitInfo.transform.parent.gameObject.GetComponent<BoundingBoxInteractionManager>();
                                   if (boundingBoxSelected != null)
                                   {
                                       OnBoundingBoxFounded?.Invoke(this,EventArgs.Empty);
                                       boundingBoxSelected.EnablePlaneRotation();
                                       _ConsoleDebugger.SetText("Piano colpito con successo può essere ruotato");
                                   }
                               }
                               
                              }
                          }
                          else if (_SimulationManager.status == 1 )
                              {
                                  var activeChar = _SimulationManager.activeCharacter.GetComponent<CharacterManager>();
                                  if (activeChar.walkMode)
                                  {
                                      activeChar.Move(hitInfo);
                                  }
                              }
                          
                  
                          return;
                      }

     public void Apressed(InputAction.CallbackContext ctx) 
     {
         if (_SimulationManager.status == 1)
         {
             screenshotManager.TakeScreenshot();
         }
     }
     public void Bpressed(InputAction.CallbackContext ctx)
     {
         allowPanelsRecentering = true;
         if (allowPanelsRecentering)
         {
             Debug.Log("Ho ricentrato i pannelli");
             _ConsoleDebugger.SetText("Ho ricentrato i pannelli");
             centerUIPanel.CenterPanels();
         }
         else
         {
             Debug.Log("La variabile è false quindi non puoi ricentrare i panenlli");
         }
         
     }
     public async void SpawnAnchor() 
     {
         rightControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hit);
         Pose hitPose = new Pose(hit.point, Quaternion.LookRotation(-hit.normal));
         var result = await anchorManager.TryAddAnchorAsync(hitPose);
         if (result.status.IsSuccess()) {
             var anchor = result.value;
             _ConsoleDebugger.SetText("Ancora creata con suggesso posizione" + anchor.transform.position);
         }
     }
     private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();  // Aspetta la fine del frame per assicurarsi che il rendering sia completato

        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        _mainCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);

        _mainCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        _mainCamera.targetTexture = null;
        RenderTexture.active = null;  // Rilascia la texture attiva
        Destroy(rt);

        // Visualizza lo screenshot sulla UI
        _screenshot.texture = screenShot;
        _screenshot.enabled = true;  // Abilita la visualizzazione
        
    }
    private void Update()
    {
        // Se il piano sta muovendosi, aggiorna la posizione
        if (isMovingPlane && planeTransform!=null)
        {
            MovePlaneWithController(initialPlaneLocalPosition,boundingBoxSize);
        }
    }
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Se non c'è un primo interattore, lo assegniamo
        if (firstInteractor == null)
        {
            currSelectedObject = args.interactableObject.transform.gameObject;
            firstInteractor = args.interactorObject;
            Debug.Log("First interactor assigned.");
        }
        // Se c'è già un primo interattore, assegniamo il secondo interattore
        else if (secondInteractor == null)
        {
            secondInteractor = args.interactorObject;
            Debug.Log("Second interactor assigned.");
            
            // Quando viene afferrato da due mani/controller, salviamo la distanza iniziale e la scala iniziale
            initialDistance = Vector3.Distance(firstInteractor.transform.position, secondInteractor.transform.position);
            initialScale = currSelectedObject.transform.localScale;
            Debug.Log($"Initial Distance: {initialDistance}, Initial Scale: {initialScale}");
        }
    }
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Se il secondo interattore rilascia, lo resettiamo
        if (args.interactorObject == secondInteractor)
        {
            secondInteractor = null;
        }
        // Se il primo interattore rilascia, spostiamo il secondo al primo posto
        else if (args.interactorObject == firstInteractor)
        {
            firstInteractor = secondInteractor;
            secondInteractor = null;
        }
        // Se uno dei due interattori rilascia, aggiorniamo la scala attuale
        currSelectedObject.transform.localScale = currentScale;
        initialScale = currSelectedObject.transform.localScale;
        // Dopo il rilascio di entrambi i controller, imposta la scala corrente come nuova scala iniziale
        if (firstInteractor == null && secondInteractor == null)
        {
            currentScale = initialScale;
            currSelectedObject = null;
        }
    }
    private void StartMovingPlane(Vector3 planePosition)
    {
        isMovingPlane = true;
        initialControllerY = activeControllerRay.transform.position.y; // Memorizza la Y iniziale del controller
        initialPlaneLocalPosition = planePosition;
    }
    
    private void OnGripHold(InputAction.CallbackContext context)
    {
        // Controlla se il ray interactor sta selezionando il piano
        if (activeControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            
            if (hit.transform.name=="Plane")
            {
                planeTransform = hit.transform;
                initialPlaneLocalPosition = planeTransform.localPosition;
                boundingBoxSize = hit.transform.GetComponent<BoundingBoxReference>().GetBoundingBoxSize();
                if (!isMovingPlane)
                {
                    StartMovingPlane(hit.transform.localPosition);
                }
                boundingBoxSelected=hit.transform.parent.gameObject.GetComponent<BoundingBoxInteractionManager>();
                if (boundingBoxSelected != null)
                {
                    OnBoundingBoxFounded?.Invoke(this,EventArgs.Empty);
                    boundingBoxSelected.EnablePlaneRotation();
                    _ConsoleDebugger.SetText("Piano colpito con successo può essere ruotato");
                }
            }
        }
    }
    
    private void OnGripRelease(InputAction.CallbackContext context)
    {
        isMovingPlane = false;
    }



    public void changeLaserState()
    {
       
        possibleInteraction = true;

        // Alterna lo stato del raggio e del LineRenderer tra abilitato e disabilitato
        leftControllerRay.enabled = !leftControllerRay.enabled;
        rightControllerRay.enabled = !rightControllerRay.enabled;

        var lr = leftXRController.GetComponent<LineRenderer>();
        lr.enabled = !lr.enabled;

        lr = rightXRController.GetComponent<LineRenderer>();
        lr.enabled = !lr.enabled;
    }
    public void ActivateLaser()
    {
        possibleInteraction = true;

        // Alterna lo stato del raggio e del LineRenderer tra abilitato e disabilitato
        leftControllerRay.enabled = true;
        rightControllerRay.enabled = true;
        /*
        var lr = leftXRController.GetComponent<LineRenderer>();
        lr.enabled = true;

        lr = rightXRController.GetComponent<LineRenderer>();
        lr.enabled = true;
        */
        GameObject rint = rightXRController.transform.Find("Near-Far Interactor").gameObject;
        GameObject lint = leftXRController.transform.Find("Near-Far Interactor").gameObject;
        rint.SetActive(false); 
        lint.SetActive(false);

    }

    public void DeactivateLaser()
    {
        Debug.Log("Y button pressed");
        possibleInteraction = false;

        // Alterna lo stato del raggio e del LineRenderer tra abilitato e disabilitato
        leftControllerRay.enabled = false;
        rightControllerRay.enabled = false;
        /*
        var lr = leftXRController.GetComponent<LineRenderer>();
        lr.enabled = false;

        lr = rightXRController.GetComponent<LineRenderer>();
        lr.enabled = false;
        */
        GameObject rint = rightXRController.transform.Find("Near-Far Interactor").gameObject;
        GameObject lint = leftXRController.transform.Find("Near-Far Interactor").gameObject;
       
        rint.SetActive(true); 
        lint.SetActive(true);
    }
    
    

    public void Yreleased(InputAction.CallbackContext ctx)
    {
        Debug.Log("Y button released");
        
    }

    public void SetObjectToSpawn(GameObject prefab)
    {
        objectToSpawn = prefab;
    }

    

 // function to spawn objects
 public async void SpawnObject()
 {
     if (objectToSpawn == null)
     {
         Debug.LogError("l'oggetto è nullo");
         return;
     }
     
     // Ottieni il punto di impatto e il GameObject colpito, se presente
     if (activeControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
     {
         Vector3 hitPosition = hitInfo.point; // Coordinate del punto di impatto
         Vector3 hitNormal = hitInfo.normal; // Normale della superficie colpita
         // aggiusto la rotazione con quella del prefab 


         GameObject hitObject = hitInfo.collider.gameObject; // GameObject colpito

         Debug.Log($"Il raggio del controller sinistro ha colpito: {hitObject.name} alle coordinate {hitPosition}");
      
       // persistentAnchorManager.CreateAnchor(hitPosition,objectToSpawn);
        
          // Instanzia l'oggetto da spawnare
         
              GameObject spawnedObject = Instantiate(objectToSpawn);
              spawnedObject.name = objectToSpawn.name;
              // Imposta la posizione dell'oggetto
              spawnedObject.transform.position = hitPosition;

              // Allinea l'oggetto alla superficie del piano utilizzando la normale della superficie
              Quaternion surfaceRotation = Quaternion.LookRotation(hitNormal);

              // Ottieni la direzione verso l'utente (ad esempio, la camera o il controller)
              Vector3 directionToUser = (activeControllerRay.transform.position - hitPosition).normalized;

              // Ignora la componente verticale della direzione (asse Y) per calcolare solo la direzione orizzontale
              directionToUser.y = 0; // Ignora l'altezza (asse Y) per evitare inclinazioni verso l'alto o il basso

              // Calcola la rotazione in modo che l'oggetto guardi verso l'utente solo sull'asse XZ
              Quaternion lookAtUserRotation = Quaternion.LookRotation(directionToUser, hitNormal);

              // Applica la rotazione combinata all'oggetto spawnato
              spawnedObject.transform.rotation = lookAtUserRotation;

              // Aggiungi l'oggetto alla lista degli oggetti spawnati
              _SimulationManager.spawnedGameObjects.Add(spawnedObject);

              OnObjectPlaced.Invoke(this, EventArgs.Empty);
          
          
     }
     else
      {
          Debug.Log("Il raggio del controller sinistro non ha colpito nulla.");
      }
 }

 public void MoveCharacter(object sender, EventArgs obj)
 {
     if (activeControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
     {
         // trovo il personaggio
         var activeChar = _SimulationManager.activeCharacter;
         
         
         // chiamo la funzione di move fornendogli le informazioni della collisione 
         activeChar.GetComponent<CharacterManager>().Move(hitInfo);
     }
     else
     {
         
     }
 }
 private void MovePlaneWithController(Vector3 initialPlaneLocalPosition, Vector3 boundingBoxSize)
 {
     Debug.Log("Entering MovePlaneWithController");
     Debug.Log($"initialPlaneLocalPosition: {initialPlaneLocalPosition}, boundingBoxSize: {boundingBoxSize}");

     float currentControllerY = activeControllerRay.transform.position.y; // Ottieni la Y attuale del controller
     float deltaY = currentControllerY - initialControllerY; // Differenza rispetto alla Y iniziale
     Debug.Log(activeControllerRay.transform.position);
     // Calcola la nuova posizione del piano
     Vector3 newLocalPosition = initialPlaneLocalPosition + new Vector3(0, deltaY, 0);

     // Calcola i limiti della bounding box lungo l'asse Y
     float minY = initialPlaneLocalPosition.y - boundingBoxSize.y / 2;
     float maxY = initialPlaneLocalPosition.y + boundingBoxSize.y / 2;

     // Vincola la posizione del piano all'interno della bounding box
     newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, minY, maxY);
    
     // Aggiorna la posizione del piano solo sull'asse Y
     planeTransform.localPosition = new Vector3(initialPlaneLocalPosition.x, newLocalPosition.y, initialPlaneLocalPosition.z);
 }
 

    public void KillAllObjects()
    {
        foreach (var obj in _SimulationManager.spawnedGameObjects)
        {
            GameObject.Destroy(obj);
        }
        _SimulationManager.spawnedGameObjects.Clear();
    }
}
    

