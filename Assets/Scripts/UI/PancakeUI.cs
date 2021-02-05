using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 날짜 : 2021-02-05 PM 4:54:39
// 작성자 : Rito

/// <summary> V키 누르면 나오는 화면 중앙 UI </summary>
public class PancakeUI : MonoBehaviour
{
    [SerializeField] private Image _centerImage;
    [SerializeField] private Image[] _pieceImages;
    [SerializeField] private Transform _arrowHolderTran;

    private RectTransform[] _pieceRects;

    private int _pieceLen;
    private Vector2[] _pieceDirections;
    private float _arrowHolderZRotation;

    [SerializeField]
    public int _selectedIndex = -1; // PRIVATE
    
    [Space]
    [Range(0.3f, 2f)]
    public float _apparenceDuration = .5f; // 등장에 걸리는 시간

    public float _pieceDist = 180f; // 중앙으로부터 각 조각의 거리
    public float _centerDistThreshold = 0.05f; // 중앙에서부터의 마우스 거리 기준

    private static readonly Color ColorSelected = new Color(1f, 1f, 1f, 1f);
    private static readonly Color ColorNotSelected = new Color(1f, 1f, 1f, 0.3f);

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .

    private void Awake()
    {
        InitComponents();
        Init();
        HideAll();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .
    private void InitComponents()
    {
        _pieceLen = _pieceImages.Length;

        _centerImage.color = ColorNotSelected;

        _pieceRects = new RectTransform[_pieceLen];
        for (int i = 0; i < _pieceLen; i++)
        {
            // Init Rects
            _pieceImages[i].TryGetComponent(out _pieceRects[i]);

            // Init Colors
            _pieceImages[i].color = ColorNotSelected;
        }
    }
    private void Init()
    {
        // init pieces dirs
        _pieceDirections = new Vector2[_pieceLen];

        float piCoef = Mathf.PI * 2f / _pieceLen;

        for (int i = 0; i < _pieceLen; i++)
        {
            float deg = -piCoef * i + Mathf.PI * 0.5f;

            _pieceDirections[i] = new Vector2(
                Mathf.Cos(deg), Mathf.Sin(deg)
            );
        }
    }
    private void ShowAll()
    {
        gameObject.SetActive(true);
    }
    private void HideAll()
    {
        gameObject.SetActive(false);
    }
    private void ResetAllColors()
    {
        _centerImage.color = ColorNotSelected;
        for (int i = 0; i < _pieceLen; i++)
        {
            _pieceImages[i].color = ColorNotSelected;
        }
    }
    private void SetSelectedPieceColors()
    {
        ResetAllColors();
        if (_selectedIndex == -1) _centerImage.color = ColorSelected;
        else _pieceImages[_selectedIndex].color = ColorSelected;
    }
    private void SetArrowRotation(bool show)
    {
        _arrowHolderTran.gameObject.SetActive(show);

        if (!show)
            return;
        else
        {
            _arrowHolderTran.eulerAngles =
                Vector3.forward * _arrowHolderZRotation;
        }
    }
    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .

    /// <summary> 팬케이크 등장 </summary>
    public void Show()
    {
        ShowAll();
        ResetAllColors();
        SetArrowRotation(false);
        _selectedIndex = -1;

        StartCoroutine("BeginRoutine");
    }

    /// <summary> 팬케이크 사라지면서 인덱스 리턴 </summary>
    public int FadeAndGetIndex()
    {
        StopCoroutine("BeginRoutine");
        HideAll();

        return _selectedIndex;
    }

    #endregion
    /***********************************************************************
    *                               Coroutines
    ***********************************************************************/
    #region .
    private IEnumerator BeginRoutine()
    {
        // 1. Appear
        float t = 0;

        while (t < _apparenceDuration)
        {
            for (int i = 0; i < _pieceLen; i++)
            {
                _pieceRects[i].anchoredPosition
                     = _pieceDirections[i] * t * _pieceDist / _apparenceDuration;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // 2. Watch Mouse Pos
        while (true)
        {
            bool showArrow = false;

            // 0.0 ~ 1.0 Mouse Pos
            var mPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            // Diff, Dir
            var mDiff = new Vector2(mPos.x - 0.5f, mPos.y - 0.5f);
            var mDir = mDiff.normalized;

            // Mouse Pos Dist from center
            var distCenter = mDiff.magnitude;

            if (distCenter < _centerDistThreshold)
            {
                _selectedIndex = -1;
            }
            else
            {
                // Get Radian
                var mDeg = Mathf.Atan2(mDir.y, mDir.x);

                // Adjust deg 0 direction
                mDeg = (Mathf.PI * 0.5f - mDeg);
                if (mDeg < 0f) mDeg += Mathf.PI * 2f;

                // Set Arrow Rotation
                _arrowHolderZRotation = -mDeg * Mathf.Rad2Deg;
                showArrow = true;

                // Calculate Array Index
                mDeg *= 0.5f * _pieceLen / Mathf.PI;
                _selectedIndex = Mathf.RoundToInt(mDeg) % 5;
            }

            SetSelectedPieceColors();
            SetArrowRotation(showArrow);

            yield return null;
        }
    }

    #endregion
}