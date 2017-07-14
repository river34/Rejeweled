using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    protected RectTransform rect;
    protected Image image;
	protected Text text;
	protected const float ANIM_DELAY = 0.5f;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        text = GetComponent<Text>();
    }

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Show(float delay)
	{
		Invoke("Show", delay);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Hide(float delay)
	{
		Invoke("Hide", delay);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}

    public virtual void SetSize(int _x, int _y, float _width, float _height)
    {
        if (rect == null) return;
        rect.anchoredPosition = new Vector2(_x * _width, _y * _height);
        rect.sizeDelta = new Vector2(_width, _height);
    }

    public virtual void SetImage(ref Sprite _sprite)
    {
        if (image == null) return;
        image.sprite = _sprite;
    }

    public void SetColor(Color _color)
    {
        if (image != null)
		{
			image.color = _color;
		}

		if (text != null)
		{
			text.color = _color;
		}
    }

    public void SetText(string _text)
    {
		if (text == null) return;
        text.text = _text;
    }

	public void SetColorSuccess()
	{
		SetColor(Color.yellow);
	}

	public void SetColorWarning()
	{
		SetColor(Color.red);
	}

	public void SetColorFade()
	{
		SetColor(new Color32(255, 255, 255, 80));
	}

	public void SetColorPossible()
	{
		SetColor(Color.green);
	}

	public void SetColorClear()
	{
		SetColor(Color.clear);
	}

	public Vector3[] GetCornors()
	{
		Vector3[] corners = new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

		if (rect == null) return corners;

		rect.GetWorldCorners(corners);

		return corners;
	}

	public Vector2 GetSize()
	{
		Vector2 size = Vector2.zero;

		if (rect == null) return size;

		size = rect.sizeDelta;

		if (size.Equals(Vector2.zero))
		{
			size = rect.rect.size;
		}

		return size;
	}

	public void MoveTo(Vector2 _target, float _duration = ANIM_DELAY)
    {
        if (rect == null) return;

		LeanTween.value(gameObject, rect.anchoredPosition, _target, _duration).setEase(LeanTweenType.easeOutQuint).setOnUpdate((Vector2 value) =>
			{
				rect.anchoredPosition = value;
			});
    }

	public void FadeOut(float _duration = ANIM_DELAY)
	{
		if (image != null)
		{
			LeanTween.value(gameObject, image.color, Color.clear, _duration).setEase(LeanTweenType.easeOutQuint).setOnUpdate((Color value) =>
			{
				image.color = value;
			}).setOnComplete(Destroy);
		}

		if (text != null)
		{
			LeanTween.value(gameObject, text.color, Color.clear, _duration).setEase(LeanTweenType.easeOutQuint).setOnUpdate((Color value) =>
			{
				text.color = value;
			}).setOnComplete(Destroy);
		}
	}

	public void MoveToAndDestroy(Vector2 _target, float _duration = ANIM_DELAY)
	{
		if (rect == null) return;

		LeanTween.value(gameObject, rect.anchoredPosition, _target, _duration).setEase(LeanTweenType.easeOutQuint).setOnUpdate((Vector2 value) =>
		{
			rect.anchoredPosition = value;
		}).setOnComplete(Destroy);
	}

	public void MoveUp(float _duration = ANIM_DELAY)
	{
		if (rect == null) return;

		Vector2 size = GetSize();

		LeanTween.value(gameObject, 0, size.y, _duration).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float value) =>
		{
			rect.offsetMax = new Vector2(0, -value);
		});
	}
}
