using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    public float Range;
    public float EnemyRange;
    public const float g = -9.81f;

    [SerializeField] float StartVelocity;
    [SerializeField] GameObject BounceIndicator;
    [SerializeField] GameObject EnemyIndicator;
    [SerializeField] Animator BodyAnimator;
    public Camera MainCam;
    public float IndicatorMinSize;
    public float BounceCooldown;
    public float BounceIndicatorGrowRate;
    public float RegularFOV;
    public float DashFOV;
    public float FOVAcceleration;
    public float FOVReturn;
    public float BouncePower; //(percent)
    public float DashBoost;
    public float EnemyDashBoost;
    public float BounceRingOffset;
    public float BounceRadius;
    public float MaxWobbleSpeed;
    public float WobbleSpeedMultiplier;
    public float EnemyIndicatorDistance;
    public float HideEnemyIndicatorDistance;
    public float BounceDMG;

    public GameObject DashTube;
    public GameObject DashRing;
    public GameObject DashImpact;

    Vector3 velocity;
    float CollisionTolerance = 0.1f;
    float BounceCD;
    bool IsDashing;
    Vector3 BodyUpDir;

    RaycastHit hit;

    void Start()
    {
        velocity = transform.forward * StartVelocity;
        BounceIndicator.SetActive(false);
        EnemyIndicator.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        BounceCD = Mathf.Clamp(BounceCD - Time.deltaTime, 0, BounceCooldown);
        DetectJump();
        UpdateFOV();

        if (IsDashing)
        {
            EnemyIndicator.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        DetectCollision();
        UpdateVelocity();
    }

    void UpdateVelocity()
    {
        velocity += (IsDashing ? 0 : g) * Vector3.up * Time.fixedDeltaTime;
        transform.position += velocity * Time.fixedDeltaTime;
        transform.LookAt(transform.position + velocity, BodyUpDir);
    }

    void DetectCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, velocity.magnitude * Time.fixedDeltaTime + CollisionTolerance,
            LayerMask.GetMask("Collision") | LayerMask.GetMask("Sensor")))
        {
            velocity = Quaternion.AngleAxis(Vector3.Angle(-hit.normal, velocity),Vector3.Cross(hit.normal,velocity)) * hit.normal * velocity.magnitude * BouncePower;
            BodyAnimator.SetTrigger("R"+Random.Range(0,4).ToString());
            BodyAnimator.speed = Mathf.Clamp((velocity.magnitude * Mathf.Sin(Vector3.Angle(velocity, hit.normal) * Mathf.Deg2Rad)) / (4 * Mathf.PI * BounceRadius) * WobbleSpeedMultiplier,
                0, MaxWobbleSpeed);
            BodyUpDir = -Vector3.Cross(velocity, Vector3.Cross(hit.normal, velocity));
            if (IsDashing)
            {
                Instantiate(DashImpact, hit.point, Quaternion.LookRotation(hit.normal));
                IsDashing = false;
            }
            
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Sensor"))
            {
                Damage(hit.collider.GetComponent<SensorCollider>().Parent, BounceDMG);
            }
        }
    }

    void DetectJump()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0)), out hit, EnemyRange,
            LayerMask.GetMask("Collision") | LayerMask.GetMask("SensorTolerance")))
        {
            bool HitIsEnemy = hit.collider.gameObject.layer == LayerMask.NameToLayer("SensorTolerance");

            if (HitIsEnemy || Vector3.Distance(Camera.main.transform.position, hit.point) <= Range)
            {
                if (HitIsEnemy)
                {
                    if (Vector3.Distance(transform.position, hit.collider.transform.position) > HideEnemyIndicatorDistance)
                    {
                        EnemyIndicator.SetActive(true);
                    }
                    BounceIndicator.SetActive(false);
                    ArrangeBounceIndicator(EnemyIndicator, hit.collider.transform.position + (transform.position - hit.collider.transform.position).normalized * EnemyIndicatorDistance,
                        (transform.position - hit.collider.transform.position).normalized);
                }
                else
                {
                    BounceIndicator.SetActive(true);
                    EnemyIndicator.SetActive(false);
                    ArrangeBounceIndicator(BounceIndicator, hit.point, hit.normal);
                }

                if (BounceCD == 0)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (HitIsEnemy)
                        {
                            transform.LookAt(hit.collider.transform.position);
                            velocity = transform.forward * EnemyDashBoost;
                            InstantiateBounceEffects(hit.collider.transform.position);
                        }
                        else
                        {
                            transform.LookAt(hit.point);
                            velocity = transform.forward * DashBoost;
                            InstantiateBounceEffects(hit.point);
                        }
                        IsDashing = true;

                        BounceCD = BounceCooldown;
                    }
                }
            }

            else
            {
                BounceIndicator.SetActive(false);
                EnemyIndicator.SetActive(false);
            }
        }
        else
        {
            BounceIndicator.SetActive(false);
            EnemyIndicator.SetActive(false);
        }
    }

    void ArrangeBounceIndicator(GameObject indicator, Vector3 point, Vector3 normal)
    {
        indicator.transform.position = point;
        indicator.transform.rotation = Quaternion.LookRotation(normal);
    }

    void InstantiateBounceEffects(Vector3 point)
    {
        Instantiate(DashTube, (transform.position + point) / 2f, Quaternion.LookRotation(-transform.position + point));
        Instantiate(DashRing, transform.position + (-transform.position + point).normalized * BounceRingOffset, Quaternion.LookRotation(-transform.position + point));
    }

    void UpdateFOV()
    {
        if (IsDashing)
        {
            MainCam.fieldOfView = Mathf.Clamp(Mathf.MoveTowards(MainCam.fieldOfView, DashFOV, FOVAcceleration * Time.deltaTime), RegularFOV, DashFOV);
        }
        else
        {
            MainCam.fieldOfView = Mathf.Clamp(Mathf.MoveTowards(MainCam.fieldOfView, RegularFOV, FOVReturn * Time.deltaTime), RegularFOV, DashFOV);
        }
    }

    void Damage(Sensor target, float dmg)
    {
        target.Health -= dmg;
        if(target.Health <= 0)
        {
            target.Die();
        }
    }
}
