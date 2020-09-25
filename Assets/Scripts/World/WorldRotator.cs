using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotator
{
    #region variables

    World world;
    Transform rotatorParent;
    Transform RotatorParent { get
        { 
            //if null, create empty object
            if (rotatorParent == null)
            {
                rotatorParent = new GameObject("RotatorParent").transform;
            }

            //get rotator parent in world position
            rotatorParent.position = world.transform.position;
            return rotatorParent;
        } }

    Coroutine rotatingWorld_Coroutine;

    #endregion

    public WorldRotator(World world)
    {
        this.world = world;
    }

    #region rotate world
    
    void SetCoordinates(Dictionary<Coordinates, Cell> oldCells, Coordinates coords, Coordinates newCoords)
    {
        //set new cell (in dictionary and inside the object)
        cells[newCoords] = oldCells[coords];
        cells[newCoords].coordinates = newCoords;
    }

    Coordinates UpdateCoordinatesCompleteFace(Coordinates coords, int numberCells, bool forward)
    {
        Coordinates newCoords = coords;

        if (forward)
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }
        else
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }

        return newCoords;
    }

    void SelectCell(Coordinates coordinates, List<GameObject> cellsToRotate, List<Coordinates> cellsKeys, out List<GameObject> _cellsToRotate, out List<Coordinates> _cellsKeys)
    {
        _cellsToRotate = cellsToRotate;
        _cellsToRotate.Add(cells[coordinates].gameObject);

        _cellsKeys = cellsKeys;
        _cellsKeys.Add(coordinates);
    }

    void SelectAllFaceCells(Face face, int _numberCells, List<GameObject> cellsToRotate, List<Coordinates> cellsKeys, out List<GameObject> _cellsToRotate, out List<Coordinates> _cellsKeys)
    {
        _cellsToRotate = new List<GameObject>();
        _cellsKeys = new List<Coordinates>();

        for (int x = 0; x < _numberCells; x++)
        {
            for (int y = 0; y < _numberCells; y++)
            {
                SelectCell(new Coordinates(face, y, x), cellsToRotate, cellsKeys, out _cellsToRotate, out _cellsKeys);
            }
        }
    }

    #region animations

    void SetParent(List<GameObject> cellsToRotate, Transform parent)
    {
        foreach (GameObject c in cellsToRotate)
        {
            c.transform.parent = parent;
        }
    }

    bool SkipAnimation(bool controlPlayer, float time, float animTime, float timeBeforeSkip)
    {
        //if player can control and pressed an input, if some time already passed, then skip animation
        if (controlPlayer && Input.anyKeyDown && time - Time.time - animTime < -timeBeforeSkip)
            return true;

        return false;
    }

    IEnumerator Animation_RotateXAxis(List<GameObject> cellsToRotate, float animTime, bool toUp, Transform _rotatorParent, bool controlPlayer, float timeBeforeSkip = 0.05f)
    {
        //set parent
        SetParent(cellsToRotate, _rotatorParent);

        //set time necessary to do the animation
        float time = Time.time + animTime;

        if (toUp)
        {
            //animation to up
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 270, 0)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(_rotatorParent.eulerAngles.x + 90 * Time.deltaTime / animTime, 0, 0);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            //animation to down
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 90, 0)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(_rotatorParent.eulerAngles.x - 90 * Time.deltaTime / animTime, 0, 0);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(-90, 0, 0);
        }

        //remove parent and reset its rotation
        SetParent(cellsToRotate, worldParent);
        _rotatorParent.rotation = Quaternion.identity;

        animationCoroutine = null;

        if (onFinishRotation != null)
            onFinishRotation();
    }

    IEnumerator Animation_RotateYAxis(List<GameObject> cellsToRotate, float animTime, bool toRight, Transform _rotatorParent, bool controlPlayer, float timeBeforeSkip = 0.05f)
    {
        //set parent
        SetParent(cellsToRotate, _rotatorParent);

        //set time necessary to do the animation
        float time = Time.time + animTime;

        if (toRight)
        {
            //animation to right
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 270, 0)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(0, _rotatorParent.eulerAngles.y - 90 * Time.deltaTime / animTime, 0);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            //animation to left
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 90, 0)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(0, _rotatorParent.eulerAngles.y + 90 * Time.deltaTime / animTime, 0);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(0, 90, 0);
        }

        //remove parent and reset its rotation
        SetParent(cellsToRotate, worldParent);
        _rotatorParent.rotation = Quaternion.identity;

        animationCoroutine = null;

        if (onFinishRotation != null)
            onFinishRotation();
    }

    IEnumerator Animation_RotateZAxis(List<GameObject> cellsToRotate, float animTime, bool forward, Transform _rotatorParent, bool controlPlayer, float timeBeforeSkip = 0.05f)
    {
        //set parent
        SetParent(cellsToRotate, _rotatorParent);

        //set time necessary to do the animation
        float time = Time.time + animTime;

        if (forward)
        {
            //animation to right
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 0, 270)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(0, 0, _rotatorParent.eulerAngles.z - 90 * Time.deltaTime / animTime);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            //animation to left
            //Quaternion.Angle(worldParent.rotation, Quaternion.Euler(0, 0, 90)) > 0.1f * speed
            while (Time.time < time)
            {
                //we need to do a 90° rotation -> 90 * Time.deltaTime /animTime
                _rotatorParent.eulerAngles = new Vector3(0, 0, _rotatorParent.eulerAngles.z + 90 * Time.deltaTime / animTime);

                //skip animation
                if (SkipAnimation(controlPlayer, time, animTime, timeBeforeSkip)) break;

                yield return null;
            }

            //final rotation
            _rotatorParent.eulerAngles = new Vector3(0, 0, 90);
        }

        //remove parent and reset its rotation
        SetParent(cellsToRotate, worldParent);
        _rotatorParent.rotation = Quaternion.identity;

        animationCoroutine = null;

        if (onFinishRotation != null)
            onFinishRotation();
    }

    #endregion

    #region rotate row

    #region lateral

    List<GameObject> SelectLateralRowCells(int line, out List<Coordinates> cellsKeys, bool selectCompleteFace, Face face = Face.up)
    {
        List<GameObject> cellsToRotate = new List<GameObject>();
        cellsKeys = new List<Coordinates>();

        //select line in every lateral face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            for (int column = 0; column < numberCells; column++)
            {
                SelectCell(new Coordinates((Face)faceIndex, line, column), cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        //select all face down or up
        if (selectCompleteFace)
        {
            if (numberCells > 1)
            {
                SelectAllFaceCells(face, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
            else
            {
                //if only one cell, then both
                SelectAllFaceCells(Face.up, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
                SelectAllFaceCells(Face.down, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        return cellsToRotate;
    }

    void UpdateDictionaryLateralRow(List<Coordinates> cellsKeys, bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = CreateDictionaryCopy(cells);

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.up && coords.face != Face.down)
            {
                //get coords same position but next or prev face
                newCoords.face = (Face)Simple_Utility.SelectIndex((int)coords.face, toRight, 4);

                //face: front -> right   //right -> back   //back -> left   //left -> front
            }
            else if (coords.face == Face.up)
            {
                //rotate the face, so change coordinates x and y, but not the face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, toRight);
            }
            else
            {
                //down face rotation is inverse of up face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, !toRight);
            }

            //set new coordinates
            SetCoordinates(oldCells, coords, newCoords);
        }
    }

    #endregion

    #region up and down

    List<GameObject> SelectUpDownRowCells(int line, out List<Coordinates> cellsKeys, bool selectCompleteFace, Face face = Face.up)
    {
        List<GameObject> cellsToRotate = new List<GameObject>();
        cellsKeys = new List<Coordinates>();

        //select line in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.front) f = Face.up;
            if ((Face)faceIndex == Face.back) f = Face.down;

            for (int x = 0; x < numberCells; x++)
            {
                Coordinates coordinates = new Coordinates();

                if (f == Face.right)
                {
                    //right select the column instead of row
                    coordinates = new Coordinates(f, x, line);
                }
                else if (f == Face.left)
                {
                    //left select the column instead of row
                    //but if you are rotating the first row, this is the last column, if you are rotating the last row, this is the first column
                    coordinates = new Coordinates(f, x, Math.InverseN(line, numberCells));
                }
                else if (f == Face.down)
                {
                    //down select the row but inverse of up
                    coordinates = new Coordinates(f, Math.InverseN(line, numberCells), x);
                }
                else
                {
                    //up select the row
                    coordinates = new Coordinates(f, line, x);
                }

                SelectCell(coordinates, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        //select all face front or back
        if (selectCompleteFace)
        {
            if (numberCells > 1)
            {
                SelectAllFaceCells(face, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
            else
            {
                //if only one cell, then both
                SelectAllFaceCells(Face.front, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
                SelectAllFaceCells(Face.back, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        return cellsToRotate;
    }

    void UpdateDictionaryUpDownRow(List<Coordinates> cellsKeys, bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = CreateDictionaryCopy(cells);

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.front && coords.face != Face.back)
            {
                //change face and also coordinates
                newCoords = CoordsToRight(coords, toRight);
            }
            else if (coords.face == Face.front)
            {
                //front face rotation is inverse of back face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, !toRight);
            }
            else
            {
                //rotate the face, so change coordinates x and y, but not the face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, toRight);
            }

            //set new coordinates
            SetCoordinates(oldCells, coords, newCoords);
        }
    }

    Coordinates CoordsToRight(Coordinates coords, bool toRight)
    {
        Coordinates newCoords = coords;

        //calculate new face
        newCoords.face = Simple_Utility.FindFaceUpToRight(coords.face, toRight);

        //get coordinates x,y of face to the right or face to the left
        if (toRight)
        {
            Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }

        return newCoords;
    }

    Coordinates TestRow(Coordinates coords, bool toRight)
    {
        Coordinates newCoords = coords;

        switch (coords.face)
        {
            case Face.up:
                if (toRight)
                {
                    newCoords.face = Face.right;

                    Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.left;

                    Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.right:
                if (toRight)
                {
                    newCoords.face = Face.down;

                    Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.up;

                    Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.down:
                if (toRight)
                {
                    newCoords.face = Face.left;

                    Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.right;

                    Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.left:
                if (toRight)
                {
                    newCoords.face = Face.up;

                    Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.down;

                    Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
        }

        return newCoords;
    }

    #endregion

    #endregion

    #region rotate column

    #region front

    List<GameObject> SelectFrontColumnCells(int line, out List<Coordinates> cellsKeys, bool selectCompleteFace, Face face = Face.up)
    {
        List<GameObject> cellsToRotate = new List<GameObject>();
        cellsKeys = new List<Coordinates>();

        //select line in every front face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of right and left, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.right) f = Face.up;
            if ((Face)faceIndex == Face.left) f = Face.down;

            for (int y = 0; y < numberCells; y++)
            {
                //set l equal to line
                int l = line;

                //but when is face back is the inverse of the other faces, so column 0 is 2, column 1 is 1, column 2 is 0
                if (f == Face.back)
                    l = Math.InverseN(line, numberCells);

                SelectCell(new Coordinates(f, y, l), cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        //select all face right or left
        if (selectCompleteFace)
        {
            if (numberCells > 1)
            {
                SelectAllFaceCells(face, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
            else
            {
                //if only one cell, then both
                SelectAllFaceCells(Face.right, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
                SelectAllFaceCells(Face.left, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        return cellsToRotate;
    }

    void UpdateDictionaryFrontColumn(List<Coordinates> cellsKeys, bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = CreateDictionaryCopy(cells);

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.right && coords.face != Face.left)
            {
                //change face and coordinates
                newCoords = CoordsFrontColumn(coords, toUp);
            }
            else if (coords.face == Face.left)
            {
                //rotate the face, so change coordinates x and y, but not the face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, toUp);
            }
            else
            {
                //right face rotation is inverse of left face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, !toUp);
            }

            //set new coordinates
            SetCoordinates(oldCells, coords, newCoords);
        }
    }

    Coordinates CoordsFrontColumn(Coordinates coords, bool toUp)
    {
        Coordinates newCoords = coords;

        //calculate new face
        newCoords.face = Simple_Utility.FindFaceFrontToUp(coords.face, toUp);

        //get coordinates x,y of face to the top or face to the down
        if (coords.face == Face.back || newCoords.face == Face.back)
        {
            //if the prev face or next face is Face.back, then you Self_InverseInverse
            Vector2Int v = Vector2Math.Self_InverseInverse(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }
        else        {
            
            ////otherwise, change only face but coordinates x,y are the same
            //Vector2Int v = Vector2Math.Self_EqualEqual(coords.x, coords.y);
            //newCoords.x = v.x;
            //newCoords.y = v.y;            
        }

        return newCoords;
    }

    Coordinates TestFrontColumn(Coordinates coords, bool toUp)
    {
        Coordinates newCoords = coords;

        switch (coords.face)
        {
            case Face.front:
                if (toUp)
                {
                    newCoords.face = Face.up;

                    Vector2Int v = Vector2Math.Self_EqualEqual(coords.column, coords.row);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.down;

                    Vector2Int v = Vector2Math.Self_EqualEqual(coords.column, coords.row);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.up:
                if (toUp)
                {
                    newCoords.face = Face.back;

                    Vector2Int v = Vector2Math.Self_InverseInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.front;

                    Vector2Int v = Vector2Math.Self_EqualEqual(coords.column, coords.row);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.back:
                if (toUp)
                {
                    newCoords.face = Face.down;

                    Vector2Int v = Vector2Math.Self_InverseInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.up;

                    Vector2Int v = Vector2Math.Self_InverseInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
            case Face.down:
                if (toUp)
                {
                    newCoords.face = Face.front;

                    Vector2Int v = Vector2Math.Self_EqualEqual(coords.column, coords.row);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                else
                {
                    newCoords.face = Face.back;

                    Vector2Int v = Vector2Math.Self_InverseInverse(coords.column, coords.row, numberCells);
                    newCoords.column = v.x;
                    newCoords.row = v.y;
                }
                break;
        }

        return newCoords;
    }

    #endregion

    #region right and left

    List<GameObject> SelectRightLeftColumnCells(int line, out List<Coordinates> cellsKeys, bool selectCompleteFace, Face face = Face.up)
    {
        List<GameObject> cellsToRotate = new List<GameObject>();
        cellsKeys = new List<Coordinates>();

        //select line in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.front) f = Face.up;
            if ((Face)faceIndex == Face.back) f = Face.down;

            for (int y = 0; y < numberCells; y++)
            {
                Coordinates coordinates = new Coordinates();

                if (f == Face.right)
                {
                    //select column
                    coordinates = new Coordinates(f, y, line);
                }
                else if (f == Face.left)
                {
                    //left select the column, but inverse (when select 0 is the last, when select last is the 0)
                    coordinates = new Coordinates(f, y, Math.InverseN(line, numberCells));
                }
                else if (f == Face.down)
                {
                    //down is inverse of up
                    coordinates = new Coordinates(f, Math.InverseN(line, numberCells), y);
                }
                else
                {
                    //up select the row instead of column
                    coordinates = new Coordinates(f, line, y);
                }

                SelectCell(coordinates, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        //select all face front or back
        if (selectCompleteFace)
        {
            if (numberCells > 1)
            {
                SelectAllFaceCells(face, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
            else
            {
                //if only one cell, then both
                SelectAllFaceCells(Face.front, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
                SelectAllFaceCells(Face.back, numberCells, cellsToRotate, cellsKeys, out cellsToRotate, out cellsKeys);
            }
        }

        return cellsToRotate;
    }

    void UpdateDictionaryRightLeftColumn(List<Coordinates> cellsKeys, bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = CreateDictionaryCopy(cells);

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.front && coords.face != Face.back)
            {
                //change face, and in back face also coordinates
                newCoords = CoordsRightLeftColumn(coords, toUp);
            }
            else if (coords.face == Face.front)
            {
                //rotate the face, so change coordinates x and y, but not the face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, toUp);
            }
            else
            {
                //back face rotation is inverse of front face
                newCoords = UpdateCoordinatesCompleteFace(coords, numberCells, !toUp);
            }

            //set new coordinates
            SetCoordinates(oldCells, coords, newCoords);
        }
    }

    Coordinates CoordsRightLeftColumn(Coordinates coords, bool toUp)
    {
        Coordinates newCoords = coords;

        //calculate new face -> work the inverse, so we use !toUp
        newCoords.face = Simple_Utility.FindFaceUpToRight(coords.face, !toUp);

        //get coordinates x,y of face to the top or face to the bottom
        if (toUp)
        {
            Vector2Int v = Vector2Math.InverseEqual(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.EqualInverse(coords.column, coords.row, numberCells);
            newCoords.column = v.x;
            newCoords.row = v.y;
        }

        return newCoords;
    }

    #endregion

    #endregion

    void OnWorldRotation(List<GameObject> cellsToRotate)
    {
        //foreach cell to rotate
        foreach (GameObject cellToRotate in cellsToRotate)
        {
            Cell cell = cellToRotate.GetComponent<Cell>();

            //if the function isn't null, call it
            if (cell && cell.onWorldRotate != null)
                cell.onWorldRotate();
        }
    }

    #endregion

    #region public API

    #region row

    public void RotateLateralRow(int line, bool toRight, float animTime, bool controlPlayer = true)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<GameObject> cellsToRotate = new List<GameObject>();
        List<Coordinates> cellsKeys = new List<Coordinates>();

        if (line <= 0)
        {
            //line 0, rotate also all face down
            cellsToRotate = SelectLateralRowCells(line, out cellsKeys, true, Face.down);
        }
        else if (line >= world.worldConfig.numberCells - 1)
        {
            //last line, rotate also all face up
            cellsToRotate = SelectLateralRowCells(line, out cellsKeys, true, Face.up);
        }
        else
        {
            //rotate only line
            cellsToRotate = SelectLateralRowCells(line, out cellsKeys, false);
        }

        //rotate animation
        rotatingWorld_Coroutine = world.StartCoroutine(Animation_RotateYAxis(cellsToRotate, animTime, toRight, parent, controlPlayer));

        //update dictionary
        UpdateDictionaryLateralRow(cellsKeys, toRight);

        //control every selected cell
        OnWorldRotation(cellsToRotate);
    }

    public void RotateUpDownRow(Face startFace, int line, bool toRight, float animTime, bool controlPlayer = true)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<GameObject> cellsToRotate = new List<GameObject>();
        List<Coordinates> cellsKeys = new List<Coordinates>();

        //up face. Down face is the inverse
        if (startFace != Face.down)
        {
            if (line <= 0)
            {
                //line 0, rotate also all face front
                cellsToRotate = SelectUpDownRowCells(line, out cellsKeys, true, Face.front);
            }
            else if (line >= numberCells - 1)
            {
                //last line, rotate also all face back
                cellsToRotate = SelectUpDownRowCells(line, out cellsKeys, true, Face.back);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectUpDownRowCells(line, out cellsKeys, false);
            }
        }
        else
        {
            if (line <= 0)
            {
                //line 0 is last line in every other face, rotate also all face back
                cellsToRotate = SelectUpDownRowCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.back);
            }
            else if (line >= numberCells - 1)
            {
                //last line is line 0 in every other face, rotate also all face front
                cellsToRotate = SelectUpDownRowCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.front);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectUpDownRowCells(Math.InverseN(line, numberCells), out cellsKeys, false);
            }

            //in the down face is inverse
            toRight = !toRight;
        }

        //rotate animation
        animationCoroutine = StartCoroutine(Animation_RotateZAxis(cellsToRotate, animTime, toRight, parent, controlPlayer));

        //update dictionary
        UpdateDictionaryUpDownRow(cellsKeys, toRight);

        //control every selected cell
        OnWorldRotation(cellsToRotate);
    }

    #endregion

    #region column

    public void RotateFrontColumn(Face startFace, int line, bool toUp, float animTime, bool controlPlayer = true)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<GameObject> cellsToRotate = new List<GameObject>();
        List<Coordinates> cellsKeys = new List<Coordinates>();

        //every front face. Back face is the inverse
        if (startFace != Face.back)
        {
            if (line <= 0)
            {
                //line 0, rotate also all face left
                cellsToRotate = SelectFrontColumnCells(line, out cellsKeys, true, Face.left);
            }
            else if (line >= numberCells - 1)
            {
                //last line, rotate also all face right
                cellsToRotate = SelectFrontColumnCells(line, out cellsKeys, true, Face.right);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectFrontColumnCells(line, out cellsKeys, false);
            }
        }
        else
        {
            if (line <= 0)
            {
                //line 0 is last line in every other face, rotate also all face right
                cellsToRotate = SelectFrontColumnCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.right);
            }
            else if (line >= numberCells - 1)
            {
                //last line is line 0 in every other face, rotate also all face left
                cellsToRotate = SelectFrontColumnCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.left);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectFrontColumnCells(Math.InverseN(line, numberCells), out cellsKeys, false);
            }

            //in the back is inverse
            toUp = !toUp;
        }

        //rotate animation
        animationCoroutine = StartCoroutine(Animation_RotateXAxis(cellsToRotate, animTime, toUp, parent, controlPlayer));

        //update dictionary
        UpdateDictionaryFrontColumn(cellsKeys, toUp);

        //control every selected cell
        OnWorldRotation(cellsToRotate);
    }

    public void RotateRightLeftColumn(Face startFace, int line, bool toUp, float animTime, bool controlPlayer = true)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<GameObject> cellsToRotate = new List<GameObject>();
        List<Coordinates> cellsKeys = new List<Coordinates>();

        //right face. Left face is the inverse
        if (startFace != Face.left)
        {
            if (line <= 0)
            {
                //line 0, rotate also all face front
                cellsToRotate = SelectRightLeftColumnCells(line, out cellsKeys, true, Face.front);
            }
            else if (line >= numberCells - 1)
            {
                //last line, rotate also all face back
                cellsToRotate = SelectRightLeftColumnCells(line, out cellsKeys, true, Face.back);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectRightLeftColumnCells(line, out cellsKeys, false);
            }
        }
        else
        {
            if (line <= 0)
            {
                //line 0 is last line in every other face, rotate also all face back
                cellsToRotate = SelectRightLeftColumnCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.back);
            }
            else if (line >= numberCells - 1)
            {
                //last line is line 0 in every other face, rotate also all face front
                cellsToRotate = SelectRightLeftColumnCells(Math.InverseN(line, numberCells), out cellsKeys, true, Face.front);
            }
            else
            {
                //rotate only line
                cellsToRotate = SelectRightLeftColumnCells(Math.InverseN(line, numberCells), out cellsKeys, false);
            }

            //in the left is inverse
            toUp = !toUp;
        }

        //rotate animation
        animationCoroutine = StartCoroutine(Animation_RotateZAxis(cellsToRotate, animTime, !toUp, parent, controlPlayer));

        //update dictionary
        UpdateDictionaryRightLeftColumn(cellsKeys, toUp);

        //control every selected cell
        OnWorldRotation(cellsToRotate);
    }

    #endregion

    #endregion
}
