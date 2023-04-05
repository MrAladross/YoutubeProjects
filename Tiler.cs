using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Tiler : MonoBehaviour
{
    [SerializeField] private int _width = 1;
    [SerializeField] private int _depth = 1;
    [SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();

    [SerializeField] private bool _useSkinnedMesh = false;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private SkinnedMeshRenderer _skinnedMesh;

    [SerializeField] private GameObject _prefab;

    [SerializeField] private float _spacingX=1;
    [SerializeField] private float _spacingZ = 1;
    [SerializeField] private float _height = 1;
    
    [SerializeField] private Color _gridColor = Color.blue;
    [SerializeField] Color _cellColor = Color.green;
    
    [SerializeField] Vector3 _angleOffset =Vector3.zero;
    [SerializeField] private bool _useAngleOffset = true;

    private Vector3 _offset;
    private Quaternion _angle;
    
    
    void Awake()
    {
        if (_prefab == null)
        {
            Debug.LogError("You forgot to assign a prefab to Tiler on gameObject " + transform.name);
            return;
        }

        _offset = transform.right * (_width - 1) * _spacingX / 2f 
            -transform.forward *(_depth-1) *_spacingZ /2f;
        _angle = _useAngleOffset ? (transform.rotation * Quaternion.Euler(_angleOffset)) : transform.rotation;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _depth; j++)
            {
                _gameObjects.Add(Instantiate(_prefab,
                    transform.position + _offset - i * _spacingX *transform.right + j * _spacingZ *transform.forward,
                    _angle
                    ));
            }
            
        }


    }

    void OnDrawGizmos()
    {
        //avoid drawing gizmos during playmode
        if (Application.isPlaying)
            return;
        //avoid Drawing gizmos until the prefab has been set
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
            {
                return;
            }
        }

        if (_width < 1 || _depth < 1)
            return;
        
        //Draw a cube at the transform position to show the bounds of the grid
        Gizmos.color = _gridColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width*_spacingX,_height,_depth*_spacingZ));
        
        //Calculate half extents to center the grid around the current transform
        _offset = Vector3.right * (_width - 1) * _spacingX / 2f - Vector3.forward * (_depth-1) * _spacingZ /2f;
        
        //Loop through our grid with a new color
        Gizmos.color = _cellColor;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _depth; j++)
            {
                Gizmos.DrawMesh( _useSkinnedMesh ? _skinnedMesh.sharedMesh : _meshFilter.sharedMesh,
                    0,
                    _offset + i*_spacingX*Vector3.left + j * _spacingZ * Vector3.forward,
                    quaternion.Euler(_angleOffset)
                    
                    );
            }
        }

    }
}
