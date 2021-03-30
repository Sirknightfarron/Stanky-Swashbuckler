using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrainSkript : MonoBehaviour
{
    
    
    [SerializeField]
    public LODInfo[] detailLevel;
    public static float maxViewDist;
    public Transform viewer;
    public static Vector2 viewerPosition;
    public static Vector2 viewerPositonOld;
    const float distanceThresholdBeforeChunkUpdate = 25f;
    const float sqrtdistanceThresholdBeforeChunkUpdate = distanceThresholdBeforeChunkUpdate * distanceThresholdBeforeChunkUpdate;
    int chunkSize;
    int chunksVisibleInViewDist;

    static MapGenerator mapGenerator;

    // Dic mit allen sichtbaren Chunks um doppeltes Erzeugen zu verhindern
    Dictionary<Vector2, TerrainChunk> terrainChunkDictornary = new Dictionary<Vector2, TerrainChunk>();
     static List<TerrainChunk> ChunksVisibleLastUpdate = new List<TerrainChunk>();
 
    private int oldCunkX;
    private int oldCunkZ;
    public Material mapMaterial;

    private void Start()

    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDist = detailLevel[detailLevel.Length -1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChunkSize_plus_1 - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        //oldCunkX = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        //oldCunkZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        UpdatevisibleChunks();
        
    }
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        if ((viewerPositonOld - viewerPosition).sqrMagnitude > sqrtdistanceThresholdBeforeChunkUpdate)
        {
            viewerPositonOld = viewerPosition;
             UpdatevisibleChunks();
        }
    }
    private void UpdatevisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);

        int currentChunkCoordZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        for (int i = 0; i< ChunksVisibleLastUpdate.Count; i++)
        {
            ChunksVisibleLastUpdate[i].setVisible(false);
        }
        // bei veränderung wird die difference der chunkkordinaten als referenz zur loeschung des alten meshes genutz
        /*if (currentChunkCoordX != oldCunkX)
        {
            float dif = oldCunkX - currentChunkCoordX;
            // der nicht benötigite chunk ist hinter der bewegungsrichtung mit dem abstand der viewdistance in cunkspace minus 1 für die verwendung des alten sicherens oldcunkx
            float oldXChunk = oldCunkX + dif + dif * (chunksVisibleInViewDist - 1);


            //kordinaten des nicht mehr benötigiens chunks alle Z Chunks für eine x richtung
            for (int zOffset = -chunksVisibleInViewDist; zOffset <= chunksVisibleInViewDist; zOffset++)
            {
                Vector2 olcChunkCoor = new Vector3(oldXChunk, oldCunkZ + zOffset);
                terrainChunkDictornary[olcChunkCoor].dispose();

            }
            oldCunkX = currentChunkCoordX;
        }
           
        if (currentChunkCoordZ != oldCunkZ)            
        {                
            float dif = oldCunkZ - currentChunkCoordZ;                
            float oldYChunk = oldCunkZ + dif + dif * (chunksVisibleInViewDist - 1);
                
            //kordinaten des nicht mehr benötigiens chunks alle X Chunks für eine Z richtungsschrit wie oben nur in andere richtung
                
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)               
            {                   
                Vector2 olcChunkCoor = new Vector3( oldCunkX + xOffset, oldYChunk );
                    terrainChunkDictornary[olcChunkCoor].dispose();
             }
            oldCunkZ = currentChunkCoordZ;
        



        }*/


         
        //Schleife ueber alle sichtbaren Chunks vor und hintereinem
        for (int zOffset = -chunksVisibleInViewDist; zOffset <= chunksVisibleInViewDist; zOffset++)
        {
            for(int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector3(currentChunkCoordX + xOffset, currentChunkCoordZ + zOffset);

                // wenn das Chunk vorhanden ist wird es geupdatet 
                if (terrainChunkDictornary.ContainsKey(viewedChunkCoord))
                {
                    // update Terrain
                    terrainChunkDictornary[viewedChunkCoord].updateTerrianChunk();
                    
                    
                }
                else
                {
                    // wenn das Chunk nicht vorhanden ist wird es erstellt
                    terrainChunkDictornary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevel, transform, mapMaterial));
                }
            }
        }
    }
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        float despornRange;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataRecived;
        int perviousLOD =-1;


        public TerrainChunk(Vector2 coord, int size,LODInfo[] detailLevels , Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("TerrainChunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            setVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i =0; i< detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, updateTerrianChunk);

            }

            mapGenerator.RequestMapData(position, onMapDataResived);


        }
        void onMapDataResived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataRecived = true;
            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.ColorMap, MapGenerator.mapChunkSize_plus_1, MapGenerator.mapChunkSize_plus_1);
            meshRenderer.material.mainTexture = texture;
            updateTerrianChunk();

        }
        void onMeshDataResived(MeshData meshData)
        {

            meshFilter.mesh = meshData.CreateMesh();
        }


        public void updateTerrianChunk()
        {
            if (mapDataRecived) {
                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistFromNearestEdge <= maxViewDist;
                if (visible)
                {
                    ChunksVisibleLastUpdate.Add(this);
                    int LodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {
                            LodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (LodIndex != perviousLOD)
                    {
                        LODMesh lodMesh = lodMeshes[LodIndex];
                        if (lodMesh.hasMesh)
                        {
                            perviousLOD = LodIndex;
                            meshFilter.mesh = lodMesh.mesh;

                        }
                        else if (!lodMesh.hasRequestedMesh) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                }

                setVisible(visible);
            }
        }
        public void setVisible(bool visible)
        {
            meshObject.SetActive(visible);

        }
        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
        public void dispose()
        {
            meshObject.SetActive(false);
        }


    }
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;
        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }
        void onMeshRecieved(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
        }
        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.requestMeshDate(mapData,lod, onMeshRecieved);

        }
    }
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        
        public float visibleDstThreshold;
    }
}
