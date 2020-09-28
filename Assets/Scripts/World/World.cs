using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum & struct

[System.Serializable]
public enum ERotateDirection
{
    right, left, up, down
}

[System.Serializable]
public enum EFace
{
    front,
    right,
    back,
    left,
    up,
    down
}

[System.Serializable]
public struct Coordinates
{
    public EFace face;
    public int x;
    public int y;

    public Coordinates(EFace face, int x, int y)
    {
        this.face = face;
        this.x = x;
        this.y = y;
    }
}

#endregion

[AddComponentMenu("Cube Invaders/World/World")]
public class World : MonoBehaviour
{
    [Header("Regen")]
    [SerializeField] bool regen = false;

    [Header("Debug")]
    [SerializeField] bool rotate = false;
    [SerializeField] Transform cameraPlayer = default;
    [SerializeField] int x = default, y = default;
    [SerializeField] ERotateDirection rotateDirection = default;

    [Header("Important")]
    public WorldConfig worldConfig = default;
    public float rotationTime = 0.2f;
    public LevelConfig levelConfig = default;

    public System.Action onStartGame;
    public System.Action onEndRotation;

    public Dictionary<Coordinates, Cell> Cells { get; private set; }

    WorldRotator worldRotator;

    private void OnValidate()
    {
        //click regen to regenerate the world
        if(regen)
        {
            regen = false;

            //start regen
            RegenWorld();
        }
    }

    private void Awake()
    {
        GenerateReferences();

        //try randomize world
        if(RandomizeWorld() == false)
        {
            //else start game after few seconds
            Invoke("StartGame", 1);
        }
    }

    void Update()
    {
        if (rotate)
        {
            rotate = false;
            Rotate(WorldUtility.SelectFace(cameraPlayer), x, y, WorldUtility.LateralFace(cameraPlayer), rotateDirection);
        }
    }

    #region private API

    #region awake

    void GenerateReferences()
    {
        //create world rotator
        worldRotator = new WorldRotator(this);

        //create dictionary
        Cells = new Dictionary<Coordinates, Cell>();
        foreach (Transform child in transform)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell != null)
            {
                Cells.Add(cell.coordinates, cell);
            }
        }
    }

    bool RandomizeWorld()
    {
        //check if there is WorldRandomRotate and start it
        WorldRandomRotate WorldRandomRotate = GetComponent<WorldRandomRotate>();
        if (WorldRandomRotate)
        {
            return WorldRandomRotate.StartRandomize(this);
        }

        return false;
    }

    void StartGame()
    {
        onStartGame?.Invoke();
    }

    #endregion

    #region regen world

    void RegenWorld()
    {
        //remove old world
        RemoveOldWorld();

        //then create new world
        CreateWorld();
    }

    void RemoveOldWorld()
    {
        //remove every child
        foreach (Transform ch in transform)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(ch.gameObject);
            };
#else
            Destroy(ch.gameObject);
