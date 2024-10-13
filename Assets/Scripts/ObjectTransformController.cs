using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectTransformController : MonoBehaviour
{
    private GameObject spawnedObject;
    private float initialDistance;
    private Vector3 initialScale;
    private float initialRotationAngle;
    private float currentRotationAngle;

    private bool objectSpawned = false;

    private float mouseScaleFactor = 0.002f;
    private float minScaleThreshold = 0.01f;
    private float rotationSpeed = 0.1f;

    private bool isRotatingWithMouse = false;

    public void SetSpawnedObject(GameObject obj)
    {
        spawnedObject = obj;
        objectSpawned = true;
        initialScale = obj.transform.localScale;
    }

    void Update()
    {
        if (!objectSpawned || spawnedObject == null) return;

        // Handle scaling
        HandleTouchScaling();
        HandleMouseScrollScaling();

        // Handle rotation
        HandleTouchRotation();
        HandleMouseRotation();
    }

    // --------- Scaling Logic ---------
    private void HandleTouchScaling()
    {
        if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touchZero = Touchscreen.current.touches[0];
            var touchOne = Touchscreen.current.touches[1];

            Vector2 touchZeroPosition = touchZero.position.ReadValue();
            Vector2 touchOnePosition = touchOne.position.ReadValue();

            float currentDistance = Vector2.Distance(touchZeroPosition, touchOnePosition);

            if (touchZero.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
                touchOne.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                initialDistance = currentDistance;
                initialScale = spawnedObject.transform.localScale;
            }
            else if (touchZero.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touchOne.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                if (initialDistance > 0)
                {
                    float scaleMultiplier = currentDistance / initialDistance;
                    Vector3 newScale = initialScale * scaleMultiplier;

                    MinimumThreshold(newScale);
                }
            }
        }
    }

    private void HandleMouseScrollScaling()
    {
        if (Mouse.current != null)
        {
            // Get the scroll delta (-120 or 120)
            Vector2 scroll = Mouse.current.scroll.ReadValue();

            if (scroll.y != 0)
            {
                // Reducing sensitivity factor for smoother scaling
                Vector3 newScale = spawnedObject.transform.localScale + Vector3.one * scroll.y * mouseScaleFactor;

                MinimumThreshold(newScale);
            }
        }
    }

    private void MinimumThreshold(Vector3 newScale)
    {
        spawnedObject.transform.localScale = new Vector3(
            Mathf.Max(newScale.x, minScaleThreshold),
            Mathf.Max(newScale.y, minScaleThreshold),
            Mathf.Max(newScale.z, minScaleThreshold)
        );
    }

    // --------- Rotation Logic ---------
    // Handle rotation with two-finger twist gesture
    private void HandleTouchRotation()
    {
        if (Touchscreen.current != null && Touchscreen.current.touches.Count == 2)
        {
            var touchZero = Touchscreen.current.touches[0];
            var touchOne = Touchscreen.current.touches[1];

            Vector2 touchZeroPosition = touchZero.position.ReadValue();
            Vector2 touchOnePosition = touchOne.position.ReadValue();

            float angle = Mathf.Atan2(touchOnePosition.y - touchZeroPosition.y, touchOnePosition.x - touchZeroPosition.x) * Mathf.Rad2Deg;

            if (touchZero.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
                touchOne.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                initialRotationAngle = angle;
            }
            else if (touchZero.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touchOne.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float rotationDelta = angle - initialRotationAngle;
                spawnedObject.transform.Rotate(0, -rotationDelta, 0);
                initialRotationAngle = angle;
            }
        }
    }

    // Handle rotation with mouse drag
    private void HandleMouseRotation()
    {
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isRotatingWithMouse = true;
            }

            if (isRotatingWithMouse && Mouse.current.leftButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                spawnedObject.transform.Rotate(0, -mouseDelta.x * rotationSpeed, 0);
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isRotatingWithMouse = false;
            }
        }
    }
}