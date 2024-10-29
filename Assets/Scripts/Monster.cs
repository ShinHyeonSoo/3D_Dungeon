using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class Monster : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int _health;
    public float _walkSpeed;
    public float _runSpeed;
    public ItemData[] _dropOnDeath;

    [Header("AI")]
    private NavMeshAgent _navAgent;
    private AIState _aiState;
    public float _detectDistance;

    [Header("Wandering")]
    public float _minWanderDistance;
    public float _maxWanderDistance;
    public float _minWanderWaitTime;
    public float _maxWanderWaitTime;

    [Header("Combat")]
    public int _damage;
    public float _attackRate;
    public float _attackDistance;
    private float _lastAttackTime;

    private float _playerDistance;

    public float _fieldOfView = 120f;

    private Animator _animator;
    private SkinnedMeshRenderer[] _meshRenderers;

    // Start is called before the first frame update
    private void Awake()
    {
        //MapManager.Instance.InitMapBake();

        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        
    }

    private void Start()
    {
        //SetState(AIState.Wandering);
    }

    // Update is called once per frame
    void Update()
    {
        _playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        _animator.SetBool("Moving", _aiState != AIState.Idle);

        switch (_aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void SetState(AIState state)
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log("Agent is on the NavMesh.");
        }
        else
        {
            Debug.Log("Agent is not on the NavMesh.");
        }

        _aiState = state;

        switch (state)
        {
            case AIState.Idle:
                _navAgent.speed = _walkSpeed;
                _navAgent.isStopped = true;
                break;
            case AIState.Wandering:
                _navAgent.speed = _walkSpeed;
                _navAgent.isStopped = false;
                break;
            case AIState.Attacking:
                _navAgent.speed = _runSpeed;
                _navAgent.isStopped = false;
                break;
        }

        _animator.speed = _navAgent.speed / _walkSpeed;
    }

    private void PassiveUpdate()
    {
        // 플레이어가 없으면 랜덤한 위치를 지정해 이동
        if (_aiState == AIState.Wandering && _navAgent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(_minWanderWaitTime, _maxWanderWaitTime));
        }

        // 플레이어를 감지하면 공격상태 돌입
        if (_playerDistance < _detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    private void WanderToNewLocation()
    {
        // 임의의 위치에 목표를 설정하고 이동하는 기능
        if (_aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        _navAgent.SetDestination(GetWanderLocation());
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // SamplePosition((Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
        // areaMask 에 해당하는 NavMesh 중에서 maxDistance 반경 내에서 sourcePosition에 가장 가까운 위치를 찾아서 그 결과를 hit에 담음
        NavMesh.SamplePosition(transform.position +
            (Random.onUnitSphere * Random.Range(_minWanderDistance, _maxWanderDistance)),
            out hit, _maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        // 수치가 너무 작을 수 있으므로 반복해서 실행 해보기
        while (Vector3.Distance(transform.position, hit.position) < _detectDistance)
        {
            NavMesh.SamplePosition(transform.position +
            (Random.onUnitSphere * Random.Range(_minWanderDistance, _maxWanderDistance)),
            out hit, _maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        }

        return hit.position;
    }

    private void AttackingUpdate()
    {
        if (_playerDistance < _attackDistance && IsPlayerInFieldOfView())
        {
            _navAgent.isStopped = true;
            if (Time.time - _lastAttackTime > _attackRate)
            {
                _lastAttackTime = Time.time;
                CharacterManager.Instance.Player.Controller.GetComponent<IDamageable>().TakePhysicalDamage(_damage);
                _animator.speed = 1f;
                _animator.SetTrigger("Attack");
            }
        }
        else
        {
            if (_playerDistance < _detectDistance)
            {
                _navAgent.isStopped = false;
                NavMeshPath path = new NavMeshPath(); // 다양한 정보로 세분화 가능 (추적 불가, 장애물의 여부 등)
                if (_navAgent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    _navAgent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    _navAgent.SetDestination(transform.position);
                    _navAgent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                _navAgent.SetDestination(transform.position);
                _navAgent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < _fieldOfView * 0.5f;
    }

    public void TakePhysicalDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }

        StartCoroutine(DamageFlash());
    }

    private void Die()
    {
        for (int i = 0; i < _dropOnDeath.Length; ++i)
        {
            Instantiate(_dropOnDeath[i]._dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < _meshRenderers.Length; ++i)
        {
            _meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < _meshRenderers.Length; ++i)
        {
            _meshRenderers[i].material.color = Color.white;
        }
    }
}
