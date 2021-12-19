using UnityEngine;

namespace Frontend
{
	public class CameraController : MonoSingleton<CameraController>
	{
		[Header("Camera positions and rotations")]
		[SerializeField] Vector3 positionWhenWhite = new Vector3(0.25f, 3.5f, -10f);
		[SerializeField] Vector3 rotationWhenWhite = new Vector3(0f, 0f, 0f);

		[SerializeField] Vector3 positionWhenBlack = new Vector3(6.75f, 3.5f, -10f);
		[SerializeField] Vector3 rotationWhenBlack = new Vector3(0f, 0f, 180f);

		[Header("Objects to flip")]
		[SerializeField] Transform _whitePlayerClock;
		[SerializeField] Transform _blackPlayerClock;

		[SerializeField] Transform _capturedPiecesByWhite;
		[SerializeField] Transform _capturedPiecesByBlack;

		public void FlipPOV()
		{
			bool isCurrentPovWhite = transform.position == positionWhenWhite;
			if (transform.position == positionWhenWhite)
			{
				ChangeCameraPositionAndRotation(positionWhenBlack, rotationWhenBlack);
			}
			else
			{
				ChangeCameraPositionAndRotation(positionWhenWhite, rotationWhenWhite);
			}

			FlipSpriteRenderers(isCurrentPovWhite);
			FlipClocks();
			FlipCapturesPieces();
		}

		void ChangeCameraPositionAndRotation(Vector3 position, Vector3 rotation)
		{
			transform.position = position;
			transform.rotation = Quaternion.Euler(rotation);
		}

		void FlipSpriteRenderers(bool flip)
		{
			foreach (SpriteRenderer sprite in FindObjectsOfType<SpriteRenderer>(true))
			{
				sprite.flipX = flip;
				sprite.flipY = flip;
			}
		}

		void FlipClocks()
		{
			var whiteOldPosition = _whitePlayerClock.localPosition;
			_whitePlayerClock.localPosition = new Vector2(_blackPlayerClock.localPosition.x, _blackPlayerClock.localPosition.y);
			_blackPlayerClock.localPosition = new Vector2(whiteOldPosition.x, whiteOldPosition.y);
		}

		void FlipCapturesPieces()
		{
			var capturedPiecesByWhiteOldPosition = _capturedPiecesByWhite.localPosition;
			_capturedPiecesByWhite.localPosition = new Vector2(_capturedPiecesByBlack.localPosition.x, _capturedPiecesByBlack.localPosition.y);
			_capturedPiecesByBlack.localPosition = new Vector2(capturedPiecesByWhiteOldPosition.x, capturedPiecesByWhiteOldPosition.y);

		}
	}
}
