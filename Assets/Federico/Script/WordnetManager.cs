using Syn.WordNet;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

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
      //     _debuggingWindow.SetText("Caricamento Wordnet Database avvenuto con successo");
        }
        else
        {
       //     _debuggingWindow.SetText("Errore nel caricamento");
        }
#endif
        Debug.Log("Load completed.");

        GetSyn("man", "Noun");
    }
    // Funzione per caricare il file su Android usando UnityWebRequest
   
    
  private IEnumerator LoadWordNetFromStreamingAssets()
{
    string sourcePath = Path.Combine(Application.streamingAssetsPath, "Wordnet");
    string targetPath = Path.Combine(Application.persistentDataPath, "Wordnet");

    // Crea la directory di destinazione se non esiste
    if (!Directory.Exists(targetPath))
    {
        Directory.CreateDirectory(targetPath);
    }

    // Elenco dei file da copiare nella cartella Wordnet
    string[] wordNetFiles = { "data.adj", "data.adv", "data.noun", "data.verb", "index.adj", "index.adv", "index.noun", "index.verb" };
    foreach (var fileName in wordNetFiles)
    {
        string sourceFilePath = Path.Combine(sourcePath, fileName);
        string targetFilePath = Path.Combine(targetPath, fileName);
    //    _debuggingWindow.SetText(sourceFilePath + "\n"+ targetFilePath);
        #if UNITY_ANDROID
        // Su Android, usa UnityWebRequest per leggere i file da StreamingAssets
        UnityWebRequest request = UnityWebRequest.Get(sourceFilePath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Errore nel caricamento del file: " + fileName + " - " + request.error);
        //    _debuggingWindow.SetText("Errore nel caricamento del file: " + fileName);
            yield break;  // Esci se c'è un errore
        }
        else
        {
            // Scrivi il contenuto del file nella directory persistente
            File.WriteAllBytes(targetFilePath, request.downloadHandler.data);
            Debug.Log("File " + fileName + " copiato in " + targetFilePath);
        }
        #else
        // Su piattaforme non Android, copia direttamente il file
        if (File.Exists(sourceFilePath))
        {
            File.Copy(sourceFilePath, targetFilePath, true);
            Debug.Log("File " + fileName + " copiato in " + targetFilePath);
            _debuggingWindow.SetText("File " + fileName + " copiato in " + targetFilePath);
        }
        else
        {
            Debug.LogError("File non trovato: " + sourceFilePath);
        }
        #endif
    }

    // Dopo aver copiato i file, usa la funzione LoadFromDirectory sulla directory di destinazione
    wordNet.LoadFromDirectory(targetPath);

    if (wordNet.AllWords.Count > 0)
    {
        Debug.Log("Caricamento WordNet completato con successo. Numero parole: " + wordNet.AllWords.Count);
   //     _debuggingWindow.SetText("Caricamento WordNet completato con successo su Android");
    }
    else
    {
        Debug.LogError("Errore nel caricamento del database WordNet.");
   //     _debuggingWindow.SetText("Errore nel caricamento del database WordNet su Android.");
    }
    yield break;  // Assicurati di terminare l'IEnumerator
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
