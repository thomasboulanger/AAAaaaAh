//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System;
using UnityEngine;

/// <summary>
/// Script that manage the evolution of player's progression along the level and can replace
/// the player to the precedent checkpoint or to the start of the level
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameEvent fadeOutEvent;
    [SerializeField] private GameEvent onLimbGrabEvent;

    private Transform[] _limbsTransforms = new Transform[4];
    private Transform[] _virtualTransforms = new Transform[4];
    private Vector3[] _tempLimbsOffsetPosition = new Vector3[4];
    private Vector3 _firstCheckpoint;
    private Vector3 _latestCheckpoint;
    private int _latestCheckpointIndex;

    public void Init(Transform[] limbTransformArray, Transform[] virtualTransformArray)
    {
        _limbsTransforms = limbTransformArray;
        _virtualTransforms = virtualTransformArray;
        _firstCheckpoint = Vector3.zero;
        _latestCheckpoint = Vector3.zero;
        _latestCheckpointIndex = 0;
    }

    public void SetNewCheckpoint(Vector3 pos, int checkpointIndex)
    {
        if (_latestCheckpointIndex > checkpointIndex) return;
        _latestCheckpointIndex = checkpointIndex;
        _latestCheckpoint = pos;
        if (_firstCheckpoint != Vector3.zero) return;
        _firstCheckpoint = pos;
    }

    public void ReturnToLastCheckpoint()
    {
        fadeOutEvent.Raise(this, true, null, null);
        for (int i = 0; i < _tempLimbsOffsetPosition.Length; i++)
            _tempLimbsOffsetPosition[i] = _limbsTransforms[i].position - transform.position;
        transform.position = _latestCheckpoint;
        for (int i = 0; i < _limbsTransforms.Length; i++)
        {
            _limbsTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            _virtualTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            onLimbGrabEvent.Raise(this, false, 5, i);
        }
    }

    public void ReturnToFirstCheckpoint()
    {
        fadeOutEvent.Raise(this, true, null, null);
        for (int i = 0; i < _tempLimbsOffsetPosition.Length; i++)
            _tempLimbsOffsetPosition[i] = _limbsTransforms[i].position - transform.position;
        transform.position = _firstCheckpoint;
        for (int i = 0; i < _limbsTransforms.Length; i++)
        {
            _limbsTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            _virtualTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            onLimbGrabEvent.Raise(this, false, 5, i);
        }
    }
}