using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform _bulletStainPrefab;
    [SerializeField] private ParticleSystem _btoomPrefab;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _raycastDistance = 0.5f;
    [SerializeField] private float _ricochetForce = 10f;
    [SerializeField] private int _reboundNumber = 2;

    private Transform _transform;
    private Vector3 _initialVelocity;
    private int _obstacleLayer;
    private bool _isQuitting;

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void OnDestroy()
    {
        if (!_isQuitting)
            Instantiate(_btoomPrefab, _transform.position, _btoomPrefab.transform.rotation);
    }

    private void OnEnable()
    {
        Destroy(gameObject, 5f);
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    private void Update()
    {
        var ray = new Ray(_transform.position, _initialVelocity);

        if (!Physics.Raycast(ray, out var hit, _raycastDistance))
        {
            Move();

            if (_reboundNumber <= 0)
                Destroy(gameObject);
        }
        else
        {
            var ricochetDirection = Vector3.Reflect(_initialVelocity, hit.normal).normalized;

            _initialVelocity = ricochetDirection * _ricochetForce;

            _reboundNumber--;
            if (hit.transform.gameObject.layer == _obstacleLayer)
            {
                Instantiate(_bulletStainPrefab, _transform.position, hit.transform.rotation);
            }
        }
    }

    private void Move()
    {
        var deltaTime = Time.deltaTime;
        //_transform.position += _initialVelocity * (deltaTime * _speed);
        _transform.Translate(_initialVelocity * (deltaTime * _speed), Space.World);

        var newVelocity = _initialVelocity + Vector3.up * (Physics.gravity.y * deltaTime);

        _initialVelocity = newVelocity;
    }


    public void SetVelocityAndAngle(Vector3 velocity, float angle)
    {
        _transform.rotation = Quaternion.Euler(angle, 0, 0);
        _initialVelocity = velocity;
    }
}