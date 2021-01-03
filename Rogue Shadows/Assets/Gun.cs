using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100;
    public float fireRate = 15f;
    public float impactForce = 30f;

    public int maxAmmo = 10;
    private int gunAmmo = 10;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    public Text ammoDisplay;

    public ParticleSystem muzzleFlash;
    public GameObject fpsCam;
    public GameObject impactEffect;
    AudioSource gunAudioSource;

    private float nextTimeToFire = 0f;

    public Animator animator;

    [SerializeField] AudioClip gunShot;
    [SerializeField] AudioClip gunReload;
    // Update is called once per frame

    private void Start()
    {
        if (gunAmmo == -1)
        {
            gunAmmo = maxAmmo;
        }
        gunAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Update()
    {
        if (isReloading)
            return;

        if (gunAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
        ammoDisplay.text = "Ammo: " + gunAmmo.ToString();

    }
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        gunAudioSource.PlayOneShot(gunReload);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        gunAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        if (gunAmmo != 0f)
        {
            gunAmmo--;
            muzzleFlash.Play();
            gunAudioSource.PlayOneShot(gunShot);
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Target target = hit.transform.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
    }

}

