using UnityEngine;
using UnityEngine.InputSystem;

public class CatalystPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _catalystPrefab;
    [SerializeField] private float _fireForce = 5.0f;
    [SerializeField] private LineRenderer _fireTraj;
    [SerializeField] private int _fireLineSegments = 30;
    [SerializeField] private float _fireLinePredictedTimestep = 0.016f;

    private Camera _camera;


    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.m_instance.m_gameState != GameManager.LevelState.InPlay) return;

        Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 catalystLauncherPosition = (Vector2)transform.position;
        Vector2 launchDirection = (mouseWorldPosition - catalystLauncherPosition).normalized;

        DrawLaunchTrajectory(launchDirection);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryLaunch(launchDirection);

        if (Mouse.current.rightButton.wasPressedThisFrame)
            transform.position = mouseWorldPosition;
    }


    private void DrawLaunchTrajectory(Vector2 launchDirection)
    {
        _fireTraj.enabled = true;
        _fireTraj.positionCount = _fireLineSegments;

        Vector2 start = transform.position;
        Vector2 initVelocity = launchDirection * _fireForce;

        for (int i = 0; i < _fireLineSegments; ++i)
        {
            float traj = _fireLinePredictedTimestep * i;
            Vector2 trajPoint = start + initVelocity * traj; // + 0.5f * gravity * traj * traj - if gravity enabled later, dont think it will be but leaving here incase of

            _fireTraj.SetPosition(i, trajPoint);
        }

    }

    private void TryLaunch(Vector2 launchDirection)
    {
        if (!LevelManager.m_instance.TryShoot()) return;

        GameObject newCatalyst = Instantiate(_catalystPrefab, transform.position, Quaternion.identity);
        newCatalyst.GetComponent<Rigidbody2D>().AddForce(launchDirection * _fireForce, ForceMode2D.Impulse);
        _fireTraj.enabled = false;


    }

}
