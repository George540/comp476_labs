using System.Collections;

using UnityEngine;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Spaceship : MonoBehaviour
{
    public float RotationSpeed = 90.0f;
    public float MovementSpeed = 2.0f;
    public float MaxSpeed = 0.2f;

    public ParticleSystem Destruction;
    public GameObject EngineTrail;
    public GameObject BulletPrefab;

    private PhotonView photonView;

    private new Rigidbody rigidbody;
    private new Collider collider;
    private new Renderer renderer;

    private float rotation = 0.0f;
    private float acceleration = 0.0f;
    private float shootingTimer = 0.0f;

    private bool controllable = true;

    public void Awake()
    {
        photonView = GetComponent<PhotonView>();

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
    }

    public void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.material.color = AsteroidsGame.GetColor(photonView.Owner.GetPlayerNumber());
    }

    public void Update()
    {
        if (!photonView.AmOwner || !controllable)
            return;

        // we don't want the master client to apply input to remote ships while the remote player is inactive
        if (this.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        rotation = Input.GetAxis("Horizontal");
        acceleration = Input.GetAxis("Vertical");

        if (Input.GetButton("Jump") && shootingTimer <= 0.0)
        {
            shootingTimer = 0.2f;

			// TODO: Remotely call the Fire(...) method in Update() on the spaceship instance across the network
			// and pass the appropriate params. Make sure to set the RPCTarget to AllViaServer.
            photonView.RPC("Fire", RpcTarget.All, rigidbody.position, rigidbody.rotation);
        }

        if (shootingTimer > 0.0f)
            shootingTimer -= Time.deltaTime;
    }

    public void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (!controllable)
            return;

        Quaternion rot = rigidbody.rotation * Quaternion.Euler(0, rotation * RotationSpeed * Time.fixedDeltaTime, 0);
        rigidbody.MoveRotation(rot);

        Vector3 force = (rot * Vector3.forward) * acceleration * 1000.0f * MovementSpeed * Time.fixedDeltaTime;
        rigidbody.AddForce(force);

        if (rigidbody.velocity.magnitude > (MaxSpeed * 1000.0f))
        {
            rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed * 1000.0f;
        }

        CheckExitScreen();
    }

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(AsteroidsGame.PLAYER_RESPAWN_TIME);

		// TODO: Remotely call the RespawnSpaceship(...) method in WaitForRespawn() on the spaceship instance across the network
		// and pass the appropriate params. Make sure to set the RPCTarget to AllViaServer.
        photonView.RPC("RespawnSpaceship", RpcTarget.AllViaServer);
    }

    private void CheckExitScreen()
    {
        if (Camera.main == null)
            return;

        if (Mathf.Abs(rigidbody.position.x) > (Camera.main.orthographicSize * Camera.main.aspect))
        {
            rigidbody.position = new Vector3(-Mathf.Sign(rigidbody.position.x) * Camera.main.orthographicSize * Camera.main.aspect, 0, rigidbody.position.z);
            rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
        }

        if (Mathf.Abs(rigidbody.position.z) > Camera.main.orthographicSize)
        {
            rigidbody.position = new Vector3(rigidbody.position.x, rigidbody.position.y, -Mathf.Sign(rigidbody.position.z) * Camera.main.orthographicSize);
            rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
        }
    }

    #region PUN CALLBACKS

    // TODO: mark this method as an RPC method

    public void DestroySpaceship()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        collider.enabled = false;
        renderer.enabled = false;

        controllable = false;

        EngineTrail.SetActive(false);
        Destruction.Play();

        if (photonView.IsMine)
        {
            object lives;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { AsteroidsGame.PLAYER_LIVES, ((int)lives <= 1) ? 0 : ((int)lives - 1) } });

                if ((int)lives > 1)
                    StartCoroutine("WaitForRespawn");
            }
        }
    }

    // TODO: mark this method as an RPC method
    [PunRPC]
    public void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        GameObject bullet;

        /** Use this if you want to fire one bullet at a time **/
        // TODO: Ask yourself why we are not instead using PhotonNetwork.Instantiate() here?
        bullet = Instantiate(BulletPrefab, position, Quaternion.identity) as GameObject;
        bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, (rotation * Vector3.forward), Mathf.Abs(lag));

        /** Use this if you want to fire two bullets at once **/
        //Vector3 baseX = rotation * Vector3.right;
        //Vector3 baseZ = rotation * Vector3.forward;

        //Vector3 offsetLeft = -1.5f * baseX - 0.5f * baseZ;
        //Vector3 offsetRight = 1.5f * baseX - 0.5f * baseZ;

        //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetLeft, Quaternion.identity) as GameObject;
        //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
        //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetRight, Quaternion.identity) as GameObject;
        //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
    }

    // TODO: mark this method as an RPC method
    [PunRPC]
    public void RespawnSpaceship()
    {
        collider.enabled = true;
        renderer.enabled = true;

        controllable = true;

        EngineTrail.SetActive(true);
        Destruction.Stop();
    }

    #endregion
}