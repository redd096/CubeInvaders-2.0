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
            LookAtEnemy_TwoAxis();
        else
            LookAtEnemy();
    }

    protected virtual Enemy GetEnemy()
    {
        return null;
    }

    #region look at enemy

    void LookAtEnemy()
    {
        //find forward direction (from model to enemy)
        Vector3 forwardDirection;
        if (GetEnemy() && buildableObject.IsActive)
        {
            forwardDirection = (GetEnemy().transform.position - objectToRotate.position).normalized;
        }
        //else normal forward
        else
        {
            forwardDirection = transform.forward;
        }

        //set new rotation
        SetRotation(objectToRotate, forwardDirection);
    }

    void LookAtEnemy_TwoAxis()
    {
        //if active and there is an enemy, rotate towards enemy
        if (buildableObject.IsActive && GetEnemy())
        {
            RotateOnAxis(baseToRotate, GetEnemy().transform.position, transform.forward, baseToRotate.up);
            RotateOnAxis(objectToRotate, GetEnemy().transform.position, baseToRotate.right, objectToRotate.right);
        }
        //else look normal forward
        else
        {
            SetRotation(baseToRotate, -transform.up);
            SetRotation(objectToRotate, transform.forward);
        }
    }

    #endregion

    #region rotate transform

    void RotateOnAxis(Transform transformToRotate, Vector3 forwardPosition, Vector3 planeAxis, Vector3 rotateAxis)
    {
        if (transformToRotate == null)
            return;

        //project enemy and object position on same plane, then calculate direction
        Vector3 enemyPosition = Vector3.ProjectOnPlane(forwardPosition, planeAxis);
        Vector3 transformPosition = Vector3.ProjectOnPlane(transformToRotate.position, planeAxis);
        Vector3 direction = (enemyPosition - transformPosition).normalized;

        //calculate angle (if angle is 0, stop rotation)
        float angle = Vector3.Angle(direction, transformToRotate.forward);
        if (angle == Mathf.Epsilon)
            return;

        //get rotation on axis 
        Quaternion rotation = Quaternion.AngleAxis(angle, rotateAxis) * transformToRotate.rotation;

        //if angle is greater, then angle must be negative
        if (Vector3.Angle(direction, rotation * Vector3.forward) > angle)
        {
            rotation = Quaternion.AngleAxis(-angle, rotateAxis) * transformToRotate.rotation;
        }

        //set rotation
        transformToRotate.rotation = rotation;
    }

    void SetRotation(Transform transformToRotate, Vector3 forwardDirection)
    {
        if (transformToRotate == null)
            return;

        //set new rotation
        Quaternion forwardRotation = Quaternion.FromToRotation(transformToRotate.forward, forwardDirection) * transformToRotate.rotation;
        transformToRotate.rotation = forwardRotation;
    }

    #endregion
}
