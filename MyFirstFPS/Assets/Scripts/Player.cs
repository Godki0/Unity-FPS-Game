using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int curHp;
    public int maxHp;

    [Header("Movement")]
    public float moveSpeed;  // movement speed in units per second
    public float jumpForce;  // force applied upwards

    [Header("Camera")]
    public float lookSensitivity;  // mouse look sensitivity
    public float maxLookX;         // lowest downward point we can look
    public float minLookX;         // highest upward point we can look
    private float rotX;            // current x rotation of the camera

    private Camera cam;
    private Rigidbody rig;
    private Weapon weapon;

    void Awake()
    {
        // get the components
        cam = Camera.main;
        rig = GetComponent<Rigidbody>();
        weapon = GetComponent<Weapon>();

        // disable cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        // initialize the UI
        GameUI.instance.UpdateHealthBar(curHp, maxHp);
        GameUI.instance.UpdateScoreText(0);
        GameUI.instance.UpdateAmmoText(weapon.curAmmo, weapon.maxAmmo);
    }

    void Update()
    {
        // don't do anything if game is paused
        if (GameManager.instance.gamePaused == true)
            return;

        Move();

        if (Input.GetButtonDown("Jump"))
            TryJump();

        if(Input.GetButton("Fire1"))
        {
            if(weapon.CanShoot())
                weapon.Shoot();
        }

        CamLook();
    }

    // move horizontally based on movement inputs
    void Move()
    {
        // get the x and z inputs
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rig.velocity.y;

        // apply the velocity
        rig.velocity = dir;
    }

    // rotate the camera based on mouse movement
    void CamLook()
    {
        // get mouse inputs
        float y = Input.GetAxis("Mouse X") * lookSensitivity;
        rotX += Input.GetAxis("Mouse Y") * lookSensitivity;

        // clamp the vetical rotation
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        // rotate the camera and player
        cam.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);
        transform.eulerAngles += Vector3.up * y;
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1.1f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;

        GameUI.instance.UpdateHealthBar(curHp, maxHp);

        if (curHp <= 0)
            Die();
    }

    void Die()
    {
        GameManager.instance.LoseGame();
    }

    public void GiveHealth(int amountToGive)
    {
        curHp = Mathf.Clamp(curHp + amountToGive, 0, maxHp);

        GameUI.instance.UpdateHealthBar(curHp, maxHp);
    }

    public void GiveAmmo(int amountToGive)
    {
        GameUI.instance.UpdateAmmoText(weapon.curAmmo, weapon.maxAmmo);

        weapon.curAmmo = Mathf.Clamp(weapon.curAmmo + amountToGive, 0, weapon.maxAmmo);
    }
}
