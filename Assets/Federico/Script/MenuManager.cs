using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public GameObject menuUIElement;
    [Header("Manager della simulazione")] 
    [SerializeField] public ControllerManager controllerManager;
    [Header("Element To spawn")] [SerializeField]
    public GameObject[] sceneObjects; 
   
    [Header("Componenti UI del menu ")]
    [SerializeField] public GameObject sceneObjectPanel;
    [SerializeField] public GameObject sceneObjectPanelOutline;
    [SerializeField] public GameObject  boundingBoxesPanel;
    [SerializeField] public GameObject  boundingBoxesPanelOutline;
    [SerializeField] public GameObject tableTopObjectsPanel;
    [SerializeField] public GameObject tableTopObjectsPanelOutline;
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<GameObject> values = new List<GameObject>();
    [SerializeField] public Dictionary<string, GameObject> myDictionary = new Dictionary<string, GameObject >();

    [SerializeField] public Image BackGroundPanelScript;
    [SerializeField] public Image BackGroundPanelObject;
    //SCRIPT 
    public int currentPage = 0;
    public int totalPageNumber=0;
    public bool firstScriptDisplayed = true;
    public List<Texture> activeImagesTextureList = new List<Texture>();
    public List<Texture> firstScript = new List<Texture>();
    public List<Texture> secondScript = new List<Texture>();
    [SerializeField] private RawImage currentImage;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject showScriptButton;
    [SerializeField] private GameObject nextButton;
    // Start is called before the first frame update
    //lista di oggetti: 
    [SerializeField] private Color SelectionColor;
    [SerializeField] private Color NotSelectedColor;
    /*
    [SerializeField] private String SelectionColor = "1870B6";
    [SerializeField] private String NotSelectedColor = "9A9A9A";
    */
    public int totalObjectListPageNumber = 2;
    public int currentObjectListPageNumber = 0;
    [SerializeField] List<GameObject> objectListPages = new List<GameObject>();
    void Start()
    {
       /*
        foreach (var obj in sceneObjects)
        {
            GameObject.Instantiate(menuUIElement,this.transform);
            
        }
        */
       // Popola il dizionario dalle liste
       for (int i = 0; i < keys.Count && i < values.Count; i++)
       {
           if (!myDictionary.ContainsKey(keys[i]))
           {
               myDictionary.Add(keys[i], values[i]);
           }
       }

       activeImagesTextureList = firstScript;
       totalPageNumber=activeImagesTextureList.Count;
       SetSceneObjects();
       //SetShowSscript();
      // SelectObject("Woman");
       ResetPageNumber();
      
       
    }

    public void SetSceneObjects()
    {
//        boundingBoxesPanel.SetActive(false);
//        boundingBoxesPanelOutline.SetActive(false);
       
        tableTopObjectsPanel.SetActive(false);
       // tableTopObjectsPanelOutline.SetActive(false);
      
        sceneObjectPanel.SetActive(true);
        sceneObjectPanelOutline.SetActive(true);
        
       
            // Assegna il colore al componente Image
            BackGroundPanelObject.color = SelectionColor;
        
        
        
            // Assegna il colore al componente Image
            BackGroundPanelScript.color = NotSelectedColor;
        
        
        
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        ResetPageNumber();
    }

    public void SetBoundingBoxes()
    {
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(false);
        tableTopObjectsPanelOutline.SetActive(false);
      
  //      boundingBoxesPanel.SetActive(true);
  //      boundingBoxesPanelOutline.SetActive(true);
        ResetPageNumber();
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        
    }
    public void SetShowSscript()
    {
        
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
       
  //      boundingBoxesPanel.SetActive(false);
  //      boundingBoxesPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(true);
        tableTopObjectsPanelOutline.SetActive(true);
        Color newColor;
        
            // Assegna il colore al componente Image
            BackGroundPanelScript.color = SelectionColor;
        
            
            BackGroundPanelObject.color = NotSelectedColor;
            

        ResetPageNumber();
        previousButton.SetActive(true);
        nextButton.SetActive(true);
    }
    public void SelectObject(string obj )
    {
        GameObject prefab;
        if(myDictionary.TryGetValue(obj,out  prefab))
        {
            Debug.Log("valore trovato oggetto"+prefab.name);
            controllerManager.SetObjectToSpawn(prefab);
        }
        else
        {
            Debug.LogError("oggetto non trovato stringa passata " + obj);
        }
    }
    public void ResetPageNumber()
    {
       
        currentPage = 0;
      if(activeImagesTextureList!=null)
        currentImage.texture = activeImagesTextureList[currentPage];
    }
    public void NextPage()
    {
        if(currentPage<totalPageNumber-1)
        {currentPage++;}
        currentImage.texture = activeImagesTextureList[currentPage];
        currentImage.GetComponent<AutoResizeRawImage>().UpdateImage(currentImage.texture);
    }
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
        }
        currentImage.texture = activeImagesTextureList[currentPage];
        currentImage.GetComponent<AutoResizeRawImage>().UpdateImage(currentImage.texture);
    }

    public void ChangeScript()
    {
        if (firstScriptDisplayed)
        {
            activeImagesTextureList = secondScript;
            firstScriptDisplayed = false;
            currentPage = 0;
            currentImage.texture = activeImagesTextureList[currentPage];
            totalPageNumber = activeImagesTextureList.Count;
        }
        else
        {
            activeImagesTextureList = firstScript;
            currentPage = 0;
            currentImage.texture = activeImagesTextureList[currentPage];
            firstScriptDisplayed = true;
            totalPageNumber = activeImagesTextureList.Count;
        }
    }

    public void NextObjectsListPage()
    {
        if (currentObjectListPageNumber<totalObjectListPageNumber-1)
        {
            currentObjectListPageNumber++;
            int index = 0;
            foreach(var obj in objectListPages)
            {
                if (index != currentObjectListPageNumber)
                {
                    obj.SetActive(false);
                }
                else
                {
                    obj.SetActive(true);
                }

                index++;
            }
        }
    }
    public void PreviousObjectListPage()
    {
        if (currentObjectListPageNumber>0)
        {
            currentObjectListPageNumber--;
            int index = 0;
            foreach(var obj in objectListPages)
            {
                if (index != currentObjectListPageNumber)
                {
                    obj.SetActive(false);
                }
                else
                {
                    obj.SetActive(true);
                }

                index++;
            }
        }
    }
}
