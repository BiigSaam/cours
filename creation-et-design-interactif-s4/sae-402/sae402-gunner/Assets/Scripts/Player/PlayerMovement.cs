using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;

    [Header("Physics")]
    public Rigidbody2D rb;
    private CapsuleCollider2D _capsuleCollider;

    private float _horizontalMovement;

    [Range(100.0f, 1000.0f)]
    [SerializeField]
    private float _moveSpeed;

    [Header("Dash System")]
    [SerializeField]
    // private float _dashSpeed = 5f;
    // private float _dashCount;
    // private float _startDashCount;
    // private float _doubleTapTime;
    // private KeyCode _lastKeyCode;
    // private int _dashSide;
    private bool _canDash = true;
    private bool _isDashing = false;
    public float dashingSpeed = 35f;
    private float _dashingTime = 0.15f;
    private float _dashingCooldown = 0.75f;
    private bool _isPressingDashKey = false;


    private bool _isFacingRight = true;
    [Header("Ground Management")]
    private bool _isGrounded = true;
    [SerializeField]
    private LayerMask listCollisionLayers;
    public Transform _groundCheck;
    [SerializeField]
    private float _groundCheckRadius;

    private PlayerListSkills _playerSkills;

    private TrailRenderer _trailRenderer;

    void Awake()
    {
        // Ici, nous allons récupérer différent composant de notre gameobject.
        // Nous pouvons également mettre les propriétés en "public" et faire un glisser-déposer depuis l'inspecteur
        // Mettre tout en public peut être confus si on a trop de propriétés
        // A noter que parfois nous ne pouvons pas forcément utiliser "GetComponent"
        _capsuleCollider = this.GetComponent<CapsuleCollider2D>();
        _animator = this.GetComponent<Animator>();
        _trailRenderer = this.GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        _playerSkills = PlayerListSkills.GetInstance();

        // _trailRenderer.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _moveSpeed = 8.0f;
        // _dashCount = _startDashCount;
    }

    // La méthode est appelée toutes les frames. Par exemple, si notre jeu tourne à 60 frames par seconde (fps)
    // Alors la méthode sera appelée 60 fois par seconde.
    // C'est notamment dans cette fonction que nous pouvons récupérer les touches appuyées
    void Update()
    {
        if (_isDashing) return;
        CheckInputs();
        ManageAnimator();

        if (
            _canDash &&
            _isPressingDashKey &&
            _playerSkills.IsSkillUnlocked(PlayerListSkills.SkillType.Dash))
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (_isDashing) return;
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, listCollisionLayers);
        Move();
    }

    void CheckInputs()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal") * _moveSpeed;
        if (Input.GetKeyDown(KeyCode.G))
        {
            DebugListSkills();
        }

        _isPressingDashKey = Input.GetKeyDown(KeyCode.X);
    }

    void DebugListSkills()
    {
        foreach (PlayerListSkills.SkillType val in System.Enum.GetValues(typeof(PlayerListSkills.SkillType)))
        {
            Debug.Log(val + " is unlocked :" + _playerSkills.IsSkillUnlocked(val));
        }
    }

    void ManageAnimator()
    {
        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_horizontalMovement * Time.fixedTime));
        _animator.SetFloat("VerticalSpeed", rb.velocity.y);
    }

    void Move()
    {
        rb.velocity = new Vector2(_horizontalMovement, rb.velocity.y);

        if (_horizontalMovement > 0 && !_isFacingRight || _horizontalMovement < 0 && _isFacingRight)
        {
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    IEnumerator Dash()
    {
        _trailRenderer.emit = true;
        _canDash = false;
        _isDashing = true;
        float originalGravity = rb.gravityScale;
        Time.timeScale = 0.65f;

        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.right.normalized.x * dashingSpeed, 0f);

        yield return new WaitForSeconds(_dashingTime);
        rb.gravityScale = originalGravity;
        _isDashing = false;
        Time.timeScale = 1f;
        _trailRenderer.ClearTrail();
        _trailRenderer.emit = false;

        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }

    void OnDrawGizmosSelected()
    {
        if (_groundCheck == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }

    public bool IsDashing()
    {
        return _isDashing;
    }
}
