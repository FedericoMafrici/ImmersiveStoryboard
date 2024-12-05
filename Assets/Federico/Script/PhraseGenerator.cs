using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;

public class PhraseGenerator : MonoBehaviour
{
    private TextMeshProUGUI phrase;
    public TextMeshProUGUI phraseTextUI;
    private string[] reflexList;
    public ActionsDataBase actionsDB;
    public SimulationManager simulationManager;

    public WordnetManager wordnet;
  //  public DictationHandler dictationEngine;
    public TextMeshProUGUI dialoguePhrase;
    public TMP_InputField _newText;

    public GameObject tastiera;

    private string[] locomotion = new string[] { "is now near", "is beside", "is near", "has moved to", "is in proximity of"};
    private string[] walks = new string[] { "walks on ", "steps on", "moves on" };
    private string[] contemporary = new string[] { "In the meanwhile", "At the same time", "In the while", "Simultaneously" };
    private string[] after = new string[] { "After that", "Afterward", "Following", "Later", "Then" };
    private string[] alone = new string[] { " is alone. ", " moves away. ", " goes away. ", " steps away. " };

   
    private System.Random random;

    private string oldAc;
    private string oldComp;
    private string oldSub;

    private int oldTime;

    private bool isAPerson;

    //Il buffer si riempie con le frasi generate. Deve essere completamente svuotato quando si fa uno screen. Prima dello screen, deve essere limitato alle ultime n frasi.
    public List<string> buffer;


    //Frase di salvataggio camminata e vicinanza con oggetto. Da stampare solo se � l'ultima cosa che il personaggio fa, prima di cambiare player o fare screen.
    public string walking;
    public string forward;
    public string proxim;
    public string combinedText = "";

    void Start()
    {
        random = new System.Random();
        oldAc = "";
        oldSub = "";
        oldComp = "";


        reflexList = actionsDB.GetReflexList(); //lista riflessivi
        phrase = phraseTextUI.GetComponent<TextMeshProUGUI>();
        phrase.text = "";
        buffer.Clear();
        oldTime = 1;
       

        walking = "";
        forward = "";
        proxim = "";
    }

    void Update()
    {
        foreach (var item in buffer)
        {
            //Debug.Log("buffer " + item.ToString());
        }

        
        
    }

    public void AggiornaTesto()
    {
        combinedText = "";
        foreach (string b in ClearBuffer())
        {
            if (Convert.ToInt32(b.Split('_')[0]) == simulationManager.GetScreenshotCount())
            {
                string phraseText = b.Split('_')[1];
                combinedText += phraseText;
            }
            
        }
        phrase.text = combinedText;
    }


        //Chiamato da Object Selection su ActionClick() ---------------- name � il type, surname � il name 
    public void GenerateSimplePhrase(string name, string type, string action, string complementName, string complementType, bool self) 

    {
            Debug.Log(name + " --" + type);
            //resetta le stringhe di walking e forward perch� non le vuole stampare se si fa un'azione
            walking = "";
            forward = "";
            proxim = "";
        
            phrase.text = GenerateTimeAdverb();

            var ac = action;
            var sub = name;
            var com = complementName;

            //prende sinonimo del verbo
            //ac = wordnet.GetSyn(ac, "Verb");

            //Se il verbo � frasale coniuga alla terza persona solo il verbo e non la preposizione che segue

        
            if (ac.Split(' ').Length > 1)
            {
                // ac = ac.Split(' ')[0] + "s " + ac.Split(' ')[1];

                var actionWord = ac.Split(' ');
                if (actionWord[0] == "stop")
                {
                    if (actionWord[1] == "dance")
                    {
                        actionWord[1] = "dancing";
                    } 
                    else
                    {
                        actionWord[1] = actionWord[1] + "ing";

                    }

                    ac = actionWord[0] + " " + actionWord[1];
                }

                var a = ac.Split(' ')[0] + "s ";
                for (int i = 1; i < ac.Split(' ').Length; i++)
                {
                    a += ac.Split(' ')[i] + " ";   
                }

                // Rimuovi lo spazio in eccesso alla fine di 'a'
                a = a.TrimEnd(' ');

                ac = a;
            }
       
            else ac = ac + "s";
        
            //Se il soprannome � uguale al nome (= non � stato dato) usa l'articolo determinativo 
            //e i sinonimi

            if (type == name)
            {
                /*
                //Prende sinonimo del soggetto
                if (name != "man")
                {
                    name = wordnet.GetSyn(name, "Noun");
                }
                */
           //   sub = "the " + name;
                sub = "" + name;
            }
            if (complementType == complementName)
            {

                if (complementName != "man")
                {
                    //Prende sinonimo del complemento
                   // complementName = wordnet.GetSyn(complementName, "Noun");
                }
                //com = "the " + complementName;
                com = "" + complementName;
            }

            //Se il verbo � riflessivo non c'� il complemento (perch� coincide con il soggetto)
            if (self)
            {
                phrase.text += sub + " " + ac + ". ";

            }
            else
            {
                if (ac == "sits")
                {
                    phrase.text += sub + " " + ac + " on " + com + ". ";
                }
                else if (ac=="talks")
                {
                    phrase.text += sub + " " + ac +" to" + " " + com + ". ";
                }
                else
                {
                    phrase.text += sub + " " + ac + " " + com + ". ";
                }

                if (ac == "lock up")
                {
                    phrase.text += sub + " " + "is locked up" + " inside the " + "wardrobe" + ". ";
                }
            }
            
            if (ac != "talks to") //altriemnti mette nel buffer ad es.: First, Paola talks to Sofia. First, Paola talks to Sofia. �Ciao Ciao.� Perch� buffer.Add(simulationManager.GetScreenshotCount() + "_" + phrase.text); � chimato anche in GenerateDialogue()
            {
                 buffer.Add(simulationManager.GetScreenshotCount() + "_" + phrase.text + "_" + simulationManager.GetScreenshotCountUndo());
            }
      

            //Se l'azione � talk o talk to, attiva la casella per l'input del dialogo
            if (action.Split(' ')[0] == "talk")
            {
             //   GenerateDialogue();
             buffer.Add(simulationManager.GetScreenshotCount() + "_" + phrase.text + "_" + simulationManager.GetScreenshotCountUndo());
            }

            //Se due soggetti diversi, nelle ultime due azioni eseguite, hanno eseguito la stessa azione sullo stesso oggetto, elimina dal buffer le ultime due frasi e metti questa
            if (oldAc == action && oldComp == complementType && oldSub != type)
            {
                buffer.RemoveAt(buffer.Count - 1);
                buffer.RemoveAt(buffer.Count - 1);
                buffer.Add(simulationManager.GetScreenshotCount() + "_"+"The " + oldSub + " and the " + type + " " + action + " the " + complementType + "." + "_" + simulationManager.GetScreenshotCountUndo());
            }

            oldAc = action;
            oldComp = complementType;
            oldSub = type;
    }

