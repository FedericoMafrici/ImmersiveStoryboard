using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SceneAnchorManager : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;

    // Prefab da istanziare per ogni ancora
    [SerializeField] private GameObject anchorPrefab;

    // Dizionario per tracciare le ancore e gli oggetti associati
    private Dictionary<ARAnchor, List<GameObject>> anchorReferences = new Dictionary<ARAnchor, List<GameObject>>();
    float minDistance = 2.0f;

    void Start()
    {
        if (anchorManager == null)
        {
            anchorManager = FindObjectOfType<ARAnchorManager>();
            if (anchorManager == null)
            {
                Debug.LogError("SceneAnchorManager: ARAnchorManager non trovato nella scena.");
            }
        }
    }

    public void AttachObjectToAnchor(CharacterAnchorManager obj)
    {
        if (anchorManager == null)
        {
            Debug.LogError("ARAnchorManager non trovato. Assicurati che sia presente nella scena.");
            return;
        }

        // Trova l'ancora più vicina
        ARAnchor nearestAnchor = FindNearestAnchor(obj.transform.position);

        if (nearestAnchor != null)
        {
            // Assegna l'oggetto all'ancora esistente
            obj.currentAnchor = nearestAnchor;
            obj.transform.parent = nearestAnchor.transform;
            AddReference(nearestAnchor, obj.gameObject);
            Debug.Log("Ancora vicina trovata, procedo ad assegnarla.");
        }
        else
        {
            // Nessuna ancora trovata, crea una nuova
            CreateAnchor(obj);
        }
    }

    public void DetachObjectFromAnchor(CharacterAnchorManager obj)
    {
        if (obj.currentAnchor != null)
        {
            RemoveReference(obj.currentAnchor, obj.gameObject);
            obj.transform.parent = null;
            obj.currentAnchor = null;
            Debug.Log("Oggetto scollegato dall'ancora.");
        }
    }

    private ARAnchor FindNearestAnchor(Vector3 position)
    {
        ARAnchor nearestAnchor = null;
        float minDistance = float.MaxValue;

        foreach (var anchor in anchorReferences.Keys)
        {
            float distance = Vector3.Distance(position, anchor.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestAnchor = anchor;
            }
        }

        return nearestAnchor;
    }

    public async void CreateAnchor(CharacterAnchorManager obj)
    {
        // Controlla e rimuove l'ancora precedente se esiste
        if (obj.currentAnchor != null)
        {
            Destroy(obj.currentAnchor.gameObject);
            obj.currentAnchor = null;
        }

        // Verifica l'esistenza di ARAnchorManager
        if (anchorManager == null)
        {
            Debug.LogError("ARAnchorManager non trovato. Assicurati che sia presente nella scena.");
            return;
        }

        try
        {
            var result = await anchorManager.TryAddAnchorAsync(new Pose(obj.transform.position, obj.transform.rotation));
            if (result.status.IsSuccess())
            {
                ARAnchor newAnchor = result.value;
                obj.currentAnchor = newAnchor;

                // Istanzia il prefab se assegnato
                if (anchorPrefab != null)
                {
                    GameObject anchorObject = Instantiate(anchorPrefab, obj.transform.position, obj.transform.rotation);
                    // Imposta il prefab come figlio dell'ancora
                    anchorObject.transform.parent = newAnchor.transform;
                }
                else
                {
                    Debug.LogWarning("Nessun prefab assegnato in SceneAnchorManager.");
                }

                // Imposta l'oggetto come figlio dell'ancora
                obj.transform.parent = newAnchor.transform;
                anchorReferences[newAnchor] = new List<GameObject> { obj.gameObject };
                Debug.Log("Ancora creata e assegnata con successo.");
            }
            else
            {
                Debug.LogWarning("Impossibile creare un'ancora alla posizione specificata.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore durante la creazione dell'ancora: " + ex.Message);
        }
    }

    private void AddReference(ARAnchor anchor, GameObject obj)
    {
        if (anchorReferences.ContainsKey(anchor))
        {
            anchorReferences[anchor].Add(obj);
        }
        else
        {
            anchorReferences[anchor] = new List<GameObject> { obj };
        }
        Debug.Log("Riferimento all'ancora aggiunto.");
    }

    private void RemoveReference(ARAnchor anchor, GameObject obj)
    {
        if (anchorReferences.ContainsKey(anchor))
        {
            anchorReferences[anchor].Remove(obj);

            // Se non ci sono più oggetti associati, distruggi l'ancora
            if (anchorReferences[anchor].Count == 0)
            {
                anchorReferences.Remove(anchor);
                Destroy(anchor.gameObject);
                Debug.Log("Ancora distrutta poiché non ha più oggetti associati.");
            }
        }
        Debug.Log("Riferimento all'ancora rimosso.");
    }
}
