using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class RockCreation : MonoBehaviour {
    
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> midPVerts = new List<Vector3>();
    private List<Vector3> doneVerts = new List<Vector3>();
    private List<Vector3> CutAreaVerts = new List<Vector3>();
    private List<int> newTris = new List<int>();

    public GameObject cutArea; Mesh mesh2; int[] cuTris;
    List<Vector3> cutVerts = new List<Vector3>();


    private Vector3 center;
    GameObject plane;    
    Plane playerPlane;
       
    void Start()
    {
        mesh2 = cutArea.GetComponent<MeshFilter>().mesh;
       

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position=transform.position;
        
        
        if (gameObject.tag == "first")
        {
            float offset = Random.Range(1, 2);

            Mesh mesh = GetComponent<MeshFilter>().mesh;

            for (int s = 0; s < mesh.vertices.Length; s++)
            {
                vertices.Add(mesh.vertices[s]);
                
                
            }
            
            center = GetComponent<Renderer>().bounds.center;
            for (int v = 0; v < vertices.Count; v++)
            {

                bool used = false;
                for (int k = 0; k < doneVerts.Count; k++)
                {
                    if (doneVerts[k] == vertices[v])
                    {
                        used = true;
                    }
                }
                if (!used)
                {
                    Vector3 curVector = vertices[v];
                    doneVerts.Add(curVector);

                    Vector3 changedVector = curVector.normalized * Random.Range(1, 1.2f);

                    //scale some parts of the mesh
                    if (changedVector.x < 0.2f && changedVector.x > -0.2f && changedVector.y > 0)
                    {
                        changedVector = new Vector3(changedVector.x, changedVector.y * 0.5f, changedVector.z);

                    }
                    //if (changedVector.x < 0.3f && changedVector.x > 0)
                    //{
                    //    changedVector = new Vector3(changedVector.x, changedVector.y * 2, changedVector.z);
                    // }

                    //partea pentru destrugere
                    //if (changedVector.x < -0.01f)
                    //{
                    //    changedVector = new Vector3(0, changedVector.y, changedVector.z);
                    //}

                    for (int s = 0; s < vertices.Count; s++)
                    {
                        if (Mathf.Approximately(vertices[s].x, curVector.x) && Mathf.Approximately(vertices[s].y, curVector.y) && Mathf.Approximately(vertices[s].z, curVector.z))
                        {
                            vertices[s] = changedVector;
                        }
                    }
                }

            }
           
            int[] oldTris = mesh.triangles;
            //int[] newTrisArr = DontShowTris(mesh);
            mesh.SetVertices(vertices);
            //mesh.triangles = newTrisArr;            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
       
    }


    private void Update()
    {
        
        Vector3 normalOfPlane =  Quaternion.AngleAxis(0, plane.transform.up) * plane.transform.up  ;
        playerPlane = new Plane(normalOfPlane, plane.transform.position);
        
        //gizmo plane -green-edges,red-normal
        DrawPlane(plane.transform.position, normalOfPlane);

        if (Input.GetKeyDown("s"))
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            

            int[] newTris= DontShowTris(mesh);
            FillVoid();
            
            mesh.SetVertices(vertices);
            mesh.triangles = newTris;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            mesh2.Clear();
            mesh2.SetVertices(cutVerts);
            mesh2.triangles = cuTris;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            cutArea.GetComponent<MeshFilter>().mesh = mesh2;

            s.Stop();
            UnityEngine.Debug.Log(s.ElapsedMilliseconds);


        }

    }

    int[] DontShowTris(Mesh mesh)
    {
        #region don't show triangles __code

        int shir = 0;

        int[] tris = mesh.triangles;

        for (int i = 0; i < tris.Length-2; i += 3)
        {
            
            Vector3 first = transform.TransformPoint(vertices[tris[i]]);
            Vector3 second = transform.TransformPoint(vertices[tris[i + 1]]);
            Vector3 third = transform.TransformPoint(vertices[tris[i + 2]]);
            float cutPos = -0.01f; //in caz ca e zero, sa fie pus la -0.01f, pentru triunghurile cu coord pozitive
            
            //if (playerPlane.GetDistanceToPoint(first) > cutPos && playerPlane.GetDistanceToPoint(second) > cutPos && playerPlane.GetDistanceToPoint(third) > cutPos)
            //{
            //    //Debug.Log(vertices.Count);
            //    newTris.Add(tris[i]);
            //    newTris.Add(tris[i + 1]);
            //    newTris.Add(tris[i + 2]);

            //    shir += 3;
            //}

            //triunghiuri intersectate de plan

            // 1 < 0 && 2 > 0 && 3 > 0 || 1 > 0 && 2 < 0 && 3 < 0
            if ((playerPlane.GetDistanceToPoint(first) < cutPos && playerPlane.GetDistanceToPoint(second) > cutPos && playerPlane.GetDistanceToPoint(third) > cutPos) || (playerPlane.GetDistanceToPoint(first) > cutPos && playerPlane.GetDistanceToPoint(second) < cutPos && playerPlane.GetDistanceToPoint(third) < cutPos))
            {
                Vector3 midP1 = GetMidPoint(second, first, plane);
                Vector3 midP2 = GetMidPoint(third, first, plane);

                //1-5-2-3-4

                //1-5-4
                //5-2-3
                //3-4-5

               
                vertices.Add(midP1);
                vertices.Add(midP2);
                

                CutAreaVerts.Add(midP1);
                CutAreaVerts.Add(midP2);


                //5-2-3
                if (playerPlane.GetDistanceToPoint(first) < cutPos)
                {
                    newTris.Add(vertices.Count - 2);
                    newTris.Add(tris[i + 1]);
                    newTris.Add(tris[i + 2]);

                    newTris.Add(tris[i + 2]);
                    newTris.Add(vertices.Count - 1);
                    newTris.Add(vertices.Count - 2);

                    shir += 6;
                }

                //1-5-4
                else if (playerPlane.GetDistanceToPoint(first) >= cutPos)
                {
                    newTris.Add(tris[i]);
                    newTris.Add(vertices.Count - 2);
                    newTris.Add(vertices.Count - 1);

                    shir += 3;
                }
            }
            //2<0 && 1>0 && 3>0 || 2>0 && 1<0 && 3<0 // 1-2-3
            else if ((playerPlane.GetDistanceToPoint(first) > cutPos && playerPlane.GetDistanceToPoint(second) < cutPos && playerPlane.GetDistanceToPoint(third) > cutPos) || (playerPlane.GetDistanceToPoint(first) < cutPos && playerPlane.GetDistanceToPoint(second) > cutPos && playerPlane.GetDistanceToPoint(third) < cutPos))
            {
                Vector3 midP1 = GetMidPoint(first, second, plane);
                Vector3 midP2 = GetMidPoint(third, second, plane);
                //1-4-2-5-3

                //1-4-5
                //1-5-3
                //4-2-5
                vertices.Add(midP1);
                vertices.Add(midP2);

                CutAreaVerts.Add(midP1);
                CutAreaVerts.Add(midP2);

                //1-4-5
                //1-5-3
                if (playerPlane.GetDistanceToPoint(second) < cutPos)
                {
                    newTris.Add(tris[i]);
                    newTris.Add(vertices.Count - 2);
                    newTris.Add(vertices.Count - 1);

                    newTris.Add(tris[i]);
                    newTris.Add(vertices.Count - 1);
                    newTris.Add(tris[i+2]);

                    shir += 6;
                }

                //4-2-5
                else if (playerPlane.GetDistanceToPoint(second) >= cutPos)
                {
                    newTris.Add(vertices.Count - 2);
                    newTris.Add(tris[i+1]);
                    newTris.Add(vertices.Count - 1);

                    shir += 3;
                }
            }
            //3<0 && 1>0 && 2>0 || 3>0 && 1<0 && 2<0 // 1-2-3
            else if ((playerPlane.GetDistanceToPoint(first) > cutPos && playerPlane.GetDistanceToPoint(second) > cutPos && playerPlane.GetDistanceToPoint(third) < cutPos) || (playerPlane.GetDistanceToPoint(first) < cutPos && playerPlane.GetDistanceToPoint(second) < cutPos && playerPlane.GetDistanceToPoint(third) > cutPos))
            {
                Vector3 midP1 = GetMidPoint(first, third, plane);
                Vector3 midP2 = GetMidPoint(second, third, plane);

                //1-2-4-3-5

                //1-2-4
                //1-4-5
                //5-4-3
                vertices.Add(midP1);
                vertices.Add(midP2);

                CutAreaVerts.Add(midP1);
                CutAreaVerts.Add(midP2);

                //1-4-5
                //1-2-4
                if (playerPlane.GetDistanceToPoint(third) < cutPos)
                {
                    newTris.Add(tris[i]);
                    newTris.Add(vertices.Count - 1);
                    newTris.Add(vertices.Count - 2);

                    newTris.Add(tris[i]);
                    newTris.Add(tris[i + 1]);
                    newTris.Add(vertices.Count - 1);                    

                    shir += 6;                    
                }

                //5-4-3
                else if (playerPlane.GetDistanceToPoint(third) >= cutPos)
                {
                    newTris.Add(vertices.Count - 2);
                    newTris.Add(vertices.Count - 1);
                    newTris.Add(tris[i+2]);

                    shir += 3;
                }
            }
            else if(playerPlane.GetDistanceToPoint(first) > cutPos && playerPlane.GetDistanceToPoint(second) > cutPos && playerPlane.GetDistanceToPoint(third) > cutPos)
            {
                //Debug.Log(vertices.Count);
                newTris.Add(tris[i]);
                newTris.Add(tris[i + 1]);
                newTris.Add(tris[i + 2]);

                shir += 3;
            }
        }
        
        int[] newTrisArr = new int[shir];
        for (int i = 0; i < shir; i++)
        {
            newTrisArr[i] = newTris[i];
        }

        return newTrisArr;
        #endregion
    }


    public void FillVoid()
    {
        Dictionary<float, Vector3> dic = new Dictionary<float, Vector3>();

        for(int i = 0; i < CutAreaVerts.Count; i++)
        {
            float angle = Mathf.Atan2(CutAreaVerts[i].x, CutAreaVerts[i].z);
            if (!dic.ContainsKey(angle))
            {
                dic.Add(angle, CutAreaVerts[i]);
            }
            
        }
        dic = dic.OrderBy(key => key.Key).ToDictionary(x => x.Key, x => x.Value);

        //9gag dic.length
        for(int i = 0; i < dic.Count; i++)
        { 
            cutVerts.Add(dic.Values.ElementAt(i));
        }
        cutVerts.Add(new Vector3(0,0,0));

        cuTris = new int[dic.Count*3];
        List<int> FinTris = new List<int>();
        int lung = 0;
        for (int i = 2; i < cutVerts.Count-1; i+=1)
        {
            FinTris.Add(i - 2);
            FinTris.Add(cutVerts.Count - 1);
            FinTris.Add(i-1) ;
            lung += 3;
        }

        
        FinTris.Add(cutVerts.Count - 2);
        FinTris.Add(cutVerts.Count - 1);
        FinTris.Add(0);

        lung += 3;
        for (int i = 0; i < FinTris.Count; i++)
        {
            cuTris[i] = FinTris[i];
        }

        
    }

    public Vector3 GetMidPoint(Vector3 firstP, Vector3 secondP, GameObject plane)
    {
        Vector3 PlanePos = transform.TransformVector(plane.transform.up);

        float a = plane.transform.up.x;
        float b = plane.transform.up.y;
        float c = plane.transform.up.z;
        float d = Vector3.Dot(plane.transform.up, plane.transform.position);
        //Debug.Log("a: " +a + " b: " + b + " c: " + c + " d: "+ d);

        float t = 1;
        float x1 = firstP.x; float x2 = secondP.x;
        float y1 = firstP.y; float y2 = secondP.y;
        float z1 = firstP.z; float z2 = secondP.z;

        t = (d - ((x1 * a) + (y1 * b) + (z1 * c))) / (a * (x2 - x1) + b * (y2 - y1) + c * (z2 - z1));
        //a*x+b * y + c * z = d;


        //Debug.Log(t);
        return new Vector3(t * (x2 - x1) + x1, t * (y2 - y1) + y1, t * (z2 - z1) + z1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        List<Vector3> verticesInWorldPos = new List<Vector3>();


        //for (int i = 0; i < CutAreaVerts.Count; i++)
        //{
        //    verticesInWorldPos.Add(transform.TransformPoint(CutAreaVerts[i]));
        //    Gizmos.DrawSphere(verticesInWorldPos[i], 0.05f);

        //}
        //Debug.Log(CutAreaVerts.Count);
        for (int i = 0; i < cutVerts.Count; i++)
        {
            Gizmos.DrawSphere(playerPlane.ClosestPointOnPlane(cutVerts[i]),0.05f);


        }

    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        //Debug.DrawLine(corner0, corner2, Color.green);

        //Debug.DrawLine(corner1, corner3, Color.green);
        //Debug.DrawLine(corner0, corner1, Color.green);
        //Debug.DrawLine(corner1, corner2, Color.green);
        //Debug.DrawLine(corner2, corner3, Color.green);
        //Debug.DrawLine(corner3, corner0, Color.green);
        //Debug.DrawRay(position, normal, Color.red);
    }

}


