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

		[Header("Bounds Padding")]
		[SerializeField] private CameraBoundsPadding _boundsPadding = new CameraBoundsPadding
		{
			xMin = 5f,
			xMax = 5f,
			zMin = 10f,
			zMax = 2f
		};

		[Header("Go To")]
		[SerializeField] private float _goToSpeed = 10f;

		[System.Serializable]
		private struct CameraBoundsPadding
		{
			public float xMin;
			public float xMax;
			public float zMin;
			public float zMax;
		}

		private PlayerInput _playerInput;
		private Vector2 _moveInput;
		private Vector2 _mousePositionInput;
		private Vector2 _initialMousePosition;
		private Vector2 _scrollMouseInput;

		private bool isGameFinished;

		private bool _isGoingToTarget;
		private Vector3 _targetPosition;

		private Vector2 _rawXLimits;
		private Vector2 _rawZLimits;

		private void Awake()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
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
			EventManager.Instance.OnPathBoundsChanged += UpdateCameraBounds;
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
			EventManager.Instance.OnPathBoundsChanged -= UpdateCameraBounds;
		}

		private void UpdateCameraBounds(Vector2 xLimits, Vector2 zLimits)
		{
			_rawXLimits = xLimits;
			_rawZLimits = zLimits;
			ApplyBoundsPadding();
		}

		private void ApplyBoundsPadding()
		{
			_xLimits = new Vector2(_rawXLimits.x - _boundsPadding.xMin, _rawXLimits.y + _boundsPadding.xMax);
			_zLimits = new Vector2(_rawZLimits.x - _boundsPadding.zMin, _rawZLimits.y + _boundsPadding.zMax);
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

			float scrollAmount = _scrollMouseInput.x + _scrollMouseInput.y;
			Vector3 zoomDirection = transform.forward;
			Vector3 move = zoomDirection * (scrollAmount * _zoomSpeed);

			float newY = transform.position.y + move.y;

			if (newY < _minY || newY > _maxY)
			{
				move = Vector3.zero;
			}

			transform.position += move;
			_scrollMouseInput = Vector2.zero;
		}

		private void ClampPosition()
		{
			var pos = transform.position;

			float forwardX = transform.forward.x;
			float forwardZ = transform.forward.z;
			float forwardY = Mathf.Abs(transform.forward.y);

			float horizontalPerY = forwardY > 0.001f
				? new Vector2(forwardX, forwardZ).magnitude / forwardY
				: 0f;

			float yDelta = pos.y - _minY;
			float horizontalOffset = yDelta * horizontalPerY;

			pos.x = Mathf.Clamp(pos.x, _xLimits.x - horizontalOffset, _xLimits.y + horizontalOffset);
			pos.z = Mathf.Clamp(pos.z, _zLimits.x - horizontalOffset, _zLimits.y + horizontalOffset);
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