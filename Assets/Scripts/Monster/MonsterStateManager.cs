using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MonsterStateManager : MonoBehaviour {
    public enum StateKey {
        Wander,
        Chase,
        Murder,
        Flashed
    }

    #region Public References
    public IReadOnlyDictionary<StateKey, IMonsterState> StateDictionary => stateDictionary;
    public StateKey CurrentStateKey => currentStateKey;
    public NavMeshAgent MonsterAgent => monsterAgent;
    public MonsterAnimator MonsterAnimator => monsterAnimator;
    public MonsterSFXManager MonsterSFXManager => monsterSFXManager;

    // Wander Mode
    public float ViewRadius => viewRadius;
    public float ViewAngle => viewAngle;
    public LayerMask WallLayer => wallLayer;
    public MonsterRandomDestinationManager MonsterRandomDestinationManager => monsterWanderModeManager;

    // Stunned Mode
    public float StunnedMinDistanceDestination => stunnedMinDistanceDestination;

    // Wander and Stunned Mode
    public float WanderAndStunnedSpeed => wanderAndStunnedSpeed;
    public NavMeshSurface WanderAndStunnedNavMeshSurface => wanderAndStunnedNavMeshSurface;

    // Chase Mode
    public float MinChasingSpeed => minChasingSpeed;
    public float MaxChasingSpeed => maxChasingSpeed;
    public float ChasingAcceleration => chasingAcceleration;
    public float KillDistance => killDistance;
    public NavMeshSurface ChaseNavMeshSurface => chaseNavMeshSurface;
    #endregion

    #region Properties
    [SerializeField]
    private NavMeshAgent monsterAgent;
    [SerializeField]
    private MonsterAnimator monsterAnimator;
    [SerializeField]
    private MonsterSFXManager monsterSFXManager;

    [Header("Wander Mode")]
    [SerializeField]
    private MonsterRandomDestinationManager monsterWanderModeManager;
    [SerializeField, Range(10, 30)]
    private float viewRadius = 20;
    [SerializeField, Range(0, 360)]
    private float viewAngle = 150;
    [SerializeField]
    private LayerMask wallLayer;

    [Header("Stunned Mode")]
    [SerializeField, Range(20, 50)]
    private float stunnedMinDistanceDestination = 30;

    [Header("Wander and Stunned Mode")]
    [SerializeField, Range(2, 4)]
    private float wanderAndStunnedSpeed = 3;
    [SerializeField]
    private NavMeshSurface wanderAndStunnedNavMeshSurface;

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
    #endregion

    private Dictionary<StateKey, IMonsterState> stateDictionary = new Dictionary<StateKey, IMonsterState>();
    private IMonsterState currentState;
    private StateKey currentStateKey;

    void OnValidate() {
        if (minChasingSpeed > maxChasingSpeed) {
            minChasingSpeed = maxChasingSpeed;
        }
    }

    void Start() {
        stateDictionary.Add(StateKey.Wander, new WanderState());
        stateDictionary.Add(StateKey.Chase, new ChaseState());
        stateDictionary.Add(StateKey.Murder, new MurderState());
        stateDictionary.Add(StateKey.Flashed, new FlashedState());

        currentStateKey = StateKey.Wander;
        currentState = stateDictionary[currentStateKey];
        currentState.EnterState(this);
    }

    void Update() {
        currentState.UpdateState();
    }

    public void ChangeState(StateKey _stateKey) {
        Debug.Log(currentStateKey + " -> " + _stateKey);

        currentState.ExitState();

        currentStateKey = _stateKey;
        currentState = stateDictionary[_stateKey];

        currentState.EnterState(this);
    }
}