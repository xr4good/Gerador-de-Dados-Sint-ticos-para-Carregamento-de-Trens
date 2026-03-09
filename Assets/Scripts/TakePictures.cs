using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakePictures : MonoBehaviour {

    int imageNum;

    string[] dataVariance = { "Day", "DayRain", "Night", "NightRain" };
    string[] dataTipe = { "images", "labels"};
    //string[] datasetTipe = { "train", "test","valid" };
    string[] datasetTipe = { "train", "test"};
    string[] kFoldDir = { "fold_1", "fold_2", "fold_3", "fold_4", "fold_5" };
    string dataPath;
    string labelsPath;

    float timer = 0f;
    float timer2 = 0f;
    float timeToSample;
    int totalImages;
    int imgPerFold;
    bool bgIMG = false;


    void Awake()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
    }

    void Start() {
        if ((GameManager.instance.bgTrainImages > 0 || GameManager.instance.bgTestImages > 0) && GameManager.instance.trainImages == 0 && GameManager.instance.testImages == 0)
        {
            bgIMG = true;
            GameManager.instance.trainImages = GameManager.instance.bgTrainImages;
            GameManager.instance.testImages = GameManager.instance.bgTestImages;
            GameManager.instance.isBGImage = true;
        }

        initTP();
        if (GameManager.instance.showDataInImage)
            InvokeRepeating("CallUIUpdate", 0f, 1f);
        CreateDirectories();              
    }

    void initTP()
    {
        totalImages = GameManager.instance.trainImages + GameManager.instance.testImages + GameManager.instance.validImages;
        GameManager.instance.imageCollected_Total = "0 / " + totalImages;
        imageNum = GameManager.instance.startImgNum;
        timeToSample = GameManager.instance.initialTimeData;

        if (imageNum <= GameManager.instance.trainImages)
        {
            GameManager.instance.isTrainImage = true;
            GameManager.instance.isTestImage = false;
        }
        else
        {
            GameManager.instance.isTrainImage = false;
            GameManager.instance.isTestImage = true;
        }

        if (GameManager.instance.useKFoldSeparation)
        {
            GameManager.instance.isTrainImage = false;
            GameManager.instance.isTestImage = false;

            imgPerFold = (int)(totalImages / 5);
            GameManager.instance.kFoldFolderID = (int)((imageNum - 1) / imgPerFold) + 1;
        }
    }

    void CallUIUpdate()
    {
        GameManager.instance.UpdateUIText(imageNum);
    }
    void Update() {
        if (GameManager.instance.collectData && !GameManager.instance.stopCollection)
        {
            timer += Time.deltaTime;
            if (timer >= timeToSample)
            {
                if(GameManager.instance.timeOfDay < 23.9f && GameManager.instance.timeOfDay > 0.1f && !(GameManager.instance.timeOfDay > 5.5f && GameManager.instance.timeOfDay < 6.5f) && !(GameManager.instance.timeOfDay > 17f && GameManager.instance.timeOfDay < 19f))
                {
                    StartCoroutine(DelayPicture());
                    timeToSample = Random.Range(GameManager.instance.timeDataMin, GameManager.instance.timeDataMax);
                    timer = 0f;
                }
                
            }
        }   
        
        if(GameManager.instance.stopCollection)
        {
            timer2 += Time.deltaTime;
            if(timer2 >= GameManager.instance.timeBtData)
            {
                GameManager.instance.stopCollection = false;
                timer2 = 0f;
            }
        }
    }

    void CreateDirectories() {

        //create parent folder
        dataPath = Application.streamingAssetsPath + "/Dataset";
        if (!File.Exists(dataPath)) {
            Directory.CreateDirectory(dataPath);
        }

        foreach(string st in dataVariance)
        {
            dataPath = Application.streamingAssetsPath + "/Dataset/" + st;
            if (!File.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            foreach (string st2 in dataTipe)
            {
                dataPath = Application.streamingAssetsPath + "/Dataset/" + st + "/" + st2;
                if (!File.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }

                if (GameManager.instance.useKFoldSeparation)
                {
                    foreach (string st4 in kFoldDir)
                    {
                        dataPath = Application.streamingAssetsPath + "/Dataset/" + st + "/" + st2 +  "/" + st4;
                        if (!File.Exists(dataPath))
                        {
                            Directory.CreateDirectory(dataPath);
                        }
                    }
                }
                else
                {
                    foreach (string st3 in datasetTipe)
                    {
                        dataPath = Application.streamingAssetsPath + "/Dataset/" + st + "/" + st2 + "/" + st3;
                        if (!File.Exists(dataPath))
                        {
                            Directory.CreateDirectory(dataPath);
                        }
                    }
                }  
            }
        }        
    }

    IEnumerator DelayPicture() {
        bool canTakePicture = false;
        string objData = "";
        string objPos;
        yield return new WaitForEndOfFrame();

        if (!GameManager.instance.isBGImage)
        {
            //update bounds of all objects
            foreach (ObjectBounds bounds in FindObjectsByType<ObjectBounds>(FindObjectsSortMode.None)) {
                bounds.UpdateBounds();
                Rect objBounds = new Rect(bounds.GetBounds());
                if(bounds.isVisible && !bounds.isFilter 
                    && (1 - objBounds.center.y / Screen.height) > 0.01f && (1 - objBounds.center.y / Screen.height) < 0.99f 
                    && (objBounds.height / Screen.height) < 0.4f && (objBounds.height / Screen.height) > 0.01f)
                {
                    //scaling data of unity to yolo format
                    objPos = bounds.objectType + " " + objBounds.center.x/Screen.width + " " + (1-objBounds.center.y / Screen.height) + " " +
                        objBounds.width/Screen.width + " " + objBounds.height / Screen.height;

                    if (canTakePicture)
                        objData += "\r";
                    objData +=  objPos;
                    canTakePicture = true;
                    //Debug.Log(objPos);
                }
            
            }
        }
        else
        {
            canTakePicture = true;
        }
            

        //whith object on screen save data
        if (canTakePicture)
        {
            SelectPath();
            SaveData(objData);      
            
            if(GameManager.instance.pauseWhenSample)
                UnityEditor.EditorApplication.isPaused = true;
        }
    }

    void SelectPath()
    {
        //Generating paths
        dataPath = Application.streamingAssetsPath + "/Dataset";
        //float gameTime = GameManager.instance.timeOfDay;
        if (GameManager.instance.isNight)
        {
            dataPath += "/Night";
        }
        else
        {
            dataPath += "/Day";
        }

        if (GameManager.instance.isRaining)
        {
            dataPath += "Rain";
        }

        if (GameManager.instance.putAllInDayFolder)
        {
            dataPath = Application.streamingAssetsPath + "/Dataset/Day";
        }

        labelsPath = dataPath + "/labels";
        dataPath += "/images";

        if (GameManager.instance.isTrainImage)
        {
            dataPath += "/train";
            labelsPath += "/train";
        }

        if (GameManager.instance.isTestImage)
        {
            dataPath += "/test";
            labelsPath += "/test";
        }

        if (GameManager.instance.useKFoldSeparation)
        {
            dataPath += "/" + kFoldDir[GameManager.instance.kFoldFolderID-1];
            labelsPath += "/" + kFoldDir[GameManager.instance.kFoldFolderID-1];
        }

        /*
        if (imageNum <= GameManager.instance.trainImages)
        {
            dataPath += "/train";
            labelsPath += "/train";
        }
        else if(imageNum <= GameManager.instance.trainImages + GameManager.instance.testImages)
        {

            dataPath += "/test";
            labelsPath += "/test";
        }
        else
        {
            dataPath += "/valid";
            labelsPath += "/valid";
        }
        //*/
    }

    void SaveData(string labelData) {
        //GameManager.instance.UpdateUIText(imageNum);
        // Saving screen shot
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        photo.Apply();
        byte[] data = photo.EncodeToJPG(75);
        DestroyImmediate(photo);

        if (!GameManager.instance.isBGImage)
        {
            //saving image
            File.WriteAllBytes(dataPath + "/" + imageNum + ".jpg", data);
            // Saving object data
            File.WriteAllText(labelsPath + "/" + imageNum + ".txt", labelData);
        }
        else
        {
            //saving image
            File.WriteAllBytes(dataPath + "/bg" + imageNum + ".jpg", data);
        }


        //Debug.Log("Sample: " + imageNum + " de " + totalImages);


        if (imageNum == GameManager.instance.earlyStop)
        {
            Debug.Log("Early stop!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        // Image counting
        if (imageNum == totalImages)
        {
            if((GameManager.instance.bgTrainImages > 0 || GameManager.instance.bgTestImages > 0) && !bgIMG)
            {
                bgIMG = true;
                GameManager.instance.trainImages = GameManager.instance.bgTrainImages;
                GameManager.instance.testImages = GameManager.instance.bgTestImages;
                GameManager.instance.isBGImage = true;
                initTP();
                GameManager.instance.stopCollection = true;
            }
            else
            {
                Debug.Log("Training data collected!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
                
        }
        else
        {
            GameManager.instance.dataCollectionProgress = Mathf.RoundToInt(100.0f * (imageNum / (totalImages * 1.0f)));
            GameManager.instance.imageCollected_Total = imageNum + " / " + totalImages;
            float elapsedTime = Time.time / 60f;

            if(elapsedTime > 60)
            {
                GameManager.instance.timeElapsed = elapsedTime / 60f + " Horas.";
            }
            else
            {
                GameManager.instance.timeElapsed = elapsedTime + " Minutos.";
            }
            imageNum++;
            if (!GameManager.instance.useKFoldSeparation)
            {
                if (imageNum == GameManager.instance.trainImages + 1)
                {
                    GameManager.instance.isTrainImage = false;
                    GameManager.instance.isTestImage = true;
                    GameManager.instance.stopCollection = true;
                }
            }
            else
            {
                GameManager.instance.kFoldFolderID = (int)((imageNum - 1) / imgPerFold) + 1;
                for (int i = 1; i < 5; i++)
                {
                    if (imageNum == (imgPerFold * i + 1))
                        GameManager.instance.stopCollection = true;
                }
            } 
        }
    }
}
