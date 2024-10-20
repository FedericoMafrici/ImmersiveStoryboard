using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PersistentAnchorManager : MonoBehaviour
{
    [SerializeField] ARAnchorManager anchorManager;
    
    // Dizionario per tenere traccia degli oggetti associati alle ancore
    private Dictionary<string, GameObject> anchorsDictionary = new Dictionary<string, GameObject>();

    [SerializeField] Dictionary<string, GameObject> GameObjectsDictionary = new Dictionary<string, GameObject>();
    // Percorso del file JSON
    private string jsonFilePath;

    [System.Serializable]
    public class AnchorData
    {
        public string anchorId;
        public string objectName;

        public AnchorData(string id, string name)
        {
            anchorId = id;
            objectName = name;
        }
    }

    [System.Serializable]
    public class AnchorList
    {
        public List<AnchorData> anchors = new List<AnchorData>();
    }

    private void Start()
    {
     //   anchorManager = FindObjectOfType<ARAnchorManager>();
        jsonFilePath = Path.Combine(Application.persistentDataPath, "anchors.json");

        // Carica le ancore salvate all'avvio
        LoadAnchors();
    }

    public async void CreateAnchor(Vector3 position, GameObject objectName)
    {
        // Crea un'ancora in una posizione data
        var pose = new Pose(position, Quaternion.identity);
        var result = await anchorManager.TryAddAnchorAsync(pose);

        if (result.status.IsSuccess())
        {
            var anchor = result.value;
            // Salva l'ancora e il nome dell'oggetto nel file JSON
            await TrySaveAnchor(anchor, objectName.GetComponent<CharacterManager>().type);
            Debug.Log($"Ancora creata: {anchor.trackableId}");

            // Instanzia un oggetto associato all'ancora
            InstantiateVirtualObject(anchor, objectName);
        }
        else
        {
            Debug.LogError("Errore nella creazione dell'ancora.");
        }
    }

    private async Task TrySaveAnchor(ARAnchor anchor, string objectName)
    {
        // Salva l'ancora in modo persistente
        var saveResult = await anchorManager.TrySaveAnchorAsync(anchor);
        if (saveResult.status.IsSuccess())
        {
            // Salva l'ID dell'ancora e il nome dell'oggetto nel file JSON
            var anchorData = new AnchorData(anchor.trackableId.ToString(), objectName);
            
            // Leggi il contenuto esistente dal file JSON
            AnchorList anchorList;
            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath);
                anchorList = JsonUtility.FromJson<AnchorList>(json);
            }
            else
            {
                anchorList = new AnchorList();
            }

            // Aggiungi il nuovo dato all'elenco
            anchorList.anchors.Add(anchorData);

            // Salva l'elenco aggiornato nel file JSON
            string jsonToSave = JsonUtility.ToJson(anchorList, true);
            File.WriteAllText(jsonFilePath, jsonToSave);
        }
        else
        {
            Debug.LogError("Errore nel salvataggio dell'ancora.");
        }
    }

    private async void LoadAnchors()
    {
        // Carica le ancore salvate dal file JSON
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            AnchorList anchorList = JsonUtility.FromJson<AnchorList>(json);

            foreach (var anchorData in anchorList.anchors)
            {
                // Carica l'ancora persistente usando il suo ID
                var trackableId = new TrackableId(0, ulong.Parse(anchorData.anchorId));
                await LoadAnchor(trackableId, anchorData.objectName);
            }
        }
        else
        {
            Debug.Log("Nessun file JSON trovato.");
        }
    }

    private async Task LoadAnchor(TrackableId trackableId, string objectName)
    {
        var loadResult = await anchorManager.TryLoadAnchorAsync(trackableId);
        if (loadResult.status.IsSuccess())
        {
            var anchor = loadResult.value;
            Debug.Log($"Ancora caricata: {anchor.trackableId} associata a {objectName}");
            GameObject obj; // Non inizializzarlo qui
            if (GameObjectsDictionary.TryGetValue(objectName,out obj))
            {
                anchorsDictionary[objectName] = InstantiateVirtualObject(anchor, obj);
            }
            else
            {
                Debug.Log("Chiave invalida " + objectName);
            }
        }
        else
        {
            Debug.LogError("Errore nel caricamento dell'ancora.");
        }
    }

    private GameObject InstantiateVirtualObject(ARAnchor anchor, GameObject prefab)
    {
        // Crea un oggetto virtuale nella posizione dell'ancora
        GameObject obj = Instantiate(prefab); // Usa il prefab desiderato
        obj.transform.position = anchor.transform.position; // Posiziona l'oggetto
        obj.transform.rotation = anchor.transform.rotation; // Imposta la rotazione dell'oggetto

        // Rendilo figlio dell'ancora per mantenerli collegati
        obj.transform.parent = anchor.transform; 

        return obj;
    }
}
