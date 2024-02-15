using System.Collections;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    private Transform _transform;
    private Camera _camera;

    private void Awake() => 
        _transform = GetComponent<Transform>();

    private void Start() => 
        _camera = Camera.main;

    public Transform GetTransform =>
        _transform;

    public void StartShake() => 
        StartCoroutine(Shake());

    private IEnumerator Shake()
    {
        Vector3 shake = new Vector3(0, 0.1f, 0);

        for (int i = 1; i <= 6; i++)
        {
            _transform.Translate(i <= 3 ? -shake : shake);
            _camera.fieldOfView += i <= 3 ? -0.6f : 0.6f;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
