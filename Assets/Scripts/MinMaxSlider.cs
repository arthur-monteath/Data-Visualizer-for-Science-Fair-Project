﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Min_Max_Slider
{
	[RequireComponent(typeof(RectTransform))]
	public class MinMaxSlider : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private enum DragState
		{
			Both,
			Min,
			Max
		}

		/// <summary>
		/// Floating point tolerance
		/// </summary>
		private const float FLOAT_TOL = 0.01f;

		[Header("UI Controls")]
		[SerializeField] private Camera customCamera = null;
		[SerializeField] private RectTransform sliderBounds = null;
		[SerializeField] private RectTransform minHandle = null;
		[SerializeField] private RectTransform maxHandle = null;
		[SerializeField] private RectTransform middleGraphic = null;

		// text components (optional)
		[Header("Display Text (Optional)")]
		[SerializeField] private TextMeshProUGUI minText = null;
		[SerializeField] private TextMeshProUGUI maxText = null;
		[SerializeField] private string textFormat = "0";

		// values
		[Header("Limits")]
		[SerializeField] private float minLimit = 0;
		[SerializeField] private float maxLimit = 100;
		[SerializeField] private float spacing = 5;

		[Header("Values")]
		public bool wholeNumbers;
		[SerializeField] private float minValue = 25;
		[SerializeField] private float maxValue = 75;

		public MinMaxValues Values => new MinMaxValues(minValue, maxValue, minLimit, maxLimit);

		/// <summary>
		/// Event invoked when either slider value has changed
		/// <para></para>
		/// T0 = min, T1 = max
		/// </summary>
		[Serializable]
		public class SliderEvent : UnityEvent<float, float>
		{
		}

		public SliderEvent onValueChanged = new SliderEvent();

		private Vector2 dragStartPosition;
		private float dragStartMinValue01;
		private float dragStartMaxValue01;
		private DragState dragState;
		private bool passDragEvents; // this allows drag events to be passed through to scrollers

		private Camera mainCamera;
		private Canvas parentCanvas;
		private bool isOverlayCanvas;

		protected override void Start()
		{
			base.Start();

			if (!sliderBounds)
				sliderBounds = transform as RectTransform;

			parentCanvas = GetComponentInParent<Canvas>();
			isOverlayCanvas = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
			mainCamera = customCamera != null ? customCamera : Camera.main;
		}

		public void SetLimits(float minLimit, float maxLimit)
		{
			this.minLimit = wholeNumbers ? Mathf.RoundToInt(minLimit) : minLimit;
			this.maxLimit = wholeNumbers ? Mathf.RoundToInt(maxLimit) : maxLimit;
		}

		public void SetValues(MinMaxValues values, bool notify = true)
		{
			SetValues(values.minValue, values.maxValue, values.minLimit, values.maxLimit, notify);
		}

		public void SetValues(float minValue, float maxValue, bool notify = true)
		{
			SetValues(minValue, maxValue, minLimit, maxLimit, notify);
		}

		public void SetValues(float minValue, float maxValue, float minLimit, float maxLimit, bool notify = true)
		{
			this.minValue = wholeNumbers ? Mathf.RoundToInt(minValue) : minValue;
			this.maxValue = wholeNumbers ? Mathf.RoundToInt(maxValue) : maxValue;
			SetLimits(minLimit, maxLimit);

			RefreshSliders();
			UpdateText();
			UpdateMiddleGraphic();

			if (notify)
			{
				// event
				onValueChanged.Invoke(this.minValue, this.maxValue);
			}
		}

		private void RefreshSliders()
		{
			SetSliderAnchors();

			float clampedMin = Mathf.Clamp(minValue, minLimit, maxLimit);
			SetMinHandleValue01(minHandle, GetPercentage(minLimit, maxLimit, clampedMin));

			float clampedMax = Mathf.Clamp(maxValue, minLimit, maxLimit);
			SetMaxHandleValue01(maxHandle, GetPercentage(minLimit, maxLimit, clampedMax));
		}

		private void SetSliderAnchors()
		{
			minHandle.anchorMin = new Vector2(0, 0.5f);
			minHandle.anchorMax = new Vector2(0, 0.5f);
			minHandle.pivot = new Vector2(0.5f, 0.5f);

			maxHandle.anchorMin = new Vector2(1, 0.5f);
			maxHandle.anchorMax = new Vector2(1, 0.5f);
			maxHandle.pivot = new Vector2(0.5f, 0.5f);
		}

		private void UpdateText()
		{
			if (minText)
				minText.SetText(minValue.ToString(textFormat));

			if (maxText)
				maxText.SetText(maxValue.ToString(textFormat));
		}

		private void UpdateMiddleGraphic()
		{
			if (!middleGraphic)
				return;

			middleGraphic.anchorMin = Vector2.zero;
			middleGraphic.anchorMax = Vector2.one;
			middleGraphic.offsetMin = new Vector2(minHandle.anchoredPosition.x, 0);
			middleGraphic.offsetMax = new Vector2(maxHandle.anchoredPosition.x, 0);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			passDragEvents = Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y);

			if (passDragEvents)
			{
				PassDragEvents<IBeginDragHandler>(x => x.OnBeginDrag(eventData));
			}
			else
			{
				Camera uiCamera = isOverlayCanvas ? null : mainCamera;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderBounds, eventData.position, uiCamera, out dragStartPosition);

				float dragStartValue = GetValueOfPointInSliderBounds01(dragStartPosition);
				dragStartMinValue01 = GetMinHandleValue01(minHandle);
				dragStartMaxValue01 = GetMaxHandleValue01(maxHandle);

				// set drag state
				if (dragStartValue < dragStartMinValue01 || RectTransformUtility.RectangleContainsScreenPoint(minHandle, eventData.position, uiCamera))
				{
					dragState = DragState.Min;
					minHandle.SetAsLastSibling();
				}
				else if (dragStartValue > dragStartMaxValue01 || RectTransformUtility.RectangleContainsScreenPoint(maxHandle, eventData.position, uiCamera))
				{
					dragState = DragState.Max;
					maxHandle.SetAsLastSibling();
				}
				else
					dragState = DragState.Both;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (passDragEvents)
			{
				PassDragEvents<IDragHandler>(x => x.OnDrag(eventData));
			}
			else if (minHandle && maxHandle)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderBounds, eventData.position, isOverlayCanvas ? null : mainCamera, out Vector2 clickPosition);

				SetSliderAnchors();

				if (dragState == DragState.Min || dragState == DragState.Max)
				{
					float dragPosition01 = GetValueOfPointInSliderBounds01(clickPosition);
					float minHandleValue = GetMinHandleValue01(minHandle);
					float maxHandleValue = GetMaxHandleValue01(maxHandle);

					if (dragState == DragState.Min)
						SetMinHandleValue01(minHandle, Mathf.Clamp(dragPosition01, 0, maxHandleValue));
					else if (dragState == DragState.Max)
						SetMaxHandleValue01(maxHandle, Mathf.Clamp(dragPosition01, minHandleValue, 1));
				}
				else
				{
					float distancePercent = (clickPosition.x - dragStartPosition.x) / sliderBounds.rect.width;
					SetMinHandleValue01(minHandle, dragStartMinValue01 + distancePercent);
					SetMaxHandleValue01(maxHandle, dragStartMaxValue01 + distancePercent);
				}

				// set values
				float min = Mathf.Lerp(minLimit, maxLimit, GetMinHandleValue01(minHandle));
				float max = Mathf.Lerp(minLimit, maxLimit, GetMaxHandleValue01(maxHandle));
				SetValues(min, max);

				UpdateText();
				UpdateMiddleGraphic();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (passDragEvents)
			{
				PassDragEvents<IEndDragHandler>(x => x.OnEndDrag(eventData));
			}
			else
			{
				float minHandleValue = GetMinHandleValue01(minHandle);
				float maxHandleValue = GetMaxHandleValue01(maxHandle);

				// this safe guards a possible situation where the slides can get stuck
				if (Math.Abs(minHandleValue) < FLOAT_TOL && Math.Abs(maxHandleValue) < FLOAT_TOL)
					maxHandle.SetAsLastSibling();
				else if (Math.Abs(minHandleValue - 1) < FLOAT_TOL && Math.Abs(maxHandleValue - 1) < FLOAT_TOL)
					minHandle.SetAsLastSibling();
			}
		}

		private void PassDragEvents<T>(Action<T> callback) where T : IEventSystemHandler
		{
			Transform parent = transform.parent;

			while (parent != null)
			{
				foreach (var component in parent.GetComponents<Component>())
				{
					if (!(component is T))
						continue;

					callback.Invoke((T) (IEventSystemHandler) component);
					return;
				}

				parent = parent.parent;
			}
		}

		/// <summary>
		/// Sets position of max handle RectTransform
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="value01">Normalized handle position</param>
		private void SetMaxHandleValue01(RectTransform handle, float value01)
		{
			handle.anchoredPosition = new Vector2(value01 * sliderBounds.rect.width - sliderBounds.rect.width + sliderBounds.offsetMax.x, handle.anchoredPosition.y);
		}

		/// <summary>
		/// Sets position of min handle RectTransform
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="value01">Normalized handle position</param>
		private void SetMinHandleValue01(RectTransform handle, float value01)
		{
			handle.anchoredPosition = new Vector2(value01 * sliderBounds.rect.width + sliderBounds.offsetMin.x, handle.anchoredPosition.y);
		}

		/// <summary>
		/// Returns normalized position of max handle RectTransform
		/// </summary>
		/// <param name="handle"></param>
		/// <returns>Normalized position of max handle RectTransform</returns>
		private float GetMaxHandleValue01(RectTransform handle)
		{
			return 1 + (handle.anchoredPosition.x - sliderBounds.offsetMax.x) / sliderBounds.rect.width;
		}

		/// <summary>
		/// Returns normalized position of min handle RectTransform
		/// </summary>
		/// <param name="handle"></param>
		/// <returns>Normalized position of min handle RectTransform</returns>
		private float GetMinHandleValue01(RectTransform handle)
		{
			return (handle.anchoredPosition.x - sliderBounds.offsetMin.x) / sliderBounds.rect.width;
		}

		/// <summary>
		/// Returns normalized position of a point in a slider bounds rectangle
		/// </summary>
		/// <param name="position"></param>
		/// <returns>Normalized position of a point in a slider bounds rectangle</returns>
		private float GetValueOfPointInSliderBounds01(Vector2 position)
		{
			var width = sliderBounds.rect.width;
			return Mathf.Clamp((position.x + width / 2) / width, 0, 1);
		}

		/// <summary>
		/// Returns percentage of input based on min and max values
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		private static float GetPercentage(float min, float max, float input)
		{
			return (input - min) / (max - min);
		}

		[Serializable]
		public struct MinMaxValues
		{
			public float minValue, maxValue, minLimit, maxLimit;
			public static MinMaxValues DEFUALT = new MinMaxValues(25, 75, 0, 100);

			public MinMaxValues(float minValue, float maxValue, float minLimit, float maxLimit)
			{
				this.minValue = minValue;
				this.maxValue = maxValue;
				this.minLimit = minLimit;
				this.maxLimit = maxLimit;
			}
			
			/// <summary>
			/// Constructor for when values equal limits
			/// </summary>
			/// <param name="minValue"></param>
			/// <param name="maxValue"></param>
			public MinMaxValues(float minValue, float maxValue)
			{
				this.minValue = minValue;
				this.maxValue = maxValue;
				this.minLimit = minValue;
				this.maxLimit = maxValue;
			}
			
			public bool IsAtMinAndMax()
			{
				return Math.Abs(minValue - minLimit) < FLOAT_TOL && Math.Abs(maxValue - maxLimit) < FLOAT_TOL;
			}

			public override string ToString()
			{
				return $"Values(min:{minValue}, max:{maxValue}) | Limits(min:{minLimit}, max:{maxLimit})";
			}
		}
	}
}