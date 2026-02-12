using System.Collections;
using UnityEngine;

/// <summary>
/// Put on enemy object.
/// EnemyName auto-syncs from GameObject name.
/// Moves in 2D (XY).
/// </summary>
public class EnemyRouteMover : MonoBehaviour
{
    [Header("Auto from object name")]
    [SerializeField] private string enemyName;

    [Header("Movement")]
    [SerializeField] private float arriveThreshold = 0.03f;
    [SerializeField] private bool faceMoveDirection = false;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    //NEW
    [SerializeField] private string attackParam = "Attack";

    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";
    [SerializeField] private string speedParam = "Speed";

    private WaveSystem waveSystem;



    private EnemyRoute _route;
    private Coroutine _followRoutine;
    private bool _running;
    private bool _paused;
    private EnemyAI _enemyAI;

    public string EnemyName => enemyName;
    public bool IsPaused => _paused;

    private void Awake()
    {
        waveSystem = WaveSystem.Instance;
        if (waveSystem == null)
            waveSystem = FindFirstObjectByType<WaveSystem>();

        enemyName = gameObject.name;
        _enemyAI = GetComponent<EnemyAI>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            enemyName = gameObject.name;
    }
#endif

    public void AssignRoute(EnemyRoute route)
    {
        _route = route;
        _enemyAI?.SetCurrentRouteDebug(_route != null ? _route.RouteName : "None");

        if (_followRoutine != null)
            StopCoroutine(_followRoutine);

        if (_route == null)
        {
            _running = false;
            return;
        }

        _route.RefreshNodesFromChildren();

        if (_route.Nodes.Count == 0)
        {
            Debug.LogWarning($"Enemy '{enemyName}' assigned route '{_route.RouteName}' with no nodes.");
            _running = false;
            return;
        }
        
        _running = true;
        _paused = false;
        _followRoutine = StartCoroutine(FollowRouteRoutine());
    }

    public void StopRoute()
    {
        _running = false;
        _paused = false;
        _enemyAI?.SetCurrentRouteDebug("None");



        if (_followRoutine != null)
        {
            StopCoroutine(_followRoutine);
            _followRoutine = null;
        }
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;
    }

    private void OnEnable()
    {
        EnemyAI.RegisterEnemy(this);
    }

    private void OnDisable()
    {
        EnemyAI.UnregisterEnemy(this);
    }


    private IEnumerator FollowRouteRoutine()
    {
        int index = 0;

        while (_running && _route != null && _route.Nodes.Count > 0)
        {
            if (_paused)
            {
                yield return null;
                continue;
            }

            if (index < 0 || index >= _route.Nodes.Count)
                yield break;

            RouteNode node = _route.Nodes[index];

            bool isLastNode = (index == _route.Nodes.Count - 1);

            

            if (node == null)
            {
                index = NextIndex(index);
                if (index == -1) yield break;
                yield return null;
                continue;
            }

            // Delay before moving to this node --------------------------------------
            if (node.delay > 0f)
            {
                float t = 0f;
                while (t < node.delay)
                {
                    if (!_running) yield break;
                    if (!_paused) t += Time.deltaTime;
                    yield return null;
                }
            }

            // Move toward node with node-specific speed
            while (_running && node != null)
            {
                Debug.Log("SetSpeed");

                if (_paused)
                {
                    yield return null;
                    continue;
                }

                Vector3 current = transform.position;
                Vector3 target = node.transform.position;
                target.z = current.z;

                Vector3 next = Vector3.MoveTowards(current, target, node.speed * Time.deltaTime);
                Vector3 delta = next - current;

                transform.position = next;

                if (Vector2.Distance(transform.position, target) <= arriveThreshold)
                {

                    if (animator != null)
                        animator.SetFloat(speedParam, 0f);
                    // If it's the last node and the route doesn't loop (or even if it does)
                    if (index == _route.Nodes.Count - 1 && !_route.loop)
                    {
                        StartCoroutine(PerformAttack());
                        yield break; // Stop moving
                    }
                    break;
                }
                yield return null;

                if (animator != null)
                {
                    Vector2 dir = new Vector2(delta.x, delta.y);


                    //this stops flickering when idle/stopped
                    if (dir.sqrMagnitude > 0.00001f)
                    {
                        dir.Normalize();
                        animator.SetFloat(moveXParam, dir.x);
                        animator.SetFloat(moveYParam, dir.y);
                    }


                    if (!string.IsNullOrEmpty(speedParam))
                        animator.SetFloat(speedParam, dir.magnitude);
                }

                //TEST
                if (faceMoveDirection && delta.sqrMagnitude > 0.000001f)
                {
                    float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }

                if (Vector2.Distance(transform.position, target) <= arriveThreshold)
                    break;

                yield return null;
            }

            index = NextIndex(index);
            if (!_route.loop && index == -1)
                yield break;
        }
    }

    private IEnumerator PerformAttack()
    {
        Debug.Log("PerformAttack triggered");

        animator.SetFloat(speedParam, 0f);
        animator.SetBool("Attacking?", true);

        _running = false;

        if (animator != null)
        {
            animator.SetBool("Attacking?", true);
            animator.SetFloat(speedParam, 0f); // stop movement animation
        }

        yield return new WaitForSeconds(1f); // match attack animation length

        GuardianHealth guardian = FindFirstObjectByType<GuardianHealth>();
        if (guardian != null)
        {
            guardian.TakeDamage(1);
        }

        Destroy(gameObject);

        if (waveSystem != null)
                waveSystem.DecreaseEnemyCount();
            else
                Debug.LogError($"No WaveSystem found in scene. Cannot decrease enemy count for {name}.");
    }


    private int NextIndex(int currentIndex)
    {
        int last = _route.Nodes.Count - 1;
        if (currentIndex < last) return currentIndex + 1;
        return _route.loop ? 0 : -1;
    }

    public void SetEnemyName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.LogWarning("Enemy name cannot be empty. Ignored.");
            return;
        }
        enemyName = newName;
        gameObject.name = newName;
    }

}