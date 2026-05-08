using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRtsCamera.Scripts
{
	public class SimpleRtsCamera : MonoBehaviour
	{
		[Header("RTS Camera Settings")]
		[SerializeField] private bool isMovementWithMouseEnabled = true;
		[SerializeField] private bool isMovementWithKeyboardEnabled = true;
		[SerializeField] private bool isZoomEnabled = true;

		[Header("Move")]
		[SerializeField] private float _moveSpeed = 200;
		[SerializeField] private float _edgeThreshold = 5;

		[Header("Zoom")]
		[SerializeField] private float _zoomSpeed = 200;
		[SerializeField] private float _minY = 10f;
		[SerializeField] private float _maxY = 80f;

		[Header("Bounds")]
		[SerializeField] private Vector2 _xLimits = new Vector2(-100, 100);
		[SerializeField] private Vector2 _zLimits = new Vector2(-100, 100);

		[Header("Go To")]
		[SerializeField] private float _goToSpeed = 10f;

		private PlayerInput _playerInput;
		private Vector2 _moveInput;
		private Vector2 _mousePositionInput;
		private Vector2 _initialMousePosition;
		private Vector2 _scrollMouseInput;

		private bool isGameFinished;

		private bool _isGoingToTarget;
		private Vector3 _targetPosition;

		private void Awake()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
		}

		private void OnEnable()
		{
			isGameFinished = false;

			_playerInput.actions["CameraMove"].performed += MoveHandler;
			_playerInput.actions["CameraMove"].canceled += MoveHandler;

			_playerInput.actions["MousePosition"].performed += MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled += MousePositionHandler;

			_playerInput.actions["RightMouse"].started += InitialMousePositionHandler;

			_playerInput.actions["ScrollMouse"].performed += ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled += ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started += InitialMousePositionHandler;

			EventManager.Instance.OnGameOver += GameFinished;
			EventManager.Instance.OnLevelCompleted += GameFinished;
		}

		private void LateUpdate()
		{
			if (isGameFinished) return;

			if (_isGoingToTarget)
			{
				UpdateGoTo();
			}
			else
			{
				if (isMovementWithKeyboardEnabled) MoveCamera();
				if (isMovementWithMouseEnabled) MoveCameraWithCursor();
			}

			if (isZoomEnabled) ZoomCamera();

			ClampPosition();
		}

		private void OnDisable()
		{
			if (!_playerInput) return;

			_playerInput.actions["CameraMove"].performed -= MoveHandler;
			_playerInput.actions["CameraMove"].canceled -= MoveHandler;

			_playerInput.actions["MousePosition"].performed -= MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled -= MousePositionHandler;

			_playerInput.actions["ScrollMouse"].performed -= ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled -= ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started -= InitialMousePositionHandler;

			EventManager.Instance.OnGameOver -= GameFinished;
			EventManager.Instance.OnLevelCompleted -= GameFinished;
		}

		private void GameFinished()
		{
			isGameFinished = true;
		}

		private void MoveHandler(InputAction.CallbackContext callbackContext) =>
			_moveInput = callbackContext.ReadValue<Vector2>();

		private void MousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_mousePositionInput = callbackContext.ReadValue<Vector2>();

		private void InitialMousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_initialMousePosition = _mousePositionInput;

		private void ScrollMouseHandler(InputAction.CallbackContext callbackContext) =>
			_scrollMouseInput = callbackContext.ReadValue<Vector2>();

		private void MoveCamera()
		{
			if (_moveInput.sqrMagnitude > 0.01f)
				_isGoingToTarget = false;

			var moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed * Time.deltaTime);
			transform.position += moveDirection;
		}

		private void MoveCameraWithCursor()
		{
			var moved = false;

			if (_mousePositionInput.x < _edgeThreshold)
			{
				transform.position += Vector3.left * (_moveSpeed * Time.deltaTime);
				moved = true;
			}
			else if (_mousePositionInput.x > Screen.width - _edgeThreshold)
			{
				transform.position += Vector3.right * (_moveSpeed * Time.deltaTime);
				moved = true;
			}

			if (_mousePositionInput.y < _edgeThreshold)
			{
				transform.position += Vector3.back * (_moveSpeed * Time.deltaTime);
				moved = true;
			}
			else if (_mousePositionInput.y > Screen.height - _edgeThreshold)
			{
				transform.position += Vector3.forward * (_moveSpeed * Time.deltaTime);
				moved = true;
			}

			if (moved)
				_isGoingToTarget = false;
		}

		private void ZoomCamera()
		{
			if (Mathf.Approximately(_scrollMouseInput.sqrMagnitude, 0f)) return;

			var zoomDirection = transform.forward;

			transform.position += zoomDirection * (_scrollMouseInput.x * _zoomSpeed) +
			                      zoomDirection * (_scrollMouseInput.y * _zoomSpeed);

			_scrollMouseInput = Vector2.zero;
		}

		private void ClampPosition()
		{
			var pos = transform.position;

			pos.x = Mathf.Clamp(pos.x, _xLimits.x, _xLimits.y);
			pos.z = Mathf.Clamp(pos.z, _zLimits.x, _zLimits.y);
			pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

			transform.position = pos;
		}

		private void UpdateGoTo()
		{
			transform.position = Vector3.Lerp(
				transform.position,
				_targetPosition,
				_goToSpeed * Time.deltaTime
			);

			if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
			{
				transform.position = _targetPosition;
				_isGoingToTarget = false;
			}
		}

		public void GoTo(Transform target)
		{
			if (!target) return;

			var currentOffset = transform.position - GetLookPoint();

			var desiredPosition = target.position + currentOffset;

			desiredPosition.y = transform.position.y;

			desiredPosition.x = Mathf.Clamp(desiredPosition.x, _xLimits.x, _xLimits.y);
			desiredPosition.z = Mathf.Clamp(desiredPosition.z, _zLimits.x, _zLimits.y);

			_targetPosition = desiredPosition;
			_isGoingToTarget = true;
		}

		private Vector3 GetLookPoint()
		{
			var ray = new Ray(transform.position, transform.forward);

			return Physics.Raycast(ray, out var hit, 1000f)
				? hit.point
				: transform.position + transform.forward * 50f;
		}
	}
}