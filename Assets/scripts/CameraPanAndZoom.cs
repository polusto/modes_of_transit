﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraPanAndZoom : MonoBehaviour 
{
	[SerializeField]
	private float mouseZoomScale = 1;

	[SerializeField]
	private float touchZoomScale = 1;

	[SerializeField]
	private float smoothTime = 0.5f;

	private bool dragging = false;

	private Camera gameCamera;

	private bool isZooming = false;

	private float pinchDistance = 0;

	private Vector3 origin;

	private Vector3 velocity;

	private bool underInertia;

	private float time = 0.0f;

	private void Awake()
	{
		gameCamera = GetComponent<Camera>();
	}

	private void Update()
	{		
		HandelDrag();	

		HandleZoom();

		HandleInertia();
	}

	private void HandelDrag()
	{
		Vector3 difference = Vector3.zero;

		if (Input.GetMouseButton(0))
		{
			difference = (gameCamera.ScreenToWorldPoint(Input.mousePosition)) - transform.position;
			if (!dragging)
			{
				dragging = true;
				origin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
			}
			underInertia = false;
		}
		else
		{
			if (Input.GetMouseButtonUp(0)) 
			{
				underInertia = true;
			}

			dragging = false;
		}

		if (dragging) 
		{
			var newPos = origin - difference;
			velocity = newPos - transform.position;
			transform.position = newPos;
		}
	}

	private void HandleZoom()
	{
		float step = Input.GetAxis("Mouse ScrollWheel") * gameCamera.orthographicSize * mouseZoomScale;
		gameCamera.orthographicSize -= step;

		if (Input.touchCount > 2) 
		{
			var newPinchDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

			if (isZooming) 
			{
				var delta = newPinchDistance - pinchDistance;
				delta /= Screen.height;
				delta *= gameCamera.orthographicSize * touchZoomScale;
				gameCamera.orthographicSize -= delta;
			}

			pinchDistance = newPinchDistance;

			isZooming = true;
		}
	}

	private void HandleInertia()
	{
		if (underInertia && time <= smoothTime) 
		{
			transform.position += velocity;
			velocity = Vector3.Lerp (velocity, Vector3.zero, time);
			time += Time.smoothDeltaTime;
		} 
		else 
		{
			underInertia = false;
			time = 0.0f;
		}
	}
}