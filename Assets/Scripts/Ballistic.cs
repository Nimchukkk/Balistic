using UnityEngine;
using System.Collections.Generic;

public class Ballistic : MonoBehaviour
{
    [SerializeField] private PowerSlider _powerSlider;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Capsule _capsule;
    [SerializeField] private Transform _aim;
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _scrollSpeed = 10;
    [SerializeField] private int _maxCurveLength = 50;

    private Transform _transform;
    private LineRenderer _lineRenderer;
    private float _maxForce;

    private const string MouseScrollWheelName = "Mouse ScrollWheel";
    private const string VerticalAxisName = "Vertical";
    private const string HorizontalAxisName = "Horizontal";

    private Vector3 Velocity => _aim.rotation * new Vector3(0, 0, _force);

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _powerSlider.SetMaxValue(_force);
        _maxForce = _force;
    }

    private void Update()
    {
        BallisticRotate();
        TrajectoryLine();
        PowerRegulation();
        Shoot();
    }

    private void BallisticRotate()
    {
        Vector3 vertical = Vector3.left * (Input.GetAxis(VerticalAxisName) * _speed / 2 * Time.deltaTime);
        Vector3 horizontal = Vector3.up * (Input.GetAxis(HorizontalAxisName) * _speed * Time.deltaTime);

        _transform.Rotate(horizontal);
        
        _capsule.GetTransform.localRotation.ToAngleAxis(out float angle, out _);

        float rotationLimits = Mathf.Clamp(angle + vertical.x, 30, 100);

        if (rotationLimits is <= 30 or >= 100)
            return;

        _capsule.GetTransform.Rotate(vertical);
    }

    private void TrajectoryLine()
    {
        List<Vector3> points = new List<Vector3> { _aim.position };

        Vector3 currentPosition = _aim.position;
        Vector3 currentVelocity = Velocity;

        Ray ray = new Ray(currentPosition, currentVelocity.normalized);
        RaycastHit hit;
        
        while (!Physics.Raycast(ray, out hit, 1f) &&
               Vector3.Distance(_aim.position, currentPosition) < _maxCurveLength)
        {
            currentVelocity += Physics.gravity * Time.fixedDeltaTime;
            currentPosition += currentVelocity * Time.fixedDeltaTime;

            points.Add(currentPosition);

            ray = new Ray(currentPosition, currentVelocity.normalized);
        }

        if (hit.transform)
        {
            points.Add(hit.point);
        }
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
    }

    private void PowerRegulation()
    {
        float scroll = Input.GetAxis(MouseScrollWheelName);
        float forceValue = _force + scroll * _scrollSpeed;
        float forceLimits = Mathf.Clamp(forceValue, 0, _maxForce);

        if (forceLimits < 0 || forceLimits > _maxForce)
            return;

        _force = forceLimits;
        _powerSlider.SetValue(_force);
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _capsule.StartShake();

            Quaternion rotation = _aim.rotation;

            rotation.ToAngleAxis(out var angle, out _);

            CreateProjectile(rotation, angle);
        }
    }

    private void CreateProjectile(Quaternion rotation, float angle)
    {
        Projectile projectile = Instantiate(_projectilePrefab, _aim.position, rotation);

        projectile.SetVelocityAndAngle(Velocity, -90 + angle);
    }
}