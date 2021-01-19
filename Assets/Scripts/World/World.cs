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

    public Coordinates(EFace face, Vector2Int v)
    {
        this.face = face;
        this.x = v.x;
        this.y = v.y;
    }

    public override string ToString()
    {
        return "Face " + face + " (" + x + "," + y + ")";
    }

    public static Coordinates operator +(Coordinates a, Vector2Int b) => new Coordinates(a.face, a.x + b.x, a.y + b.y);

    public static bool operator !=(Coordinates a, Coordinates b) => a.face != b.face || a.x != b.x || a.y != b.y;
    public static bool operator ==(Coordinates a, Coordinates b) => a.face == b.face && a.x == b.x && a.y == b.y;

    public override bool Equals(object obj)
    {
        return obj is Coordinates coordinates &&
               face == coordinates.face &&
               x == coordinates.x &&
               y == coordinates.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 628288303;
        hashCode = hashCode * -1521134295 + face.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}

#endregion

[AddComponentMenu("Cube Invaders/World/World")]
public class World : MonoBehaviour
{
    #region variables

    [Header("Base")]
    public WorldConfig worldConfig;
    public RandomWorldConfig randomWorldConfig;

    [Header("Important")]
    public BiomesConfig biomesConfig;

    public System.Action onEndRotation;

    public Dictionary<Coordinates, Cell> Cells;

    WorldRotator worldRotator;

    #endregion

    void Awake()
    {
        GenerateReferences();
    }

