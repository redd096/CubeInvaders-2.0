using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

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

            //get rotator parent in transform position
            rotatorParent.position = world.transform.position;
            return rotatorParent;
        } }

    Coroutine rotatingWorld_Coroutine;

    #endregion

    public WorldRotator(World world)
    {
        this.world = world;
    }

    #region private API

    #region cell and coordinates

    void SetCoordinates(Cell oldCell, Coordinates newCoords)
    {
        //set new cell (in dictionary and inside the object)
        world.Cells[newCoords] = oldCell;
        world.Cells[newCoords].coordinates = newCoords;
    }

    Coordinates UpdateCoordinatesCompleteFace(Coordinates coords, bool forward)
    {
        Coordinates newCoords = coords;

        if (forward)
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.InverseEqual(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }
        else
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.EqualInverse(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }

        return newCoords;
    }

    void SelectAllFace(int line, Face face1, Face face2, ref List<Cell> cellsToRotate, ref List<Coordinates> cellsKeys)
    {
        //select all face 1 or 2
        if (line <= 0 || line >= world.worldConfig.NumberCells - 1)
        {
            if (world.worldConfig.NumberCells > 1)
            {
                Face face = line <= 0 ? face1 : face2;
                SelectAllFaceCells(face, ref cellsToRotate, ref cellsKeys);
            }
            else
            {
                //if only one cell, then select both faces
                SelectAllFaceCells(face1, ref cellsToRotate, ref cellsKeys);
                SelectAllFaceCells(face2, ref cellsToRotate, ref cellsKeys);
            }
        }
    }

    void SelectCell(Coordinates coordinates, ref List<Cell> cellsToRotate, ref List<Coordinates> cellsKeys)
    {
        //add cell and coordinates
        cellsToRotate.Add(world.Cells[coordinates]);
        cellsKeys.Add(coordinates);
    }

    void SelectAllFaceCells(Face face, ref List<Cell> cellsToRotate, ref List<Coordinates> cellsKeys)
    {
        //add cell and coordinates for every row and column on this face
        for (int x = 0; x < world.worldConfig.NumberCells; x++)
        {
            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                SelectCell(new Coordinates(face, x, y), ref cellsToRotate, ref cellsKeys);
            }
        }
    }

    #endregion

    #region general

    void OnWorldRotate(List<Cell> cellsToRotate)
    {
        //foreach cell to rotate
        foreach (Cell cell in cellsToRotate)
        {
            //if the cell isn't null, call onWorldRotate
            if (cell)
                cell.onWorldRotate?.Invoke();
        }
    }

    #endregion

    #region animations    

    bool SkipAnimation(bool playerMove, float delta)
    {
        //if is a player move and pressed an input, if some time already passed, then skip animation
        if (playerMove && Input.anyKeyDown && delta > 0.1f)
            return true;

        return false;
    }

    IEnumerator AnimationRotate(List<Cell> cellsToRotate, Vector3 rotateAxis, bool forward, bool playerMove)
    {
        //set parent
        cellsToRotate.SetParent(RotatorParent);

        //we need to do a 90° rotation
        float rotationToReach = forward ? 90 : -90;

        //animation
        float delta = 0;
        float animTime = playerMove ? world.rotationTime : world.worldConfig.Randomize.rotationTime;
        while (delta < 1)
        {
            delta += Time.deltaTime / animTime;

            float rotated = Mathf.Lerp(0, rotationToReach, delta);
            RotatorParent.eulerAngles = rotateAxis * rotated;

            //skip animation
            if (SkipAnimation(playerMove == false, delta)) break;

            yield return null;
        }

        //final rotation
        RotatorParent.eulerAngles = rotateAxis * rotationToReach;

        //remove parent and reset its rotation
        cellsToRotate.SetParent(world.transform);
        RotatorParent.rotation = Quaternion.identity;

        //call end rotation
        world.onEndRotation?.Invoke();

        rotatingWorld_Coroutine = null;
    }

    #endregion

    #region rotate row

    #region lateral

    List<Cell> SelectLateralRowCells(int line, out List<Coordinates> cellsKeys)
    {
        List<Cell> cellsToRotate = new List<Cell>();
        cellsKeys = new List<Coordinates>();

        //select line in every lateral face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            for (int x = 0; x < world.worldConfig.NumberCells; x++)
            {
                SelectCell(new Coordinates((Face)faceIndex, x, line), ref cellsToRotate, ref cellsKeys);
            }
        }

        //select all face down or up
        SelectAllFace(line, Face.down, Face.up, ref cellsToRotate, ref cellsKeys);

        return cellsToRotate;
    }

    void UpdateDictionaryLateralRow(List<Coordinates> cellsKeys, bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.up && coords.face != Face.down)
            {
                //get coords same position but next or prev face
                newCoords.face = (Face)WorldUtility.SelectIndex((int)coords.face, toRight, 4);

                //face: front -> right   //right -> back   //back -> left   //left -> front
            }
            else
            {
                //rotate the row, so change coordinates x and y, but not the face
                bool rotateToRight = coords.face == Face.up ? toRight : !toRight;
                newCoords = UpdateCoordinatesCompleteFace(coords, rotateToRight);
            }

            //set new coordinates
            SetCoordinates(oldCells[coords], newCoords);
        }
    }

    #endregion

    #region up and down

    List<Cell> SelectUpDownRowCells(int line, out List<Coordinates> cellsKeys)
    {
        List<Cell> cellsToRotate = new List<Cell>();
        cellsKeys = new List<Coordinates>();

        //select line in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.front) f = Face.up;
            if ((Face)faceIndex == Face.back) f = Face.down;

            for (int x = 0; x < world.worldConfig.NumberCells; x++)
            {
                Coordinates coordinates = new Coordinates();

                switch (f)
                {
                    case Face.up:
                        //up select the row
                        coordinates = new Coordinates(f, x, line);
                        break;
                    case Face.down:
                        //down select the row but inverse of up
                        coordinates = new Coordinates(f, x, WorldMath.InverseN(line, world.worldConfig.NumberCells));
                        break;
                    case Face.right:
                        //right select the column instead of row
                        coordinates = new Coordinates(f, line, x);
                        break;
                    case Face.left:
                        //left select the column instead of row
                        //but if you are rotating the first row, this is the last column, if you are rotating the last row, this is the first column
                        coordinates = new Coordinates(f, WorldMath.InverseN(line, world.worldConfig.NumberCells), x);
                        break;
                }

                SelectCell(coordinates, ref cellsToRotate, ref cellsKeys);
            }
        }

        //select all face front or back
        SelectAllFace(line, Face.front, Face.back, ref cellsToRotate, ref cellsKeys);

        return cellsToRotate;
    }

    void UpdateDictionaryUpDownRow(List<Coordinates> cellsKeys, bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;
            
            if(coords.face != Face.front && coords.face != Face.back)
            {
                //change face and also coordinates
                newCoords = CoordsToRight(coords, toRight);
            }
            else
            {
                //rotate the row, so change coordinates x and y, but not the face
                bool rotateToRight = coords.face == Face.back ? toRight : !toRight;
                newCoords = UpdateCoordinatesCompleteFace(coords, rotateToRight);
            }

            //set new coordinates
            SetCoordinates(oldCells[coords], newCoords);
        }
    }

    Coordinates CoordsToRight(Coordinates coords, bool toRight)
    {
        Coordinates newCoords = coords;

        //calculate new face
        newCoords.face = WorldUtility.FindFaceUpToRight(coords.face, toRight);

        //get coordinates x,y of face to the right or to the left
        if (toRight)
        {
            Vector2Int v = Vector2Math.EqualInverse(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.InverseEqual(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }

        return newCoords;
    }

    #endregion

    #endregion

    #region rotate column

    #region front

    List<Cell> SelectFrontColumnCells(int line, out List<Coordinates> cellsKeys)
    {
        List<Cell> cellsToRotate = new List<Cell>();
        cellsKeys = new List<Coordinates>();
        
        //select line in every front face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of right and left, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.right) f = Face.up;
            if ((Face)faceIndex == Face.left) f = Face.down;

            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                //set l equal to line
                int l = line;

                //but when is face back is the inverse of the other faces, so column 0 is 2, column 1 is 1, column 2 is 0
                if (f == Face.back)
                    l = WorldMath.InverseN(line, world.worldConfig.NumberCells);

                SelectCell(new Coordinates(f, l, y), ref cellsToRotate, ref cellsKeys);
            }
        }

        //select all face right or left
        SelectAllFace(line, Face.left, Face.right, ref cellsToRotate, ref cellsKeys);

        return cellsToRotate;
    }

    void UpdateDictionaryFrontColumn(List<Coordinates> cellsKeys, bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.right && coords.face != Face.left)
            {
                //change face and coordinates
                newCoords = CoordsFrontColumn(coords, toUp);
            }
            else
            {
                //rotate the column, so change coordinates x and y, but not the face
                bool rotateToUp = coords.face == Face.left ? toUp : !toUp;
                newCoords = UpdateCoordinatesCompleteFace(coords, rotateToUp);
            }

            //set new coordinates
            SetCoordinates(oldCells[coords], newCoords);
        }
    }

    Coordinates CoordsFrontColumn(Coordinates coords, bool toUp)
    {
        Coordinates newCoords = coords;

        //calculate new face
        newCoords.face = WorldUtility.FindFaceFrontToUp(coords.face, toUp);

        //get coordinates x,y of face to the top or to the down
        if (coords.face == Face.back || newCoords.face == Face.back)
        {
            //if the prev face or next face is Face.back, then you Self_InverseInverse
            Vector2Int v = Vector2Math.Self_InverseInverse(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }

        return newCoords;
    }

    #endregion

    #region right and left

    List<Cell> SelectRightLeftColumnCells(int line, out List<Coordinates> cellsKeys)
    {
        List<Cell> cellsToRotate = new List<Cell>();
        cellsKeys = new List<Coordinates>();

        //select line in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            Face f = (Face)faceIndex;

            if ((Face)faceIndex == Face.front) f = Face.up;
            if ((Face)faceIndex == Face.back) f = Face.down;

            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                Coordinates coordinates = new Coordinates();

                switch (f)
                {
                    case Face.right:
                        //select column
                        coordinates = new Coordinates(f, line, y);
                        break;
                    case Face.left:
                        //left select the column, but inverse (when select 0 is the last, when select last is the 0)
                        coordinates = new Coordinates(f, WorldMath.InverseN(line, world.worldConfig.NumberCells), y);
                        break;
                    case Face.up:
                        //up select the row instead of column
                        coordinates = new Coordinates(f, y, line);
                        break;
                    case Face.down:
                        //down is inverse of up
                        coordinates = new Coordinates(f, y, WorldMath.InverseN(line, world.worldConfig.NumberCells));
                        break;
                }

                SelectCell(coordinates, ref cellsToRotate, ref cellsKeys);
            }
        }

        //select all face front or back
        SelectAllFace(line, Face.front, Face.back, ref cellsToRotate, ref cellsKeys);

        return cellsToRotate;
    }

    void UpdateDictionaryRightLeftColumn(List<Coordinates> cellsKeys, bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates coords in cellsKeys)
        {
            Coordinates newCoords = coords;

            if (coords.face != Face.front && coords.face != Face.back)
            {
                //change face, and in back face also coordinates
                newCoords = CoordsRightLeftColumn(coords, toUp);
            }
            else
            {
                //rotate the column, so change coordinates x and y, but not the face
                bool rotateToUp = coords.face == Face.front ? toUp : !toUp;
                newCoords = UpdateCoordinatesCompleteFace(coords, rotateToUp);
            }

            //set new coordinates
            SetCoordinates(oldCells[coords], newCoords);
        }
    }

    Coordinates CoordsRightLeftColumn(Coordinates coords, bool toUp)
    {
        Coordinates newCoords = coords;

        //calculate new face -> work the inverse, so we use !toUp
        newCoords.face = WorldUtility.FindFaceUpToRight(coords.face, !toUp);

        //get coordinates x,y of face to the top or face to the bottom
        if (toUp)
        {
            Vector2Int v = Vector2Math.InverseEqual(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.EqualInverse(coords.x, coords.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }

        return newCoords;
    }

    #endregion

    #endregion

    #endregion

    #region public API

    #region row

    public void RotateLateralRow(int line, bool toRight, bool playerMove)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<Cell> cellsToRotate;
        List<Coordinates> cellsKeys;

        //rotate line
        cellsToRotate = SelectLateralRowCells(line, out cellsKeys);

        //rotate animation
        rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(cellsToRotate, Vector3.up, !toRight, playerMove));

        //update dictionary
        UpdateDictionaryLateralRow(cellsKeys, toRight);

        //control every selected cell
        OnWorldRotate(cellsToRotate);
    }

    public void RotateUpDownRow(Face face, int line, bool toRight, bool playerMove)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<Cell> cellsToRotate;
        List<Coordinates> cellsKeys;

        //rotate line. Down face is the inverse
        if (face == Face.up)
        {
            cellsToRotate = SelectUpDownRowCells(line, out cellsKeys);
        }
        else
        {
            cellsToRotate = SelectUpDownRowCells(WorldMath.InverseN(line, world.worldConfig.NumberCells), out cellsKeys);

            //in the down face is inverse
            toRight = !toRight;
        }

        //rotate animation
        rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(cellsToRotate, Vector3.forward, !toRight, playerMove));

        //update dictionary
        UpdateDictionaryUpDownRow(cellsKeys, toRight);

        //control every selected cell
        OnWorldRotate(cellsToRotate);
    }

    #endregion

    #region column

    public void RotateFrontColumn(Face face, int line, bool toUp, bool playerMove)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<Cell> cellsToRotate;
        List<Coordinates> cellsKeys;

        //rotate line. Back face is the inverse
        if (face != Face.back)
        {
            cellsToRotate = SelectFrontColumnCells(line, out cellsKeys);
        }
        else
        {
            cellsToRotate = SelectFrontColumnCells(WorldMath.InverseN(line, world.worldConfig.NumberCells), out cellsKeys);

            //in the back face is inverse
            toUp = !toUp;
        }

        //rotate animation
        rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(cellsToRotate, Vector3.right, toUp, playerMove));

        //update dictionary
        UpdateDictionaryFrontColumn(cellsKeys, toUp);

        //control every selected cell
        OnWorldRotate(cellsToRotate);
    }

    public void RotateRightLeftColumn(Face startFace, int line, bool toUp, bool playerMove)
    {
        //can't rotate during another rotation
        if (rotatingWorld_Coroutine != null)
            return;

        List<Cell> cellsToRotate;
        List<Coordinates> cellsKeys;

        //right face. Left face is the inverse
        if (startFace == Face.right)
        {
            cellsToRotate = SelectRightLeftColumnCells(line, out cellsKeys);
        }
        else
        {
            cellsToRotate = SelectRightLeftColumnCells(WorldMath.InverseN(line, world.worldConfig.NumberCells), out cellsKeys);

            //in the left is inverse
            toUp = !toUp;
        }

        //rotate animation
        rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(cellsToRotate, Vector3.forward, toUp, playerMove));

        //update dictionary
        UpdateDictionaryRightLeftColumn(cellsKeys, toUp);

        //control every selected cell
        OnWorldRotate(cellsToRotate);
    }

    #endregion

    #endregion
}
