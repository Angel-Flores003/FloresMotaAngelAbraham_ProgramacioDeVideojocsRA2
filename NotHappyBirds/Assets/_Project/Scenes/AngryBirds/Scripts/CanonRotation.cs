using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ITurretActions
{
    private Vector2 mousePosition;
	public GameObject Bullet;
	public GameObject ShootPoint;
	public GameObject PotencyBar;
	private InputSystem_Actions inputActions;
	public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;

		inputActions = new InputSystem_Actions();
        inputActions.Turret.SetCallbacks(this);
	}

    void Update()
    {
        Vector2 mousePos=mousePosition; //obtenir el valor del click del cursor (Fer amb new input system)
		_distanceBetweenMouseAndPlayer = Vector2.zero; //obtenir el vector distància entre el canó i el cursor
        Vector3 mousePostition = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 dist = mousePostition - transform.position;
		//Vector2 dist = mousePos - _distanceBetweenMouseAndPlayer;
		var ang = (Mathf.Atan2(dist.y, dist.x) * 180f / Mathf.PI + _offset);
		transform.rotation = Quaternion.Euler(0, ang, 0); //en quin dels tres eixos va l'angle? eje y
		if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + Mathf.Clamp(ProjectileSpeed, MinSpeed, MaxSpeed); //ProjectileSpeed; //acotar entre dos valors (mirar variables)
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }

    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
			PotencyBar.transform.localScale.y,
            1f/*transform.localScale.z*/);
    }

	public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
        }
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, transform.position, ShootPoint.transform.rotation);//, Quaternion.identity); //canviar la posició on s'instancia //ShootPoint
			projectile.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            ProjectileSpeed = 0f;
            isRaising = false;
        }
    }

	public void OnAim(InputAction.CallbackContext context)
	{
		mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
	}

	public void OnEnable()
    {
		inputActions.Enable();
	}

    public void OnDisable()
    {
        inputActions.Disable();
    }
}