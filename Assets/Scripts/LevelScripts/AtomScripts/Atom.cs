using System;
using System.Collections;
using UnityEngine;

public class Atom : MonoBehaviour
{
    //event that is called when a fusion happens successfully, notifies levelmanager to update how many reactions left
    public static event Action m_atomFused;

    public bool m_reacting = false;

    public AtomState m_atomState { get; private set; }

    [SerializeField] private SpriteRenderer _spRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D _glow;
    [SerializeField] private Color _defaultColour = Color.white;
    [SerializeField] private Color _hitColour = Color.purple;
    [SerializeField] private Color _fusedColour = Color.red;
    [SerializeField] private float _forceRequiredToFuse = 2.0f;
    [SerializeField] private float _explosionRadius = 2.0f;
    [SerializeField] private float _atomExplosionScale = 4.0f;
    [SerializeField] private GameObject _fusionParticleEffect;
    [SerializeField] private GameObject _explosionParticleEffect;
    [SerializeField] private float _defaultGlowIntensity = 1.0f;
    [SerializeField] private float _hitGlowIntensity = 2.0f;
    [SerializeField] private float _FuseGlowIntensity = 3.0f;
    [SerializeField] private float _restitution = 0.8f;



    void Start()
    {
        m_atomState = AtomState.Default;
        //self register to the game manager
        _rb = GetComponent<Rigidbody2D>();

        LevelManager.m_instance.atomRegister();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 collisionNormal = ((Vector2)transform.position - collision.ClosestPoint(transform.position)).normalized;

            _rb.linearVelocity = Vector2.Reflect(_rb.linearVelocity, collisionNormal) * _restitution;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.TryGetComponent<Catalyst>(out _)) //out _ as we dont need catalyst information 
        {
            if (m_atomState != AtomState.Default) return;
            UpdateAtomState(AtomState.Hit);
            return;
        }

        if (!collision.gameObject.TryGetComponent<Atom>(out Atom other)) return;


        //float velocityImpact = collision.relativeVelocity.magnitude;
        //if (velocityImpact <= _forceRequiredToFuse)
        //    return;

        AtomCollision(other);


    }

    /// <summary>
    /// Updates the atoms current play state.
    /// </summary>
    /// <param name="state">Atom state the owned state is being set to.</param>
    private void UpdateAtomState(AtomState state)
    {
        m_atomState = state;
        UpdateAtomVisuals();
    }

    private void UpdateAtomVisuals()
    {
        switch (m_atomState)
        {
            case AtomState.Default:
                _spRenderer.color = _defaultColour;
                _glow.color = _defaultColour;
                _glow.intensity = _defaultGlowIntensity;
                break;

            case AtomState.Hit:
                _spRenderer.color = _hitColour;
                _glow.color = _hitColour;
                _glow.intensity = _hitGlowIntensity;
                transform.localScale *= 1.2f;
                break;


            case AtomState.Fused:
                _spRenderer.color = _fusedColour;
                _glow.color = _fusedColour;
                _glow.intensity = _FuseGlowIntensity;
                transform.localScale *= 1.4f;
                break;
        }
    }



    /// <summary>
    /// Handles atom state updating based on atom to atom collisions, calls exploding and fusing
    /// </summary>
    /// <param name="other">Atom being collided with.</param>
    private void AtomCollision(Atom other)
    {
        if (m_reacting) return;

        if (m_atomState == AtomState.Hit)
        {
            m_reacting = true;
            other.DestroyForFusion();
            Fuse();
            return;
        }

        if (other.m_atomState == AtomState.Hit)
        {
            other.m_reacting = true;
            DestroyForFusion();
            other.Fuse();
            return;
        }

        if (m_atomState == AtomState.Fused)
        {
            m_reacting = true;
            Explode();
            return;
        }

        if (other.m_atomState == AtomState.Fused)
        {
            m_reacting = true;
            other.Explode();
            return;
        }

    }

    /// <summary>
    /// Called by one of 2 atoms that are currently colliding to have its colour changed, state updated, and size increased.
    /// </summary>
    private void Fuse()
    {
        UpdateAtomState(AtomState.Fused);
        ResetReaction();
    }

    private IEnumerator ResetReaction()
    {
        yield return new WaitForFixedUpdate();
        m_reacting = false;
    }

    /// <summary>
    /// Called by one of 2 atoms that are currently colliding to destroy one of the atoms during a fusion collision.
    /// </summary>
    public void DestroyForFusion()
    {
        Instantiate(_fusionParticleEffect, transform.position, Quaternion.identity);
        m_atomFused?.Invoke();
        Destroy(gameObject);
    }


    public void Explode()
    {
        Instantiate(_explosionParticleEffect, transform.position, Quaternion.identity);

        Collider2D[] closeAtoms = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        for (int i = 0; i < closeAtoms.Length; ++i)
        {
            Collider2D collider = closeAtoms[i];
            if (collider.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                //dir from explosion to coll
                Vector2 forceDirection = (collider.transform.position - transform.position).normalized;

                rb.AddForce(forceDirection * _atomExplosionScale, ForceMode2D.Impulse);

                if (collider.gameObject.TryGetComponent<Atom>(out Atom other))
                    other.UpdateAtomState(AtomState.Hit);

            }
        }

        m_atomFused?.Invoke();
        Destroy(gameObject);
    }

}
