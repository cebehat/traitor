using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
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
