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

        Animation();
    }

    protected virtual Enemy GetEnemy()
    {
        return null;
    }

    void Animation()
    {
        //need model for animation
        if (objectToRotate == null) 
            return;

        //find up direction (from model to enemy)
        Vector3 upDirection;
        if (GetEnemy() && buildableObject.IsActive)
        {
            upDirection = (GetEnemy().transform.position - objectToRotate.position).normalized;
        }
        //else normal position
        else
        {
            upDirection = buildableObject.CellOwner.transform.up;
        }

        //set new rotation
        SetRotation(upDirection);
    }

    void SetRotation(Vector3 upDirection)
    {
        //set new rotation
        Quaternion upRotation = Quaternion.FromToRotation(objectToRotate.up, upDirection) * objectToRotate.rotation;
        objectToRotate.rotation = upRotation;
    }
}
