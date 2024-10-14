using Syn.WordNet;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;
//using Syn.WordNet;

public class WordnetManager : MonoBehaviour
{
    [SerializeField] private ConsoleDebugger _debuggingWindow;
    private WordNetEngine wordNet;
    public List<string> syns; //Lista di sinonimi
    private string syn; //Il sinonimo che ritorna

    void Start()
    {
        wordNet = new WordNetEngine();
        syns = new List<string>();
        syn = "";
        string path = "";
        Debug.Log("Loading database...");
#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(LoadWordNetFromStreamingAssets());
#else
        // In Editor o altre piattaforme, puoi caricare il file direttamente
        path = Path.Combine(Application.streamingAssetsPath, "Wordnet"); 
        wordNet.LoadFromDirectory(path);

        if (wordNet.AllWords.Count != 0)
        {
            _debuggingWindow.SetText("Caricamento avvenuto con successo");
        }
        else
        {
            _debuggingWindow.SetText("Errore nel caricamento");
        }
#endif
     
       
        
        if (wordNet.AllWords.Count != 0)
        {
            _debuggingWindow.SetText("Caricamento avvenuto con successo");
            
        }
        else
        {
            _debuggingWindow.SetText("errore nel caricamento");
        }
        Debug.Log("Load completed.");

        GetSyn("man", "Noun");
    }
    // Funzione per caricare il file su Android usando UnityWebRequest
    private IEnumerator LoadWordNetFromStreamingAssets()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Wordnet");

        // Aggiungi il prefisso "file://" per Android
        string androidPath = "jar:file://" + path;

        UnityWebRequest request = UnityWebRequest.Get(androidPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Errore nel caricamento di WordNet su Android: " + request.error);
            _debuggingWindow.SetText("Errore nel caricamento");
        }
        else
        {
            // Puoi accedere ai dati qui
            // wordNet.LoadFromDirectory(request.downloadHandler.text); // Se necessario modificare la logica qui
            Debug.Log("Caricamento avvenuto con successo su Android.");
            _debuggingWindow.SetText("Caricamento avvenuto con successo su Android");
        }
    }

    
    public string GetSyn(string word, string wordType) 
    {
        if (GetComponent<WordnetManager>().enabled) //Se il componente stesso non � attivo (= non ha mai eseguito la Start()), ritorna semplicemente la parola 
        {
            syns.Clear();
            syn = "";

            var synSetList = wordNet.GetSynSets(word);

            if (synSetList.Count == 0)
            {
                Debug.Log($"No SynSet found for '{word}'");
                return word;
            }

            /*
            foreach (var synSet in synSetList)
            {
                if (synSet.PartOfSpeech.ToString() == wordType)
                {
                    //var words = string.Join(", ", synSet.Words);
                    foreach (string w in synSet.Words)
                    {
                        if (!w.Contains("_"))
                        {
                            syns.Add(w);
                            //Debug.Log(w);
                        }
                    }

                    //Debug.Log($"\nWords: {words}");
                    //Debug.Log($"POS: {synSet.PartOfSpeech}");
                    //Debug.Log($"Gloss: {synSet.Gloss}");
                }
            }
            */

            //Tentativo con il primo solo synset

            if (synSetList[0].PartOfSpeech.ToString() == wordType)
            {
                //var words = string.Join(", ", synSet.Words);
                foreach (string w in synSetList[0].Words)
                {
                    if (!w.Contains("_"))
                    {
                        syns.Add(w);
                        //Debug.Log(w);
                    }
                }

                //Debug.Log($"\nWords: {words}");
                //Debug.Log($"POS: {synSet.PartOfSpeech}");
                //Debug.Log($"Gloss: {synSet.Gloss}");
            }



            //Pesca un sinonimo a caso, lo assegna a syn e lo ritorna. Se la lista � vuota, ritorna word.
            if (syns.Count > 0)
            {
                syn = syns[Random.Range(0, syns.Count)];
            }
            else syn = word;
            return syn;
        }
        else return word;
    }

    void Update()
    {
        
    }
}
