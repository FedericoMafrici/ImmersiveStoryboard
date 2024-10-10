using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

public class ActionsDataBase : MonoBehaviour
{
    //public DataTable DataBase = new DataTable("DataBase");
    public Hashtable O_A_table = new Hashtable();
    public Hashtable O_S_table = new Hashtable();
    public Hashtable S_A_table = new Hashtable();


    public List<string> tmp;
    public List<string> tmp2;


    public TextAsset textJSON;

    // public Model selectedModel;
    public Hashtable actionsTable = new Hashtable();

    [System.Serializable]
    public class Model
    {
        public string name;
        public List<string> transitions;
        public List<string> S_A;
    }

    [System.Serializable]
    public class ModelList
    {
        public List<Model> model;
    }

    public ModelList myModelList = new ModelList();


    public DataTable transitions_table = new DataTable("transitions_table");



    private bool firstLoop;
    private List<string> supportList = new List<string>();

    void Start()
    {
        StartCoroutine(ActionsSetup());
    }

    IEnumerator ActionsSetup()
    {
        myModelList = JsonUtility.FromJson<ModelList>(textJSON.text);

        string basePath = Path.Combine(Application.streamingAssetsPath, "actions.csv");

#if UNITY_EDITOR || UNITY_STANDALONE
        basePath = "file://" + basePath;
#elif UNITY_ANDROID
        basePath = basePath; // Su Android, `Application.streamingAssetsPath` è già corretto.
#endif

        UnityWebRequest www = UnityWebRequest.Get(basePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Legge il contenuto del file CSV
            string fileContent = www.downloadHandler.text;
        
            // Separa il contenuto per linee
            var lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Per ogni linea, separa i valori e popola la hashtable
            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (values.Length > 1) // Assicurati che ci siano abbastanza valori per inserire nella tabella
                {
                    supportList.Clear();
                    firstLoop = true;

                    foreach (var v in values)
                    {
                        if (!firstLoop)
                        {
                            supportList.Add(v.Trim()); // Rimuovi eventuali spazi bianchi
                        }
                        else
                        {
                            firstLoop = false;
                        }
                    }

                    // Aggiungi la prima colonna come chiave e il resto come lista nella hashtable
                    actionsTable[values[0].Trim()] = new List<string>(supportList);
                }
            }

            Debug.Log("Caricamento completato. Numero di record nella actionsTable: " + actionsTable.Count);
        }
        else
        {
            Debug.LogError("Errore nel caricamento del file: " + www.error);
        }
    }

   



    public string[] ReturnActions(string character, string complement, List<string> state, bool self)
    {

        string[] arr;
        tmp = new List<string>();
        tmp2 = new List<string>();
        List<string> support = new List<string>();

        List<string> notStates = new List<string>(); //lista degli stati in cui l'oggetto NON si trova
        List<string> support2 = new List<string>();


        if (self)
            complement = "self";

        tmp.Clear();
        if ((List<string>)actionsTable[character + "-" + complement] != null)
        {
            //tmp = (List<string>)actionsTable[character + "-" + complement];
            foreach (string s in (List<string>)actionsTable[character + "-" + complement])
            {
                tmp.Add(s);
            }
    
        }



        if (state != null && state.Count != 0 && !self)
        {
            tmp2.Clear();
            foreach (Model m in myModelList.model)
            {
                if (m.name == complement)
                {
                    foreach (string s in m.S_A)
                    {
                        var st = s.Split(':')[0];
                        if (st == state[0])
                        {

                            foreach (string a in s.Split(':')[1].Split(','))
                            {
                                tmp2.Add(a);
                            }
                        }

                    }
                }
            }


            if (state.Count > 1)
            {
                int i = 1;
                while (state.Count >= (i + 1))
                {
                    foreach (Model m in myModelList.model)
                    {
                        if (m.name == complement)
                        {
                            foreach (string s in m.S_A)
                            {
                                var st = s.Split(':')[0];
                                if (st == state[i])
                                {

                                    foreach (string a in s.Split(':')[1].Split(','))
                                    {
                                        tmp2.Add(a);
                                    }
                                }

                            }
                        }
                    }
                    i++;
                }
            }

        }

        if (state != null && state.Count != 0 && self)
        {
            tmp2.Clear();
            foreach (Model m in myModelList.model)
            {
                if (m.name == character)
                {
                    foreach (string s in m.S_A)
                    {
                        var st = s.Split(':')[0];
                        if (st == state[0])
                        {

                            foreach (string a in s.Split(':')[1].Split(','))
                            {
                                tmp2.Add(a);
                            }
                        }

                    }
                }
            }


            if (state.Count > 1)
            {
                int i = 1;
                while (state.Count >= (i + 1))
                {
                    foreach (Model m in myModelList.model)
                    {
                        if (m.name == character)
                        {
                            foreach (string s in m.S_A)
                            {
                                var st = s.Split(':')[0];
                                if (st == state[i])
                                {

                                    foreach (string a in s.Split(':')[1].Split(','))
                                    {
                                        tmp2.Add(a);
                                    }
                                }

                            }
                        }
                    }
                    i++;
                }
            }

        }

        if (state != null)
        {
            support.Clear();
            notStates.Clear();
            support2.Clear();

            if (!self)
            {
                if (GetPossibleStates(complement) != null)
                {
                    foreach (string s in GetPossibleStates(complement))
                    {
                        if (!state.Contains(s))
                            notStates.Add(s);
                    }

                }
            }
            else
            {
                if (GetPossibleStates(character) != null)
                {
                    foreach (string s in GetPossibleStates(character))
                    {
                        if (!state.Contains(s))
                            notStates.Add(s);
                    }

                }
            }
            // cerco gli stati in cui l'oggetto NON si trova


            foreach (string a in tmp2)
            {
                support.Add(a);
            }
            foreach (string a in support)
            {
                if (!tmp.Contains(a)) tmp2.Remove(a);
            }

            foreach (Model m in myModelList.model)
            {
                if (!self)
                {
                    if (m.name == complement)
                    {
                        foreach (string s in m.S_A)
                        {
                            foreach (string ns in notStates)
                            {

                                if (s.Split(':')[0] == ns)
                                {
                                    foreach (string a in s.Split(':')[1].Split(','))
                                    {
                                        support2.Add(a);

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (m.name == character)
                    {
                        foreach (string s in m.S_A)
                        {
                            foreach (string ns in notStates)
                            {

                                if (s.Split(':')[0] == ns)
                                {
                                    foreach (string a in s.Split(':')[1].Split(','))
                                    {
                                        support2.Add(a);

                                    }
                                }
                            }
                        }
                    }
                }
            }



            foreach (string s in support2)
            {
                if (tmp.Contains(s)) tmp.Remove(s);
            }

        }


        if (tmp == null && tmp2 != null)
            arr = tmp2.ToArray();
        else if (tmp != null && tmp2 == null)
            arr = tmp.ToArray();
        else if (tmp != null && tmp2 != null)
            arr = tmp.Concat(tmp2).ToList().Distinct().ToArray();
        else return null;
        Debug.Log("arr count" + arr.Count());
        return arr;
    }



    public List<String> GetPossibleStates(string n)
    {
        List<string> tmp = new List<String>();
        var help = false;
        foreach (Model m in myModelList.model)
        {
            if (m.name == n)
            {
                foreach (string s in m.transitions)
                {
                    tmp.Add(s.Split('>')[0]);
                    help = true;
                }
            }
        }
        List<string> tmp2 = tmp.Distinct().ToList();
        if (help) return tmp2;
        else return null;
    }

    public string[] GetStateTransition(string action, string n)
    {
        string[] arr = new string[3];
        var help = true;
        /*
        if (transitions_table.Rows.Find(action) != null)
        {
            return transitions_table.Rows.Find(action).ItemArray.Select(x => x.ToString()).ToArray();
        }
        else return null;
        */
        foreach (Model m in myModelList.model)
        {
            if (m.name == n)
            {
                foreach (string s in m.transitions)
                {
                    if (s.Split('>')[1] == action) //vuol dire che l'azione cambia lo stato
                    {
                        arr[0] = s.Split('>')[0];
                        arr[1] = s.Split('>')[1];
                        arr[2] = s.Split('>')[2];
                        help = false;
                    }
                }
            }

        }
        if (help) return null;
        else return arr;

    }

    public string[] GetReflexList()
    {       //DEPRECATO

        return new string[] { "jump", "wait", "sit", "stand", "laugh", "smile", "cry" }; //lista riflessivi

    }

    void Update()
    {

    }

}
