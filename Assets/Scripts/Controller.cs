using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Interaction Settings")]
    public float rayDistance = 2.0f;
    public float grabRadius = 0.5f;
    public float grabHeightOffset = 0.2f;
    public Transform holdPoint;
    public LayerMask interactableLayer;

    private Rigidbody rb;
    private Vector2 moveInput;
    private GameObject carriedBox;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleInput();

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (carriedBox == null)
            {
                TryPickUp();
            }
            else
            {
                DropBox();
            }
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        Vector2 currentInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) currentInput.y = 1;
            if (Keyboard.current.sKey.isPressed) currentInput.y = -1;
            if (Keyboard.current.aKey.isPressed) currentInput.x = -1;
            if (Keyboard.current.dKey.isPressed) currentInput.x = 1;
        }
        moveInput = currentInput.normalized;
    }

    private void ApplyMovement()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void TryPickUp()
    {
        RaycastHit hit;

        Vector3 origin = transform.position + Vector3.up * grabHeightOffset;

        // SphereCast: Origen, Radio, Direccion, Resultado, Distancia, Capa
        if (Physics.SphereCast(origin, grabRadius, transform.forward, out hit, rayDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Box"))
            {
                PickUp(hit.collider.gameObject);
            }
        }
    }

    private void PickUp(GameObject box)
    {
        carriedBox = box;
        Rigidbody boxRb = carriedBox.GetComponent<Rigidbody>();

        if (boxRb != null)
        {
            boxRb.linearVelocity = Vector3.zero;
            boxRb.angularVelocity = Vector3.zero;
            boxRb.isKinematic = true;
        }

        carriedBox.transform.SetParent(holdPoint);

        carriedBox.transform.localPosition = Vector3.zero;
        carriedBox.transform.localRotation = Quaternion.identity;
    }

    private void DropBox()
    {
        if (carriedBox == null) return;

        carriedBox.transform.SetParent(null);

        Rigidbody boxRb = carriedBox.GetComponent<Rigidbody>();
        if (boxRb != null)
        {
            boxRb.isKinematic = false;
            boxRb.useGravity = true;
            boxRb.WakeUp();
            boxRb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }

        carriedBox = null;
    }

    private void OnDrawGizmos()
    {
        if (transform == null) return;

        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * grabHeightOffset;

        Gizmos.DrawRay(origin, transform.forward * rayDistance);

        Gizmos.DrawWireSphere(origin, grabRadius);
        Gizmos.DrawWireSphere(origin + transform.forward * rayDistance, grabRadius);
    }
}