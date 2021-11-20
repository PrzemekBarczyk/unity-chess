using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] Vector3 positionWhenWhite = new Vector3(0.25f, 3.5f, -10f);
	[SerializeField] Vector3 rotationWhenWhite = new Vector3(0f, 0f, 0f);

	[SerializeField] Vector3 positionWhenBlack = new Vector3(6.75f, 3.5f, -10f);
	[SerializeField] Vector3 rotationWhenBlack = new Vector3(0f, 0f, 180f);

	public void WhitePlayerPOV()
	{
		transform.position = positionWhenWhite;
		transform.rotation = Quaternion.Euler(rotationWhenWhite);
		foreach (SpriteRenderer sprite in FindObjectsOfType<SpriteRenderer>(true))
		{
			sprite.flipX = false;
			sprite.flipY = false;
		}

		Transform whiteClock = GameObject.Find("White Clock").transform;
		Transform blackClock = GameObject.Find("Black Clock").transform;
		whiteClock.localPosition = new Vector2(whiteClock.position.x, -Mathf.Abs(whiteClock.position.y));
		blackClock.localPosition = new Vector2(blackClock.position.x, Mathf.Abs(blackClock.position.y));
	}

	public void BlackPlayerPOV()
	{
		transform.position = positionWhenBlack;
		transform.rotation = Quaternion.Euler(rotationWhenBlack);
		foreach (SpriteRenderer sprite in FindObjectsOfType<SpriteRenderer>(true))
		{
			sprite.flipX = true;
			sprite.flipY = true;
		}

		Transform whiteClock = GameObject.Find("White Clock").transform;
		Transform blackClock = GameObject.Find("Black Clock").transform;
		whiteClock.localPosition = new Vector2(whiteClock.localPosition.x, Mathf.Abs(whiteClock.localPosition.y));
		blackClock.localPosition = new Vector2(blackClock.localPosition.x, -Mathf.Abs(blackClock.localPosition.y));
	}
}
