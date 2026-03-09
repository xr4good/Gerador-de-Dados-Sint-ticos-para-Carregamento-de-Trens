using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    [SerializeField] private List<Transform> objPositions = new List<Transform>();
    
    private Rigidbody rb;
    private int noPos1, noPos2, pos;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        
        if(Random.value < GameManager.instance.objPorcent1 && !GameManager.instance.isBGImage)
        {
            pos = (int)(Random.value * (objPositions.Count));
            SpawObject(pos);
            noPos1 = pos;
            if (Random.value < GameManager.instance.objPorcent2)
            {
                while(noPos1 == pos)
                {
                    pos = (int)(Random.value * (objPositions.Count));
                }
                SpawObject(pos);
                noPos2 = pos;
                if (Random.value < GameManager.instance.objPorcent3)
                {
                    while (noPos1 == pos || noPos2 == pos)
                    {
                        pos = (int)(Random.value * (objPositions.Count));
                    }
                    SpawObject(pos);
                }
            }
        }

        
    }

    void Update()
    {
        rb.velocity = new Vector3(-GameManager.instance.wagonSpeed *0.13f, 0, 0);
    }

    void SpawObject(int pos)
    {

        if(Random.value < GameManager.instance.porcentageOfObjects)
        {
            if (GameManager.instance.isTrainImage)
            {
                InstanObj(GameManager.instance.objTrain, pos);
            }
            if (GameManager.instance.isTestImage)
            {
                InstanObj(GameManager.instance.objTest, pos);
            }
            if (GameManager.instance.useKFoldSeparation)
            {
                switch (GameManager.instance.kFoldFolderID)
                {
                    case 1:
                        InstanObj(GameManager.instance.objectsList1, pos);
                        break;
                    case 2:
                        InstanObj(GameManager.instance.objectsList2, pos);
                        break;
                    case 3:
                        InstanObj(GameManager.instance.objectsList3, pos);
                        break;
                    case 4:
                        InstanObj(GameManager.instance.objectsList4, pos);
                        break;
                    case 5:
                        InstanObj(GameManager.instance.objectsList5, pos);
                        break;
                }
            }
        }
        else
        {
            if (GameManager.instance.isTrainImage)
            {
                InstanObj(GameManager.instance.livingObjTrain, pos);
            }
            if (GameManager.instance.isTestImage)
            {
                InstanObj(GameManager.instance.livingObjTest, pos);
            }
            if (GameManager.instance.useKFoldSeparation)
            {
                switch (GameManager.instance.kFoldFolderID)
                {
                    case 1:
                        InstanObj(GameManager.instance.livingObjList1, pos);
                        break;
                    case 2:
                        InstanObj(GameManager.instance.livingObjList2, pos);
                        break;
                    case 3:
                        InstanObj(GameManager.instance.livingObjList3, pos);
                        break;
                    case 4:
                        InstanObj(GameManager.instance.livingObjList4, pos);
                        break;
                    case 5:
                        InstanObj(GameManager.instance.livingObjList5, pos);
                        break;
                }
            }
        }           
    }

    void InstanObj(List<GameObject> objList,int pos) 
    {
        int objPos = (int)(Random.value * (objList.Count));
        if (objPos == objList.Count)
        {
            objPos = objList.Count - 1;
        }

        var objInstance = Instantiate(objList[objPos], objPositions[pos].position, Quaternion.identity);
        objInstance.transform.parent = this.gameObject.transform;
        objInstance.transform.eulerAngles = new Vector3(objInstance.transform.eulerAngles.x, Random.value * 360, objInstance.transform.eulerAngles.z);
    } 

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Spawn")
        {
            GameManager.instance.InstanteateWagon();
        }
        if(other.tag == "Destroy")
        {
            Destroy(gameObject);
        }
    }
}
