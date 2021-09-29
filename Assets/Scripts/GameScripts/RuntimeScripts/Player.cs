using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif

public class Player : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Input action assets to affect when inputs are enabled or disabled.")]
    List<InputActionAsset> m_ActionAssets;

    [SerializeField]
    public InputActionReference MoveAction;

    [SerializeField]
    public InputActionReference LookAction;

    public NetworkVariable<Vector3> NetworkPosition = new NetworkVariable<Vector3>(NetworkVariableReadPermission.Everyone);
    public NetworkVariable<Quaternion> NetworkRotation = new NetworkVariable<Quaternion>(NetworkVariableReadPermission.Everyone);



    Transform cameraTransform;
    float pitch = 0f;

    [Range(1f, 90f)]
    public float maxPitch = 85f;
    [Range(-1f, -90f)]
    public float minPitch = -85f;
    [Range(0.1f, 0.5f)]
    public float mouseSensitivity = 0.1f;

    CharacterController characterController;
    Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        cameraTransform = GetComponentInChildren<Camera>().transform;
        characterController = GetComponent<CharacterController>();
        if (!IsLocalPlayer)
        {
            cameraTransform.gameObject.SetActive(false);
        }
        else
        {
            
            collider = GetComponent<CapsuleCollider>();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PlayerCollidable"))
        {
            Debug.Log("Player Trigger");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player Collision");
    }

    private void OnClientConnected(ulong obj)
    {
        Debug.Log("Player connect");
    }

    private IInteractible targettedInteractible = null;
    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            Look();
            Move();
            HandleInteractions();
        }
    }

    void Look()
    {
        var action = LookAction.ToInputAction();
        if (action != null && action.phase == InputActionPhase.Started)
        {
            Vector2 input = action.ReadValue<Vector2>();
            float xInput = input.x * mouseSensitivity;
            float yInput = input.y * mouseSensitivity;

            RotateServerRpc(xInput);

            pitch -= yInput;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rot = Quaternion.Euler(pitch, 0, 0);
            cameraTransform.localRotation = rot;
        }
    }

    void Move()
    {
        var action = MoveAction.ToInputAction();
        if (action != null && action.phase == InputActionPhase.Started)
        {
            Vector2 value = action.ReadValue<Vector2>();

            float hAxis = value.x;
            float vAxis = value.y;

            var forward = transform.forward;
            var right = transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            var move = forward * vAxis + right * hAxis;
            //var move = new Vector3(hAxis, 0f, vAxis);

            if (move != Vector3.zero)
            {
                characterController.Move(move.normalized * Time.deltaTime * 5f);                
                TranslateServerRpc(move.normalized * Time.deltaTime * 5f);
                //transform.Translate(move * Time.deltaTime * 5f);
            }
        }
    }

    [ServerRpc]
    void TranslateServerRpc(Vector3 translation)
    {
        this.characterController.Move(translation);
        //this.transform.Translate(translation);
        TranslateClientRpc(translation);
    }

    [ClientRpc]
    void TranslateClientRpc(Vector3 translation)
    {
        characterController.Move(translation);
        //this.transform.Translate(translation);
    }

    [ServerRpc]
    void RotateServerRpc(float rotation)
    {
        this.transform.Rotate(0, rotation, 0);
        RotateClientRpc(rotation);
    }

    [ClientRpc]
    void RotateClientRpc(float rotation)
    {
        this.transform.Rotate(0, rotation, 0);
    }

    private void HandleInteractions()
    {
        LayerMask mask = LayerMask.GetMask("Interactible");
        RaycastHit hitInfo;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out hitInfo, 20f) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Interactible"))
        {
            var interactible = hitInfo.collider.GetComponent<IInteractible>();
            if (targettedInteractible != interactible)
            {
                Debug.Log("Hit Interactible: " + hitInfo.collider.name);
                interactible.Target(true);
                if (targettedInteractible != null)
                {
                    targettedInteractible.Target(false);
                }

            }
            targettedInteractible = interactible;
        }
        else
        {
            if (targettedInteractible != null)
            {
                targettedInteractible.Target(false);
                targettedInteractible = null;
            }
        }
    }
}
