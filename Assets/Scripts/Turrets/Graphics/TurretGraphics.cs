using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Graphics")]
public class TurretGraphics : BuildableGraphics
{
    Turret turret;

    [Header("Time Before Destroy - Turret Modifier")]
    [SerializeField] Transform objectToColor = default;
    [SerializeField] Color colorTimeBeforeDestroy = Color.red;

    Dictionary<Renderer, Color> normalColors = new Dictionary<Renderer, Color>();

    [Header("No Turrets On Same Face - Turret Modifier")]
    [SerializeField] LineRenderer linePrefab = default;
    [SerializeField] Color colorNoTurretsOnSameFace = Color.red;
    [SerializeField] Transform linePosition = default;

    static Dictionary<EFace, LineRenderer> line = new Dictionary<EFace, LineRenderer>();

    protected override void Awake()
    {
        base.Awake();

        //get logic component as turret
        turret = buildableObject as Turret;
    }

    void Start()
    {
        AddEvents();

        //set normalColors dictionary
        SetNormalColor();
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    protected virtual void AddEvents()
    {
        turret.updateTimeBeforeDestroy += SetColor;

        turret.startTimerTurretsOnSameFace += CreateLine;
        turret.updateFeedbackTurretsOnSameFace += SetLineColor;
        turret.updateNumberOfTurretsOnSameFace += SetPositions;
        turret.stopTimerTurretsOnSameFace += DestroyLine;
    }

    protected virtual void RemoveEvents()
    {
        turret.updateTimeBeforeDestroy -= SetColor;

        turret.startTimerTurretsOnSameFace -= CreateLine;
        turret.updateFeedbackTurretsOnSameFace -= SetLineColor;
        turret.updateNumberOfTurretsOnSameFace -= SetPositions;
        turret.stopTimerTurretsOnSameFace -= DestroyLine;
    }

    #endregion

    #region timer before destroy

    void SetNormalColor()
    {
        Renderer[] renderers = objectToColor.GetComponentsInChildren<Renderer>();

        //foreach renderer save normal color in dictionary
        foreach (Renderer renderer in renderers)
        {
            normalColors.Add(renderer, renderer.material.color);
        }
    }

    void SetColor(float delta)
    {
        //foreach renderer in dictionary, update color
        foreach (Renderer renderer in normalColors.Keys)
        {
            renderer.material.color = Color.Lerp(normalColors[renderer], colorTimeBeforeDestroy, delta);
        }
    }

    #endregion

    #region no turrets on same face

    void CreateLine(List<Turret> turrets)
    {
        EFace face = turret.CellOwner.coordinates.face;

        //instantiate line prefab if null and save normal color
        if (line.ContainsKey(face) == false)
        {
            line.Add(face, Instantiate(linePrefab));
        }

        //set positions and active
        SetPositions(face, turrets);
        line[face].gameObject.SetActive(true);
    }

    void SetLineColor(float delta)
    {
        if (line.ContainsKey(turret.CellOwner.coordinates.face))
        {
            //set color of the line renderer on this face
            line[turret.CellOwner.coordinates.face].material.color = Color.Lerp(linePrefab.sharedMaterial.color, colorNoTurretsOnSameFace, delta);
        }
    }

    void SetPositions(EFace face, List<Turret> turrets)
    {
        if (line.ContainsKey(face))
        {
            //get position of every turret
            List<Vector3> positions = new List<Vector3>();
            foreach (Turret t in turrets)
                positions.Add(t.GetComponent<TurretGraphics>().linePosition.position);

            //set positions
            line[face].positionCount = positions.Count;
            line[face].SetPositions(positions.ToArray());
        }
    }

    public void DestroyLine(EFace face)
    {
        //deactive line
        if (line.ContainsKey(face))
        {
            line[face].gameObject.SetActive(false);
        }
    }

    #endregion
}
