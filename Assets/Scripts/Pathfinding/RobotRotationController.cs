﻿using System;
using UnityEngine;

public class RobotRotationController : MonoBehaviour
{
	private readonly VectorPid angularVelocityController = new VectorPid(33.7766f, 0, 0.2553191f);
	[SerializeField] private VectorPid headingController = new VectorPid(9.244681f, 0, 0.06382979f);

	[SerializeField] private Transform playerHead;
	[SerializeField] private bool physicsRotation;
	[SerializeField, Header("Animated Rotation")] public float rotationEasing = 1f;

	public Rigidbody rb;
	private RobotMovementController movementController;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		movementController = GetComponent<RobotMovementController>();
	}

	private void Update()
	{
		if(!physicsRotation)
		{
			rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(LookAtDir, Vector3.up), Time.deltaTime * rotationEasing));
		}
	}

	private void FixedUpdate()
	{
		if (physicsRotation)
		{
			Vector3 angularVelocityError = rb.angularVelocity * -1f;
			Vector3 angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
			rb.AddTorque(angularVelocityCorrection, ForceMode.Acceleration);

			//forward heading correction
			Vector3 desiredHeading = LookAtDir;
			Vector3 currentHeading = transform.forward;
			Vector3 headingError = Vector3.Cross(currentHeading, desiredHeading);
			Vector3 headingCorrection = headingController.Update(headingError, Time.deltaTime);
			headingCorrection.x = 0;
			headingCorrection.z = 0;
			rb.AddTorque(headingCorrection, ForceMode.Acceleration);

			//up heading correction
			desiredHeading = Vector3.up - transform.up;
			currentHeading = transform.up;
			headingError = Vector3.Cross(currentHeading, desiredHeading);
			headingCorrection = headingController.Update(headingError, Time.deltaTime);
            headingCorrection.x = 0;
            headingCorrection.z = 0;
            rb.AddTorque(headingCorrection, ForceMode.Acceleration);
		}
		
	}

	public Vector3 LookAtDir
	{
		get
		{
			if (movementController.HasTarget)
			{
				return movementController.CurrentTargetPosition - rb.position;
			}
			return playerHead.position - rb.position;
		}
	}

	[Serializable]
	public class VectorPid
	{
		public float pFactor, iFactor, dFactor;

		private Vector3 integral;
		private Vector3 lastError;

		public VectorPid(float pFactor, float iFactor, float dFactor)
		{
			this.pFactor = pFactor;
			this.iFactor = iFactor;
			this.dFactor = dFactor;
		}

		public Vector3 Update(Vector3 currentError, float timeFrame)
		{
			integral += currentError * timeFrame;
			var deriv = (currentError - lastError) / timeFrame;
			lastError = currentError;
			return currentError * pFactor
				+ integral * iFactor
				+ deriv * dFactor;
		}
	}
}