using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class meshGenerator : MonoBehaviour
{

    [SerializeField]
    MeshCollider meshCollider;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangeles;

    public int xSize = 20;
    public int zSize = 20;



    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        createShape();
        UpdateMesh();
    }
    
    void createShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize +1)];

        for (int i = 0, z = 0; z<=zSize;z++)
        {
            for (int x = 0; x<=xSize; x++)
            {
                float y = Mathf.PerlinNoise(x*.3f,z *.3f) *2f;
                vertices[i] = new Vector3(x,y,z);
                i++;
            }
        }

        triangeles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
        
                triangeles[tris + 0] = vert + 0;
                triangeles[tris + 1] = vert + xSize +1;
                triangeles[tris + 2] = vert + 1;
                triangeles[tris + 3] = vert + 1;
                triangeles[tris + 4] = vert + xSize + 1;
                triangeles[tris + 5] = vert + xSize + 2;

                vert++;
                tris+=6;
            }
            vert++;
        }
    }


   
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices =vertices;
        mesh.triangles = triangeles;

        mesh.RecalculateNormals();
    }
    
}
