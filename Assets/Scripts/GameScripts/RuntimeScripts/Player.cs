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

    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        if (!IsLocalPlayer)
        {
            DisableInput();
        }
        else
        {
            EnableInput();
        }
    }

    public void EnableInput()
    {
        if (m_ActionAssets == null)
            return;

        foreach (var actionAsset in m_ActionAssets)
        {
            if (actionAsset != null)
            {
                actionAsset.Enable();
            }
        }
    }

    public void DisableInput()
    {
        if (m_ActionAssets == null)
            return;

        foreach (var actionAsset in m_ActionAssets)
        {
            if (actionAsset != null)
            {
                actionAsset.Disable();
            }
        }
    }




    private IInteractible targettedInteractible = null;
    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            LayerMask mask = LayerMask.GetMask("Interactible");
            RaycastHit hitInfo;

            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

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
}
