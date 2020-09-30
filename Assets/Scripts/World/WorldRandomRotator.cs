using System.Collections;
using UnityEngine;

public class WorldRandomRotator : WorldRotator
{
    #region variables

    bool waitRotation;
    Coroutine randomizeWorld_Coroutine;

    public WorldRandomRotator(World world) : base(world)
    {
    }

    #endregion

    public void StartRandomize()
    {
        //start randomize
        if (randomizeWorld_Coroutine != null)
            world.StopCoroutine(randomizeWorld_Coroutine);

        randomizeWorld_Coroutine = world.StartCoroutine(RandomizeWorld());
    }

    IEnumerator RandomizeWorld()
    {
        //wait before randomize
        yield return new WaitForSeconds(world.randomWorldConfig.TimeBeforeRandomize);

        //for n times, rotate row or column
        for (int i = 0; i < world.randomWorldConfig.RandomizeTimes; i++)
        {
            //randomize rotation
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, world.worldConfig.NumberCells);
            int y = Random.Range(0, world.worldConfig.NumberCells);
            ERotateDirection randomDirection = (ERotateDirection)Random.Range(0, 5);

            //effective rotation
            Rotate(face, x, y, EFace.front, randomDirection);

            //wait until the end of the rotation
            OnStartRotation();
            yield return new WaitWhile(() => waitRotation);

            //if not last rotation, wait time between every rotation
            if (i < world.randomWorldConfig.RandomizeTimes - 1)
                yield return new WaitForSeconds(world.randomWorldConfig.TimeBetweenRotation);

            //repeat
            if (world.randomWorldConfig.Loop)
                i = 0;
        }

        //call start game
        GameManager.instance.levelManager.StartGame();
    }

    void OnStartRotation()
    {
        //start wait rotation
        waitRotation = true;
        world.onEndRotation += OnEndRotation;
    }

    void OnEndRotation()
    {
        //end wait rotation
        waitRotation = false;
        world.onEndRotation -= OnEndRotation;
    }

    #region override world rotator

    protected override float GetRotationTime()
    {
        //use random rotation time
        return world.randomWorldConfig.RotationTime;
    }

    protected override bool SkipAnimation(float delta)
    {
        //can't skip random rotation
        return false;
    }

    #endregion
}
