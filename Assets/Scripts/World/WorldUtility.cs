using UnityEngine;

public static class WorldUtility
{
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

    #region find face

    /// <summary>
    /// Face up, right, down, left
    /// </summary>
    public static EFace FindFaceUpToRight(EFace startFace, bool toRight)
    {
        EFace newFace = startFace;

        switch (startFace)
        {
            case EFace.up:
                if (toRight)
                    newFace = EFace.right;
                else
                    newFace = EFace.left;
                break;
            case EFace.right:
                if (toRight)
                    newFace = EFace.down;
                else
                    newFace = EFace.up;
                break;
            case EFace.down:
                if (toRight)
                    newFace = EFace.left;
                else
                    newFace = EFace.right;
                break;
            case EFace.left:
                if (toRight)
                    newFace = EFace.up;
                else
                    newFace = EFace.down;
                break;
        }

        return newFace;
    }

    /// <summary>
    /// Face front, up, back, down
    /// </summary>
    public static EFace FindFaceFrontToUp(EFace startFace, bool toUp)
    {
        EFace newFace = startFace;

        switch (startFace)
        {
            case EFace.front:
                if (toUp)
                    newFace = EFace.up;
                else
                    newFace = EFace.down;
                break;
            case EFace.up:
                if (toUp)
                    newFace = EFace.back;
                else
                    newFace = EFace.front;
                break;
            case EFace.back:
                if (toUp)
                    newFace = EFace.down;
                else
                    newFace = EFace.up;
                break;
            case EFace.down:
                if (toUp)
                    newFace = EFace.front;
                else
                    newFace = EFace.back;
                break;
        }

        return newFace;
    }

    #endregion

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