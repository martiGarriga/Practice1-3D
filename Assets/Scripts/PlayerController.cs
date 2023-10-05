using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Shoot")]
    public float m_MaxShootDist;
    public LayerMask m_layerMask;
    public GameObject m_HitPatriclePrefab;

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

    float m_LastTimeOnFloor;
    float m_FloorTime;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //#if UNITY_EDITOR

        //#else

        //#endif

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
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDist, m_layerMask.value))
        {
            CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
            SetShootWeaponAnimation();
        }
    }
    void CreateShootParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject l_HitParticle = GameObject.Instantiate(m_HitPatriclePrefab);
        l_HitParticle.transform.position = Position;
        l_HitParticle.transform.rotation = Quaternion.LookRotation(Normal);
    }
    private bool CanJump()
    {
        return m_LastTimeOnFloor < m_FloorTime;
    }
    void Reload()
    {
        SetReloadWeaponAnimation();
    }
    bool CanReload()
    {
        return true;
    }
    bool CanShoot()
    {
        return true;
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
        //m_ReloadAnimation;
    }

}

