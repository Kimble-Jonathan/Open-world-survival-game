using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;
using Unity.AI.Navigation;



public class TerrainChunk
{
	
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	
	public GameObject meshObject;
	GameObject treeObject;
	Vector2 sampleCenter;
	Bounds bounds;

	//NavMeshSurface surface;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;
	Vector2 position;
	List<Vector2> points;

	test test;

	 

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	Transform viewer;
	Transform parent;

	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, GameObject treeObject) 
	{
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;
		this.parent = parent;
		this.treeObject = treeObject;

		
		
		
		sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		position = coord * meshSettings.meshWorldSize ;
		bounds = new Bounds(position,Vector2.one*meshSettings.meshWorldSize);

		meshObject = new GameObject("Terrain Chunk");
		
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		//surface = meshObject.AddComponent<NavMeshSurface>();
		meshRenderer.material = material;

		/*
		surface.center = meshObject.transform.position;
		surface.size = new Vector3(meshSettings.meshWorldSize,100,meshSettings.meshWorldSize);
		surface.collectObjects = CollectObjects.Volume;
		surface.layerMask = LayerMask.GetMask("Default");
		*/
		meshObject.transform.position = new Vector3(position.x,0,position.y);

		meshObject.transform.parent = parent;
		
		SetVisible(false);

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++) 
		{
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex) 
			{
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;


	}
	public void Bake ()
	{
		//surface.BuildNavMesh();
	}
	public void Load() 
	{
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCenter), OnHeightMapReceived);

	}

	public Vector3 GetChunkCoords()
	{
		return meshObject.transform.position;
	}

	 public void SpawnTrees(Vector3 chunkOffset)
	{
		
		float maxViewDist = detailLevels[detailLevels.Length -1].visibleDstThreshold;
		int chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDist/meshSettings.meshWorldSize);
		List<Vector2>points =PoissonDiscSampling.GeneratePoints(50f,new Vector2(241,241),position,10);
	
		RaycastHit hit;
	
		foreach (var point in points)
		{

			if(Physics.Raycast(new Vector3(point.x+meshObject.transform.position.x, 100, point.y + meshObject.transform.position.z ), Vector3.down, out hit, 200f)&& hit.point.y >.3f)
			{

				Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				

				GameObject treeClone = GameObject.Instantiate(treeObject, hit.point,spawnRotation);
				treeClone.transform.parent = meshObject.transform;
				
				
			}
		}
		

	}

	

	void OnHeightMapReceived(object heightMapObject) 
	{
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;
		

		UpdateTerrainChunk ();
	}

	Vector2 viewerPosition 
	{
		get 
		{
			return new Vector2 (viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() 
	{
		if (heightMapReceived) 
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));

			bool wasVisible = IsVisible ();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible) 
			{
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++) 
				{
					if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) 
					{
						lodIndex = i + 1;
					} else 
					{
						break;
					}
				}

				if (lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshes [lodIndex];
					if (lodMesh.hasMesh) {
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh (heightMap, meshSettings);
					}
				}


			}

			if (wasVisible != visible) 
			{
				
				SetVisible (visible);
				if (onVisibilityChanged != null) 
				{
					onVisibilityChanged (this, visible);
					
				}
			}

		}
	}

	public void UpdateCollisionMesh() {
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance (viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes [colliderLODIndex].hasRequestedMesh) {
					lodMeshes [colliderLODIndex].RequestMesh (heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes [colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes [colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

	public void SetVisible(bool visible) 
	{
		meshObject.SetActive (visible);
	}

	public bool IsVisible() 
	{
		return meshObject.activeSelf;
	}

}

class LODMesh {

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
		mesh = ((MeshData)meshDataObject).CreateMesh ();
		hasMesh = true;

		updateCallback ();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}
