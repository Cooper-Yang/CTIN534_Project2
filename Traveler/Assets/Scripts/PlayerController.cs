using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    [Range(0, 10f)]
    [SerializeField]
    private float headBobAmp = 0.5f;
    
    [Range(0, 5f)]
    [SerializeField]
    private float headBobFreq = 0.5f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private InputManager inputManager;

    private Transform cameraTransform;

    public Cinemachine.CinemachineVirtualCamera vcam;
    private Cinemachine.CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        noise = vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        //Debug.Log(noise.m_AmplitudeGain);
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlaerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;
        controller.Move(move * Time.deltaTime * playerSpeed);
        AdjustHeadBobBasedOnVelocity(move.magnitude);

        // Changes the height position of the player..
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void AdjustHeadBobBasedOnVelocity(float velocityMagnitude)
    {
        if (noise != null && velocityMagnitude != 0)
        {
            noise.m_AmplitudeGain = headBobAmp; 
            noise.m_FrequencyGain = headBobFreq; 
            //Debug.Log("amp:" + noise.m_AmplitudeGain +"    freq:"+noise.m_FrequencyGain);
        }
        else if(noise != null)
        {
            noise.m_AmplitudeGain = 1;
            noise.m_FrequencyGain = 1;
        }
    }
}
