using System.Collections.Generic;
using UnityEngine;
using redd096;

public static class WorldUtility
{
    #region select index

    /// <summary>
    /// add or subtract 1 at index, when reached limit or lesser then 0, restart
    /// </summary>
    public static int SelectIndex(int index, bool added, int maxLimit)
    {
        //move right or move left, when exceed limit, restart
        if (added)
        {
            index += 1;

            if (index >= maxLimit)
                index = 0;
        }
        else
        {
            index -= 1;

            if (index < 0)
                index = maxLimit - 1;
        }

        return index;
    }

    /// <summary>
    /// Select new cell
    /// </summary>
    public static Coordinates SelectCell(EFace startFace, int x, int y, EFace lookingFace, ERotateDirection rotateDirection)
    {
        Coordinates selectedCell = new Coordinates(startFace, x, y);

        //select right left
        if (rotateDirection == ERotateDirection.right || rotateDirection == ERotateDirection.left)
        {
            bool forward = rotateDirection == ERotateDirection.right;

            if (startFace == EFace.up || startFace == EFace.down)
            {
                //if face up or face down, the inputs are differents based on the rotation of the camera
                switch (lookingFace)
                {
                    case EFace.front:
                        selectedCell.x = SelectIndex(x, forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.right:
                        if (startFace == EFace.up)
                            selectedCell.y = SelectIndex(y, forward, GameManager.instance.world.worldConfig.NumberCells);
                        else
                            selectedCell.y = SelectIndex(y, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.back:
                        selectedCell.x = SelectIndex(x, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.left:
                        if (startFace == EFace.up)
                            selectedCell.y = SelectIndex(y, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        else
                            selectedCell.y = SelectIndex(y, forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                }
            }
            else
            {
                //else just select row lateral faces
                selectedCell.x = SelectIndex(x, forward, GameManager.instance.world.worldConfig.NumberCells);
            }
        }
        //select up down
        else
        {
            bool forward = rotateDirection == ERotateDirection.up;

            //if face up or face down, the inputs are differents based on the rotation of the camera
            if (startFace == EFace.up || startFace == EFace.down)
            {
                switch (lookingFace)
                {
                    case EFace.front:
                        selectedCell.y = SelectIndex(y, forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.right:
                        if (startFace == EFace.up)
                            selectedCell.x = SelectIndex(x, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        else
                            selectedCell.x = SelectIndex(x, forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.back:
                        selectedCell.y = SelectIndex(y, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                    case EFace.left:
                        if (startFace == EFace.up)
                            selectedCell.x = SelectIndex(x, forward, GameManager.instance.world.worldConfig.NumberCells);
                        else
                            selectedCell.x = SelectIndex(x, !forward, GameManager.instance.world.worldConfig.NumberCells);
                        break;
                }
            }
            else
            {
                //else just select column
                selectedCell.y = SelectIndex(y, forward, GameManager.instance.world.worldConfig.NumberCells);
            }
        }

        return selectedCell;
    }

    #endregion

    #region find face

    /// <summary>
    /// Face up, right, down, left
    /// </summary>
    public static EFace FindFaceUpToRight(EFace startFace, bool toRight)
    {
        switch (startFace)
        {
            case EFace.up:
                if (toRight)
                    return EFace.right;
                else
                    return EFace.left;
            case EFace.right:
                if (toRight)
                    return EFace.down;
                else
                    return EFace.up;
            case EFace.down:
                if (toRight)
                    return EFace.left;
                else
                    return EFace.right;
            case EFace.left:
                if (toRight)
                    return EFace.up;
                else
                    return EFace.down;
        }

        return startFace;
    }

    /// <summary>
    /// Face front, up, back, down
    /// </summary>
    public static EFace FindFaceFrontToUp(EFace startFace, bool toUp)
    {
        switch (startFace)
        {
            case EFace.front:
                if (toUp)
                    return EFace.up;
                else
                    return EFace.down;
            case EFace.up:
                if (toUp)
                    return EFace.back;
                else
                    return EFace.front;
            case EFace.back:
                if (toUp)
                    return EFace.down;
                else
                    return EFace.up;
            case EFace.down:
                if (toUp)
                    return EFace.front;
                else
                    return EFace.back;
        }

        return startFace;
    }

    /// <summary>
    /// Get random face, but ignoring previous in queue
    /// </summary>
    public static EFace GetRandomFace(Queue<EFace> facesQueue, int numberOfPreviousFacesToIgnore)
    {
        //check every possible face
        List<EFace> faces = new List<EFace>();
        for (int i = 0; i < System.Enum.GetNames(typeof(EFace)).Length; i++)
        {
            //if not inside facesQueue, add to list
            EFace tryingFace = (EFace)i;
            if (facesQueue.Contains(tryingFace) == false)
            {
                faces.Add(tryingFace);
            }
        }

        //select random face in list
        EFace selectedFace = faces[Random.Range(0, faces.Count)];

        //add to queue
        facesQueue.Enqueue(selectedFace);

        //clamp list of faces to ignore
        if (facesQueue.Count > numberOfPreviousFacesToIgnore)
            facesQueue.Dequeue();

        return selectedFace;
    }

    /// <summary>
    /// Get opposite face
    /// </summary>
    public static EFace GetOppositeFace(EFace currentFace)
    {
        switch (currentFace)
        {
            case EFace.front:
                return EFace.back;
            case EFace.right:
                return EFace.left;
            case EFace.back:
                return EFace.front;
            case EFace.left:
                return EFace.right;
            case EFace.up:
                return EFace.down;
            case EFace.down:
                return EFace.up;
            default:
                return EFace.front;
        }
    }

    /// <summary>
    /// In possible cells, remove everyone where overlap
    /// </summary>
    /// <param name="position">current position, used to calculate distance</param>
    /// <param name="coordinatesToAttackPosition">coordinates to attack, used to calculate distance</param>
    /// <param name="possibleCells">possible cells to teleport</param>
    public static void CheckOverlap(Vector3 position, Vector3 coordinatesToAttackPosition, List<Cell> possibleCells)
    {
        //get distance
        float distance = Vector3.Distance(position, coordinatesToAttackPosition);

        //foreach possible cell
        foreach (Cell cell in possibleCells.CreateCopy())
        {
            //if overlap in new position, remove from list
            Vector3 newPosition = GameManager.instance.world.CoordinatesToPosition(cell.coordinates, distance);   //adjacent coordinates, but same distance
            if (Physics.OverlapBox(newPosition, Vector3.one * 0.2f, Quaternion.identity, CreateLayer.LayerAllExcept(""), QueryTriggerInteraction.Collide).Length > 0)
                possibleCells.Remove(cell);
        }
    }

    #endregion

    #region face based on transform

    /// <summary>
    /// returns what face the transform is looking at
    /// </summary>
    public static EFace SelectFace(Transform transform)
    {
        if (WorldMath.NegativeAxis(transform.eulerAngles.x) > 40)
            return EFace.up;
        else if (WorldMath.NegativeAxis(transform.eulerAngles.x) < -40)
            return EFace.down;
        else
            return LateralFace(transform);
    }

    /// <summary>
    /// returns one of the lateral faces
    /// </summary>
    public static EFace LateralFace(Transform transform)
    {
        if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= 45 && WorldMath.NegativeAxis(transform.eulerAngles.y) > -45)
        {
            return EFace.front;
        }
        else if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= -45 && WorldMath.NegativeAxis(transform.eulerAngles.y) > -135)
        {
            return EFace.right;
        }
        else if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= 135 && WorldMath.NegativeAxis(transform.eulerAngles.y) > 45)
        {
            return EFace.left;
        }
        else// if (Math.NegativeAxis(transform.eulerAngles.y) <= -135 || Math.NegativeAxis(transform.eulerAngles.y) > 135)
        {
            return EFace.back;
        }
    }

    #endregion

    #region rotate vector based on face

    /// <summary>
    /// Rotate vector based on face
    /// </summary>
    public static Vector3 RotateTowardsFace(Vector3 current, EFace face)
    {
        switch (face)
        {
            case EFace.front:
                return current;
            case EFace.right:
                return new Vector3(-current.z, current.y, current.x);
            case EFace.back:
                return new Vector3(-current.x, current.y, -current.z);
            case EFace.left:
                return new Vector3(current.z, current.y, -current.x);
            case EFace.up:
                return new Vector3(current.x, -current.z, current.y);
            case EFace.down:
                return new Vector3(current.x, current.z, -current.y);
        }

        return current;
    }

    #endregion
}

public static class WorldMath
{
    public static int EqualN(int n)
    {
        //0 -> 0
        //1 -> 1
        //2 -> 2

        return n;
    }

    public static int InverseN(int n, int numberCells)
    {
        //0 -> 2
        //1 -> 1
        //2 -> 0

        return numberCells - 1 - n;
    }

    /// <summary>
    /// use negative axis, for using eulerAngles
    /// </summary>
    public static float NegativeAxis(float axis)
    {
        //use negative values
        if (axis > 180)
            axis -= 360;

        return axis;
    }
}

public static class Vector2Math
{
    #region another

    public static Vector2Int EqualEqual(int x, int y)
    {
        //0,0 -> 0,0   //1,0 -> 0,1   //2,0 -> 0,2
        //0,1 -> 1,0   //1,1 -> 1,1   //2,1 -> 1,2
        //0,2 -> 2,0   //1,2 -> 2,1   //2,2 -> 2,2

        int newX = WorldMath.EqualN(y);
        int newY = WorldMath.EqualN(x);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int EqualInverse(int x, int y, int numberCells)
    {
        //0,0 -> 0,2    //1,0 -> 0,1    //2,0 -> 0,0
        //0,1 -> 1,2    //1,1 -> 1,1    //2,1 -> 1,0
        //0,2 -> 2,2    //1,2 -> 2,1    //2,2 -> 2,0

        int newX = WorldMath.EqualN(y);
        int newY = WorldMath.InverseN(x, numberCells);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int InverseEqual(int x, int y, int numberCells)
    {
        //0,0 -> 2,0    //1,0 -> 2,1    //2,0 -> 2,2
        //0,1 -> 1,0    //1,1 -> 1,1    //2,1 -> 1,2
        //0,2 -> 0,0    //1,2 -> 0,1    //2,2 -> 0,2

        int newX = WorldMath.InverseN(y, numberCells);
        int newY = WorldMath.EqualN(x);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int InverseInverse(int x, int y, int numberCells)
    {
        //0,0 -> 2,2   //1,0 -> 2,1   //2,0 -> 2,0
        //0,1 -> 1,2   //1,1 -> 1,1   //2,1 -> 1,0
        //0,2 -> 0,2   //1,2 -> 0,1   //2,2 -> 0,0

        int newX = WorldMath.InverseN(y, numberCells);
        int newY = WorldMath.InverseN(x, numberCells);

        return new Vector2Int(newX, newY);
    }

    #endregion

    #region self

    public static Vector2Int Self_EqualEqual(int x, int y)
    {
        //0,0 -> 0,0   //1,0 -> 1,0   //2,0 -> 2,0
        //0,1 -> 0,1   //1,1 -> 1,1   //2,1 -> 2,1
        //0,2 -> 0,2   //1,2 -> 1,2   //2,2 -> 2,2

        int newX = WorldMath.EqualN(x);
        int newY = WorldMath.EqualN(y);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int Self_EqualInverse(int x, int y, int numberCells)
    {
        //0,0 -> 0,2   //1,0 -> 1,2   //2,0 -> 2,2
        //0,1 -> 0,1   //1,1 -> 1,1   //2,1 -> 2,1
        //0,2 -> 0,0   //1,2 -> 1,0   //2,2 -> 2,0

        int newX = WorldMath.EqualN(x);
        int newY = WorldMath.InverseN(y, numberCells);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int Self_InverseEqual(int x, int y, int numberCells)
    {
        //0,0 -> 2,0   //1,0 -> 1,0   //2,0 -> 0,0
        //0,1 -> 2,1   //1,1 -> 1,1   //2,1 -> 0,1
        //0,2 -> 2,2   //1,2 -> 1,2   //2,2 -> 0,2

        int newX = WorldMath.InverseN(x, numberCells);
        int newY = WorldMath.EqualN(y);

        return new Vector2Int(newX, newY);
    }

    public static Vector2Int Self_InverseInverse(int x, int y, int numberCells)
    {
        //0,0 -> 2,2   //1,0 -> 1,2   //2,0 -> 0,2
        //0,1 -> 2,1   //1,1 -> 1,1   //2,1 -> 0,1
        //0,2 -> 2,0   //1,2 -> 1,0   //2,2 -> 0,0

        int newX = WorldMath.InverseN(x, numberCells);
        int newY = WorldMath.InverseN(y, numberCells);

        return new Vector2Int(newX, newY);
    }

    #endregion
}