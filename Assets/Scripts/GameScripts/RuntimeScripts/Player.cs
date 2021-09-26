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

    /// <summary>
    /// Enable all actions referenced by this component.
    /// </summary>
    /// <remarks>
    /// This function will automatically be called when this <see cref="InputActionManager"/> component is enabled.
    /// However, this method can be called to enable input manually, such as after disabling it with <see cref="DisableInput"/>.
    /// <br />
    /// Note that enabling inputs will only enable the action maps contained within the referenced
    /// action map assets (see <see cref="actionAssets"/>).
    /// </remarks>
    /// <seealso cref="DisableInput"/>
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

    /// <summary>
    /// Disable all actions referenced by this component.
    /// </summary>
    /// <remarks>
    /// This function will automatically be called when this <see cref="InputActionManager"/> component is disabled.
    /// However, this method can be called to disable input manually, such as after enabling it with <see cref="EnableInput"/>.
    /// <br />
    /// Note that disabling inputs will only disable the action maps contained within the referenced
    /// action map assets (see <see cref="actionAssets"/>).
    /// </remarks>
    /// <seealso cref="EnableInput"/>
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
