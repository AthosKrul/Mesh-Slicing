using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
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

    public List<GameObject> cuttingPlanes;
    public GameObject sphere;
    Vector3[] vertices;
    List<Vector3> verticesList;
    List<Vector3> verticesList2;
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
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(mid1, 0.005f);
        //Gizmos.DrawSphere(mid2, 0.005f);
        //Gizmos.color = Color.white;
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(p, 0.02f);
        //Gizmos.color = Color.white;
    }

    void Update() {


        if (Input.GetKeyDown(KeyCode.S))
        {
            //foreach (var cuttingPlane in cuttingPlanes)
            //{

            newPlane = new Plane(transform.rotation * Vector3.up, -Vector3.Dot(transform.up, transform.position));
            foreach (var mesh in meshes)
            {

            
                vertices = mesh.GetComponent<MeshFilter>().mesh.vertices;
                //foreach (var vert in vertices)
                //{
                //    verticesList.Add(vert);
                //}

                int[] triangles = mesh.GetComponent<MeshFilter>().mesh.triangles;
                trisList = new List<int>();
                trisList2 = new List<int>();
                verticesList = new List<Vector3>();
                verticesList2 = new List<Vector3>();

                List<Vector3> fillFacePoints = new List<Vector3>();
                List<Vector3> fillFacePoints2 = new List<Vector3>();
                for (int i = 0, z = 0, z2 = 0; i < triangles.Length; i += 3)
                {
                    var i0 = sphere.transform.TransformPoint(vertices[triangles[i]]);
                    var i1 = sphere.transform.TransformPoint(vertices[triangles[i + 1]]);
                    var i2 = sphere.transform.TransformPoint(vertices[triangles[i + 2]]);

                    if (newPlane.GetDistanceToPoint(i0) <= 0 &&
                    newPlane.GetDistanceToPoint(i1) <= 0 &&
                    newPlane.GetDistanceToPoint(i2) <= 0)
                    {
                        verticesList.Add(i0);
                        verticesList.Add(i1);
                        verticesList.Add(i2);

                        z = AddTris(z, 1);
                    }

                    if (newPlane.GetDistanceToPoint(i0) > 0 &&
                    newPlane.GetDistanceToPoint(i1) > 0 &&
                    newPlane.GetDistanceToPoint(i2) > 0)
                    {
                        //continue;
                        verticesList2.Add(i0);
                        verticesList2.Add(i1);
                        verticesList2.Add(i2);

                        z2 = AddTris2(z2, 1);
                    }

                    if (newPlane.GetDistanceToPoint(i0) > 0 &&
                    newPlane.GetDistanceToPoint(i1) <= 0 &&
                    newPlane.GetDistanceToPoint(i2) <= 0)
                    {

                        var LineForward = (i0 - i2) / (i0 - i2).magnitude;
                        float t = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = i0 + LineForward * t;

                        verticesList.Add(p);
                        verticesList.Add(i1);
                        verticesList.Add(i2); //fill c


                        var LineForward2 = (i0 - i1) / (i0 - i1).magnitude;
                        float t2 = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = i0 + LineForward2 * t2;

                        verticesList.Add(p);
                        verticesList.Add(p2);
                        verticesList.Add(i1); //fill c

                        z = AddTris(z, 2);

                        fillFacePoints.Add(p);
                        fillFacePoints.Add(p2);
                    }
                    if (newPlane.GetDistanceToPoint(i0) <= 0 &&
                 newPlane.GetDistanceToPoint(i1) > 0 &&
                 newPlane.GetDistanceToPoint(i2) > 0)
                    {
                        // continue;
                        var LineForward = (i0 - i2) / (i0 - i2).magnitude;
                        float t = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = i0 + LineForward * t;

                        verticesList2.Add(p);
                        verticesList2.Add(i1);
                        verticesList2.Add(i2);


                        var LineForward2 = (i0 - i1) / (i0 - i1).magnitude;
                        float t2 = Vector3.Dot(transform.position - i0, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = i0 + LineForward2 * t2;

                        verticesList2.Add(p);
                        verticesList2.Add(p2);
                        verticesList2.Add(i1);

                        z2 = AddTris2(z2, 2);

                        fillFacePoints2.Add(p);
                        fillFacePoints2.Add(p2);
                    }
                    #region one point bellow
                    if ((newPlane.GetDistanceToPoint(i0) > 0 &&
                    newPlane.GetDistanceToPoint(i1) <= 0 &&
                    newPlane.GetDistanceToPoint(i2) > 0) ||

                    (newPlane.GetDistanceToPoint(i0) <= 0 &&
                    newPlane.GetDistanceToPoint(i1) > 0 &&
                    newPlane.GetDistanceToPoint(i2) > 0) ||

                    (newPlane.GetDistanceToPoint(i0) > 0 &&
                    newPlane.GetDistanceToPoint(i1) > 0 &&
                    newPlane.GetDistanceToPoint(i2) <= 0))
                    {
                        Vector3 pointB = new Vector3(); //point bellow
                        Vector3 pointA1 = new Vector3(); //point above 1
                        Vector3 pointA2 = new Vector3();//point above 2

                        if (newPlane.GetDistanceToPoint(i1) <= 0)
                        {
                            pointB = i1;
                            pointA1 = i0;
                            pointA2 = i2;
                        }
                        if (newPlane.GetDistanceToPoint(i0) <= 0)
                        {
                            pointB = i0;
                            pointA1 = i1;
                            pointA2 = i2;
                        }
                        if (newPlane.GetDistanceToPoint(i2) <= 0)
                        {
                            pointB = i2;
                            pointA1 = i0;
                            pointA2 = i1;
                        }
                        var LineForward = (pointB - pointA1) / (pointB - pointA1).magnitude;
                        float t = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = pointB + LineForward * t;

                        var LineForward2 = (pointB - pointA2) / (pointB - pointA2).magnitude;
                        float t2 = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = pointB + LineForward2 * t2;
                        if (newPlane.GetDistanceToPoint(i0) <= 0)
                        {
                            verticesList.Add(pointB);
                            verticesList.Add(p);
                            verticesList.Add(p2);
                        }
                        if (newPlane.GetDistanceToPoint(i1) <= 0)
                        {

                            verticesList.Add(p);
                            verticesList.Add(pointB);
                            verticesList.Add(p2);
                        }
                        if (newPlane.GetDistanceToPoint(i2) <= 0)
                        {
                            verticesList.Add(pointB);
                            verticesList.Add(p);
                            verticesList.Add(p2);
                        }
                        z = AddTris(z, 1);
                        fillFacePoints.Add(p);
                        fillFacePoints.Add(p2);
                    }
                    #endregion
                    #region one point above


                    if ((newPlane.GetDistanceToPoint(i0) <= 0 &&
                    newPlane.GetDistanceToPoint(i1) > 0 &&
                    newPlane.GetDistanceToPoint(i2) <= 0) ||

                    (newPlane.GetDistanceToPoint(i0) > 0 &&
                    newPlane.GetDistanceToPoint(i1) <= 0 &&
                    newPlane.GetDistanceToPoint(i2) <= 0) ||

                    (newPlane.GetDistanceToPoint(i0) <= 0 &&
                    newPlane.GetDistanceToPoint(i1) <= 0 &&
                    newPlane.GetDistanceToPoint(i2) > 0))
                    {
                        //    continue;
                        Vector3 pointB = new Vector3(); //point above
                        Vector3 pointA1 = new Vector3(); //point bellow 1
                        Vector3 pointA2 = new Vector3();//point bellow 2

                        if (newPlane.GetDistanceToPoint(i1) > 0)
                        {
                            pointB = i1;
                            pointA1 = i0;
                            pointA2 = i2;
                        }
                        if (newPlane.GetDistanceToPoint(i0) > 0)
                        {
                            pointB = i0;
                            pointA1 = i1;
                            pointA2 = i2;
                        }
                        if (newPlane.GetDistanceToPoint(i2) > 0)
                        {
                            pointB = i2;
                            pointA1 = i0;
                            pointA2 = i1;
                        }
                        var LineForward = (pointB - pointA1) / (pointB - pointA1).magnitude;
                        float t = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = pointB + LineForward * t;

                        var LineForward2 = (pointB - pointA2) / (pointB - pointA2).magnitude;
                        float t2 = Vector3.Dot(transform.position - pointB, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = pointB + LineForward2 * t2;



                        if (newPlane.GetDistanceToPoint(i0) > 0)
                        {
                            verticesList2.Add(pointB);
                            verticesList2.Add(p);
                            verticesList2.Add(p2);
                            z2 = AddTris2(z2, 1);
                        }
                        if (newPlane.GetDistanceToPoint(i1) > 0)
                        {

                            verticesList2.Add(p);
                            verticesList2.Add(pointB);
                            verticesList2.Add(p2);
                            z2 = AddTris2(z2, 1);
                        }
                        if (newPlane.GetDistanceToPoint(i2) > 0)
                        {
                            verticesList2.Add(pointB);
                            verticesList2.Add(p);
                            verticesList2.Add(p2);
                            z2 = AddTris2(z2, 1);
                        }
                        fillFacePoints2.Add(p);
                        fillFacePoints2.Add(p2);
                    }
                    #endregion

                    #region two points bellow
                    if (
                    (newPlane.GetDistanceToPoint(i0) > 0 &&
                   newPlane.GetDistanceToPoint(i1) <= 0 &&
                   newPlane.GetDistanceToPoint(i2) <= 0) ||

                   (newPlane.GetDistanceToPoint(i0) <= 0 &&
                   newPlane.GetDistanceToPoint(i1) > 0 &&
                   newPlane.GetDistanceToPoint(i2) <= 0) ||

                   (newPlane.GetDistanceToPoint(i0) <= 0 &&
                   newPlane.GetDistanceToPoint(i1) <= 0 &&
                   newPlane.GetDistanceToPoint(i2) > 0)
                   )
                    {


                        Vector3 pointB1 = new Vector3(); //point bellow 1
                        Vector3 pointB2 = new Vector3(); //point bellow 2
                        Vector3 pointA = new Vector3();//point above 

                        if (newPlane.GetDistanceToPoint(i0) > 0)
                        {
                            pointB1 = i1;
                            pointB2 = i2;
                            pointA = i0;
                        }
                        if (newPlane.GetDistanceToPoint(i1) > 0)
                        {
                            pointB1 = i0;
                            pointB2 = i2;
                            pointA = i1;
                        }
                        if (newPlane.GetDistanceToPoint(i2) > 0)
                        {
                            pointB1 = i0;
                            pointB2 = i1;
                            pointA = i2;
                        }

                        var LineForward = (pointB1 - pointA) / (pointB1 - pointA).magnitude;
                        float t = Vector3.Dot(transform.position - pointB1, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = pointB1 + LineForward * t;

                        var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).magnitude;
                        float t2 = Vector3.Dot(transform.position - pointB2, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = pointB2 + LineForward2 * t2;



                        if (newPlane.GetDistanceToPoint(i0) > 0)
                        {
                            //verticesList.Add(p);
                            //verticesList.Add(pointB1);
                            //verticesList.Add(p2);

                            //verticesList.Add(pointB1);
                            //verticesList.Add(pointB1);
                            //verticesList.Add(p2);
                        }
                        if (newPlane.GetDistanceToPoint(i1) > 0)
                        {

                            verticesList.Add(p);
                            verticesList.Add(p2);
                            verticesList.Add(pointB1);

                            verticesList.Add(pointB1);
                            verticesList.Add(p2);
                            verticesList.Add(pointB2);
                            z = AddTris(z, 2);
                        }
                        if (newPlane.GetDistanceToPoint(i2) > 0)
                        {

                            verticesList.Add(p);
                            verticesList.Add(pointB1);
                            verticesList.Add(p2);

                            verticesList.Add(pointB1);
                            verticesList.Add(pointB2);
                            verticesList.Add(p2);
                            z = AddTris(z, 2);
                        }



                        mid1 = p;
                        mid2 = p2;

                        fillFacePoints.Add(p);
                        fillFacePoints.Add(p2);
                    }
                    #endregion

                    #region two above bellow
                    if (
                    (newPlane.GetDistanceToPoint(i0) <= 0 &&
                   newPlane.GetDistanceToPoint(i1) > 0 &&
                   newPlane.GetDistanceToPoint(i2) > 0) ||

                   (newPlane.GetDistanceToPoint(i0) > 0 &&
                   newPlane.GetDistanceToPoint(i1) <= 0 &&
                   newPlane.GetDistanceToPoint(i2) > 0) ||

                   (newPlane.GetDistanceToPoint(i0) > 0 &&
                   newPlane.GetDistanceToPoint(i1) > 0 &&
                   newPlane.GetDistanceToPoint(i2) <= 0)
                   )
                    {

                        //  continue;
                        Vector3 pointB1 = new Vector3(); //point bellow 1
                        Vector3 pointB2 = new Vector3(); //point bellow 2
                        Vector3 pointA = new Vector3();//point above 

                        if (newPlane.GetDistanceToPoint(i0) <= 0)
                        {
                            pointB1 = i1;
                            pointB2 = i2;
                            pointA = i0;
                        }
                        if (newPlane.GetDistanceToPoint(i1) <= 0)
                        {
                            pointB1 = i0;
                            pointB2 = i2;
                            pointA = i1;
                        }
                        if (newPlane.GetDistanceToPoint(i2) <= 0)
                        {
                            pointB1 = i0;
                            pointB2 = i1;
                            pointA = i2;
                        }

                        var LineForward = (pointB1 - pointA) / (pointB1 - pointA).magnitude;
                        float t = Vector3.Dot(transform.position - pointB1, -transform.up) / Vector3.Dot(LineForward, -transform.up);
                        var p = pointB1 + LineForward * t;

                        var LineForward2 = (pointB2 - pointA) / (pointB2 - pointA).magnitude;
                        float t2 = Vector3.Dot(transform.position - pointB2, -transform.up) / Vector3.Dot(LineForward2, -transform.up);
                        var p2 = pointB2 + LineForward2 * t2;



                        if (newPlane.GetDistanceToPoint(i0) <= 0)
                        {
                            //verticesList2.Add(p);
                            //verticesList2.Add(pointB2);
                            //verticesList2.Add(p2);

                            //verticesList2.Add(p2);
                            //verticesList2.Add(pointB1);
                            //verticesList2.Add(pointB2);

                            //  z2 = AddTris2(z2, 1);
                        }
                        if (newPlane.GetDistanceToPoint(i1) <= 0)
                        {

                            verticesList2.Add(p);
                            verticesList2.Add(p2);
                            verticesList2.Add(pointB1);

                            verticesList2.Add(pointB1);
                            verticesList2.Add(p2);
                            verticesList2.Add(pointB2);
                            z2 = AddTris2(z2, 2);
                        }
                        if (newPlane.GetDistanceToPoint(i2) <= 0)
                        {

                            verticesList2.Add(p);
                            verticesList2.Add(pointB1);
                            verticesList2.Add(p2);

                            verticesList2.Add(pointB1);
                            verticesList2.Add(pointB2);
                            verticesList2.Add(p2);
                            z2 = AddTris2(z2, 2);
                        }
                        fillFacePoints2.Add(p);
                        fillFacePoints2.Add(p2);
                        //z2 = AddTris2(z2, 2);
                    }
                    #endregion

                }
                Fill(fillFacePoints, true);
                Fill(fillFacePoints2, false);

                Debug.Log(vertices.Length + " " + triangles.Length);
                Debug.Log(verticesList.Count + " " + trisList.Count);


                CreateMeshes(mesh.GetComponent<MeshFilter>().mesh,mesh);
            }
        }
    }

    Vector3 mid1 = new Vector3();
    Vector3 mid2 = new Vector3();

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
    void Fill(List<Vector3> fillFacePoints, bool firstObj) {

        float xMax = fillFacePoints.Max(x => x.x);
        float xMin = fillFacePoints.Min(x => x.x);
        float yMax = fillFacePoints.Max(y => y.y);
        float yMin = fillFacePoints.Min(y => y.y);
        float zMax = fillFacePoints.Max(z => z.z);
        float zMin = fillFacePoints.Min(z => z.z);

        Vector3 centerPoint = new Vector3(xMin + (xMax - xMin) / 2.0f, yMin + (yMax - yMin) / 2.0f, zMin + (zMax - zMin) / 2.0f);
        p = centerPoint;

        Dictionary<int, float> positions = new Dictionary<int, float>();
        for (int i = 0; i < fillFacePoints.Count; i++)
        {
            positions.Add(i, Vector3.SignedAngle(centerPoint, fillFacePoints[i], new Vector3(1, 0, 0)));
        }
        positions.OrderBy(x => x.Value);
        for (int i = 0; i < positions.Count - 1; i++)
        {
            int a = positions.ElementAt(i).Key;
            int a2 = positions.ElementAt(i + 1).Key;
            Vector3 b = fillFacePoints[a];
            Vector3 b2 = fillFacePoints[a2];

            if(firstObj) {
                verticesList.Add(b2);
                trisList.Add(verticesList.Count - 1);
                verticesList.Add(b);
                trisList.Add(verticesList.Count - 1);
                verticesList.Add(centerPoint);
                trisList.Add(verticesList.Count - 1);
            }else {
                verticesList2.Add(b2);
                trisList2.Add(verticesList2.Count - 1);
                verticesList2.Add(b);
                trisList2.Add(verticesList2.Count - 1);
                verticesList2.Add(centerPoint);
                trisList2.Add(verticesList2.Count - 1);
            }


        }
    }

    void CreateMeshes(Mesh mesh,Transform parent) {
        mesh.Clear();
        mesh.ClearBlendShapes();
        mesh.vertices = vertices = verticesList.ToArray();
        mesh.triangles = trisList.ToArray();
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = vertices[i] * new Vector2(0.5f, 0.5f);
        }

        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GameObject secondMesh = Instantiate( mesh2, parent.position,Quaternion.identity);
        var meshFilter = secondMesh.GetComponent<MeshFilter>();
        
        meshFilter.mesh.Clear();

        meshFilter.mesh.ClearBlendShapes();
        meshFilter.mesh.vertices = verticesList2.ToArray();
        meshFilter.mesh.triangles = trisList2.ToArray();
        Vector2[] uvs2 = new Vector2[meshFilter.mesh.vertices.Length];
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            uvs2[i] = meshFilter.mesh.vertices[i] * new Vector2(0.5f, 0.5f) ;
        }

        meshFilter.mesh.uv = uvs2;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
    }

    public Vector3 p=Vector3.zero;
  
}
