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
    public static Face FindFaceUpToRight(Face startFace, bool toRight)
    {
        Face newFace = startFace;

        switch (startFace)
        {
            case Face.up:
                if (toRight)
                    newFace = Face.right;
                else
                    newFace = Face.left;
                break;
            case Face.right:
                if (toRight)
                    newFace = Face.down;
                else
                    newFace = Face.up;
                break;
            case Face.down:
                if (toRight)
                    newFace = Face.left;
                else
                    newFace = Face.right;
                break;
            case Face.left:
                if (toRight)
                    newFace = Face.up;
                else
                    newFace = Face.down;
                break;
        }

        return newFace;
    }

    /// <summary>
    /// Face front, up, back, down
    /// </summary>
    public static Face FindFaceFrontToUp(Face startFace, bool toUp)
    {
        Face newFace = startFace;

        switch (startFace)
        {
            case Face.front:
                if (toUp)
                    newFace = Face.up;
                else
                    newFace = Face.down;
                break;
            case Face.up:
                if (toUp)
                    newFace = Face.back;
                else
                    newFace = Face.front;
                break;
            case Face.back:
                if (toUp)
                    newFace = Face.down;
                else
                    newFace = Face.up;
                break;
            case Face.down:
                if (toUp)
                    newFace = Face.front;
                else
                    newFace = Face.back;
                break;
        }

        return newFace;
    }

    #endregion

    /// <summary>
    /// returns what face the transform is looking at
    /// </summary>
    public static Face SelectFace(Transform transform)
    {
        if (WorldMath.NegativeAxis(transform.eulerAngles.x) > 40)
            return Face.up;
        else if (WorldMath.NegativeAxis(transform.eulerAngles.x) < -40)
            return Face.down;
        else
            return LateralFace(transform);
    }

    /// <summary>
    /// returns one of the lateral faces
    /// </summary>
    public static Face LateralFace(Transform transform)
    {
        if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= 45 && WorldMath.NegativeAxis(transform.eulerAngles.y) > -45)
        {
            return Face.front;
        }
        else if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= -45 && WorldMath.NegativeAxis(transform.eulerAngles.y) > -135)
        {
            return Face.right;
        }
        else if (WorldMath.NegativeAxis(transform.eulerAngles.y) <= 135 && WorldMath.NegativeAxis(transform.eulerAngles.y) > 45)
        {
            return Face.left;
        }
        else// if (Math.NegativeAxis(transform.eulerAngles.y) <= -135 || Math.NegativeAxis(transform.eulerAngles.y) > 135)
        {
            return Face.back;
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