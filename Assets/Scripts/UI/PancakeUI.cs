using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rito.CustomAttributes;

// 날짜 : 2021-02-05 PM 4:54:39
// 작성자 : Rito

/// <summary> V키 누르면 나오는 화면 중앙 UI </summary>
public class PancakeUI : MonoBehaviour
{
    [Header("Options")]
    public int _pieceCount = 8;

    [Range(0.2f, 1f)]
    public float _apparenceDelay = .3f; // 등장에 걸리는 시간

    public float _pieceDist = 180f; // 중앙으로부터 각 조각의 거리

    [Range(0.01f, 0.5f)]
    public float _centerDistThreshold = 0.1f; // 중앙에서부터의 마우스 거리 기준

    public bool _showCenterPiece;

    [Header("Objects")]
    [SerializeField] private GameObject _pieceSample;
    [SerializeField] private Transform _arrowHolderTran;

    private Sprite _sampleSprite; // 각 조각 스프라이트가 없는 경우 대비
    private Image _centerImage;
    private Image[] _pieceImages;
    private RectTransform[] _pieceRects;

    private Vector2[] _pieceDirections;
    private float _arrowHolderZRotation;
    [SerializeField, Readonly, Header("Debug")]
    private int _selectedIndex = -1;

    

    private static readonly Color ColorSelected = new Color(1f, 1f, 1f, 1f);
    private static readonly Color ColorNotSelected = new Color(1f, 1f, 1f, 0.3f);

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .

    private void Awake()
    {
        InitPieces();
        InitComponents();
        Init();
        HideAll();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .
    /// <summary> 조각 샘플 복제하여 조각들 생성 </summary>
    private void InitPieces()
    {
        _pieceSample.SetActive(true);

        _pieceImages = new Image[_pieceCount];

        for (int i = 0; i <= _pieceCount; i++)
        {
            var clone = Instantiate(_pieceSample, transform);
            Transform cloneChild = clone.transform.GetChild(0);

            if (i == _pieceCount)
            {
                clone.name = "Piece Center";
                cloneChild.TryGetComponent(out _centerImage);
            }
            else
            {
                clone.name = $"Piece {i}";
                cloneChild.TryGetComponent(out _pieceImages[i]);
            }
        }

        _pieceSample.SetActive(false);
        _sampleSprite = _pieceSample.GetComponent<Image>().sprite;

        _centerImage.gameObject.SetActive(_showCenterPiece);
    }

    private void InitComponents()
    {
        _pieceCount = _pieceImages.Length;

        _centerImage.color = ColorNotSelected;

        _pieceRects = new RectTransform[_pieceCount];
        for (int i = 0; i < _pieceCount; i++)
        {
            // Init Rects
            _pieceImages[i].transform.parent.TryGetComponent(out _pieceRects[i]);

            // Init Colors
            _pieceImages[i].color = ColorNotSelected;
        }
    }
    private void Init()
    {
        // init pieces dirs
        _pieceDirections = new Vector2[_pieceCount];

        float piCoef = Mathf.PI * 2f / _pieceCount;

        for (int i = 0; i < _pieceCount; i++)
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
        for (int i = 0; i < _pieceCount; i++)
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

    /// <summary> 팬케이크 사라지면서 인덱스 리턴 </summary>
    public int Hide() => FadeAndGetIndex();

    /// <summary> 각각 피스 이미지(스프라이트) 등록 </summary>
    public void SetPieceImageSprites(Sprite[] sprites)
    {
        int i = 0;
        int sLen = sprites.Length;
        for (; i < _pieceCount; i++)
        {
            if (i < sLen && sprites[i] != null)
            {
                _pieceImages[i].sprite = sprites[i];
            }
            else
            {
                _pieceImages[i].sprite = _sampleSprite;
            }
        }
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

        while (t < _apparenceDelay)
        {
            for (int i = 0; i < _pieceCount; i++)
            {
                _pieceRects[i].anchoredPosition
                     = _pieceDirections[i] * t * _pieceDist / _apparenceDelay;
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
                mDeg *= 0.5f * _pieceCount / Mathf.PI;
                _selectedIndex = Mathf.RoundToInt(mDeg) % _pieceCount;
            }

            SetSelectedPieceColors();
            SetArrowRotation(showArrow);

            yield return null;
        }
    }

    #endregion
}