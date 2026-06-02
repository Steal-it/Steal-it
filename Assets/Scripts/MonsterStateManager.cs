using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStateManager : MonoBehaviour {
    public enum StateKey {
        Wander,
        Chase,
        Stunned
    }

    public Dictionary<StateKey, IMonsterState> StateDictionary => stateDictionary;
    public MonsterWanderModeManager MonsterWanderModeManager => monsterWanderModeManager;
    public Monster Monster => monster;
    public NavMeshAgent Agent => monster.GetComponent<NavMeshAgent>();
    public float ViewRadius => viewRadius;
    public float ViewAngle => viewAngle;
    public float MinChasingSpeed => minChasingSpeed;
    public float MaxChasingSpeed => maxChasingSpeed;
    public float ChasingAcceleration => chasingAcceleration;
    public float KillDistance => killDistance;
    public LayerMask EverythingButAvatarLayer => everythingButAvatarLayer;
    public NavMeshSurface WanderNavMeshSurface => wanderNavMeshSurface;
    public NavMeshSurface ChaseNavMeshSurface => chaseNavMeshSurface;
    public float LightExposureTime => lightExposureTime;

    [SerializeField]
    private Monster monster;
    [SerializeField, Range(0.2f, 1)]
    private float lightExposureTime = 0.5f;

    [Header("Wander Mode")]
    [SerializeField]
    private MonsterWanderModeManager monsterWanderModeManager;
    [SerializeField, Range(10, 30)]
    private float viewRadius = 20;
    [SerializeField, Range(0, 360)]
    private float viewAngle = 150;
    [SerializeField]
    private LayerMask everythingButAvatarLayer;
    [SerializeField]
    private NavMeshSurface wanderNavMeshSurface;

    [Header("Chase Mode")]
    [SerializeField, Range(1, 3)]
    private float minChasingSpeed = 1.5f;
    [SerializeField, Range(3, 6)]
    private float maxChasingSpeed = 5;
    [SerializeField, Range(1, 3)]
    private float chasingAcceleration = 2;
    [SerializeField, Range(0, 4)]
    private float killDistance = 1.5f;
    [SerializeField]
    private NavMeshSurface chaseNavMeshSurface;

    private Dictionary<StateKey, IMonsterState> stateDictionary = new Dictionary<StateKey, IMonsterState>();
    private IMonsterState currentState;

    void Start() {
        stateDictionary.Add(StateKey.Wander, new WanderState());
        stateDictionary.Add(StateKey.Chase, new ChaseState());

        currentState = stateDictionary[StateKey.Wander];
        currentState.EnterState(this);
    }

    void Update() {
        currentState.UpdateState();
    }

    public void ChangeState(StateKey _stateKey) {
        currentState.ExitState();

        currentState = stateDictionary[_stateKey];

        currentState.EnterState(this);
    }
}