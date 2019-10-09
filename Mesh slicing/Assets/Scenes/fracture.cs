using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Diagnostics;
using System;
using EzySlice;

public class fracture : MonoBehaviour 
{

    
    //ax+by+cz+d=ax+b-y

    //ax+by+cz+d=0 plane formula
    //ax+b-y=0
    //a = transform.up.x;
    //b = transform.up.y;
    //c = transform.up.z;
    //d = -Vector3.Dot(transform.up, transform.position);

    //var normal = Quaternion.Euler(yourAngle) * Vector3.up; //Presuming the plane faces upwards
    //a = normal.x; //etcetera

    Vector3[] vertices;
   
    List<Vector3> verticesList = new List<Vector3>();
    List<Vector3> verticesList2= new List<Vector3>();

    public List<Transform> meshes;
    public GameObject mesh2;
    UnityEngine.Plane newPlane;
    List<int> trisList = new List<int>(5000);
    List<int> trisList2 = new List<int>(5000);
    void Start() {
   


    }
    void OnTriggerStay(Collider other)
    {
        if(!meshes.Contains(other.transform)){
            meshes.Add(other.transform);
        }
    }
    void OnTriggerExit(Collider other)
    {
        meshes.Remove(other.transform);
    }
  
    public Dictionary<Vector3, int> pairs = new Dictionary<Vector3, int>(5000);
    public Dictionary<Vector3, int> pairs2 = new Dictionary<Vector3, int>(5000);
    public bool useExistingVertixes;
    void Update() {


        if (Input.GetKeyDown(KeyCode.S))
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            FirstSliceMethod();         
            s.Stop();
            UnityEngine.Debug.Log(s.ElapsedMilliseconds);
        
        }       
    }
    List<KeyValuePair<int, float>> m;
    Vector3 point;

    //bool firstList;
    List<Vector2> uvs = new List<Vector2>(5000);
    List<Vector2> uvs2 = new List<Vector2>(5000);
    List<Vector3> points = new List<Vector3>(5000);
    List<Vector3> points2 = new List<Vector3>(5000);
    int z = 0, z2 = 0;
    void CheckContains(bool firstList) {
        if(firstList) {
            if (!useExistingVertixes)
            {
                if (!pairs.ContainsKey(point))
                {
                    uvs.Add(uvPoint);
                    pairs.Add(point, z);
                    trisList.Add(z);
                    z++;
                }
                else
                {
                    //  Debug.Log(pairs.e)
                    trisList.Add(pairs[point]);
                }

            }
            else
            {
                points.Add(point);
                uvs.Add(uvPoint);
                trisList.Add(z);
                z++;
            }
           

        }
        else {

            if (!useExistingVertixes)
            {
                if (!pairs2.ContainsKey(point))
                {
                    uvs2.Add(uvPoint);
                    pairs2.Add(point, z2);
                    trisList2.Add(z2);
                    z2++;
                }
                else
                {
                    //  Debug.Log(pairs.e)
                    trisList2.Add(pairs2[point]);
                }
            }
            else
            {
                points2.Add(point);
                uvs2.Add(uvPoint);
                trisList2.Add(z2);
                z2++;
            }
         
        
           
        }
    }
    void FillFacePointsCheckContains(bool firstList) {
        if (firstList)
        {
            if (!fillFacePoints.ContainsKey(point))
            {
                fillFacePoints.Add(point, 0);
            }            
        }
        else {
            if (!fillFacePoints2.ContainsKey(point))
            {
                fillFacePoints2.Add(point, 0);            
            }   
        }
    }
    Dictionary<Vector3,int> fillFacePoints = new Dictionary<Vector3,int>(5000);
    Dictionary<Vector3,int> fillFacePoints2 = new Dictionary<Vector3,int>(5000);
    Vector2[] initialUVs;
    Vector2 uvPoint;
    void FirstSliceMethod() {
        newPlane = new UnityEngine.Plane(transform.rotation * Vector3.up, -Vector3.Dot(transform.up, transform.position));
        foreach (var meshTransform in meshes)
        {

            z = z2 = 0;
    
            vertices = meshTransform.GetComponent<MeshFilter>().mesh.vertices;
         
            int[] triangles = meshTransform.GetComponent<MeshFilter>().mesh.triangles;
            trisList = new List<int>();
            trisList2 = new List<int>();
    
       


            Vector3 pointB = new Vector3(); //point bellow
            Vector3 pointA1 = new Vector3(); //point above 1
            Vector3 pointA2 = new Vector3();//point above 2


            Vector3 pointB1 = new Vector3(); //point bellow 1
            Vector3 pointB2 = new Vector3(); //point bellow 2
            Vector3 pointA = new Vector3();//point above 

            Vector3 transformPosition =transform.position;
            initialUVs = meshTransform.GetComponent<MeshFilter>().mesh.uv;
            uvs = new List<Vector2>();
            uvs2 = new List<Vector2>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var i0 = meshTransform.TransformPoint(vertices[triangles[i]]);
                var i1 = meshTransform.TransformPoint(vertices[triangles[i + 1]]);
                var i2 = meshTransform.TransformPoint(vertices[triangles[i + 2]]);
                
                var uv0= meshTransform.TransformPoint(initialUVs[triangles[i]]);
                var uv1 = meshTransform.TransformPoint(initialUVs[triangles[i+1]]);
                var uv2 = meshTransform.TransformPoint(initialUVs[triangles[i+2]]);

                float dist0 = newPlane.GetDistanceToPoint(i0);
                float dist1 = newPlane.GetDistanceToPoint(i1);
                float dist2 = newPlane.GetDistanceToPoint(i2);

                if (dist0 <= 0 &&
                    dist1 <= 0 &&
                    dist2 <= 0)
                {
                    uvPoint = uv0;
                    point = i0;
                    CheckContains(true);
                    uvPoint = uv1;
                    point = i1;
                    CheckContains(  true);
                    uvPoint = uv2;
                    point = i2;
                    CheckContains(  true);
                    
                }

                if (dist0 > 0 && 
                    dist1 > 0 && 
                    dist2 > 0)
                {
                    //continue;
                    uvPoint = uv0;
                    point = i0;
                    CheckContains(  false);
                    uvPoint = uv1;
                    point = i1;
                   CheckContains(  false);
                    uvPoint = uv2;
                    point = i2;
                    CheckContains(  false);
                }

                if (dist0 > 0 &&
                    dist1 <= 0 &&
                    dist2 <= 0)
                {

                    var LineForward = (i0 - i2) / (i0 - i2).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                    Vector3 p = i0 + LineForward * t;


                    uvPoint = p;
                    point = p;
                    CheckContains(   true);
                    //uvPoint= uv1;
                    point = i1;
                    CheckContains(   true);
                    //uvPoint= uv2;
                    point = i2;
                    CheckContains(  true);


                    var LineForward2 = (i0 - i1) / (i0 - i1).magnitude;
                    float t2 = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    Vector3 p2 = i0 + LineForward2 * t2;

                    //uvPoint= p;
                    point = p;
                    CheckContains(  true);
                    //uvPoint= p2;
                    point = p2;
                    CheckContains(  true);
                    //uvPoint= uv1;
                    point = i1;
                    CheckContains(   true);


                    point = p;
                    FillFacePointsCheckContains(true);
                    point = p2;
                    FillFacePointsCheckContains(true);
                }
                if (dist0 <= 0 &&
                dist1 > 0 &&
                dist2 > 0)
                {
                    // continue;
                    var LineForward = (i0 - i2) / (i0 - i2).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                     var p = i0 + LineForward * t;

                    //uvPoint= p;
                    point = p;
                    CheckContains( false);
                    //uvPoint= uv1;
                    point = i1;
                    CheckContains( false);
                    //uvPoint= uv2;
                    point = i2;
                    CheckContains( false);


                    var LineForward2 = (i0 - i1) / (i0- i1).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    var p2 =  i0 + LineForward2 * t2;

                    //uvPoint= p;
                    point = p;
                    CheckContains(  false);
                    //uvPoint= p2;
                    point = p2;
                    CheckContains( false);
                    //uvPoint= uv1;
                    point = i1;
                    CheckContains( false);

                    point = p;
                    FillFacePointsCheckContains(false);
                    point = p2;
                    FillFacePointsCheckContains(false);
                }
                #region one point bellow
                if ((dist0 > 0 &&
                dist1 <= 0 &&
                dist2 > 0) ||

                (dist0 <= 0 &&
                dist1 > 0 &&
                dist2 > 0) ||

                (dist0 > 0 &&
                dist1 > 0 &&
                dist2 <= 0))
                {
                   

                    if (dist1 <= 0)
                    {
                        pointB = i1;
                        pointA1 = i0;
                        pointA2 = i2;
                    }
                    if (dist0 <= 0)
                    {
                        pointB = i0;
                        pointA1 = i1;
                        pointA2 = i2;
                    }
                    if (dist2 <= 0)
                    {
                        pointB = i2;
                        pointA1 = i0;
                        pointA2 = i1;
                    }
                    var LineForward = (pointB - pointA1) / (pointB - pointA1).sqrMagnitude;                  
                    float t = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                    var p = pointB + (LineForward * t);

                    var LineForward2 = (pointB - pointA2) / (pointB - pointA2).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    var p2 = pointB + LineForward2 * t2;
                    if (dist0 <= 0)
                    {
                        //uvPoint= pointB;
                        point = pointB;
                       CheckContains( true);
                        //uvPoint= p;
                        point = p;
                       CheckContains( true);
                        //uvPoint= p2;
                        point = p2;
                       CheckContains( true);

                    }
                    if (dist1 <= 0)
                    {
                        //uvPoint= p;
                        point = p;
                       CheckContains( true);
                        //uvPoint= pointB;
                        point = pointB;
                       CheckContains( true);
                        //uvPoint= p2;
                        point = p2;
                       CheckContains( true);
                    }
                    if (dist2 <= 0)
                    {
                        //uvPoint= pointB;
                        point = pointB;
                       CheckContains( true);
                        //uvPoint= p;
                        point = p;
                       CheckContains( true);
                        //uvPoint= p2;
                        point = p2;
                       CheckContains( true);
                    }

                    point = p;
                    FillFacePointsCheckContains(true);
                    point = p2;
                    FillFacePointsCheckContains(true);
                }
                #endregion
                #region one point above


                if ((dist0 <= 0 &&
                dist1 > 0 &&
                dist2 <= 0) ||

                (dist0 > 0 &&
                dist1 <= 0 &&
                dist2 <= 0) ||

                (dist0 <= 0 &&
                dist1 <= 0 &&
                dist2 > 0))
                {
               

                    if (dist1 > 0)
                    {
                        pointB = i1;
                        pointA1 = i0;
                        pointA2 = i2;
                    }
                    if (dist0 > 0)
                    {
                        pointB = i0;
                        pointA1 = i1;
                        pointA2 = i2;
                    }
                    if (dist2 > 0)
                    {
                        pointB = i2;
                        pointA1 = i0;
                        pointA2 = i1;
                    }
                    var LineForward = (pointB - pointA1) / (pointB - pointA1).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                    var p = pointB + LineForward * t;

                    var LineForward2 = (pointB - pointA2) / (pointB - pointA2).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    var p2 = pointB + LineForward2 * t2;



                    if (dist0 > 0)
                    {
                        //uvPoint= pointB;
                        point = pointB;
                        CheckContains( false);
                        //uvPoint= p;
                        point = p;
                        CheckContains(  false);
                        //uvPoint= p2;
                        point = p2;
                        CheckContains( false);

                    }
                    if (dist1 > 0)
                    {
                        //uvPoint= p;
                        point = p;
                        CheckContains( false);
                        //uvPoint= pointB;
                        point = pointB;
                        CheckContains( false);
                        //uvPoint= p2;
                        point = p2;
                        CheckContains( false);
                    
                    }
                    if (dist2 > 0)
                    {
                        uvPoint = pointB;
                        point = pointB;
                        CheckContains( false);
                        uvPoint = p;
                        point = p;
                        CheckContains( false);
                        uvPoint = p2;
                        point = p2;
                        CheckContains( false);
                    
                    }
                    point = p;
                    FillFacePointsCheckContains(false);
                    point = p2;
                    FillFacePointsCheckContains(false);
                }
                #endregion

                #region two points bellow
                if (
                (dist0 > 0 &&
               dist1 <= 0 &&
               dist2 <= 0) ||

               (dist0 <= 0 &&
               dist1 > 0 &&
               dist2 <= 0) ||

               (dist0 <= 0 &&
               dist1 <= 0 &&
               dist2 > 0)
               )
                {



                    if (dist0 > 0)
                    {
                        pointB1 = i1;
                        pointB2 = i2;
                        pointA = i0;
                    }
                    if (dist1 > 0)
                    {
                        pointB1 = i0;
                        pointB2 = i2;
                        pointA = i1;
                    }
                    if (dist2 > 0)
                    {
                        pointB1 = i0;
                        pointB2 = i1;
                        pointA = i2;
                    }

                    var LineForward = (pointB1 - pointA) / (pointB1 - pointA).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - pointB1, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                    var p = pointB1 + LineForward * t;

                    var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB2, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    var p2 = pointB2 + LineForward2 * t2;



                    if (dist0 > 0)
                    {
                        //verticesList.Add(p);
                        //verticesList.Add(pointB1);
                        //verticesList.Add(p2);

                        //verticesList.Add(pointB1);
                        //verticesList.Add(pointB1);
                        //verticesList.Add(p2);
                    }
                    if (dist1 > 0)
                    {
                        uvPoint = p;
                        point = p;
                       CheckContains( true);
                        uvPoint = p2;
                        point = p2;
                        CheckContains(  true);
                        uvPoint = pointB1;
                        point = pointB1;
                       CheckContains( true);

                        uvPoint = pointB1;
                        point = pointB1;
                       CheckContains( true);
                        uvPoint = p2;
                        point = p2;
                       CheckContains( true);
                        uvPoint = pointB2;
                        point = pointB2;
                       CheckContains( true);
                     
                    }
                    if (dist2 > 0)
                    {
                        uvPoint = p;
                        point = p;
                       CheckContains( true);
                        uvPoint = pointB1;
                        point = pointB1;
                       CheckContains( true);
                        uvPoint = p2;
                        point = p2;
                       CheckContains( true);

                        uvPoint = pointB1;
                        point = pointB1;
                       CheckContains( true);
                        uvPoint = pointB2;
                        point = pointB2;
                       CheckContains( true);
                        uvPoint = p2;
                        point = p2;
                       CheckContains( true);
                    }

                    point = p;
                    FillFacePointsCheckContains(true);
                    point = p2;
                    FillFacePointsCheckContains(true);
                }
                #endregion

                #region two above bellow
                if (
                (dist0 <= 0 &&
               dist1 > 0 &&
               dist2 > 0) ||

               (dist0 > 0 &&
               dist1 <= 0 &&
               dist2 > 0) ||

               (dist0 > 0 &&
               dist1 > 0 &&
               dist2 <= 0)
               )
                {

                    //  continue;
               

                    if (dist0 <= 0)
                    {
                        pointB1 = i1;
                        pointB2 = i2;
                        pointA = i0;
                    }
                    if (dist1 <= 0)
                    {
                        pointB1 = i0;
                        pointB2 = i2;
                        pointA = i1;
                    }
                    if (dist2 <= 0)
                    {
                        pointB1 = i0;
                        pointB2 = i1;
                        pointA = i2;
                    }

                    var LineForward = (pointB1 - pointA) / (pointB1 - pointA).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - pointB1, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                    var p = pointB1 + LineForward * t;

                    var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB2, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    var p2 = pointB2 + LineForward2 * t2;



                    if (dist0 <= 0)
                    {
                        //verticesList2.Add(p);
                        //verticesList2.Add(pointB2);
                        //verticesList2.Add(p2);

                        //verticesList2.Add(p2);
                        //verticesList2.Add(pointB1);
                        //verticesList2.Add(pointB2);

                        //  z2 = AddTris2(z2, 1);
                    }
                    if (dist1 <= 0)
                    {
                        uvPoint = p;
                        point = p;
                        CheckContains( false);
                        uvPoint = p2;
                        point = p2;
                        CheckContains( false);
                        uvPoint = pointB1;
                        point = pointB1;
                        CheckContains( false);

                        uvPoint = pointB1;
                        point = pointB1;
                        CheckContains( false);
                        uvPoint = p2;
                        point = p2;
                        CheckContains( false);
                        uvPoint = pointB2;
                        point = pointB2;
                        CheckContains( false);
                                             
                    }
                    if (dist2 <= 0)
                    {
                        uvPoint = p;
                        point = p;
                        CheckContains( false);
                        uvPoint = pointB1;
                        point = pointB1;
                        CheckContains( false);
                        uvPoint = p2;
                        point = p2;
                        CheckContains( false);

                        uvPoint = pointB1;
                        point = pointB1;
                        CheckContains( false);
                        uvPoint = pointB2;
                        point = pointB2;
                        CheckContains( false);
                        uvPoint = p2;
                        point = p2;
                        CheckContains( false);
                    }
                    point = p;
                    FillFacePointsCheckContains(false);
                    point = p2;
                    FillFacePointsCheckContains(false);

                }
                #endregion
              
            }
            if (fillCutout)
            {
                FillVoid(fillFacePoints, true);
                FillVoid(fillFacePoints2, false);

            }

            //UnityEngine.Debug.Log(vertices.Length + " " + triangles.Length);
            //UnityEngine.Debug.Log(points.Count + " " + trisList.Count);

            CreateObjects(meshTransform);


        }
    }

    
 
    public bool fillCutout = false;
    int AddTris(int z,int times) {
        for (int i = 0; i < times; i++)
        {
            
            trisList.Add(z);
            trisList.Add(z + 1);
            trisList.Add(z + 2);
            z += 3;
        }      
        return z;
    }
    int AddTris2(int z, int times)
    {
        for (int i = 0; i < times; i++)
        {
          
            trisList2.Add(z);
            trisList2.Add(z + 1);
            trisList2.Add(z + 2);
            z += 3;            
        }
        return z;
    }
    void Fill(Dictionary<Vector3,int> fillFacePoints, bool firstObj)
    {
        Vector3 centerPoint = new Vector3();
        for (int i = 0; i < fillFacePoints.Count; i++)
        {
            centerPoint += fillFacePoints.ElementAt(i).Key;
        }
        centerPoint /= fillFacePoints.Count;
      
       

        Dictionary<int, float> positions = new Dictionary<int, float>();
        Vector3 one = new Vector3(1, 0, 0);
        for (int i = 0; i < fillFacePoints.Count; i++)
        {
            positions.Add(i, Vector3.SignedAngle(centerPoint, fillFacePoints.ElementAt(i).Key, transform.rotation*transform.up));
        }
        m= positions.OrderBy(x => x.Value).ToList();

        for (int i = 0; i < m.Count-1; i++)
        {
            int a = m.ElementAt(i).Key;
            int a2 = m.ElementAt(i + 1).Key;
            Vector3 b = fillFacePoints.ElementAt(a).Key;
            Vector3 b2 = fillFacePoints.ElementAt(a2).Key;

            if (!b.Equals(b2))
            {
                if(firstObj)
                {
                    uvPoint = centerPoint;
                    point = centerPoint;
                    CheckContains(firstObj);
                    uvPoint = b;
                    point = b;
                    CheckContains(firstObj);
                    uvPoint = b2;
                    point = b2;
                    CheckContains(firstObj);
                }
                else
                {
                    uvPoint = centerPoint;
                    point = centerPoint;
                    CheckContains(false);
                    uvPoint = b2;
                    point = b2;
                    CheckContains(false);
                    uvPoint = b;
                    point = b;
                    CheckContains(false);
                  
                }

            }
        }
        int aa = m.ElementAt(m.Count - 1).Key;
        int aa2 = m.ElementAt(0).Key;
        Vector3 bb = fillFacePoints.ElementAt(aa).Key;
        Vector3 bb2 = fillFacePoints.ElementAt(aa2).Key;

        if (!bb.Equals(bb2))
        {
            if (firstObj)
            {
                uvPoint = centerPoint;
                point = centerPoint;
                CheckContains(firstObj);
                uvPoint = bb;
                point = bb;
                CheckContains(firstObj);
                uvPoint = bb2;
                point = bb2;
                CheckContains(firstObj);
            }
            else
            {
                uvPoint = centerPoint;
                point = centerPoint;
                CheckContains(false);
                uvPoint = bb2;
                point = bb2;
                CheckContains(false);
                uvPoint = bb;
                point = bb;
                CheckContains(false);

            }

        }
    }

    void CreateObjects(Transform parent) {        
        for (int i = 0; i < 2; i++)
        {
            CreateObject(parent.gameObject, i == 0);
        }
    }
    void CreateObject(GameObject initialObject, bool first)
    {
        verticesList = new List<Vector3>();
        verticesList2 = new List<Vector3>();
        Vector3[] finalVertices;
        int[] finalTris;
        GameObject go;
        Vector2[] finalUVs;
        if (first)
        {
            finalTris = trisList.ToArray();
            finalUVs = uvs.ToArray();
            go = initialObject;
            if (useExistingVertixes)
            {
                finalVertices = points.ToArray();
            }
            else
            {
                foreach (var item in pairs)
                {
                    verticesList.Add(item.Key);
                }
                finalVertices = verticesList.ToArray();
            }
        }
        else
        {
            finalTris = trisList2.ToArray();
            finalUVs = uvs2.ToArray();
            go = Instantiate(mesh2, initialObject.transform.position, Quaternion.identity);
            if (useExistingVertixes)
            {
                finalVertices = points2.ToArray();
            }
            else
            {
                foreach (var item in pairs2)
                {
                    verticesList2.Add(item.Key);
                }
                finalVertices = verticesList2.ToArray();
            }
        }

        var mesh = go.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.ClearBlendShapes();

     
        mesh.vertices = finalVertices;
        mesh.triangles = finalTris;

        mesh.uv = finalUVs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        Destroy(go.GetComponent<MeshCollider>());
        var meshCol = go.AddComponent<MeshCollider>();
        meshCol.convex = true;
    }
    Dictionary<float, Vector3> dic = new Dictionary<float, Vector3>(5000);
    Dictionary<float, Vector3> dic2 = new Dictionary<float, Vector3>(5000);


    public void FillVoid(Dictionary<Vector3, int> fillFacePoints, bool firstObj)
    {
        Vector3 centerPoint = new Vector3();
        Vector3 currentPoint = new Vector3();

        
        for (int i = 0; i < fillFacePoints.Count; i++)
        {
            currentPoint = fillFacePoints.ElementAt(i).Key;
            centerPoint += currentPoint;
            float angle = Mathf.Atan2(currentPoint.x, currentPoint.z);
            if (firstObj)
            {

                if (!dic.ContainsKey(angle))
                {
                    dic.Add(angle, currentPoint);
                }
            }
            else
            {

                if (!dic2.ContainsKey(angle))
                {
                    dic2.Add(angle, currentPoint);
                }
            }
        }

        if (firstObj)
        {

            dic = dic.OrderBy(key => key.Key).ToDictionary(x => x.Key, x => x.Value);

        }

        else
        {

            dic2 = dic2.OrderBy(key => key.Key).ToDictionary(x => x.Key, x => x.Value);

        }

        centerPoint /= fillFacePoints.Count;


        //9gag dic.length
        if (firstObj)
        {
            for (int i = 0; i < dic.Count - 1; i++)
            {
                point = dic.Values.ElementAt(i + 1);
                CheckContains(firstObj);
                point = centerPoint;
                CheckContains(firstObj);
                point = dic.Values.ElementAt(i);
                CheckContains(firstObj);
            }

            point = dic.Values.ElementAt(0);
            CheckContains(firstObj);
            point = centerPoint;
            CheckContains(firstObj);
            point = dic.Values.ElementAt(dic.Count - 1);
            CheckContains(firstObj);
        }
        else
        {
            for (int i = 0; i < dic2.Count - 1; i++)
            {
                point = dic2.Values.ElementAt(i + 1);
                CheckContains(firstObj);
                point = centerPoint;
                CheckContains(firstObj);
                point = dic2.Values.ElementAt(i);
                CheckContains(firstObj);
            }

            point = dic2.Values.ElementAt(0);
            CheckContains(firstObj);
            point = centerPoint;
            CheckContains(firstObj);
            point = dic2.Values.ElementAt(dic.Count - 1);
            CheckContains(firstObj);
        }



    }
}
