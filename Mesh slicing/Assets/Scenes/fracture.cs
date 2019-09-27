using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Diagnostics;
using System;

public struct FasterVector3: IEquatable<FasterVector3>
{
    public readonly float x;
    public readonly float y;
    public readonly float z;
   
    public FasterVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public FasterVector3(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
    public override bool Equals(object other)
    {
        if (!(other is FasterVector3))
            return false;
        FasterVector3 vector3 = (FasterVector3)other;
        if (this.x.Equals(vector3.x) && this.y.Equals(vector3.y))
            return this.z.Equals(vector3.z);
        return false;
    }
    public bool Equals(FasterVector3 vector3)
    {
        if (Mathf.Approximately(x, vector3.x) && Mathf.Approximately(y, vector3.y) && Mathf.Approximately(y, vector3.y) )           
            return true;
        return false;
    }
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
    }
    public bool Equals(FasterVector3 x, FasterVector3 y)
    {
        if (x.x.Equals(y.x) && x.y.Equals(y.y))
            return x.z.Equals(y.z);
        return false;
    }
    public static FasterVector3 operator +(FasterVector3 first, FasterVector3 second)
    {
        return new FasterVector3(first.x + second.x, first.y + second.y, first.z + second.z);
    }
    public static FasterVector3 operator -(FasterVector3 first, FasterVector3 second)
    {
        return new FasterVector3(first.x - second.x, first.y - second.y, first.z - second.z);
    }
    public static FasterVector3 operator /(FasterVector3 first, float second)
    {
        return new FasterVector3(first.x /second, first.y / second, first.z /second);
    }
    public static FasterVector3 operator *(FasterVector3 first, float second)
    {
        return new FasterVector3(first.x * second, first.y * second, first.z * second);
    }
    public float sqrMagnitude {
        get { return (x * x + y * y + z * z); }
    }
    public Vector3 vector3 {
        get { return new Vector3(x, y, z); }
    }

    public static Vector3[] ToArray(List<FasterVector3> list) {
        Vector3[] array = new Vector3[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i].vector3;
        }

        return array;
    }

}

public class fracture : MonoBehaviour 
{

    
    //ax+by+cz+d=ax+b-y

        //ax+by+cz+d=0 plane formula
        //ax+b-y=0
        //a = transform.up.x;
        //b = transform.up.y;
        //c = transform.up.z;
        //d = -FasterVector3.Dot(transform.up, transform.position);

        //var normal = Quaternion.Euler(yourAngle) * FasterVector3.up; //Presuming the plane faces upwards
        //a = normal.x; //etcetera

    public List<GameObject> cuttingPlanes;
    public GameObject sphere;
    Vector3[] vertices;
    [SerializeField]
   
    public List<FasterVector3> verticesList = new List<FasterVector3>();

    public List<FasterVector3> verticesList2= new List<FasterVector3>();

