using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Radar Graphics")]
public class RadarGraphics : BuildableGraphics
{
    [Header("Flick")]
    [SerializeField] Transform objectToFlick = default;
    [SerializeField] float flickSpeed = 1;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color flickColor = Color.red;

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

        //sin from 0 to 1
        float flick = Mathf.Abs(Mathf.Sin(Time.time * flickSpeed));

        //if enemy is attacking, flick color
        if(GetEnemy() && buildableObject.IsActive)
        {
            SetColor(flickColor, flick);
        }
        //else show normal color
        else
        {
            SetColor(normalColor, flick);
        }
    }

    void SetColor(Color color, float delta)
    {
        Renderer[] renderers = objectToFlick.GetComponentsInChildren<Renderer>();

        //foreach renderer set color and emission
        foreach(Renderer renderer in renderers)
        {
            renderer.material.color = Color.Lerp(normalColor, color, delta);
            renderer.material.SetColor("_EmissionColor", Color.Lerp(normalColor, color, delta));
        }
    }
}
