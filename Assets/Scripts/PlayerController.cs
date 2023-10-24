using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    public Transform m_pitchController;
    public float m_YawSpeed;
    public float m_pitchSpeed;
    public bool m_YawSpeedInverted;
    public bool m_PitchSpeedInverted;

    public float m_MaxPitch;
    public float m_MInPitch;

    CharacterController m_CharacterController;

    private Puntuation m_Puntuation;

    [Header("Shoot")]
    public float m_MaxShootDist;
    public LayerMask m_layerMask;
    public GameObject m_HitPatriclePrefab;
    public static Action OnRestart;
    //CPoolElements m_PoolElements;

    [Header("Particles")]
    bool m_IsEnemy;
    public GameObject m_EnemyParticle;
    public GameObject m_ShootParticle;
    public GameObject m_ImpactParticle;
    private ParticleSystem m_Shoot;
    private ParticleSystem m_Impact;
    private ParticleSystem m_EnemyImpact;


    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_BackKeyCode = KeyCode.S;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_ForwardKeyCode = KeyCode.W;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public int m_lefMouseButton;

    [Header("Debug Input")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    [Header("Animation")]
    public Animation m_WeaponAnimation;
    public AnimationClip m_IdleAnimation;
    public AnimationClip m_ShootAnimation;
    public AnimationClip m_ReloadAnimation;

    public int m_ShootMousenButton = 0;

    public float m_jumpSpeed;
    public float m_sprintSpeed;

    public float m_Speed;

    float m_VerticalSpeed;

    public Camera m_Camera;
    Vector3 m_StratPosition;
    Quaternion m_StartRotation;

    float m_LastTimeOnFloor;
    float m_FloorTime;

    float m_AmmoRemaining;
    public float m_TotalAmmo;
    float m_MAX_TotalAmmo;
    public float m_CHARGERCAPACITY;

    public float m_Cadence;
    float timer;

    [Header("Life and Shield")]
    float m_ActualLife;
    public float m_MAX_Life;
    float m_Shield;
    public float m_MAX_Shield;

    [Header("UI Player")]
    public Text m_AmmoDisplay;
    public Text m_TotalAmmoDisplay;
    public Text m_LifeDisplay;
    public Text m_ShieldDisplay;

    private void Awake()
    {
        

        m_ActualLife = m_MAX_Life;
        m_Shield = m_MAX_Shield;
        m_AmmoRemaining = m_CHARGERCAPACITY;
        m_MAX_TotalAmmo = m_TotalAmmo;

        m_CharacterController = GetComponent<CharacterController>();
        if(GameController.GetGameController().m_Player == null)
        {
            GameController.GetGameController().m_Player = this;
            GameController.DontDestroyOnLoad(gameObject);
            m_StratPosition = transform.position;
            m_StartRotation = transform.rotation;
            m_Yaw=transform.rotation.eulerAngles.y;
        }
        else
        {
            GameController.GetGameController().m_Player.SetStartPosition(transform);
            GameObject.Destroy(this.gameObject);
        }

        timer = m_Cadence;
    }
    // Start is called before the first frame update
    void Start()
    {
        //#if UNITY_EDITOR

        //#else

        //#endif
        //m_PoolElements = new CPoolElements(20, m_HitPatriclePrefab, null);
        m_Puntuation = GetComponent<Puntuation>();
        Cursor.lockState = CursorLockMode.Locked;
        SetIdWeaponAnimation();
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
#endif
        m_AmmoDisplay.text = m_AmmoRemaining.ToString();
        m_TotalAmmoDisplay.text = m_TotalAmmo.ToString();
        m_LifeDisplay.text = m_ActualLife.ToString();
        m_ShieldDisplay.text = m_Shield.ToString();

        timer = timer + Time.deltaTime;

        float l_HorizontalMovement = Input.GetAxis("Mouse X"); //movimiento x
        float l_VerticalMovement = Input.GetAxis("Mouse Y"); //movimiento y
        float l_Speed = m_Speed; // le da a la variable local el valor de la variable global

        if (m_AngleLocked)
        {
            l_HorizontalMovement = 0.0f;
            l_VerticalMovement = 0.0f;
        }

        if (Input.GetKeyDown(m_JumpKeyCode) && m_VerticalSpeed == 0.0f)
            m_VerticalSpeed = m_jumpSpeed;

        if (Input.GetKey(m_SprintKeyCode))
            l_Speed = m_sprintSpeed;

        float l_YawInverted = 1.0f;
        float l_PitchInverted = 1.0f;
        if (m_YawSpeedInverted)
            l_YawInverted = -1.0f;
        if (m_PitchSpeedInverted)
            l_PitchInverted = -1.0f;
        //float l_YawInverted = m_YawSpeedInverted ? -1.0f : 1.0f;
        //float l_PitchInverted = m_PitchSpeedInverted ? -1.0f : 1.0f;

        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90) * Mathf.Deg2Rad;

        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));

        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_LeftKeyCode))
            l_Movement = -l_Right;
        else if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_Right;

        if (Input.GetKey(m_ForwardKeyCode))
            l_Movement += l_Forward;
        else if (Input.GetKey(m_BackKeyCode))
            l_Movement -= l_Forward;

        l_Movement.Normalize();
        l_Movement *= l_Speed * Time.deltaTime;


        m_Yaw = m_Yaw + m_YawSpeed * l_HorizontalMovement * Time.deltaTime * l_YawInverted;
        m_Pitch = m_Pitch + m_pitchSpeed * l_VerticalMovement * Time.deltaTime * l_PitchInverted;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MInPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_pitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
            m_VerticalSpeed = 0.0f;
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;

        if (Input.GetMouseButtonDown(m_lefMouseButton) && CanShoot())
            Shoot();
        if (Input.GetKeyDown(m_ReloadKeyCode) && CanReload())
            Reload();

        m_LastTimeOnFloor += Time.deltaTime;
        if (CanJump())
            m_LastTimeOnFloor = 0.0f;
    }
    void Shoot()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDist, m_layerMask.value) && m_AmmoRemaining > 0)
        {
            FindObjectOfType<AudioManager>().Play("ShootWeapon");
            m_Shoot.Play();
            if (l_RaycastHit.transform.tag == "PracticeEnemy")
            {
                SetShootWeaponAnimation();
                CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
                BulletShooted();
                FindObjectOfType<AudioManager>().Play("ImpactBullet");
                EnemyTarget l_EnemyTarget = l_RaycastHit.transform.gameObject.GetComponent<EnemyTarget>();
                l_EnemyTarget.DefusePractice();
                m_Puntuation.PlusPoints();
                m_Impact.Play();
            }
            else if(l_RaycastHit.transform.tag == "Box")
            {
                SetShootWeaponAnimation();
                CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
                BulletShooted();
                FindObjectOfType<AudioManager>().Play("ImpactBullet");
                OnRestart?.Invoke();
                m_Puntuation.RestartPoints();
                m_Impact.Play();
            }
            else if(l_RaycastHit.transform.tag == "Enemy")
            {
                m_IsEnemy = true;
                SetShootWeaponAnimation();
                CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
                l_RaycastHit.collider.GetComponent<HitCollider>().Hit();
                BulletShooted();
                FindObjectOfType<AudioManager>().Play("ImpactBullet");
                m_EnemyImpact.Play();
            }
            else
            {
                SetShootWeaponAnimation();
                CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
                BulletShooted();
            }

            
        }
        else
        {
            if (m_AmmoRemaining > 0)
            {
                m_AmmoRemaining--;
                FindObjectOfType<AudioManager>().Play("ShootWeapon");
                SetShootWeaponAnimation();

            }
        }
    }
    void CreateShootParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject l_HitParticle = GameObject.Instantiate(m_HitPatriclePrefab, GameController.GetGameController().m_DestroyObjects.transform);
        if (m_IsEnemy)
        {
            GameObject l_ImpactParticle = GameObject.Instantiate(m_EnemyParticle);
            l_ImpactParticle.transform.position = Position;
            l_ImpactParticle.transform.rotation = Quaternion.LookRotation(Normal);
        }
        else
        {
            GameObject l_ImpactParticle = GameObject.Instantiate(m_ImpactParticle);
            l_ImpactParticle.transform.position = Position;
            l_ImpactParticle.transform.rotation = Quaternion.LookRotation(Normal);
        }
        //GameObject l_HitParticles = m_PoolElements.GetNextElement();
        l_HitParticle.transform.position = Position;
        l_HitParticle.transform.rotation = Quaternion.LookRotation(Normal);
        m_IsEnemy = false;
    }
    private bool CanJump()
    {
        return m_LastTimeOnFloor < m_FloorTime;
    }
    void Reload()
    {
        SetReloadWeaponAnimation();
        FindObjectOfType<AudioManager>().Play("ReloadWeapon");
        float l_nextCharger = m_CHARGERCAPACITY - m_AmmoRemaining;
        if (l_nextCharger > m_TotalAmmo)
        {
            m_AmmoRemaining += m_TotalAmmo;
            m_TotalAmmo = 0;
        }
        else
        {
            m_TotalAmmo = m_TotalAmmo - l_nextCharger;
            m_AmmoRemaining = m_CHARGERCAPACITY;
        }
    }
    bool CanReload()
    {
        if (m_TotalAmmo != 0)
            return true;
        else
            return false;

    }
    bool CanShoot()
    {
        if (timer > m_Cadence)
        {
            timer = 0;
            return true;
        }
        else return false;

       
    }
    void SetIdWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_IdleAnimation.name);
    }
    void SetShootWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ShootAnimation.name, 0.1f);
        m_WeaponAnimation.CrossFadeQueued(m_IdleAnimation.name, 0.1f);

    }
    void SetReloadWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ReloadAnimation.name, 0.1f);
        m_WeaponAnimation.CrossFadeQueued(m_IdleAnimation.name, 0.1f);
    }
    public void RestartLevel()
    {
        m_CharacterController.enabled=false;
        transform.position = m_StratPosition;
        transform.rotation = m_StartRotation;
        m_Yaw=transform.rotation.eulerAngles.y;
        m_Pitch= 0.0f;
        m_ActualLife = 100f;
        m_CharacterController.enabled=true;
        
    }
    void SetStartPosition(Transform StartTransform)
    {
        m_StratPosition=StartTransform.position;
        m_StartRotation=StartTransform.rotation;
        m_CharacterController.enabled=false;
        transform.position = m_StratPosition;
        transform.rotation = m_StartRotation;
        m_Yaw=transform.rotation.eulerAngles.y;
        m_Pitch= 0.0f;
        m_CharacterController.enabled=true;
    }
    public bool CanPickAmmo()
    {
        if(m_TotalAmmo >= m_MAX_TotalAmmo)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }
    public void AddAmmo(int AmmoCount)
    {
        m_TotalAmmo += AmmoCount;
        m_TotalAmmo = Mathf.Clamp(m_TotalAmmo, 0f, m_MAX_TotalAmmo);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item l_Item=other.GetComponent<Item>();
            if(l_Item.CanPick())
                l_Item.Pick();
        }
        else if(other.tag == "DeadZone")
        {
            Kill();
        }
    }
    public bool CanPickLife()
    {
        if(m_ActualLife >= m_MAX_Life)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void AddLife(int LifeCount)
    {
        m_ActualLife += LifeCount;
        m_ActualLife = Mathf.Clamp(m_ActualLife, 0f, m_MAX_Life);
    }
    public bool CanPickShield()
    {
        if(m_Shield >= m_MAX_Shield)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void AddShield(int ShieldCount)
    {
        m_Shield += ShieldCount;
        m_Shield = Mathf.Clamp(m_Shield, 0f, m_MAX_Shield);
    }



    public void BulletShooted()
    {
        if(m_AmmoRemaining>0)
            m_AmmoRemaining--;

    }
    public void UpdateLife(float l_Life, float l_Shield)
    {
        m_ActualLife = l_Life;
        m_Shield = l_Shield;
    }
    public void TakeDamage(float l_Damage)
    {
        if(m_Shield <= 0)
        {
            m_ActualLife = m_ActualLife - l_Damage;
            if (m_ActualLife <= 0)
                Kill();
        }
        else
        {
            m_Shield = m_Shield - l_Damage*0.75f;
            if(m_Shield < 0)
            {
                m_Shield = 0;
            }
            m_ActualLife = m_ActualLife - l_Damage*0.25f;
            if (m_ActualLife <= 0)
                Kill();
        }

        
    }
    public void Kill()
    {
        m_ActualLife = 0;
        GameController.GetGameController().RestartLevel();
    }
}

