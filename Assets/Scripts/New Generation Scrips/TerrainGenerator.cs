using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainGenerator : MonoBehaviour 
{

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;


	public GameObject treeObject;
		

	public Transform viewer;
	public Material mapMaterial;
	HeightMap heightMap;
	
	Vector2 viewedChunkCoord;
	Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	float meshWorldSize;
	int chunksVisibleInViewDst;
	int chunkCount;
	int chunkCountOld;

	int terrainChunkDictionaryOld;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	void Awake() 
	{
		heightMapSettings.noiseSettings.seed = Random.Range(1,10000000);
		heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine,heightMapSettings,viewedChunkCoord);
		
		textureSettings.ApplyToMaterial (mapMaterial);
		textureSettings.UpdateMeshHeight (mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);
		
		UpdateVisibleChunks ();
		Debug.Log(terrainChunkDictionary.Count);
		
	}

	void Start()
	{	
		StartCoroutine(SpawnTrees());
		
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh ();

				
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
			StartCoroutine(SpawnTrees());
		}
		
		
		
	}

	public IEnumerator SpawnTrees()
	{

		yield return new WaitForSeconds(0.5f);

		foreach(var chunk in terrainChunkDictionary)
		{
			chunk.Value.SpawnTrees();
			//chunk.Value.Bake();
		}
	}


	void UpdateVisibleChunks() {
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
		for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) 
		{
			alreadyUpdatedChunkCoords.Add (visibleTerrainChunks [i].coord);
			visibleTerrainChunks [i].UpdateTerrainChunk ();
		}
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) 
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) 
			{
				 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains (viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
						terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();

						
						
					} else{
						TerrainChunk newChunk = new TerrainChunk (viewedChunkCoord,heightMapSettings,meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial, treeObject);
						terrainChunkDictionary.Add (viewedChunkCoord, newChunk);
						
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load();

						

						
					}
				}

			}
		}
		
	}


	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add (chunk);
		} else {
			visibleTerrainChunks.Remove (chunk);
		}
	}

}


[System.Serializable]
public struct LODInfo {
	[Range(0,MeshSettings.numSupportedLOD-1)]
	public int lod;
	public float visibleDstThreshold;


	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}