using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using InputDevice = UnityEngine.InputSystem.InputDevice;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] public ButtonController controlli;
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] public SimulationManager _SimulationManager;
    [SerializeField] public ConsoleDebugger _ConsoleDebugger;
    [SerializeField] public XRRayInteractor leftControllerRay;
    [SerializeField] public XRRayInteractor rightControllerRay;
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

   
    
    private void OnEnable()
    {
        // Sottoscrivi gli eventi quando un interattore afferra l'oggetto
        // grabInteractable.selectEntered.AddListener(OnSelectEntered);
        // grabInteractable.selectExited.AddListener(OnSelectExited);
       
    }

    private void OnDisable()
    {
        // Rimuovi gli eventi quando l'oggetto non è più attivo
        // grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        // grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }
    void Start()
    {
        controlli = new ButtonController();
        controlli.Left.Enable();
        controlli.Left.Y.performed += ctx => Ypressed(ctx);
     //   controlli.Left.Y.canceled += ctx => Yreleased(ctx);
        controlli.Left.X.performed += ctx => X(ctx);
        
        // Alterna lo stato del raggio e del LineRenderer tra abilitato e disabilitato
        leftControllerRay.enabled = false;
        rightControllerRay.enabled = false;
        possibleInteraction = false;
        var lr = leftXRController.GetComponent<LineRenderer>();
        lr.enabled = false;

        lr = rightXRController.GetComponent<LineRenderer>();
        lr.enabled = false;
      
        GameObject rint = rightXRController.transform.Find("Near-Far Interactor").gameObject;
        GameObject lint = leftXRController.transform.Find("Near-Far Interactor").gameObject;
        rint.SetActive(true); 
        lint.SetActive(false);
        
        
    }
    public void Ypressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Y button pressed");

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
                        
                          if (leftControllerRay == null)
                          {
                              Debug.LogError("controller sinistro non trovato");
                          }


                          if (leftControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo) && _SimulationManager.status == 0 )
                          {
                              if (hitInfo.transform.gameObject.layer==9)
                              {
                                  Destroy(hitInfo.transform.gameObject);
                              }
                              else
                              {
                                  SpawnObject();
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

        Debug.Log("Screenshot visualizzato sulla UI: " + _screenshot.name);
        _ConsoleDebugger.AddText("Screenshot visualizzato sulla UI: " + _screenshot.name);
    }
        
      
        private void Update()
            {
             
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





    public void changeLaserState()
    {
        Debug.Log("Y button pressed");
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
        Debug.Log("Y button pressed");
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
 public void SpawnObject()
 {
     // Ottieni il punto di impatto e il GameObject colpito, se presente
     if (leftControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
     {
         Vector3 hitPosition = hitInfo.point; // Coordinate del punto di impatto
         Vector3 hitNormal = hitInfo.normal;  // Normale della superficie colpita
         // aggiusto la rotazione con quella del prefab 
       
         
         GameObject hitObject = hitInfo.collider.gameObject; // GameObject colpito

         Debug.Log($"Il raggio del controller sinistro ha colpito: {hitObject.name} alle coordinate {hitPosition}");

         // Instanzia l'oggetto da spawnare
         GameObject spawnedObject = Instantiate(objectToSpawn);
         spawnedObject.name = objectToSpawn.name;
         // Imposta la posizione dell'oggetto
         spawnedObject.transform.position = hitPosition;

         // Allinea l'oggetto alla superficie del piano utilizzando la normale della superficie
         Quaternion surfaceRotation = Quaternion.LookRotation(hitNormal);

         // Ottieni la direzione verso l'utente (ad esempio, la camera o il controller)
         Vector3 directionToUser = (leftXRController.transform.position - hitPosition).normalized;

         // Ignora la componente verticale della direzione (asse Y) per calcolare solo la direzione orizzontale
         directionToUser.y = 0;  // Ignora l'altezza (asse Y) per evitare inclinazioni verso l'alto o il basso

         // Calcola la rotazione in modo che l'oggetto guardi verso l'utente solo sull'asse XZ
         Quaternion lookAtUserRotation = Quaternion.LookRotation(directionToUser, hitNormal);

         // Applica la rotazione combinata all'oggetto spawnato
         spawnedObject.transform.rotation = lookAtUserRotation;
         
            // Aggiungi l'oggetto alla lista degli oggetti spawnati
         _SimulationManager.spawnedGameObjects.Add(spawnedObject);
         OnObjectPlaced.Invoke(this,EventArgs.Empty);
     }
     else
     {
         Debug.Log("Il raggio del controller sinistro non ha colpito nulla.");
     }
 }

 public void MoveCharacter(object sender, EventArgs obj)
 {
     if (leftControllerRay.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
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


    public void KillAllObjects()
    {
        foreach (var obj in _SimulationManager.spawnedGameObjects)
        {
            GameObject.Destroy(obj);
        }
        _SimulationManager.spawnedGameObjects.Clear();
    }
}
    