#endif
        }

        //then clear dictionary
        Cells = new Dictionary<Coordinates, Cell>();
    }

    void CreateWorld()
    {
        //create sun
        InstantiateSun();

        //create front and back face
        CreateFrontBack(0, -90, EFace.front);
        CreateFrontBack(worldConfig.FaceSize, 90, EFace.back);

        //create right and left face
        CreateRightLeft(worldConfig.FaceSize - worldConfig.HalfCell, -90, EFace.right);
        CreateRightLeft(-worldConfig.HalfCell, 90, EFace.left);

        //create up and down face
        CreateUpDown(worldConfig.FaceSize - worldConfig.HalfCell, 0, EFace.up);
        CreateUpDown(-worldConfig.HalfCell, 180, EFace.down);
    }

    void InstantiateSun()
    {
        //create sun and set name, and position
        Transform sun = Instantiate(worldConfig.SunPrefab).transform;
        sun.name = "Sun";
        sun.position = transform.position;

        //set sun size
        float size = worldConfig.FaceSize - worldConfig.CellsSize;
        sun.localScale = new Vector3(size, size, size);

        //set parent
        sun.parent = transform;
    }

    #region cube

    #region create rows and columns

    void CreateFrontBack(float z, float rotX, EFace face)
    {
        Vector3 distFromCenter = worldConfig.HalfCubePivot - transform.position;

        //create front or back face
        int xCoords = -1;

        if (face == EFace.front)
        {
            for (int x = 0; x < worldConfig.NumberCells; x++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y * worldConfig.CellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), new Coordinates(face, xCoords, y));
                }
            }
        }
        else
        {
            for (int x = worldConfig.NumberCells - 1; x >= 0; x--)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y * worldConfig.CellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), new Coordinates(face, xCoords, y));
                }
            }
        }
    }

    void CreateRightLeft(float x, float rotZ, EFace face)
    {
        Vector3 distFromCenter = worldConfig.HalfCubePivot - transform.position;

        //create right or left face
        int xCoords = -1;

        if (face == EFace.right)
        {
            for (int z = 0; z < worldConfig.NumberCells; z++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x, y * worldConfig.CellsSize, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(0, 0, rotZ), new Coordinates(face, xCoords, y));
                }
            }
        }
        else
        {
            for (int z = worldConfig.NumberCells - 1; z >= 0; z--)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x, y * worldConfig.CellsSize, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(0, 0, rotZ), new Coordinates(face, xCoords, y));
                }
            }
        }
    }

    void CreateUpDown(float y, float rotX, EFace face)
    {
        Vector3 distFromCenter = worldConfig.HalfCubePivot - transform.position;

        //create up or down face

        if (face == EFace.up)
        {
            for (int x = 0; x < worldConfig.NumberCells; x++)
            {
                int yCoords = -1;

                for (int z = 0; z < worldConfig.NumberCells; z++)
                {
                    yCoords++;

                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(rotX, 0, 0), new Coordinates(face, x, yCoords));
                }
            }
        }
        else
        {
            for (int x = 0; x < worldConfig.NumberCells; x++)
            {
                int yCoords = -1;

                for (int z = worldConfig.NumberCells - 1; z >= 0; z--)
                {
                    yCoords++;

                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(rotX, 0, 0), new Coordinates(face, x, yCoords));
                }
            }
        }
    }

    #endregion

    #region create cell

    void CreateAndSetCell(Vector3 position, Vector3 eulerRotation, Coordinates coordinates)
    {
        //create cell
        Cell cell = CreateCell(position, eulerRotation, coordinates.face);

        //set it
        Cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
    }

    Cell CreateCell(Vector3 position, Vector3 eulerRotation, EFace face)
    {
        //create and set position and rotation
        Cell cell = InstantiateCellBasedOnFace(face);
        cell.transform.position = position;
        cell.transform.eulerAngles = eulerRotation;

        //set scale
        float size = worldConfig.CellsSize;
        cell.transform.localScale = new Vector3(size, size, size);

        //set parent
        cell.transform.parent = transform;

        return cell;
    }

    Cell InstantiateCellBasedOnFace(EFace face)
    {
        //create biome based on face
        switch (face)
        {
            case EFace.front:
                return Instantiate(levelConfig.Biomes.front);
            case EFace.right:
                return Instantiate(levelConfig.Biomes.right);
            case EFace.back:
                return Instantiate(levelConfig.Biomes.back);
            case EFace.left:
                return Instantiate(levelConfig.Biomes.left);
            case EFace.up:
                return Instantiate(levelConfig.Biomes.up);
            case EFace.down:
                return Instantiate(levelConfig.Biomes.down);
        }

        return null;
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region public API

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="startFace">face to start from</param>
    /// <param name="x">column</param>
    /// <param name="y">row</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void Rotate(EFace startFace, int x, int y, EFace lookingFace, ERotateDirection rotateDirection)
    {
        worldRotator.Rotate(startFace, x, y, lookingFace, rotateDirection, rotationTime, true);
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="startFace">face to start from</param>
    /// <param name="x">column</param>
    /// <param name="y">row</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    /// <param name="rotationSpeed">time to do the animation</param>
    public void RandomRotate(EFace startFace, int x, int y, ERotateDirection rotateDirection, float rotationSpeed)
    {
        worldRotator.Rotate(startFace, x, y, EFace.front, rotateDirection, rotationSpeed, false);
    }

    /// <summary>
    /// returns the position in the world of the cell at those coordinates
    /// </summary>
    public Vector3 CoordinatesToPosition(Coordinates coordinates)
    {
        Vector3 v = Vector3.zero;

        Vector3 halfCubePivot_POSITIVE = transform.position + worldConfig.HalfCubePivot;                            //1, 1, 1.5f
        Vector3 halfCubePivot_NEGATIVE = transform.position - worldConfig.HalfCubePivot;                            //-1, -1, -1.5f
        Vector3 halfCube_POSITIVE = transform.position + worldConfig.HalfCube;

        switch (coordinates.face)
        {
            case EFace.front:
                v.x = halfCubePivot_NEGATIVE.x + (coordinates.x * worldConfig.CellsSize);                           //-1, 1
                v.y = halfCubePivot_NEGATIVE.y + (coordinates.y * worldConfig.CellsSize);                           //-1, 1
                v.z = halfCubePivot_NEGATIVE.z;                                                                     //-1.5f
                break;
            case EFace.right:
                v.x = halfCubePivot_POSITIVE.x + worldConfig.HalfCell;                                              //1.5f
                v.y = halfCubePivot_NEGATIVE.y + (coordinates.y * worldConfig.CellsSize);                           //-1, 1
                v.z = halfCubePivot_NEGATIVE.z + (coordinates.x * worldConfig.CellsSize) + worldConfig.HalfCell;    //-1, 1
                break;
            case EFace.back:
                v.x = halfCubePivot_POSITIVE.x - (coordinates.x * worldConfig.CellsSize);                           //1, -1
                v.y = halfCubePivot_NEGATIVE.y + (coordinates.y * worldConfig.CellsSize);                           //-1, 1
                v.z = halfCubePivot_POSITIVE.z;                                                                     //1.5f
                break;
            case EFace.left:
                v.x = halfCubePivot_NEGATIVE.x - worldConfig.HalfCell;                                              //-1.5f
                v.y = halfCubePivot_NEGATIVE.y + (coordinates.y * worldConfig.CellsSize);                           //-1, 1
                v.z = halfCubePivot_POSITIVE.z - (coordinates.x * worldConfig.CellsSize) - worldConfig.HalfCell;    //1, -1
                break;
            case EFace.up:
                v.x = halfCubePivot_NEGATIVE.x + (coordinates.x * worldConfig.CellsSize);                           //-1, 1
                v.y = halfCubePivot_POSITIVE.y + worldConfig.HalfCell;                                              //1.5f
                v.z = halfCubePivot_NEGATIVE.z + (coordinates.y * worldConfig.CellsSize) + worldConfig.HalfCell;    //-1, 1
                break;
            case EFace.down:
                v.x = halfCubePivot_NEGATIVE.x + (coordinates.x * worldConfig.CellsSize);                           //-1, 1
                v.y = halfCubePivot_NEGATIVE.y - worldConfig.HalfCell;                                              //-1.5f
                v.z = halfCubePivot_POSITIVE.z - (coordinates.y * worldConfig.CellsSize) - worldConfig.HalfCell;    //1, -1
                break;
        }

        return v;
    }

    #endregion
}
