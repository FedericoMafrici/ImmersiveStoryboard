using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Application = UnityEngine.Application;

public class OutputGenerator : MonoBehaviour
{
    public string sceneName=" Storyboard scena 01";
    private string path="";
    private List<string> _timestamps;
    [SerializeField] public int sceneCode = -1;
    private ScreenshotManager _screenshotManager;
    private PhraseGenerator _phraseGenerator;
    private List<String> buffer;
    public string riformulato = string.Empty;
    private string _html;
    private string description;

    private bool _StoryboardGenerato=true;
    
    public static EventHandler<EventArgs> storyboardSaved;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timestamps = new List<String>();
        sceneCode = UnityEngine.Random.Range(100000, 999999);
        UnityEngine.Debug.Log("Scene code: " + sceneCode);
        
        _screenshotManager = FindObjectOfType<ScreenshotManager>();
        _phraseGenerator = FindObjectOfType<PhraseGenerator>();
    }
  public void GenerateFile()
    {
        Debug.Log("Generate File chiamata con successo");
        _timestamps.Clear();
        //prende tutti gli screenshot count, esclude i doppioni e li ordina per sapere quali immagini mostrare

        buffer = _phraseGenerator.ClearBuffer();

       
        foreach (string b in buffer)
        {
            _timestamps.Add(b.Split('_')[0]);
        }
        _timestamps = _timestamps.Distinct().ToList().OrderBy(i => int.Parse(i)).ToList();
       


       // string basePath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "storyboards/Storyboard.html");
        path =Path.Combine(Application.persistentDataPath, "Federico");
    
       if (Application.platform == RuntimePlatform.Android) // Questo include Meta Quest
       {
           // Salva nella memoria esterna del Meta Quest
           path = Path.Combine(Application.persistentDataPath, "Federico", "Storyboards");
       }
       else
       {
           // Usa persistentDataPath per altre piattaforme
           path = Path.Combine(Application.persistentDataPath, "Federico", "Storyboards");
           Debug.Log(path);
       }
       Directory.CreateDirectory(path);

       // Definisci il percorso completo del file
       path = Path.Combine(path, "Storyboard.html");

       if (File.Exists(path))
       {
           Debug.Log("cancello il file precedente");
           File.Delete(path);
       }
       
        using (var writer = new StreamWriter(path))
        {
            _html = "<!DOCTYPE html> <html> <body style = 'font-family: Courier New, monospace'> " +
                "<h1 contenteditable = \"false\" style = \"text-align:center\">" + sceneName + " Storyboard</h1> <p style = \"text-align:center\">Panels are editable! Click on them to make adjustments. </p> <p style = \"text-align:center\">Go to 'print' and select 'save as pdf' to download your storyboard!</p>";
            var j = 0;
            var tsps = "";
            var intervals = "";
            var t_old = "0";
            var inq = "";

            foreach (string t in _timestamps)
            {
                tsps += t + ", ";

                //UnityEngine.Debug.Log("t: " + t + "; t_old: " + t_old);
                intervals += (Convert.ToInt32(t) - Convert.ToInt32(t_old)).ToString() + ", ";

                j++;
                //prende buffer e raggruppa laddove si presenta lo stesso timestamp (quello corrente)
                description = "";


                foreach (string b in buffer)
                {
                    if (b.Split('_')[0] == t)
                    {
                        description += b.Split('_')[1];
                    }
                }
              //  StartCoroutine(_openAIChatGpt.GetChatGPTResponse(description, OnResponseReceived));
                

     



                _html += "<h3 style = \"text-align:center; margin-top: 20px\">Shot #" + j + "</h3>" +
                    "<div class= \"flex-container\"  style=\"display: flex; flex-direction: row; justify-content: center; margin: 8px; align-items: center; background-color:#c0e9fa;\" >" +
                        "<div><img src=\"Screenshots/" + "img_" + t + ".png\" width=\"550\" height=\"300\" style=\"padding: 20px; margin: 10px\" onerror=\"this.src = 'not_found.png'; \"> </div>" +
                        "<div contenteditable = \"true\">" +
                            "<p>Duration: " + _screenshotManager.actionTimes[t] + " sec</p>" +
                            "<p> Focal length: " + _screenshotManager.focalTable[t] + " mm</p>" +
                        //"<p>Shot type: " + inq + "</p>" +
                        "</div> " +
                    "</div>" +
                    "<p contenteditable = \"true\" style = \"text-align:center; width: '300'; margin-bottom: 10px\"> Original: " + description + "</p>" +
                    "<p contenteditable = \"true\" style = \"text-align:center; width: '300'; margin-bottom: 10px\"> AI edit: " + riformulato + "</p>";



                t_old = t;




            }
            
            
            SaveStoryboard(_html);

            writer.WriteLine(_html);

            _html = "";
        }

        _StoryboardGenerato = true;

        if (_StoryboardGenerato)
        {
            //FEEDBACK LATO UTENTE
            // loadingCanvas.SetActive(false); ?? 
            // _soundManager.GetComponent<AudioSource>().PlayOneShot(_notificationSound);
         
           /* TestoFinale.text = "Storyboard created!\n\n" +
                "Remove your Oculus, you'll find\n" +
                "the storyboard on the PC.";
            */
            _StoryboardGenerato = false;
        }
        


       // Application.OpenURL(path);
       path = path.Replace("Screenshots", "Storyboard.html");
       Debug.Log("PATH PRIMA DELLA OPEN URL  " + path); 
       Application.OpenURL("file://"+path);

    }

    public void SaveStoryboard(string html)
    {
        int index = 0;
        List<String> parent = new List<string>();
        string foldername = "Folder";
        string screenshotsFolder = "Screenshots";
        string folderPath;
        string screenshotsPath;
        if (Application.platform == RuntimePlatform.Android) // Meta Quest e Android
        {
            path= path.Replace("/Storyboard.html", "");
            path = Path.Combine(path, screenshotsFolder);
        }
        else
        {
            path= path.Replace("/Storyboard.html", "");
            path = Path.Combine(path, screenshotsFolder);
        }
        
        
        Directory.CreateDirectory(path);


        _screenshotManager.screenshotsTexture.ForEach((i) =>
        {
            // Converte la texture in PNG
            byte[] pngData = i.EncodeToPNG();
            // Salva il PNG sul disco
            string filePath = Path.Combine(path,"img_" + index + ".png");
            File.WriteAllBytes(filePath, pngData);
            index++;
        });

    }

    void OnResponseReceived(string response)
        {
            Debug.Log("ChatGPT Response: " + response);
        }
    
}
