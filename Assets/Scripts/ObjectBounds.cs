using System.Collections;
using UnityEngine;

public class ObjectBounds : MonoBehaviour {


    public int objectType = 0;
    public bool isFilter = false;
    [HideInInspector] public bool isVisible = true;

    Camera cam;
    Rect currBox = new Rect();
    Rect photoRect = new Rect();


    void Start() {
        cam = Camera.main;

    }

    private void Update()
    {

    }

    void OnGUI() {

        if (!GameManager.instance.showBoundBox || isFilter || !isVisible) {
            return;
        }

        //resize box for GUI coords
        Vector2 min = new Vector2(currBox.xMin, currBox.yMin);
        Vector2 max = new Vector2(currBox.xMax, currBox.yMax);
        Rect rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

        //Render the box
        GUI.skin = GameManager.instance.guiSkin;
        GUI.Box(rect, "");
    }

    public Rect GetBounds() {
        return photoRect;
    }

    //*
    public void UpdateBounds() {

        if (!gameObject.activeSelf) {
            return;
        }

        Vector3[] verts = MeshUtility.GetMesh(transform).vertices;

        //create new box
        currBox = new Rect
        {
            xMin = 10000,
            xMax = -1,
            yMin = 10000,
            yMax = -1
        };
        //convert to world point, then screen space
        for (int i = 0; i < verts.Length; i+= 1+ verts.Length/100000)
        {
            verts[i] = cam.WorldToScreenPoint(transform.TransformPoint(verts[i]));

            //find min and max screen space values
            currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
            currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
            currBox.yMin = currBox.yMin < verts[i].y ? currBox.yMin : verts[i].y;
            currBox.yMax = currBox.yMax > verts[i].y ? currBox.yMax : verts[i].y;
        }


        if (currBox.yMax < 0)
            Destroy(gameObject);

        if (!isFilter)
        {
            isVisible = true;
            //top cut
            if (currBox.yMax > Screen.height && currBox.yMin < Screen.height)
            {
                currBox.yMax = Screen.height;

                currBox.xMin = 10000;
                currBox.xMax = -1;
                for (int i = 0; i < verts.Length; i += 1 + verts.Length / 100000)
                {
                    if (verts[i].y < Screen.height)
                    {
                        currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
                        currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
                    }
                }
            }
                
            //botton cut
            if (currBox.yMin < 0)
            {
                currBox.yMin = 0;
                currBox.xMin = 10000;
                currBox.xMax = -1;
                for (int i = 0; i < verts.Length; i += 1 + verts.Length / 100000)
                {
                    if(verts[i].y > 0)
                    {
                        currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
                        currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
                    }                        
                }
            } 
            
            if (currBox.yMin > Screen.height)
                isVisible = false;
            else
            {
                //filtering obj behind wagon walls
                foreach (ObjectBounds bounds in FindObjectsByType<ObjectBounds>(FindObjectsSortMode.None))
                {
                    if (bounds.isFilter)
                    {
                        bounds.UpdateBounds();
                        Rect filterBounds = new Rect(bounds.GetBounds());
                        if (filterBounds.y < Screen.height)
                        {
                            if (filterBounds.yMin < currBox.yMin && filterBounds.yMax > currBox.yMax)
                            {
                                isVisible = false;
                            }
                            if (filterBounds.yMin < currBox.yMin && filterBounds.yMax > currBox.yMin && filterBounds.yMax < currBox.yMax)
                            {
                                currBox.yMin = filterBounds.yMax;
                                currBox.xMin = 10000;
                                currBox.xMax = -1;
                                for (int i = 0; i < verts.Length; i += 1 + verts.Length / 100000)
                                {
                                    if (verts[i].y > filterBounds.yMax)
                                    {
                                        currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
                                        currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }       

        photoRect = currBox;

        currBox.yMin = Screen.height - currBox.yMin;
        currBox.yMax = Screen.height - currBox.yMax;
    }
    //*/

    /* 
    public void UpdateBounds() {

        if (!gameObject.activeSelf) {
            return;
        }

        Vector3[] verts = MeshUtility.GetMesh(transform).vertices;

        //convert to world point, then screen space
        for (int i = 0; i < verts.Length; i++) {
            verts[i] = cam.WorldToScreenPoint(transform.TransformPoint(verts[i]));
            //make sure we dont go off screen

            if (verts[i].x < 0 || verts[i].x > Screen.width || verts[i].y < 0 || verts[i].y > Screen.height) {
                verts[i] = verts[0];
            }
            
        }

        //create new box
        currBox = new Rect {
            xMin = verts[0].x,
            xMax = verts[0].x,
            yMin = verts[0].y,
            yMax = verts[0].y
        };

        //find min and max screen space values
        for (int i = 0; i < verts.Length; i++) {
            currBox.xMin = currBox.xMin < verts[i].x ? currBox.xMin : verts[i].x;
            currBox.xMax = currBox.xMax > verts[i].x ? currBox.xMax : verts[i].x;
            currBox.yMin = currBox.yMin < verts[i].y ? currBox.yMin : verts[i].y;
            currBox.yMax = currBox.yMax > verts[i].y ? currBox.yMax : verts[i].y;
        }

        photoRect = currBox;

        if (!isFilter)
        {
            // botton cut filter
            if (photoRect.width > 0 && photoRect.height > 0 && photoRect.y < 0 && photoRect.height + photoRect.y > 0)
            {
                photoRect.height = photoRect.height + photoRect.y;
                photoRect.y = 0;
            }

            //top cut filter
            if (photoRect.width > 0 && photoRect.height > 0 && photoRect.yMax > Screen.height && photoRect.y < Screen.height)
            {
                photoRect.height = photoRect.height - (photoRect.yMax - Screen.height);
                photoRect.yMax = Screen.height;
            }
        }

        

        //filtering obj behind wagon walls
        if (!isFilter && photoRect.x >= 0 && photoRect.y >= 0 && photoRect.width > 0 && photoRect.height > 0)
        {
            Rect filterBounds = new Rect();
            foreach (ObjectBounds bounds in FindObjectsByType<ObjectBounds>(FindObjectsSortMode.None))
            {
                if (bounds.isFilter)
                {
                    bounds.UpdateBounds();
                    Rect objBounds = new Rect(bounds.GetBounds());
                    if (objBounds.yMax > 0 && objBounds.y < Screen.height )
                    {
                        filterBounds = bounds.GetBounds();

                        if(filterBounds.yMin < photoRect.yMin && filterBounds.yMax > photoRect.yMax)
                        {
                            photoRect.yMin = 0;
                            photoRect.yMax = 0;
                        }
                        if (filterBounds.yMin < photoRect.yMin && filterBounds.yMax > photoRect.yMin && filterBounds.yMax < photoRect.yMax)
                        {
                            photoRect.yMin = filterBounds.yMax;
                        }
                    }
                }
            }
        }

        

        currBox = photoRect;

        currBox.yMin = Screen.height - currBox.yMin;
        currBox.yMax = Screen.height - currBox.yMax;
    }
    //*/
}
