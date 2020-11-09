using UnityEngine;

public class BuildableGraphics : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Transform objectToRotate = default;
    [SerializeField] Transform baseToRotate = default;

    protected BuildableObject buildableObject;

    protected virtual void Start()
    {
        //set logic component
        buildableObject = GetComponent<BuildableObject>();
    }

    protected virtual void Update()
    {
        //do nothing when preview mode
        if (buildableObject.IsPreview)
            return;

        if (baseToRotate)
            TwoAxis_LookAtEnemy();
        else
            LookAtEnemy();
    }

    protected virtual Enemy GetEnemy()
    {
        return null;
    }

    #region two axis

    void TwoAxis_LookAtEnemy()
    {
        //need model for animation
        if (objectToRotate == null)
            return;

        //find forward direction (from model to enemy)
        Vector3 forwardBase;
        if (GetEnemy() && buildableObject.IsActive)
        {
            //get enemy position, but z axis is the same of the base
            Vector3 enemyBasePosition = GetEnemy().transform.position;
            SetZAt(enemyBasePosition, GetEnemy().coordinatesToAttack.face, baseToRotate.position);

            forwardBase = (enemyBasePosition - baseToRotate.position).normalized;

            Vector3 enemyObjectPosition = GetEnemy().transform.position;

        }
        //else normal position
        else
        {
            forwardBase = transform.forward;
        }

        //set new rotation
        TwoAxis_SetRotation(forwardBase);
    }

    void TwoAxis_SetRotation(Vector3 forwardDirection)
    {
        //già questo sta dando un problema, non gira solo su un asse come volevo .-.

        //set new rotation
        Quaternion forwardRotation = Quaternion.FromToRotation(baseToRotate.forward, forwardDirection) * baseToRotate.rotation;
        //baseToRotate.rotation = forwardRotation;
        baseToRotate.rotation = Quaternion.LookRotation(forwardDirection);
    }

    Vector3 SetZAt(Vector3 vector, EFace face, Vector3 value)
    {
        switch (face)
        {
            case EFace.front:
                vector.z = value.z;
                break;
            case EFace.right:
                vector.x = value.x;
                break;
            case EFace.back:
                vector.z = value.z;
                break;
            case EFace.left:
                vector.x = value.x;
                break;
            case EFace.up:
                vector.y = value.y;
                break;
            case EFace.down:
                vector.y = value.y;
                break;
        }

        return vector;
    }

    Vector3 SetXAt(Vector3 vector, EFace face, Vector3 value)
    {
        //nasce un problema, non so dove dovrei girare la torretta

        switch (face)
        {
            case EFace.front:
                break;
            case EFace.right:
                break;
            case EFace.back:
                break;
            case EFace.left:
                break;
            case EFace.up:
                break;
            case EFace.down:
                break;
        }

        return vector;
    }

    #endregion

    #region all axis

    void LookAtEnemy()
    {
        //need model for animation
        if (objectToRotate == null) 
            return;

        //find forward direction (from model to enemy)
        Vector3 forwardDirection;
        if (GetEnemy() && buildableObject.IsActive)
        {
            forwardDirection = (GetEnemy().transform.position - objectToRotate.position).normalized;
        }
        //else normal position
        else
        {
            forwardDirection = transform.forward;
        }

        //set new rotation
        SetRotation(forwardDirection);
    }

    void SetRotation(Vector3 forwardDirection)
    {
        //set new rotation
        Quaternion forwardRotation = Quaternion.FromToRotation(objectToRotate.forward, forwardDirection) * objectToRotate.rotation;
        objectToRotate.rotation = forwardRotation;
    }

    #endregion
}
