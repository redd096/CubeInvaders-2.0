using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum & struct

[System.Serializable]
public enum Face
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
    public Face face;
    public int x;
    public int y;

    public Coordinates(Face face, int x, int y)
    {
        this.face = face;
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public enum EBiomes
{
    Forest,
    Canyon,
    Volcano,
    Sea,
    Swamp,
    Radar,
    Nothing
}

[System.Serializable]
public struct BiomesMenu
{
    public Cell front, right, back, left, up, down;
}

#endregion

public class World : MonoBehaviour
{
    [Header("Regen")]
    [SerializeField] bool regen = false;

    [Header("Debug")]
    [SerializeField] Transform cameraPlayer = default;
    [SerializeField] int x = default, y = default;
    [SerializeField] bool toForward = default, rotateRow = default, rotateColumn = default;

    [Header("Important")]
    public WorldConfig worldConfig = default;
    public float rotationTime = 0.2f;
    [SerializeField] BiomesMenu biomes = default;

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
        worldRotator = new WorldRotator(this);

        //create dictionary
        Cells = new Dictionary<Coordinates, Cell>();
        foreach(Transform child in transform)
        {
            Cell cell = child.GetComponent<Cell>();
            if(cell != null)
            {
                Cells.Add(cell.coordinates, cell);
            }
        }
    }

    private void Update()
    {
        if (rotateRow)
        {
            rotateRow = false;
            Rotate(WorldUtility.SelectFace(cameraPlayer), x, y, WorldUtility.LateralFace(cameraPlayer), toForward ? (byte)0 : (byte)1, true);
            //RotateRow(face, y, toForward, true);
        }
        if (rotateColumn)
        {
            rotateColumn = false;
            Rotate(WorldUtility.SelectFace(cameraPlayer), x, y, WorldUtility.LateralFace(cameraPlayer), toForward ? (byte)2 : (byte)3, true);
            //RotateColumn(face, x, toForward, true);
        }
    }

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
        CreateFrontBack(0, -90, Face.front);
        CreateFrontBack(worldConfig.FaceSize, 90, Face.back);

        //create right and left face
        CreateRightLeft(worldConfig.FaceSize - worldConfig.HalfCell, -90, Face.right);
        CreateRightLeft(-worldConfig.HalfCell, 90, Face.left);

        //create up and down face
        CreateUpDown(worldConfig.FaceSize - worldConfig.HalfCell, 0, Face.up);
        CreateUpDown(-worldConfig.HalfCell, 180, Face.down);
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

    void CreateFrontBack(float z, float rotX, Face face)
    {
        Vector3 distFromCenter = worldConfig.HalfCube - transform.position;

        //create front or back face
        int xCoords = -1;

        if (face == Face.front)
        {
            for (int x = 0; x < worldConfig.NumberCells; x++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y * worldConfig.CellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), face, xCoords, y);
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
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y * worldConfig.CellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), face, xCoords, y);
                }
            }
        }
    }

    void CreateRightLeft(float x, float rotZ, Face face)
    {
        Vector3 distFromCenter = worldConfig.HalfCube - transform.position;

        //create right or left face
        int xCoords = -1;

        if (face == Face.right)
        {
            for (int z = 0; z < worldConfig.NumberCells; z++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.NumberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x, y * worldConfig.CellsSize, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(0, 0, rotZ), face, xCoords, y);
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
                    CreateAndSetCell(new Vector3(x, y * worldConfig.CellsSize, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(0, 0, rotZ), face, xCoords, y);
                }
            }
        }
    }

    void CreateUpDown(float y, float rotX, Face face)
    {
        Vector3 distFromCenter = worldConfig.HalfCube - transform.position;

        //create up or down face

        if (face == Face.up)
        {
            for (int x = 0; x < worldConfig.NumberCells; x++)
            {
                int yCoords = -1;

                for (int z = 0; z < worldConfig.NumberCells; z++)
                {
                    yCoords++;

                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(rotX, 0, 0), face, x, yCoords);
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
                    CreateAndSetCell(new Vector3(x * worldConfig.CellsSize, y, z * worldConfig.CellsSize + worldConfig.HalfCell) - distFromCenter, new Vector3(rotX, 0, 0), face, x, yCoords);
                }
            }
        }
    }

    #endregion

    #region create cell

    void CreateAndSetCell(Vector3 position, Vector3 eulerRotation, Face face, int x, int y)
    {
        //create cell
        Cell cell = CreateCell(position, eulerRotation, face);

        //set it
        Coordinates coordinates = new Coordinates(face, x, y);
        Cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
    }

    Cell CreateCell(Vector3 position, Vector3 eulerRotation, Face face)
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

    Cell InstantiateCellBasedOnFace(Face face)
    {
        //create biome based on face
        switch (face)
        {
            case Face.front:
                return Instantiate(biomes.front);
            case Face.right:
                return Instantiate(biomes.right);
            case Face.back:
                return Instantiate(biomes.back);
            case Face.left:
                return Instantiate(biomes.left);
            case Face.up:
                return Instantiate(biomes.up);
            case Face.down:
                return Instantiate(biomes.down);
        }

        return null;
    }

    #endregion

    #endregion

    #endregion

    #region public API

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="startFace">face to start from</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="x">column</param>
    /// <param name="y">row</param>
    /// <param name="rotateDirection">0 = right, 1 = left, 2 = up, 3 = down</param>
    /// <param name="playerMove">is a player move? Or is from editor (randomize at start for example)</param>
    public void Rotate(Face startFace, int x, int y, Face lookingFace, byte rotateDirection, bool playerMove)
    {
        //rotate row
        if (rotateDirection == 0 || rotateDirection == 1)
        {
            bool forward = rotateDirection == 0;

            if (startFace == Face.up || startFace == Face.down)
            {
                //if face up or face down, the inputs are differents based on the rotation of the camera
                switch (lookingFace)
                {
                    case Face.front:
                        worldRotator.RotateUpDownRow(startFace, y, forward, playerMove);
                        break;
                    case Face.right:
                        if (startFace == Face.up)
                            worldRotator.RotateFrontColumn(startFace, x, forward, playerMove);
                        else
                            worldRotator.RotateFrontColumn(startFace, x, !forward, playerMove);
                        break;
                    case Face.back:
                        worldRotator.RotateUpDownRow(startFace, y, !forward, playerMove);
                        break;
                    case Face.left:
                        if (startFace == Face.up)
                            worldRotator.RotateFrontColumn(startFace, x, !forward, playerMove);
                        else
                            worldRotator.RotateFrontColumn(startFace, x, forward, playerMove);
                        break;
                }
            }
            else
            {
                //else just rotate row lateral faces
                worldRotator.RotateLateralRow(y, forward, playerMove);
            }
        }
        //rotate column
        else
        {
            bool forward = rotateDirection == 2;

            //if face up or face down, the inputs are differents based on the rotation of the camera
            if (startFace == Face.up || startFace == Face.down)
            {
                switch (lookingFace)
                {
                    case Face.front:
                        worldRotator.RotateFrontColumn(startFace, x, forward, playerMove);
                        break;
                    case Face.right:
                        if (startFace == Face.up)
                            worldRotator.RotateUpDownRow(startFace, y, !forward, playerMove);
                        else
                            worldRotator.RotateUpDownRow(startFace, y, forward, playerMove);
                        break;
                    case Face.back:
                        worldRotator.RotateFrontColumn(startFace, x, !forward, playerMove);
                        break;
                    case Face.left:
                        if (startFace == Face.up)
                            worldRotator.RotateUpDownRow(startFace, y, forward, playerMove);
                        else
                            worldRotator.RotateUpDownRow(startFace, y, !forward, playerMove);
                        break;
                }
            }
            else
            {
                //else just rotate column
                if (startFace == Face.right || startFace == Face.left)
                {
                    //rotate column face right or left
                    worldRotator.RotateRightLeftColumn(startFace, x, forward, playerMove);
                }
                else
                {
                    //rotate column front faces (front, up, back, down)
                    worldRotator.RotateFrontColumn(startFace, x, forward, playerMove);
                }
            }
        }
    }

    /// <summary>
    /// Rotate row. AnimTime and canControlPlayer are only for randomize at start, doesn't need in runtime
    /// </summary>
    public void RotateRow(Face startFace, int line, bool toRight, bool playerMove)
    {
        if (startFace != Face.up && startFace != Face.down)
        {
            //rotate row lateral faces
            worldRotator.RotateLateralRow(line, toRight, playerMove);
        }
        else
        {
            //rotate row face up or face down
            worldRotator.RotateUpDownRow(startFace, line, toRight, playerMove);
        }
    }

    /// <summary>
    /// Rotate column. AnimTime and canControlPlayer are only for randomize at start, doesn't need in runtime
    /// </summary>
    public void RotateColumn(Face startFace, int line, bool toUp, bool playerMove)
    {
        if (startFace != Face.right && startFace != Face.left)
        {
            //front faces (not right or left), rotate front column
            worldRotator.RotateFrontColumn(startFace, line, toUp, playerMove);
        }
        else
        {
            //rotate column face right or face left
            worldRotator.RotateRightLeftColumn(startFace, line, toUp, playerMove);
        }
    }

    #endregion
}
