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
    public int row;
    public int column;

    public Coordinates(Face face, int row, int column)
    {
        this.face = face;
        this.row = row;
        this.column = column;
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

    [Header("Important")]
    public WorldConfig worldConfig = default;
    [SerializeField] float rotationTime = 0.2f;
    [SerializeField] BiomesMenu biomes = default;

    Dictionary<Coordinates, Cell> cells;

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
        cells = new Dictionary<Coordinates, Cell>();
    }

    void CreateWorld()
    {
        //create sun
        InstantiateSun();

        //create front and back face
        CreateFrontBack(0, -90, Face.front);
        CreateFrontBack(worldConfig.faceSize, 90, Face.back);

        //create right and left face
        CreateRightLeft(worldConfig.faceSize - worldConfig.halfCell, -90, Face.right);
        CreateRightLeft(-worldConfig.halfCell, 90, Face.left);

        //create up and down face
        CreateUpDown(worldConfig.faceSize - worldConfig.halfCell, 0, Face.up);
        CreateUpDown(-worldConfig.halfCell, 180, Face.down);
    }

    void InstantiateSun()
    {
        //create sun and set name, and position
        Transform sun = Instantiate(worldConfig.sunPrefab).transform;
        sun.name = "Sun";
        sun.position = transform.position;

        //set sun size
        float size = worldConfig.faceSize - worldConfig.cellsSize;
        sun.localScale = new Vector3(size, size, size);

        //set parent
        sun.parent = transform;
    }

    #region cube

    #region create rows and columns

    void CreateFrontBack(float z, float rotX, Face face)
    {
        Vector3 distFromCenter = worldConfig.halfCube - transform.position;

        //create front or back face
        int xCoords = -1;

        if (face == Face.front)
        {
            for (int x = 0; x < worldConfig.numberCells; x++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.numberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.cellsSize, y * worldConfig.cellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), face, xCoords, y);
                }
            }
        }
        else
        {
            for (int x = worldConfig.numberCells - 1; x >= 0; x--)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.numberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.cellsSize, y * worldConfig.cellsSize, z) - distFromCenter, new Vector3(rotX, 0, 0), face, xCoords, y);
                }
            }
        }
    }

    void CreateRightLeft(float x, float rotZ, Face face)
    {
        Vector3 distFromCenter = worldConfig.halfCube - transform.position;

        //create right or left face
        int xCoords = -1;

        if (face == Face.right)
        {
            for (int z = 0; z < worldConfig.numberCells; z++)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.numberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x, y * worldConfig.cellsSize, z * worldConfig.cellsSize + worldConfig.halfCell) - distFromCenter, new Vector3(0, 0, rotZ), face, xCoords, y);
                }
            }
        }
        else
        {
            for (int z = worldConfig.numberCells - 1; z >= 0; z--)
            {
                xCoords++;

                for (int y = 0; y < worldConfig.numberCells; y++)
                {
                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x, y * worldConfig.cellsSize, z * worldConfig.cellsSize + worldConfig.halfCell) - distFromCenter, new Vector3(0, 0, rotZ), face, xCoords, y);
                }
            }
        }
    }

    void CreateUpDown(float y, float rotX, Face face)
    {
        Vector3 distFromCenter = worldConfig.halfCube - transform.position;

        //create up or down face

        if (face == Face.up)
        {
            for (int x = 0; x < worldConfig.numberCells; x++)
            {
                int yCoords = -1;

                for (int z = 0; z < worldConfig.numberCells; z++)
                {
                    yCoords++;

                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.cellsSize, y, z * worldConfig.cellsSize + worldConfig.halfCell) - distFromCenter, new Vector3(rotX, 0, 0), face, x, yCoords);
                }
            }
        }
        else
        {
            for (int x = 0; x < worldConfig.numberCells; x++)
            {
                int yCoords = -1;

                for (int z = worldConfig.numberCells - 1; z >= 0; z--)
                {
                    yCoords++;

                    //-dist from center to set center at Vector3.zero
                    CreateAndSetCell(new Vector3(x * worldConfig.cellsSize, y, z * worldConfig.cellsSize + worldConfig.halfCell) - distFromCenter, new Vector3(rotX, 0, 0), face, x, yCoords);
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
        cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
    }

    Cell CreateCell(Vector3 position, Vector3 eulerRotation, Face face)
    {
        //create and set position and rotation
        Cell cell = InstantiateCellBasedOnFace(face);
        cell.transform.position = position;
        cell.transform.eulerAngles = eulerRotation;

        //set scale
        float size = worldConfig.cellsSize;
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
    /// Rotate row. AnimTime and canControlPlayer are only for randomize at start, doesn't need in runtime
    /// </summary>
    public void RotateRow(Face startFace, int line, bool toRight, float animTime = 0, bool canControlPlayer = true)
    {
        if (startFace != Face.up && startFace != Face.down)
        {
            //rotate row lateral faces
            worldRotator.RotateLateralRow(line, toRight, animTime, canControlPlayer);
        }
        else
        {
            //rotate row face up or face down
            worldRotator.RotateUpDownRow(startFace, line, toRight, animTime, canControlPlayer);
        }
    }

    /// <summary>
    /// Rotate column. AnimTime and canControlPlayer are only for randomize at start, doesn't need in runtime
    /// </summary>
    public void RotateColumn(Face startFace, int line, bool toUp, float animTime = 0, bool canControlPlayer = true)
    {
        if (startFace != Face.right && startFace != Face.left)
        {
            //front faces (not right or left), rotate front column
            worldRotator.RotateFrontColumn(startFace, line, toUp, animTime, canControlPlayer);
        }
        else
        {
            //rotate column face right or face left
            worldRotator.RotateRightLeftColumn(startFace, line, toUp, animTime, canControlPlayer);
        }
    }

    #endregion
}
