using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarScreen : MonoBehaviour
{
    [SerializeField] LayerMask _layerMask;
    [SerializeField][Range(100, 5000)] float _radarRange = 500f;
    [SerializeField][Range(0, 1000)] float _lockOnRange = 1000f;
    [SerializeField][Range(0, 45)] float _lockOnRadius = 15f;
    [SerializeField] int _maxTargets = 200;
    [SerializeField] float _refreshDelay = 0.25f;
    //[SerializeField] GameObject _radarScreen;
    [SerializeField] Transform _player;

    Transform _transform;
    WaitForSeconds _waitForSeconds;
    List<Transform> _targetsInRange;

    Renderer _radarRenderer;
    Vector3 _localDirection;
    float _angleToTarget;
    float _playerAngle;
    float _radarAngle;
    float _normalizedDistance;
    float _angleRadians;
    float _blipX, _blipY;
    float _pitch;
    Collider[] _targetColliders;

    public Transform LockedOnTarget { get; private set; }
    int TargetsInRange => _targetsInRange.Count;
    bool InCombat { get; set; }

    void Awake()
    {
        //if (!_radarScreen) return;
        _targetsInRange = new List<Transform>();

        //_radarRenderer = _radarScreen.GetComponent<Renderer>();
        _transform = transform;
        LockedOnTarget = null;
    }

    void Start()
    {
        //if (!_radarRenderer) return;
        _targetColliders = new Collider[_maxTargets];
        _waitForSeconds = new WaitForSeconds(_refreshDelay);
        //var bounds = _radarRenderer.bounds;
    }

    void OnEnable()
    {
        StartCoroutine(nameof(RefreshTargetList));
    }

    void OnDisable()
    {
        StopCoroutine(nameof(RefreshTargetList));
    }

    void LateUpdate()
    {
        UIManager.Instance.UpdateTargetIndicators(_targetsInRange, LockedOnTarget ? LockedOnTarget.GetInstanceID() : -1);
        if (TargetsInRange > 0)
        {
            if (InCombat) return;
            InCombat = true;
            //GameManager.Instance.InCombat(true);
            return;
        }

        if (!InCombat) return;

        InCombat = false;
        //GameManager.Instance.InCombat(false);
    }

    IEnumerator RefreshTargetList()
    {
        int size = 0;
        while (true)
        {
            _targetsInRange.Clear();
            LockedOnTarget = null;
            float closest = _lockOnRange;
            var myPosition = _transform.position;
            size = Physics.OverlapSphereNonAlloc(_transform.position, _radarRange, _targetColliders, _layerMask);
            for (int i = 0; i < size; ++i)
            {
                var target = GetRootTransform(i);
                if (!target.gameObject.activeSelf) continue;

                closest = TryLockOnTarget(target, myPosition, closest);

                if (!_targetsInRange.Contains(target))
                {
                    _targetsInRange.Add(target);
                }

            }

            if (_targetsInRange.Count > 0)
            {
                string targets = string.Join(", ", _targetsInRange.ConvertAll(t => t.name));
                //Debug.Log($"Targets in range ({_targetsInRange.Count}): {targets}");
            }
            else
            {
                //Debug.Log("No targets in range.");
            }

            yield return _waitForSeconds;
        }
    }

    float TryLockOnTarget(Transform target, Vector3 myPosition, float closest)
    {
        var targetPosition = target.position;
        var distance = Vector3.Distance(targetPosition, myPosition);
        var direction = targetPosition - myPosition;
        var angle = Vector3.Angle(direction, _transform.forward);
        if (distance < closest && angle < _lockOnRadius)
        {
            closest = distance;
            LockedOnTarget = target;
        }

        return closest;
    }

    Transform GetRootTransform(int i)
    {
        Transform root = _targetColliders[i].transform;
        int layer = root.gameObject.layer;
        while (root.parent && layer == root.parent.gameObject.layer)
        {
            root = root.parent;
        }

        return root;
    }
}