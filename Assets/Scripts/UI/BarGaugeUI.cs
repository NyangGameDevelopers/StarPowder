using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 날짜 : 2021-02-02 PM 10:08:29
// 작성자 : Rito

/// <summary> 잔상을 남기며 변화하는 HP, MP 바 게이지 </summary>
public class BarGaugeUI : GaugeUI
{
	[SerializeField]
	protected Image _currentImage;
	[SerializeField]
	protected Image _expectedImage;

	[SerializeField, Range(0.1f, 1f)]
	protected float _updateSpeed = 1f; // 이미지 업데이트 속도

	// 현재 값 갱신 이전의 비율 기억
	private float _prevRatio;

	// 변화 목표 비율값
	private float _goalRatio;

	public override void InitValues(in float currentValue, in float maxValue)
    {
        base.InitValues(currentValue, maxValue);

		_currentImage.fillAmount = CurrentRatio;
		_expectedImage.fillAmount = CurrentRatio;
	}

    public override void UpdateMaxValue(in float value)
	{
		// 1. 이전 값 기억
		_prevRatio = CurrentRatio;

		// 2. 현재 값 업데이트
		base.UpdateMaxValue(value);

		// 3. 게이지 변화 시작
		_goalRatio = CurrentRatio;
		BeginUpdate();
	}

    public override void UpdateCurrentValue(in float value)
	{
		// 1. 이전 값 기억
		_prevRatio = CurrentRatio;

		// 2. 현재 값 업데이트
		base.UpdateCurrentValue(value);

		// 3. 게이지 변화 시작
		_goalRatio = CurrentRatio;
		BeginUpdate();
	}


	/// <summary> "현재값 / 최댓값" 꼴로 텍스트 표시 </summary>
	protected override void UpdateValueText()
	{
		_valueText.text = $"{CurrentValue:F0}/{MaxValue:F0}";
	}

	private void BeginUpdate()
	{
		StopCoroutine(IncreseRoutine());
		StopCoroutine(DecreaseRoutine());

		// 값이 상승하는 경우
		if (_prevRatio < CurrentRatio)
		{
			StartCoroutine(IncreseRoutine());
		}
		// 값이 하락하는 경우
		else
		{
			StartCoroutine(DecreaseRoutine());
		}
	}

	private IEnumerator IncreseRoutine()
	{
		while (_prevRatio < _goalRatio)
		{
			float mulitiplier = Mathf.Max((_goalRatio - _prevRatio), 0.1f);

			_prevRatio += _updateSpeed * 0.01f * mulitiplier;

			_currentImage.fillAmount = _prevRatio;
			_expectedImage.fillAmount = _goalRatio;
			yield return null;
		}
	}

	private IEnumerator DecreaseRoutine()
	{
		while (_goalRatio < _prevRatio)
		{
			float mulitiplier = Mathf.Max((_prevRatio - _goalRatio), 0.1f);

			_prevRatio -= _updateSpeed * 0.01f * mulitiplier;

			_expectedImage.fillAmount = _prevRatio;
			_currentImage.fillAmount = _goalRatio;
			yield return null;
		}
	}
}