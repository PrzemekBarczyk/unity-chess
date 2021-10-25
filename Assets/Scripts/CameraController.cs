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
	}
}
