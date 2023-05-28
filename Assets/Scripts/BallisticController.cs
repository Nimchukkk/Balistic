using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BallisticController : MonoBehaviour
{
    [SerializeField] private PowerSlider _powerSlider;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _capsule;
    [SerializeField] private Transform _aim;
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _scrollSpeed = 10;
    [SerializeField] private int _maxCurveLength = 50;

    private Camera _camera;
    private Transform _transform;
    private LineRenderer _lineRenderer;
    private float _maxForce;
    
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
        _camera = Camera.main;
    }

    private void Update()
    {
        Shoot();
        BallisticRotate();
        TrajectoryLine();
        PowerRegulation();
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Shake());

            var rotation = _aim.rotation;

            rotation.ToAngleAxis(out var angle, out _);

            var projectile = Instantiate(_projectilePrefab, _aim.position, rotation);

            projectile.SetVelocityAndAngle(Velocity, -90 + angle);
        }
    }

    private void BallisticRotate()
    {
        var vertical = Vector3.left * (Input.GetAxis("Vertical") * _speed / 2 * Time.deltaTime);
        var horizontal = Vector3.up * (Input.GetAxis("Horizontal") * _speed * Time.deltaTime);

        _transform.Rotate(horizontal);
        
        _capsule.localRotation.ToAngleAxis(out var angle, out _);

        var rotationLimits = Mathf.Clamp(angle + vertical.x, 30, 100);

        if (rotationLimits is <= 30 or >= 100)
            return;

        _capsule.Rotate(vertical);
    }

    private void TrajectoryLine()
    {
        var aimPosition = _aim.position;

        var points = new List<Vector3> { aimPosition };

        var currentPosition = aimPosition;
        var currentVelocity = Velocity;

        var ray = new Ray(currentPosition, currentVelocity.normalized);
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
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        var forceValue = _force + scroll * _scrollSpeed;
        var forceLimits = Mathf.Clamp(forceValue, 0, _maxForce);

        if (forceLimits < 0 || forceLimits > _maxForce)
            return;

        _force = forceLimits;
        _powerSlider.SetValue(_force);
    }

    private IEnumerator Shake()
    {
        var shake = new Vector3(0, 0.1f, 0);

        for (var i = 1; i <= 6; i++)
        {
            _capsule.Translate(i <= 3 ? -shake : shake);
            _camera.fieldOfView += i <= 3 ? -0.6f : 0.6f;
            yield return new WaitForSeconds(0.03f);
        }
    }
}