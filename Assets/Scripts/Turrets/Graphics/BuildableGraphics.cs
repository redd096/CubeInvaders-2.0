using UnityEngine;

public class BuildableGraphics : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Transform objectToRotate = default;

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

        LookAtEnemy();
    }

    protected virtual Enemy GetEnemy()
    {
        return null;
    }

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
            forwardDirection = buildableObject.CellOwner.transform.forward;
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
}
