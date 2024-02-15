using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform _bulletStainPrefab;
    [SerializeField] private ParticleSystem _btoomFXPrefab;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _raycastDistance = 0.5f;
    [SerializeField] private float _ricochetForce = 10f;
    [SerializeField] private int _reboundNumber = 2;

    private Transform _transform;
    private Vector3 _velocity;
    private int _obstacleLayer;
    private bool _isQuitting;
    
    private const string ObstacleLayerName = "Obstacle";

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _obstacleLayer = LayerMask.NameToLayer(ObstacleLayerName);
    }

    private void Update()
    {
        Ray ray = new Ray(_transform.position, _velocity);

        if (!Physics.Raycast(ray, out RaycastHit hit, _raycastDistance))
        {
            Move();

            if (_reboundNumber <= 0)
                Destroy(gameObject);
        }
        else
        {
            SetNewVelocity(hit);

            _reboundNumber--;
            
            CreateBulletStain(hit);
        }
    }

    private void OnEnable() => 
        Destroy(gameObject, 5f);

    private void OnApplicationQuit() => 
        _isQuitting = true;

    private void OnDestroy()
    {
        if (!_isQuitting)
            Instantiate(_btoomFXPrefab, _transform.position, _btoomFXPrefab.transform.rotation);
    }

    public void SetVelocityAndAngle(Vector3 velocity, float angle)
    {
        _transform.rotation = Quaternion.Euler(angle, 0, 0);
        _velocity = velocity;
    }

    private void Move()
    {
        _transform.Translate(_velocity * (_speed * Time.deltaTime), Space.World);

        Vector3 newVelocity = _velocity + Vector3.up * (Physics.gravity.y * Time.deltaTime);

        _velocity = newVelocity;
    }

    private void CreateBulletStain(RaycastHit hit)
    {
        if (hit.transform.gameObject.layer == _obstacleLayer) 
            Instantiate(_bulletStainPrefab, _transform.position, hit.transform.rotation);
    }

    private void SetNewVelocity(RaycastHit hit)
    {
        Vector3 ricochetDirection = Vector3.Reflect(_velocity, hit.normal).normalized;

        _velocity = ricochetDirection * _ricochetForce;
    }
}