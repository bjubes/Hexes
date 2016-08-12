using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFollowGameObject : MonoBehaviour {

	public RectTransform canvasRectTransform;
	public RectTransform UIElement;
	public Transform objectToFollow;   //in this case the hex

	void Start() {
		canvasRectTransform = (RectTransform)transform.parent.parent;
		UIElement = (RectTransform)this.transform;
	}

	void Update()
	{
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
		UIElement.anchoredPosition = screenPoint - canvasRectTransform.sizeDelta / 2f;
	}
}