    void OnDestroy()
    {
        //be sure to reset event
        onEndRotation = null;
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

    #endregion

    #region regen world

    void RemoveOldWorld()
    {
        //remove every child
        foreach (Transform ch in transform)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(ch.gameObject);
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
        CreateFrontBack();

        //create right and left face
        CreateRightLeft();

        //create up and down face
        CreateUpDown();
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

    void CreateFrontBack()
    {
        for (int x = 0; x < worldConfig.NumberCells; x++)
        {
            for (int y = 0; y < worldConfig.NumberCells; y++)
            {
                //front
                CreateAndSetCell(new Vector3(0, 180, 0), new Coordinates(EFace.front, x, y));

                //back
                CreateAndSetCell(new Vector3(0, 0, 0), new Coordinates(EFace.back, x, y));
            }
        }
    }

    void CreateRightLeft()
    {
        for (int z = 0; z < worldConfig.NumberCells; z++)
        {
            for (int y = 0; y < worldConfig.NumberCells; y++)
            {
                //right
                CreateAndSetCell(new Vector3(0, 90, 0), new Coordinates(EFace.right, z, y));

                //left
                CreateAndSetCell(new Vector3(0, -90, 0), new Coordinates(EFace.left, z, y));
            }
        }
    }

    void CreateUpDown()
    {
        for (int x = 0; x < worldConfig.NumberCells; x++)
        {
            //rotate 180 y axis just to look same direction (when rotate on local z axis - RotateAngleOrSide)
            for (int z = 0; z < worldConfig.NumberCells; z++)
            {
                //up
                CreateAndSetCell(new Vector3(-90, 180, 0), new Coordinates(EFace.up, x, z));

                //down
                CreateAndSetCell(new Vector3(90, 180, 0), new Coordinates(EFace.down, x, z));
            }
        }
    }

    #endregion

    #region create cell

    void CreateAndSetCell(Vector3 eulerRotation, Coordinates coordinates)
    {
        //create cell
        Cell cell = CreateCell(coordinates, eulerRotation);

        //set it
        Cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
        cell.SelectModel(worldConfig.NumberCells);
    }

    Cell CreateCell(Coordinates coordinates, Vector3 eulerRotation)
    {
        //create and set position and rotation
        Cell cell = InstantiateCellBasedOnFace(coordinates.face);
        cell.transform.position = CoordinatesToPosition(coordinates);
        cell.transform.eulerAngles = eulerRotation;
        cell.transform.Rotate(RotateAngleOrSide(coordinates), Space.Self);

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
                return Instantiate(biomesConfig.Front);
            case EFace.right:
                return Instantiate(biomesConfig.Right);
            case EFace.back:
                return Instantiate(biomesConfig.Back);
            case EFace.left:
                return Instantiate(biomesConfig.Left);
            case EFace.up:
                return Instantiate(biomesConfig.Up);
            case EFace.down:
                return Instantiate(biomesConfig.Down);
        }

        return null;
    }

    Vector3 RotateAngleOrSide(Coordinates coordinates)
    {
        //left
        if (coordinates.x <= 0)
        {
            //down
            if (coordinates.y <= 0)
            {
                return new Vector3(0, 0, 180);
            }
            //center or up
            else
            {
                return new Vector3(0, 0, -90);
            }
        }
        //right
        else if (coordinates.x >= worldConfig.NumberCells - 1)
        {
            //up
            if (coordinates.y >= worldConfig.NumberCells - 1)
            {
                return Vector3.zero;
            }
            //center or down
            else
            {
                return new Vector3(0, 0, 90);
            }
        }
        //center
        else
        {
            //down
            if (coordinates.y <= 0)
            {
                return new Vector3(0, 0, 180);
            }
            //center or up
            else
            {
                return Vector3.zero;
            }
        }
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region public API

    /// <summary>
    /// Generate the world
    /// </summary>
    public void RegenWorld()
    {
        //remove old world
        RemoveOldWorld();

        //then create new world
        CreateWorld();
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="coordinates">coordinates to rotate</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void Rotate(Coordinates coordinates, EFace lookingFace, ERotateDirection rotateDirection)
    {
        worldRotator.Rotate(coordinates, lookingFace, rotateDirection);
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="coordinates">coordinates to rotate</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void Rotate(Coordinates[] coordinates, EFace lookingFace, ERotateDirection rotateDirection)
    {
        worldRotator.Rotate(coordinates, lookingFace, rotateDirection);
    }

    /// <summary>
    /// Start random rotation
    /// </summary>
    public void RandomRotate()
    {
        new WorldRandomRotator(this).StartRandomize();
    }

    /// <summary>
    /// Returns the position in the world of the cell at these coordinates
    /// <param name="distanceFromWorld">distance from the cell position</param>
    /// </summary>
    public Vector3 CoordinatesToPosition(Coordinates coordinates, float distanceFromWorld = 0)
    {
        //position is index * size (then one axis is -distanceFromWorld or FaceSize + distanceFromWorld)
        Vector3 v = Vector3.zero;

        switch (coordinates.face)
        {
            case EFace.front:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = -distanceFromWorld;
                break;
            case EFace.right:
                v.x = worldConfig.FaceSize + distanceFromWorld;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = coordinates.x * worldConfig.CellsSize;
                break;
            case EFace.back:
                //x inverse of front
                v.x = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = worldConfig.FaceSize + distanceFromWorld;
                break;
            case EFace.left:
                //z inverse of right
                v.x = -distanceFromWorld;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                break;
            case EFace.up:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = worldConfig.FaceSize + distanceFromWorld;
                v.z = coordinates.y * worldConfig.CellsSize;
                break;
            case EFace.down:
                //z inverse of up
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = -distanceFromWorld;
                v.z = (worldConfig.NumberCells - 1 - coordinates.y) * worldConfig.CellsSize;
                break;
        }

        //is the angle in the lower left (front face) of the cube
        Vector3 cubeStartPosition = transform.position - worldConfig.HalfCube;

        //return start position + cell position + pivot position (cause we start from the angle of the cube, but we need the center of the cell as pivot)
        return cubeStartPosition + v + worldConfig.PivotBasedOnFace(coordinates.face);
    }

    public List<Cell> GetCellsAround(Coordinates coordinates)
    {
        List<Cell> cellsAround = new List<Cell>();
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        //foreach direction
        foreach (Vector2Int direction in directions)
        {
            //if there is a cell and is != null
            if (Cells.ContainsKey(coordinates + direction))
            {
                Cell cell = Cells[coordinates + direction];
                if (cell != null)
                {
                    //add to the list
                    cellsAround.Add(cell);
                }
            }
        }

        return cellsAround;
    }

    #endregion
}