    public List<Transform> meshes;
    public GameObject mesh2;
    Plane newPlane;
    List<int> trisList = new List<int>();
    List<int> trisList2 = new List<int>();
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
    void OnDrawGizmos() {
        //foreach (var item in vertices)
        //{
        //    if(newPlane.GetDistanceToPoint(sphere.transform.TransformPoint( item)) <= 0) {
        //        Gizmos.DrawSphere(sphere.transform.TransformPoint(item), 0.005f);
        //    }
        //}
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(mid1.vector3, 0.005f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(mid2.vector3, 0.005f);
        Gizmos.color = Color.white;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p, 0.02f);
        Gizmos.color = Color.white;
    }
    public Dictionary<FasterVector3, int> pairs = new Dictionary<FasterVector3, int>();
    public Dictionary<FasterVector3, int> pairs2 = new Dictionary<FasterVector3, int>();
    public bool useExistingVertixes;
    int ii = 0;
    void Update() {


        if (Input.GetKeyDown(KeyCode.S))
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            FirstSliceMethod();
            s.Stop();
            UnityEngine.Debug.Log(s.ElapsedMilliseconds);
        
        }
        if(Input.GetKeyDown(KeyCode.N) && ii < m.Count - 1) {       
            
            //int a = m.ElementAt(ii).Key;
            //int a2 = m.ElementAt(ii + 1).Key;
            //FasterVector3 b = fillFacePoints.ElementAt(a).Key;
            //FasterVector3 b2 = fillFacePoints.ElementAt(a2).Key;

            //if(!b.Equals(b2)) {
            //    point = new FasterVector3(p);
            //    CheckContains(true);
            //    point = b;
            //    CheckContains(true);
            //    point = b2;
            //    CheckContains(true);
            //    CreateMeshes(meshes[0].GetComponent<MeshFilter>().mesh, meshes[0]);
              
            //    mid1 = b;
            //    mid2 = b2;
            //}
            //ii++;
            //UnityEngine.Debug.Log(b.Equals(b2) +" "+ b2.vector3.ToString("F6") + " "+b.vector3.ToString("F6"));
            ////mid1 = b;
            ////mid2 = b2;
        }
    }
    List<KeyValuePair<int, float>> m;
    FasterVector3 point;

    //bool firstList;
    int z = 0, z2 = 0;
    void CheckContains(bool firstList) {
        if(firstList) {
            if (!pairs.ContainsKey(point) || !useExistingVertixes)
            {
                pairs.Add(point, z);
                trisList.Add(z);
                z++;
            }
            else {
              //  Debug.Log(pairs.e)
                trisList.Add(pairs[point]);
            }
        }
        else {
            if (!pairs2.ContainsKey(point) || !useExistingVertixes)
            {
                pairs2.Add(point, z2);
                trisList2.Add(z2);
                z2++;
            }
            else
            {
                trisList2.Add(pairs2[point]);
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
    Dictionary<FasterVector3,int> fillFacePoints = new Dictionary<FasterVector3,int>();
    Dictionary<FasterVector3,int> fillFacePoints2 = new Dictionary<FasterVector3,int>();
    void FirstSliceMethod() {
        newPlane = new Plane(transform.rotation * Vector3.up, -Vector3.Dot(transform.up, transform.position));
        foreach (var mesh in meshes)
        {

            z = z2 = 0;
            fillFacePoints = new Dictionary<FasterVector3, int>();
            fillFacePoints2 = new Dictionary<FasterVector3, int>();
            verticesList = new List<FasterVector3>();
            verticesList2 = new List<FasterVector3>();
            pairs = new Dictionary<FasterVector3, int>();
            pairs2 = new Dictionary<FasterVector3, int>();
            vertices = mesh.GetComponent<MeshFilter>().mesh.vertices;
            //foreach (var vert in vertices)
            //{
            //    verticesList.Add(vert);
            //}

            int[] triangles = mesh.GetComponent<MeshFilter>().mesh.triangles;
            trisList = new List<int>();
            trisList2 = new List<int>();
    
            verticesList = new List<FasterVector3>();
            verticesList2 = new List<FasterVector3>();

           


            FasterVector3 pointB = new FasterVector3(); //point bellow
            FasterVector3 pointA1 = new FasterVector3(); //point above 1
            FasterVector3 pointA2 = new FasterVector3();//point above 2


            FasterVector3 pointB1 = new FasterVector3(); //point bellow 1
            FasterVector3 pointB2 = new FasterVector3(); //point bellow 2
            FasterVector3 pointA = new FasterVector3();//point above 

            FasterVector3 transformPosition =new FasterVector3( transform.position);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var i0V = mesh.transform.TransformPoint(vertices[triangles[i]]);
                var i1V = mesh.transform.TransformPoint(vertices[triangles[i + 1]]);
                var i2V = mesh.transform.TransformPoint(vertices[triangles[i + 2]]);
                FasterVector3 i0 = new FasterVector3(i0V);
                FasterVector3 i1 = new FasterVector3(i1V);
                FasterVector3 i2 = new FasterVector3(i2V);

                float dist0 = newPlane.GetDistanceToPoint(i0V);
                float dist1 = newPlane.GetDistanceToPoint(i1V);
                float dist2 = newPlane.GetDistanceToPoint(i2V);

                if (dist0 <= 0 &&
                    dist1 <= 0 &&
                    dist2 <= 0)
                {
                    point = i0;
                    CheckContains(true);
                    point = i1;
                   CheckContains(  true);
                    point = i2;
                    CheckContains(  true);
                    
                }

                if (dist0 > 0 && 
                    dist1 > 0 && 
                    dist2 > 0)
                {
                    //continue;
                    point = i0;
                    CheckContains(  false);
                    point = i1;
                   CheckContains(  false);
                    point = i2;
                    CheckContains(  false);
                }

                if (dist0 > 0 &&
                    dist1 <= 0 &&
                    dist2 <= 0)
                {

                    var LineForward = (i0 - i2) / (i0 - i2).sqrMagnitude;
                    float t = Vector3.Dot(transform.position - i0.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                    FasterVector3 p = i0 + LineForward * t;

                    point = p;
                    CheckContains(   true);
                    point = i1;
                    CheckContains(   true);
                    point = i2;
                    CheckContains(  true);


                    var LineForward2 = (i0V - i1V) / (i0V - i1V).magnitude;
                    float t2 = Vector3.Dot(transform.position - i0V, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                    FasterVector3 p2 = new FasterVector3(i0V + LineForward2 * t2);

                    point = p;
                    CheckContains(  true);
                    point = p2;
                    CheckContains(  true);
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
                    float t = Vector3.Dot(transform.position - i0.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                     var p = i0 + LineForward * t;

                    point = p;
                    CheckContains( false);
                    point = i1;
                    CheckContains( false);
                    point = i2;
                    CheckContains( false);


                    var LineForward2 = (i0 - i1) / (i0- i1).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - i0.vector3, -transform.up) / Vector3.Dot(LineForward2.vector3, -transform.up);
                    var p2 =  i0 + LineForward2 * t2;

                    point = p;
                    CheckContains(  false);
                    point = p2;
                    CheckContains( false);
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
                    float t = Vector3.Dot(transform.position - pointB.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                    var p = pointB + (LineForward * t);

                    var LineForward2 = (pointB - pointA2) / (pointB - pointA2).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB.vector3, -transform.up) / Vector3.Dot(LineForward2.vector3, -transform.up);
                    var p2 = pointB + LineForward2 * t2;
                    if (dist0 <= 0)
                    {
                        point = pointB;
                       CheckContains( true);
                        point = p;
                       CheckContains( true);
                        point = p2;
                       CheckContains( true);

                    }
                    if (dist1 <= 0)
                    {
                        point = p;
                       CheckContains( true);
                        point = pointB;
                       CheckContains( true);
                        point = p2;
                       CheckContains( true);
                    }
                    if (dist2 <= 0)
                    {
                        point = pointB;
                       CheckContains( true);
                        point = p;
                       CheckContains( true);
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
                    float t = Vector3.Dot(transform.position - pointB.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                    var p = pointB + LineForward * t;

                    var LineForward2 = (pointB - pointA2) / (pointB - pointA2).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB.vector3, -transform.up) / Vector3.Dot(LineForward2.vector3, -transform.up);
                    var p2 = pointB + LineForward2 * t2;



                    if (dist0 > 0)
                    {
                        point = pointB;
                        CheckContains( false);
                        point = p;
                        CheckContains(  false);
                        point = p2;
                        CheckContains( false);

                    }
                    if (dist1 > 0)
                    {
                        point = p;
                        CheckContains( false);
                        point = pointB;
                        CheckContains( false);
                        point = p2;
                        CheckContains( false);
                    
                    }
                    if (dist2 > 0)
                    {
                        point = pointB;
                        CheckContains( false);
                        point = p;
                        CheckContains( false);
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
                    float t = Vector3.Dot(transform.position - pointB1.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                    var p = pointB1 + LineForward * t;

                    var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB2.vector3, -transform.up) / Vector3.Dot(LineForward2.vector3, -transform.up);
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
                        point = p;
                       CheckContains( true);
                        point = p2;
                        CheckContains(  true);
                        point = pointB1;
                       CheckContains( true);

                        point = pointB1;
                       CheckContains( true);
                        point = p2;
                       CheckContains( true);
                        point = pointB2;
                       CheckContains( true);
                     
                    }
                    if (dist2 > 0)
                    {
                        point = p;
                       CheckContains( true);
                        point = pointB1;
                       CheckContains( true);
                        point = p2;
                       CheckContains( true);

                        point = pointB1;
                       CheckContains( true);
                        point = pointB2;
                       CheckContains( true);
                        point = p2;
                       CheckContains( true);
                    }



                    mid1 = p;
                    mid2 = p2;

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
                    float t = Vector3.Dot(transform.position - pointB1.vector3, -transform.up) / Vector3.Dot(LineForward.vector3, -transform.up);
                    var p = pointB1 + LineForward * t;

                    var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).sqrMagnitude;
                    float t2 = Vector3.Dot(transform.position - pointB2.vector3, -transform.up) / Vector3.Dot(LineForward2.vector3, -transform.up);
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
                        point = p;
                        CheckContains( false);
                        point = p2;
                        CheckContains( false);
                        point = pointB1;
                        CheckContains( false);

                        point = pointB1;
                        CheckContains( false);
                        point = p2;
                        CheckContains( false);
                        point = pointB2;
                        CheckContains( false);
                                             
                    }
                    if (dist2 <= 0)
                    {
                        point = p;
                        CheckContains( false);
                        point = pointB1;
                        CheckContains( false);
                        point = p2;
                        CheckContains( false);

                        point = pointB1;
                        CheckContains( false);
                        point = pointB2;
                        CheckContains( false);
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
            Fill(fillFacePoints, true);
            Fill(fillFacePoints2, false);

            //            UnityEngine.Debug.Log(vertices.Length + " " + triangles.Length);

            CreateMeshes(mesh.GetComponent<MeshFilter>().mesh, mesh);


        }
    }

    
    FasterVector3 mid1 = new FasterVector3();
    FasterVector3 mid2 = new FasterVector3();

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
    void Fill(Dictionary<FasterVector3,int> fillFacePoints, bool firstObj)
    {

        float xMax = fillFacePoints.Max(x => x.Key.x);
        float xMin = fillFacePoints.Min(x => x.Key.x);
        float yMax = fillFacePoints.Max(y => y.Key.y);
        float yMin = fillFacePoints.Min(y => y.Key.y);
        float zMax = fillFacePoints.Max(z => z.Key.z);
        float zMin = fillFacePoints.Min(z => z.Key.z);

        FasterVector3 centerPoint = new FasterVector3(xMin + (xMax - xMin) / 2.0f, yMin + (yMax - yMin) / 2.0f, zMin + (zMax - zMin) / 2.0f);
        p = centerPoint.vector3;

        Dictionary<int, float> positions = new Dictionary<int, float>();
        Vector3 one = new Vector3(1, 0, 0);
        for (int i = 0; i < fillFacePoints.Count; i++)
        {
            positions.Add(i, Vector3.SignedAngle(centerPoint.vector3, fillFacePoints.ElementAt(i).Key.vector3, transform.rotation*transform.up));
        }
        m= positions.OrderBy(x => x.Value).ToList();

        for (int i = 0; i < m.Count-1; i++)
        {
            int a = m.ElementAt(i).Key;
            int a2 = m.ElementAt(i + 1).Key;
            FasterVector3 b = fillFacePoints.ElementAt(a).Key;
            FasterVector3 b2 = fillFacePoints.ElementAt(a2).Key;

            if (!b.Equals(b2))
            {
                if(firstObj) {
                    point = new FasterVector3(p);
                    CheckContains(firstObj);
                    point = b;
                    CheckContains(firstObj);
                    point = b2;
                    CheckContains(firstObj);
                }
                else
                {
                    point = new FasterVector3(p);
                    CheckContains(false);
                    point = b2;
                    CheckContains(false);
                    point = b;
                    CheckContains(false);
                  
                }

            }
        }
        int aa = m.ElementAt(m.Count - 1).Key;
        int aa2 = m.ElementAt(0).Key;
        FasterVector3 bb = fillFacePoints.ElementAt(aa).Key;
        FasterVector3 bb2 = fillFacePoints.ElementAt(aa2).Key;

        if (!bb.Equals(bb2))
        {
            if (firstObj)
            {
                point = new FasterVector3(p);
                CheckContains(firstObj);
                point = bb;
                CheckContains(firstObj);
                point = bb2;
                CheckContains(firstObj);
            }
            else
            {
                point = new FasterVector3(p);
                CheckContains(false);
                point = bb2;
                CheckContains(false);
                point = bb;
                CheckContains(false);

            }

        }
    }

    void CreateMeshes(Mesh mesh,Transform parent) {


        mesh.Clear();
        mesh.ClearBlendShapes();
        foreach (var item in pairs)
        {
            verticesList.Add(item.Key);
        }
        mesh.vertices = vertices =  FasterVector3.ToArray(verticesList);
        mesh.triangles = trisList.ToArray();
        //Vector2[] uvs = new Vector2[vertices.Length];

        //Vector2 half = new Vector2(0.5f, 0.5f);
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    uvs[i] = vertices[i] * half;
        //}

        //mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        Destroy( parent.GetComponent<MeshCollider>());
        var meshCol = parent.gameObject.AddComponent<MeshCollider>();
        meshCol.convex = true;

        GameObject secondMesh = Instantiate( mesh2, parent.position,Quaternion.identity);
        var meshFilter = secondMesh.GetComponent<MeshFilter>();
        
        meshFilter.mesh.Clear();

        meshFilter.mesh.ClearBlendShapes();

        foreach (var item in pairs2)
        {
            verticesList2.Add(item.Key);
        }
        //UnityEngine.Debug.Log(verticesList.Count + " " + trisList.Count);
        //UnityEngine.Debug.Log(verticesList2.Count + " " + trisList2.Count);

        meshFilter.mesh.vertices = FasterVector3.ToArray(verticesList2);
        meshFilter.mesh.triangles = trisList2.ToArray();
        //Vector2[] uvs2 = new Vector2[meshFilter.mesh.vertices.Length];
        //for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        //{
        //    uvs2[i] = meshFilter.mesh.vertices[i] * new Vector2(0.5f, 0.5f) ;
        //}

        //meshFilter.mesh.uv = uvs2;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
        meshCol=secondMesh.AddComponent<MeshCollider>();
        meshCol.convex = true;
    }

    public Vector3 p = Vector3.zero;
  
}
