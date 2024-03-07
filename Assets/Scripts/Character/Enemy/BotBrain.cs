using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BotBrain : MonoBehaviour, IMoveInput, IWeaponInput
{
    public UnityEvent OnSprinting { get;} = new();
    public UnityEvent OnJumped { get;} = new();
    public bool CanJump { get; set; } = true;
    public bool CanSprint { get; set; } = false;

    public UnityEvent<bool> IsSprinting { get; } = new();

    public UnityEvent<bool> IsShooting { get; } = new();

    public UnityEvent<float> OnWeaponSwitch { get; } = new();

    public UnityEvent OnWeaponDrop { get; } = new();

    public UnityEvent<bool> IsCrouching { get; } = new();

    public UnityEvent OnAmmoReload => throw new System.NotImplementedException();

    [SerializeField]
    private EyeTrigger EyeTrigger;
    private NavMeshAgent agent;
    [SerializeField]
    private Collider eye;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EyeTrigger.OnPlayerContact.AddListener(Chase);
    }

    private void Chase(Vector3 playerPosition)
    {
        agent.SetDestination(playerPosition);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
