using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

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
    TState m_LastState;
    public float m_MinDistanceToAttack;
    public List<Transform> m_PatrolPoistions;
    int m_CurrentPatrolPosition = 0;
    public float m_MaxDistanceToHear;
    public float m_MaxDistanceToSee;
    public float m_MaxDistanceToChase;
    public LayerMask m_SeesPlayerLayerMask;
    public float m_VisionConeAngle;
    //float m_Rotation = 360;
    float m_VelRoatacion = 150;
    public int m_Life;
    public int m_MaxLife =100;
    Vector3 m_StartPosition;
    Vector3 m_EnemyForward;
    Quaternion m_StartRotation;
    int m_damageEnemy = 10;
    public float m_EnemyCadence;
    float timer = 0.0f;


    public List<GameObject> m_DroppingItems;


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
        

       
    }
    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_NavMeshAgent.isStopped = false;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_NavMeshAgent.isStopped = false;
    }
    void SetDieState()
    {
        m_State = TState.DIE;
        m_NavMeshAgent.isStopped = true;
    }
    void SetHitState()
    {
        m_LastState = m_State;
        m_State = TState.HIT;
    }
    void SetAlertState()
    {
        m_EnemyForward = transform.forward;
        m_State = TState.ALERT;
        m_NavMeshAgent.isStopped = true;
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_NavMeshAgent.isStopped = true;
    }

    void UpdateIdleState()
    {
        //print("Idle/Patrol");
        SetPatrolState();
    }
    void UpdatePatrolState()
    {
        //print("patrol");
        if(HearPlayer())
            SetAlertState();
        CheckPatrol();
    
        
    }
    void UpdateChaseState()
    {
        print("Chase");
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        transform.LookAt(l_PlayerPosition);
        SetNextChasePosition();
        if (l_Distance < m_MinDistanceToAttack)
        {
            SetAttackState();
        }
        if (l_Distance >= m_MaxDistanceToChase)
        {
            SetPatrolState();
        }
        
        

    }
    void UpdateDieState()
    {
        Instantiate(m_DroppingItems[Random.Range(0,m_DroppingItems.Count)], transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
    void UpdateHitState()
    {
       switch(m_LastState)
       {
            case TState.IDLE:
                SetAlertState();
                break;
            case TState.PATROL:
                SetAlertState();
                break;    
            case TState.ALERT:
                SetAlertState();
                break; 
            case TState.CHASE:
                SetChaseState();
                break;       
            case TState.ATTACK:
                SetAttackState();
                break;
            case TState.DIE:
                SetDieState();
                break;            
       }   
    }
    void UpdateAlertState()
    {
        print("Alert");
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        if (SeesPlayer() && l_Distance > m_MaxDistanceToChase)
        { 
            SetChaseState();   
        }
        else if (SeesPlayer() && l_Distance <= m_MaxDistanceToChase)
        {
            SetAttackState();
        }
        else
            RotationAlert();
            print("Rota");
    }
    void UpdateAttackState()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        transform.LookAt(l_PlayerPosition);
        //print("Atack");
        timer += Time.deltaTime;
        if(l_Distance < m_MinDistanceToAttack && SeesPlayer())
        {
            if (timer >= m_EnemyCadence)
            {
                ShootPlayer();
                timer = 0;
            }    
        }
        else
        {
            SetChaseState();
        }
    }
    void RotationAlert()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        Vector3 l_EnemyForward = transform.forward;
        float l_Angle = Vector3.Angle(m_EnemyForward, l_EnemyForward);
        print(l_Angle);
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        transform.Rotate(Vector3.up, m_VelRoatacion * Time.deltaTime);
        if (SeesPlayer() && l_Distance <= m_MinDistanceToAttack)
        {
            SetAttackState();

        }
        else if(SeesPlayer() && l_Distance <= m_MaxDistanceToChase)
        {
            SetChaseState();
        }
        else if(l_Angle >= 90 || l_Angle == 140)
        {
            SetPatrolState();
            
        }
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(l_PlayerPosition), Time.deltaTime * m_VelRoataci�n)

    }
    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;

        Vector3 l_Direction = (l_EnemyPosition -l_PlayerPosition);
        l_Direction.Normalize();
        Vector3 l_DesiredPosition = l_PlayerPosition +l_Direction;
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
        if(HearPlayer())
        {
            m_NavMeshAgent.isStopped = true;
            SetAlertState();
        }

        m_NavMeshAgent.SetDestination(m_PatrolPoistions[m_CurrentPatrolPosition].position);
    }
    bool HearPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition,l_EnemyPosition);
        return l_Distance <= m_MaxDistanceToHear;
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
    void ShootPlayer()
    {
        Vector3 l_Origin = transform.position;
        Vector3 l_Forward = transform.forward;
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Origin, l_Forward, out l_RaycastHit, m_MinDistanceToAttack))
        {
            PlayerController l_PlayerController = GameController.GetGameController().m_Player.GetComponent<PlayerController>();
            l_PlayerController.TakeDamage(m_damageEnemy);
            
        }
    }

    public void Hit(int LifePoints)
    {
        m_Life-=LifePoints;
        if (m_Life <= 0)
        {
            SetDieState();
        }
        else
        {
            SetHitState();
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
    }

    
}