    //tolto
    public void GenerateStatusPhrase(string type, string status)
    {
        //Cerco sinonimo stato
        status = wordnet.GetSyn(status, "Adjective");

        phrase.text += "The " + type + " is now " + status + ". ";
        buffer.Add(simulationManager.GetScreenshotCount() + "_"+"The " + type + " is now " + status + ". " + "_" + simulationManager.GetScreenshotCountUndo());
    }


    public void GenerateMovementPhrase(string name, string type, string place, string placeType)
    {
        if (name == type)
        {
            if (place == "room")
            {
               // phrase.text += "The " + name + " walks. ";
                phrase.text += "" + name + " walks. ";
            }
            else
            {
                phrase.text += "" + name + " walks towards ";
                //phrase.text += "The " + name + " walks towards ";
                if (place == placeType)
                {
                    phrase.text += "" + place + ". ";
                  //  phrase.text += "the " + place + ". ";
                }
                else
                {
                    phrase.text += place + ". ";
                }
              ;
            }
        }

        else
        {
            if (place == "room")
            {
                phrase.text = name + " walks. ";
            }

            else
            {
                phrase.text = name + " walks towards ";

                if (place == placeType)
                { phrase.text += "the " + place + ". "; }
                else { phrase.text += place + ". "; }

            }



        }

        buffer.Add(simulationManager.GetScreenshotCount() + "_" + phrase.text + "_" + simulationManager.GetScreenshotCountUndo());


    }
    public void StartSpeech()
    {
        //dictationEngine.gameObject.SetActive(true); // dictation
      //  tastiera.SetActive(true);
    }
    public void EndSpeech() 
    {
     //   dictationEngine.gameObject.SetActive(false);
        
    }

    
    
    public void GenerateDialogue()
    {
        //string cleanedDialogue = dialoguePhrase.text.TrimEnd(' '); // dictation
        string cleanedDialogue = _newText.text.TrimEnd(' '); // Rimuove lo spazio finale
        phrase.text += "�" + cleanedDialogue + "� ";
        buffer.Add(simulationManager.GetScreenshotCount() + "_" + phrase.text + "_" + simulationManager.GetScreenshotCountUndo());
        //EndSpeech(); // dictation
    }
    
    public string GenerateTimeAdverb()
    {
        var p = "";
        //if (simulationManager.GetTime() == oldTime && buffer.Count > 0)
        if (simulationManager.contemporaryAction && !simulationManager.firstAction)
      
        {
            p = contemporary[random.Next(contemporary.Length)] + ", ";
        }

       // else if (simulationManager.GetTime() > oldTime)
        else if (!simulationManager.contemporaryAction && !simulationManager.firstAction)
     
        {
            //oldTime = simulationManager.GetTime();
            p = after[random.Next(after.Length)] + ", ";
        }

       // else if (buffer.Count == 0)
        else if (simulationManager.firstAction)
        {
            p = "First, ";
            simulationManager.firstAction = false;
        }
        return p;

    }

    public void UpdateForward(string f, bool personName)
    {
        forward = f;
        isAPerson = personName;

    }

    public void GenerateConditionPhrase()
    {
        //Viene generata solo se � l'ultima cosa che un personaggio fa, ovvero non ha fatto azioni dopo, dunque le due variabili non sono state resettate
        //Metodo chiamato da cameraManager quando si fa uno screen e da simulation manager quando si cambia personaggio
        if (walking != "" && forward != "")
        {
            buffer.Add(simulationManager.GetScreenshotCount() + "_" + walking + forward + ". " + "_" + simulationManager.GetScreenshotCountUndo());
        }
        if (proxim != "")
        {
            buffer.Add(simulationManager.GetScreenshotCount() + "_" + proxim + "_" + simulationManager.GetScreenshotCountUndo());
        }
        if (proxim == "" && walking == "" && forward == "")
        {
            buffer.Add(simulationManager.GetScreenshotCount() + "_" + " " + "_" + simulationManager.GetScreenshotCountUndo());
        }

        proxim = "";
        walking = "";
        forward = "";
        phrase.text = "";
    }

    public void  DeleteLastSentence()
    {
        buffer.RemoveAt(buffer.Count-1);
    }
    public List<string> ClearBuffer()
    {

        int i = buffer.Count - 1; 

        while (i > 0)
        {
            if (buffer[i] == buffer[i - 1])
            {
                buffer.RemoveAt(i);
            }

            i--;
        }

        return buffer;
        
    }
}
