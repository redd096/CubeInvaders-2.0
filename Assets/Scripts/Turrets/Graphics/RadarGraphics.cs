using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Radar Graphics")]
public class RadarGraphics : BuildableGraphics
{
    [Header("Radar")]
    [SerializeField] Transform objectToFlick = default;
    [SerializeField] Color normalColor = Color.white;

    [Header("Flick")]
    [SerializeField] Gradient flickColor = default;
    [SerializeField] float minFlick = 1;
    [SerializeField] float maxFlick = 10;

    Radar radar;

    protected override void Start()
    {
        base.Start();

        //get logic component as radar
        radar = buildableObject as Radar;
    }

    protected override void Update()
    {
        base.Update();

        if (buildableObject.IsActive)
        {
            ColorRadar();
        }
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return radar.EnemyToAttack;
    }

    void ColorRadar()
    {
        //need model for flick
        if (objectToFlick == null) 
            return;

        //if enemy is attacking, flick color
        if(GetEnemy() && buildableObject.IsActive)
        {
            SetColorFlick();
        }
        //else show normal color
        else
        {
            SetColor(normalColor, 0);
        }
    }

    void SetColorFlick()
    {
        //get flick speed based on enemy distance
        float enemyDistance = Vector3.Distance(buildableObject.CellOwner.transform.position, GetEnemy().transform.position);
        float distanceFrom0To1 = 1 - (enemyDistance / GameManager.instance.waveManager.waveConfig.distanceFromWorld);       //distance from 0 to 1
        float flickSpeed = Mathf.Lerp(minFlick, maxFlick, distanceFrom0To1);                                                //speed from minFlick to maxFlick

        //sin from 0 to 1
        float flick = Mathf.Abs(Mathf.Sin(Time.time * flickSpeed));

        //set color based on enemy distance
        Color color = flickColor.Evaluate(distanceFrom0To1);

        SetColor(color, flick);
    }
    void SetColor(Color color, float delta)
    {
        Renderer[] renderers = objectToFlick.GetComponentsInChildren<Renderer>();

        //foreach renderer set color and emission
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.Lerp(normalColor, color, delta);
            renderer.material.SetColor("_EmissionColor", Color.Lerp(normalColor, color, delta));
        }
    }
}
