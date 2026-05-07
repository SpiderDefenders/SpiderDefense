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

		private PlayerInput _playerInput;
		private Vector2 _moveInput;
		private Vector2 _mousePositionInput;
		private float _rightMouseInput;
		private Vector2 _initialMousePosition;
		private Vector2 _scrollMouseInput;
		private float _middleMouseInput;
		
		private bool isGameFinished;

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
			if (isMovementWithKeyboardEnabled) MoveCamera();
			if (isMovementWithMouseEnabled) MoveCameraWithCursor();
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
			var moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed * Time.deltaTime);
			transform.position += moveDirection;
		}

		private void MoveCameraWithCursor()
		{
			if (_mousePositionInput.x < _edgeThreshold)
			{
				transform.position += Vector3.left * (_moveSpeed * Time.deltaTime);
			}
			else if (_mousePositionInput.x > Screen.width - _edgeThreshold)
			{
				transform.position += Vector3.right * (_moveSpeed * Time.deltaTime);
			}

			if (_mousePositionInput.y < _edgeThreshold)
			{
				transform.position += Vector3.back * (_moveSpeed * Time.deltaTime);
			}
			else if (_mousePositionInput.y > Screen.height - _edgeThreshold)
			{
				transform.position += Vector3.forward * (_moveSpeed * Time.deltaTime);
			}
		}

		private void ZoomCamera()
		{
			if (Mathf.Approximately(_scrollMouseInput.sqrMagnitude, 0f)) return;

			var zoomDirection = transform.forward;

			transform.position += zoomDirection * (_scrollMouseInput.x * _zoomSpeed) + zoomDirection * (_scrollMouseInput.y * _zoomSpeed);
    
			_scrollMouseInput = new Vector2(0f, 0f);
		}
		
		private void ClampPosition()
		{
			var pos = transform.position;

			pos.x = Mathf.Clamp(pos.x, _xLimits.x, _xLimits.y);
			pos.z = Mathf.Clamp(pos.z, _zLimits.x, _zLimits.y);
			pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

			transform.position = pos;
		}
	}
}