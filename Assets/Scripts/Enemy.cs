using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent m_NavMeshAgent;
    enum TState
    {
        IDLE,
        PATROL,
        CHASE,
        DIE,
        HIT,
        ALERT,
        ATTACK
    }
    TState m_State;
    public float m_MinDistanceToAttack;
    public List<Transform> m_PatrolPoistions;
    int m_CurrentPatrolPosition = 0;
    public float m_MaxDistanceToHear;
    public float m_MaxDistanceToSee;
    public float m_MaxDistanceToChase;
    public LayerMask m_SeesPlayerLayerMask;
    public float m_VisionConeAngle;
    //float m_Rotation = 360;
    float m_VelRoatacion = 100;
    public int m_Life;
    public int m_MaxLife =100;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;


    [Header("LifeBar")]
    public Transform m_LifeBarAnchor;
    public RectTransform m_LifeBarBackGroundRectTransform;
    public Image m_ImageLifeBar;

    void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }
    void Start()
    {
        m_Life = m_MaxLife;
        GameController.GetGameController().AddEnemy(this);
        SetIdleState();
        ShowMaxLife();
        
    }
    private void OnDestroy()
    {
        GameController.GetGameController().RemoveEnemy(this);

    }

    // Update is called once per frame
    void Update()
    {
        switch(m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                    break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;


        }
        //print(SeesPlayer());
        //print(HearPlayer());
        UpdateLifeBarPosition();
    }
    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
    }
    void SetDieState()
    {
        m_State = TState.DIE;
        gameObject.SetActive(false);
    }
    void SetHitState()
    {
        m_State = TState.HIT;
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }

    void UpdateIdleState()
    {
        print("Idle/Patrol");
        CheckPatrol();
    }
    void UpdatePatrolState()
    {
        print("patrol");
        if (HearPlayer())
            SetAlertState();
        else
            SetIdleState();
    }
    void UpdateChaseState()
    {
        SetNextChasePosition();
        print("Chase");
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        if (l_Distance < m_MinDistanceToAttack)
        {
            SetAttackState();
        }

    }
    void UpdateDieState()
    {
        SetDieState();
    }
    void UpdateHitState()
    {
        SetHitState();
    }
    void UpdateAlertState()
    {
        print("Alert");
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        if (SeesPlayer())
        {
            if (l_Distance > m_MaxDistanceToChase)
            {
                SetChaseState();
            }
            else if (l_Distance <= m_MaxDistanceToChase)
            {
                SetAttackState();
            }
        }
        else
            RotationAlert();
    }
    void UpdateAttackState()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        print("Atack");
        if(l_Distance <= m_MinDistanceToAttack)
        {
            ShootEnemy();
        }
        else
        {
            SetPatrolState();
        }
    }
    void RotationAlert()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        transform.Rotate(Vector3.up, m_VelRoatacion * Time.deltaTime);
        if (SeesPlayer())
        {
            SetAlertState();
        }
        else
        {
            SetIdleState();
        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(l_PlayerPosition), Time.deltaTime * m_VelRoatación)

    }
    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;

        Vector3 l_Direction = (l_EnemyPosition -l_PlayerPosition);
        l_Direction.Normalize();
        Vector3 l_DesiredPosition = l_PlayerPosition +l_Direction* m_MinDistanceToAttack;
        m_NavMeshAgent.SetDestination(l_DesiredPosition);

    }
    void CheckPatrol()
    {
        if (!m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending)
        {
            MoveNextPatrol();
        }
    }
    void MoveNextPatrol()
    {
        m_CurrentPatrolPosition++;
        if(m_CurrentPatrolPosition>=m_PatrolPoistions.Count)
        {
            m_CurrentPatrolPosition = 0;
        }
        MoveToNextPatrolPosition();
    }
    void MoveToNextPatrolPosition()
    {
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.Stop(HearPlayer());
        m_NavMeshAgent.SetDestination(m_PatrolPoistions[m_CurrentPatrolPosition].position);
    }
    bool HearPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition,l_EnemyPosition);
        return l_Distance < m_MaxDistanceToHear;
    }
    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition,l_EnemyPosition);
        if(l_Distance<m_MaxDistanceToSee)
        {
            Vector3 l_EnemyForward = transform.forward;
            l_EnemyForward.y = 0.0f;
            l_EnemyForward.Normalize();
            Vector3 l_VectorEnemyToPlayer = l_PlayerPosition -l_EnemyPosition;
            l_VectorEnemyToPlayer.y = 0.0f;
            l_VectorEnemyToPlayer.Normalize();
            float l_DotAngle = Vector3.Dot(l_EnemyForward, l_VectorEnemyToPlayer);
            if(l_DotAngle > Mathf.Cos(Mathf.Deg2Rad*m_VisionConeAngle/2.0f))
            {
                Ray l_Ray = new Ray(l_EnemyPosition+Vector3.up*1.8f, l_VectorEnemyToPlayer);
                if(!Physics.Raycast(l_Ray, l_Distance, m_SeesPlayerLayerMask.value))
                return true;
            }
        }
        return false;
    }
    void ShootEnemy()
    {
        Vector3 l_Origin = transform.position;
        Vector3 l_Forward = transform.forward;
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Origin, l_Forward, out l_RaycastHit, m_MinDistanceToAttack))
        {
            if (l_RaycastHit.transform.tag == "Player")
            {

            }
        }
    }

    public void Hit(int LifePoints)
    {
        m_Life-=LifePoints;
        if (m_Life <= 0)
        {
            SetDieState();
        }
    }

    public void RestartLevel()
    {
        gameObject.SetActive(true);
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_NavMeshAgent.enabled = true;
        m_Life = m_MaxLife;
        SetIdleState();
        ShowMaxLife();
    }

    void ShowMaxLife()
    {
        m_ImageLifeBar.fillAmount = m_Life / (float)m_MaxLife;
    }
    void UpdateLifeBarPosition()
    {
        Vector3 l_ViewportPosition = GameController.GetGameController().m_Player.m_Camera.WorldToViewportPoint(m_LifeBarAnchor.position);
        m_LifeBarBackGroundRectTransform.anchoredPosition = new Vector3(l_ViewportPosition.x * Screen.width, -(Screen.height- l_ViewportPosition.y*Screen.height));
        m_LifeBarBackGroundRectTransform.gameObject.SetActive(l_ViewportPosition.z >= 0.0f);
    }
}
