using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Enemy enemy;
    
    [SerializeField]
    public Transform m_Target;
    NavMeshAgent agent;
    IEnemyState currentState;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();

    }
    private void Update()
    {

    }
    // Update is called once per frame
    void FixedUpdate()
    {

    }
    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
