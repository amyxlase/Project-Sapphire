using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkBot : NetworkBehaviour
{
    private CharacterController m_BotController;
    private Vector3 direction;
    public EnemyState state;
    public Transform shootOrigin;
    public float detectionRange = 30f;
    private bool isPatrolRoutineRunning;
    public float patrolDuration = 3f;
    public float idleDuration = 1f;
    public float yVelocity = 0;
    public float patrolSpeed = 2f;
    public float shootRange = 15f;
    public float chaseSpeed = 8f;
    public FPSNetworkPlayer target;
    public float gravity = -9.81f;
    [SerializeField] public GameObject RifleDestination;
    [SerializeField] public GameObject PistolDestination;

    void Start() {
        state = EnemyState.patrol;
        m_BotController = GetComponent<CharacterController>();
        direction = Vector3.zero;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        patrolSpeed *= Time.fixedDeltaTime;
        chaseSpeed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate() {
        
        ApplyGravity();
        AI();

        //Temporary for beta
        //BadAI();
    }

    private void ApplyGravity() {
        float multiplier = 55;
        m_BotController.Move(Physics.gravity * multiplier * Time.deltaTime * Time.deltaTime);
    }

    //Walk in a random direction & change that direction every 100 frames
    private void BadAI() {

        //Randomly generate direction every 100 frames
        if (Time.frameCount % 100 == 0) {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            direction = new Vector3(x, 0f, z);
        }

        //Move with speed multiplier
        float multiplier = 1f;
        m_BotController.Move(multiplier * direction * Time.deltaTime);
    }

    //AI code should go here, these steps are a rough guideline but totally optional
    private void AI() {
        //1. Collect information on where other players are, health, etc
        
        //2. Logic to decide action (walk in direction of players, avoid obstacles, shoot, etc)
        switch(state)
        {
            case EnemyState.idle:
                LookForPlayer();
                break;
            case EnemyState.patrol:
                if(!LookForPlayer())
                {
                    Patrol();
                }
                break;
            case EnemyState.chase:
                Chase();
                break;
            case EnemyState.attack:
                Attack();
                break;
            default:
                break;
        }
        //3. Execute chosen action
    }

    public bool LookForPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
        foreach(GameObject player in players) {
            Vector3 enemyToPlayer = player.transform.position - transform.position;
            if(enemyToPlayer.magnitude <= detectionRange) {
                if(Physics.Raycast(shootOrigin.position, enemyToPlayer, out RaycastHit hit, detectionRange)) {
                    if (hit.collider.CompareTag("Player")) {
                        target = hit.collider.GetComponent<FPSNetworkPlayer>();
                        if (isPatrolRoutineRunning) {
                            isPatrolRoutineRunning = false;
                            StopCoroutine(StartPatrol());
                        }
                        state = EnemyState.chase;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Patrol() {
        if(!isPatrolRoutineRunning) {
            StartCoroutine(StartPatrol());
        }
        Move(transform.forward, patrolSpeed);
    }

    private IEnumerator StartPatrol() {
        isPatrolRoutineRunning = true;
        Vector2 randomPatrolDirection = Random.insideUnitCircle.normalized;
        transform.forward = new Vector3(randomPatrolDirection.x, 0f, randomPatrolDirection.y);
       
        yield return new WaitForSeconds(patrolDuration);
        state = EnemyState.idle;
        
        yield return new WaitForSeconds(patrolDuration);
        state = EnemyState.patrol;
        isPatrolRoutineRunning = false;
    }
 
    private void Move(Vector3 direct, float speed) {
        direct.y = 0f;
        transform.forward = direct;
        Vector3 movement = transform.forward * speed;
        if(m_BotController.isGrounded) {
            yVelocity = 0f;
        }
        yVelocity += gravity;
        movement.y = yVelocity;
        m_BotController.Move(movement);
    }

    private void Chase() {
        if(CanSeeTarget()) {
            Vector3 enemyToPlayer = target.transform.position - transform.position;

            if(enemyToPlayer.magnitude <= shootRange) {
                state = EnemyState.attack;
            }
            else {
                Move(enemyToPlayer, chaseSpeed);
            }
        }
        else {
            target = null;
            state = EnemyState.patrol;
        }
    }

    private void Attack() {
        if(CanSeeTarget()) {
            Vector3 enemyToPlayer = target.transform.position - transform.position;
            transform.forward = new Vector3(enemyToPlayer.x, 0f, enemyToPlayer.z);

            if(enemyToPlayer.magnitude <= shootRange && Time.frameCount % 5 == 0) {
                //Shoot();
            }
            else {
                Move(enemyToPlayer, chaseSpeed);
            }
        }
        else {
            target = null;
            state = EnemyState.patrol;
        }
    }

    public void Shoot() {
        if(Physics.Raycast(shootOrigin.position, target.transform.position - transform.position, out RaycastHit hit , detectionRange)) {
            if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)") {
                uint botID = this.gameObject.GetComponent<NetworkIdentity>().netId;
                uint playerID = hit.transform.gameObject.GetComponent<NetworkIdentity>().netId;
                DealDamage(hit.transform);
            }
        }
    }

    public void DealDamage(Transform target) {
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
        Health playerHealth = target.gameObject.GetComponent<Health>();
        Shield playerShield = target.gameObject.GetComponent<Shield>();
        playerDamage.dealDamage(5);
    }

    private bool CanSeeTarget() {
        if(target == null) {
            return false;
        }
        if(Physics.Raycast(shootOrigin.position, target.transform.position - transform.position, out RaycastHit hit , detectionRange)) {
            if(hit.collider.CompareTag("Player")) {
                return true;
            }
        }
        return false;
    }

    public enum EnemyState {
        idle,
        patrol,
        chase,
        attack
    }

}
