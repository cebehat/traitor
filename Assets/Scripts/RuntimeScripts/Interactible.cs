using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Outline))]
public abstract class Interactible : MonoBehaviour, IInteractible
{
    Outline outline;

    [SerializeField]
    public bool IsTargetted { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactible");
        outline = gameObject.GetComponent<Outline>();
        Target(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Target(bool targetted)
    {
        if (targetted)
        {   
            outline.OutlineWidth = 4;
        }
        else
        {
            outline.OutlineWidth = 0;
        }
        IsTargetted = targetted;
    }

    public abstract void OnInteract(LocalCharacterMovement player);
}
