using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class GameManager : MonoBehaviour
{
    [Header("Linked Objects In Scene")]
    [SerializeField] private GameObject wagon;
    [SerializeField] private Transform spaw;
    [SerializeField] private GameObject sun;
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private Volume volume;
    [SerializeField] private Terrain terrain;
    [SerializeField] public GUISkin guiSkin;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private List<Material> materials = new List<Material>();

    [Space(10)]
    [Header("Configuration")]
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] public bool showBoundBox = false;
    [SerializeField] public bool showDataInImage = true;
    [SerializeField] public bool setRaining = false;

    [Space(10)]
    [Header("Wagon Configuration")]
    [SerializeField] public int randomSeedObj = 42;
    [SerializeField] public int randomSeedLvObj = 42;
    [SerializeField] public float wagonSpeed = 1;
    [SerializeField, Range(0f,1f)] public float objPorcent1 = 1f;
    [SerializeField, Range(0f, 1f)] public float objPorcent2 = 0.5f;
    [SerializeField, Range(0f, 1f)] public float objPorcent3 = 0.5f;
    [SerializeField] public List<GameObject> objects = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObj = new List<GameObject>();
    

    [Space(10)]
    [Header("Data Configuration")]
    [SerializeField] public bool collectData = false;
    [SerializeField] public bool pauseWhenSample = false;
    [SerializeField] public bool putAllInDayFolder = false;

    [SerializeField] public bool useKFoldSeparation = false;

    [SerializeField, Range(1, 5)] public int testGroup = 5;
    [SerializeField, Range(0f, 1f)] public float porcentageOfObjects = 0.5f;

    [SerializeField] public int startImgNum = 1;
    [SerializeField] public int earlyStop = 0;
    [SerializeField] public int trainImages = 10;
    [SerializeField] public int testImages = 2;
    [SerializeField] public int bgTrainImages = 10;    
    [SerializeField] public int bgTestImages = 2;
    //[SerializeField] public int validImages = 0;
    [HideInInspector] public int validImages = 0;
    [SerializeField] public float initialTimeData = 10;
    [SerializeField] public float timeBtData = 10;
    [SerializeField] public float timeDataMin = 2;
    [SerializeField] public float timeDataMax = 4;

    [Space(10)]
    [Header("Information")]
    [SerializeField] public float timeOfDay;
    [SerializeField] public bool isNight = false;
    [SerializeField] public bool isRaining = false;
    [SerializeField] public int dataCollectionProgress = 0;
    [SerializeField] public string imageCollected_Total = "0 / 0";
    [SerializeField] public string timeElapsed = "0 Minutos.";
    [Space(10)]
    [SerializeField] public bool isTrainImage = true;
    [SerializeField] public bool isTestImage = false;
    [SerializeField] public bool isBGImage = false;
    [SerializeField] public bool stopCollection = false;
    [SerializeField] public int kFoldFolderID = 0;


    [Space(10)]
    [Header("Objects Information Debug")]
    [SerializeField] public bool testObjects = false;
    [SerializeField] public int[] listPosObj;
    private GameObject[,] objectsList;
    [SerializeField] public List<GameObject> objectsList1 = new List<GameObject>();
    [SerializeField] public List<GameObject> objectsList2 = new List<GameObject>();
    [SerializeField] public List<GameObject> objectsList3 = new List<GameObject>();
    [SerializeField] public List<GameObject> objectsList4 = new List<GameObject>();
    [SerializeField] public List<GameObject> objectsList5 = new List<GameObject>();
    [Space(10)]
    [SerializeField] public int[] listPosLivObj;
    private GameObject[,] livingObjList;
    [SerializeField] public List<GameObject> livingObjList1 = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObjList2 = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObjList3 = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObjList4 = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObjList5 = new List<GameObject>();

    [Space(10)]
    [SerializeField] public List<GameObject> objTrain = new List<GameObject>();
    [SerializeField] public List<GameObject> objTest = new List<GameObject>();

    [SerializeField] public List<GameObject> livingObjTrain = new List<GameObject>();
    [SerializeField] public List<GameObject> livingObjTest = new List<GameObject>();


    private float timer = 0f;
    private static float timeToUpdateBounds = 0.05f;
    private Vector3 spawPoint;
    private float angle = 0;
    public static GameManager instance;
    private int dayCout = 1;

    //Rain config data
    private float sunLux = 115000f;
    private static float minSunLuxRain = 10000f;
    private static float maxSunLuxRain = 40000f;
    private static float initFog = 150f;
    private static float minRainFog = 10f;
    private static float maxRainFog = 30f;
    private Fog fog;
    private List<float> matSmootInit = new List<float>();
    private static float rainSmoothness = 0.6f;
    private static float rainIntensityMin = 100f;
    private static float rainIntensityMax = 5000f;
    private static float rainWindVariance = -5f;

    TerrainLayer[] layers;

    private void Awake()
    {
        instance = this;
        if (!testObjects)
        {
            objectsList1 = new List<GameObject>();
            objectsList2 = new List<GameObject>();
            objectsList3 = new List<GameObject>();
            objectsList4 = new List<GameObject>();
            objectsList5 = new List<GameObject>();

            livingObjList1 = new List<GameObject>();
            livingObjList2 = new List<GameObject>();
            livingObjList3 = new List<GameObject>();
            livingObjList4 = new List<GameObject>();
            livingObjList5 = new List<GameObject>();

            objTrain = new List<GameObject>();
            objTest = new List<GameObject>();

            livingObjTrain = new List<GameObject>();
            livingObjTest = new List<GameObject>();

            
            DivideObjs();
        }
        
    }

    void Start()
    {
        
        Random.InitState((int)System.DateTime.Now.Ticks);

        QualitySettings.vSyncCount = 0;
        if(collectData)
            Application.targetFrameRate = 30;
        else
            Application.targetFrameRate = 60;
        foreach (Material mat in materials)
        {
            matSmootInit.Add(mat.GetFloat("_Smoothness"));
        }
        volume.profile.TryGet(out fog);
        sunLux = sun.GetComponent<Light>().intensity;
        spawPoint = spaw.transform.position;
        var obj = Instantiate(wagon, spawPoint, Quaternion.identity);
        obj.name = "Wagon";

        //angle = sun.transform.eulerAngles.x;
        //if(angle < 0 ) angle = 0;
        //angle = 155;
        
        textUI.gameObject.transform.parent.gameObject.SetActive(showDataInImage);

        layers = terrain.terrainData.terrainLayers;

    }

    void Update()
    {
        if(setRaining)
            SetRain();
        //Timer to show boundin box if is using
        if (showBoundBox)
        {
            timer += Time.deltaTime;
            if (timer >= timeToUpdateBounds)
            {
                foreach (ObjectBounds bounds in FindObjectsByType<ObjectBounds>(FindObjectsSortMode.None))
                {
                    bounds.UpdateBounds();
                }
                timer = 0f;
            }
        }
        
        //Sun moviment
        angle += timeMultiplier * Time.deltaTime * 0.0041666f;
        if(angle > 360) 
        { 
            angle = 0; 
        }
        sun.transform.eulerAngles = new Vector3(angle, 25, 0);
        //sun.transform.Rotate(Time.deltaTime / timeMultiplier, 0, 0);
        //angle = sun.transform.eulerAngles.x;
        


        //Time of the day calculation
        if (angle < 270)
        {
            timeOfDay = map(angle, 0, 270, 6, 24);
        }
        else
        {
            timeOfDay = map(angle, 270, 360, 0, 6);
        }

        if (timeOfDay > 18f || timeOfDay < 5.5f)
        {
            isNight = true;
        }
        else
        {
            if (isNight)
            {
                setRaining = true;
                dayCout++;
            }
            isNight = false;
        }

        
    }

    void DivideObjs()
    {
        Random.InitState(randomSeedObj);
        int numObjPerList = (int)(objects.Count / 5);
        listPosObj = new int[objects.Count];
        for (int i = 0; i < objects.Count ; i++)
        {
            listPosObj[i] = i;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            int index = Random.Range(0, (listPosObj.Length - 1));
            int aux = listPosObj[i];
            listPosObj[i] = listPosObj[index];
            listPosObj[index] = aux;
        }

        objectsList = new GameObject[5,numObjPerList];
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < numObjPerList; j++)
            {
                objectsList[i, j] = objects[listPosObj[count]];
                if(i == testGroup-1)
                {
                    objTest.Add(objects[listPosObj[count]]);
                }
                else
                {
                    objTrain.Add(objects[listPosObj[count]]);
                }
                count++;
            }
        }

      
        for (int j = 0; j < numObjPerList; j++)
        {
            objectsList1.Add(objectsList[0, j]);
            objectsList2.Add(objectsList[1, j]);
            objectsList3.Add(objectsList[2, j]);
            objectsList4.Add(objectsList[3, j]);
            objectsList5.Add(objectsList[4, j]);

        }

        Random.InitState(randomSeedLvObj);
        numObjPerList = (int)(livingObj.Count / 5);
        listPosLivObj = new int[livingObj.Count];
        for (int i = 0; i < livingObj.Count; i++)
        {
            listPosLivObj[i] = i;
        }

        for (int i = 0; i < livingObj.Count; i++)
        {
            int index = Random.Range(0, (listPosLivObj.Length - 1));
            int aux = listPosLivObj[i];
            listPosLivObj[i] = listPosLivObj[index];
            listPosLivObj[index] = aux;
        }

        livingObjList = new GameObject[5, numObjPerList];
        count = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < numObjPerList; j++)
            {
                livingObjList[i, j] = livingObj[listPosLivObj[count]];
                if (i == testGroup - 1)
                {
                    livingObjTest.Add(livingObj[listPosLivObj[count]]);
                }
                else
                {
                    livingObjTrain.Add(livingObj[listPosLivObj[count]]);
                }
                count++;
            }
        }


        for (int j = 0; j < numObjPerList; j++)
        {
            livingObjList1.Add(livingObjList[0, j]);
            livingObjList2.Add(livingObjList[1, j]);
            livingObjList3.Add(livingObjList[2, j]);
            livingObjList4.Add(livingObjList[3, j]);
            livingObjList5.Add(livingObjList[4, j]);

        }


    }

    void OnApplicationQuit()
    {
        if (isRaining)
        {
            isRaining = false;
            SetRain();
        }
        Debug.Log("Application ending after " + Time.time/60f + " minutos.");
    }

    public void UpdateUIText(int imgNum)
    {
        float correntTime = Time.time/60f;
        if (isBGImage)
        {
            textUI.text = "Day: " + dayCout + "\n" +
                      "Hour: " + (Mathf.FloorToInt(timeOfDay) < 10 ? "0" : "") + Mathf.FloorToInt(timeOfDay) + ":" +
                                 (Mathf.FloorToInt((timeOfDay % 1) * 60f) < 10 ? "0" : "") + Mathf.FloorToInt((timeOfDay % 1) * 60f) + "\n" +
                      "Img: " + imgNum + "/" + (trainImages + testImages + validImages) + " (bg)" + "\n" +
                      "Night: " + isNight + "\n" +
                      "Rain: " + isRaining + "\n" +
                                (Mathf.FloorToInt(correntTime / 60f) < 10 ? "0" : "") + Mathf.FloorToInt(correntTime / 60f) + ":" +
                                (Mathf.FloorToInt(correntTime) < 10 ? "0" : "") + Mathf.FloorToInt(correntTime) + ":" +
                                (Mathf.FloorToInt((correntTime % 1) * 60f) < 10 ? "0" : "") + Mathf.FloorToInt((correntTime % 1) * 60f);
        }
        else
        {
            textUI.text = "Day: " + dayCout + "\n" +
                      "Hour: " + (Mathf.FloorToInt(timeOfDay) < 10 ? "0" : "") + Mathf.FloorToInt(timeOfDay) + ":" +
                                 (Mathf.FloorToInt((timeOfDay % 1) * 60f) < 10 ? "0" : "") + Mathf.FloorToInt((timeOfDay % 1) * 60f) + "\n" +
                      "Img: " + imgNum + "/" + (trainImages + testImages + validImages) + "\n" +
                      "Night: " + isNight + "\n" +
                      "Rain: " + isRaining + "\n" +
                                (Mathf.FloorToInt(correntTime / 60f) < 10 ? "0" : "") + Mathf.FloorToInt(correntTime / 60f) + ":" +
                                (Mathf.FloorToInt(correntTime) < 10 ? "0" : "") + Mathf.FloorToInt(correntTime) + ":" +
                                (Mathf.FloorToInt((correntTime % 1) * 60f) < 10 ? "0" : "") + Mathf.FloorToInt((correntTime % 1) * 60f);
        }
        
    }
    public void SetRain()
    {
        if (setRaining)
        {
            isRaining = !isRaining;
            setRaining = false;
        }
        if (isRaining)
        {
            sun.GetComponent<Light>().intensity = Random.Range(minSunLuxRain, maxSunLuxRain);
            var emission = rain.emission;
            emission.rateOverTime = Random.Range(rainIntensityMin, rainIntensityMax);
            var rainVelo = rain.velocityOverLifetime;
            rainVelo.x = Random.Range(-rainWindVariance, rainWindVariance);
            rainVelo.z = Random.Range(-rainWindVariance, rainWindVariance);
            rain.Play();
            fog.meanFreePath.value = Random.Range(minRainFog, maxRainFog);

            //foreach (Material mat in materials)
            // {
            //    mat.SetFloat("_Smoothness",rainSmoothness);
            // }

            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetFloat("_Smoothness", rainSmoothness);
            }

            // terrain.legacyShininess = 0.6f;


            //var terrainData = terrain.terrainData;


            //Terrain Smoothness

            foreach (TerrainLayer layer in layers)
            {
                layer.smoothness = 0.7f;
            }


        }
        else
        {
            sun.GetComponent<Light>().intensity = sunLux;
            fog.meanFreePath.value = initFog;
            rain.Stop();

            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetFloat("_Smoothness", matSmootInit[i]);
            }

            //Terrain Smoothness
            foreach (TerrainLayer layer in layers)
            {
                layer.smoothness = 0.0f;
            }
        }
    }
    public void InstanteateWagon() {
        var obj = Instantiate(wagon, spawPoint, Quaternion.identity);
        obj.name = "Wagon";
    }
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
