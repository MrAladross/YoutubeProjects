using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiler : MonoBehaviour
{
    [SerializeField] private int _width = 1;
    [SerializeField] private int _depth = 1;
    [SerializeField] private float _spacingX = 1f;
    [SerializeField] private float _spacingZ = 1f;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();
    private Vector3 _offset;
    private Quaternion _angle;
    [SerializeField] private bool _useAngleOffset = true;
    [SerializeField] private Vector3 _angleOffset = Vector3.zero;
    
    //gizmos
    [SerializeField] private Color _gridColor = Color.blue;
    [SerializeField] private Color _cellColor = Color.green;
    [SerializeField] private float _gizmoHeight = 1f;
    [SerializeField] private bool _useSkinnedMesh = false;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private SkinnedMeshRenderer _skinnedMesh;
    
    
    
    void Awake()
    {
        if (_prefab == null)
        {
            Debug.LogError("You forgot to assign a prefab to Tiler on gameObject"
                           + transform.name);
            return;
        }

        _offset = -transform.right * (_width - 1) * _spacingX / 2f
            -transform.forward *(_depth-1) * _spacingZ /2f;
        _angle = _useAngleOffset
            ? (transform.rotation * Quaternion.Euler(_angleOffset))
            : transform.rotation;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _depth; j++)
            {
                _gameObjects.Add(Instantiate(_prefab,
                    transform.position + _offset + i*_spacingX*transform.right 
                    + j*_spacingZ*transform.forward,
                    _angle));
            }
        }

        foreach (var gameObject in _gameObjects)
        {
            gameObject.transform.parent = transform;
        }

    }

    void OnDrawGizmos()
    {
        if (_useSkinnedMesh)
        {
            if (_skinnedMesh == null)
            {
                return;
            }
        }
        else
        {
            if (_meshFilter == null)
                return;
        }

        if (Application.isPlaying)
            return;
        if (_width < 1 || _depth < 1)
            return;

        Gizmos.color = _gridColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero,new Vector3(_width*_spacingX,
            _gizmoHeight,_depth*_spacingZ));

        _offset = Vector3.left * (_width - 1) * _spacingX / 2f
                  + Vector3.back * (_depth - 1) * _spacingZ / 2f;
        Gizmos.color = _cellColor;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _depth; j++)
            {
                Gizmos.DrawMesh( _useSkinnedMesh ? _skinnedMesh.sharedMesh :
                    _meshFilter.sharedMesh,
                    0,
                    _offset +i*_spacingX * Vector3.right 
                    + j*_spacingZ * Vector3.forward,
                    Quaternion.Euler(_angleOffset)
                );
            }
        }
    }
}

