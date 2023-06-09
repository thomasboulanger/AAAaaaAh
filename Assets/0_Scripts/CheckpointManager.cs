//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections;
using UnityEngine;

/// <summary>
/// Script that manage the evolution of player's progression along the level and can replace
/// the player to the precedent checkpoint or to the start of the level
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameEvent fadeOutEvent;
    [SerializeField] private GameEvent onOverrideGrabEvent;
    [SerializeField] private GameEvent onClearFruitLists;
    [SerializeField] private GameEvent onPlayerPressPause;

    private Transform[] _limbsTransforms = new Transform[4];
    private Transform[] _virtualTransforms = new Transform[4];
    private Vector3[] _tempLimbsOffsetPosition = new Vector3[4];
    private Vector3 _firstCheckpoint;
    private Vector3 _latestCheckpoint;
    private int _latestCheckpointIndex;
    private bool _overridingInputs;

    public void Init(Transform[] limbTransformArray, Transform[] virtualTransformArray)
    {
        _limbsTransforms = limbTransformArray;
        _virtualTransforms = virtualTransformArray;
        _firstCheckpoint = Vector3.zero;
        _latestCheckpoint = Vector3.zero;
        _latestCheckpointIndex = 0;
    }

    private void Update()
    {
        if (_overridingInputs) onOverrideGrabEvent.Raise(this, false, null, null);
    }

    public void SetNewCheckpoint(Vector3 pos, int checkpointIndex)
    {
        if (_latestCheckpointIndex > checkpointIndex) return;
        _latestCheckpointIndex = checkpointIndex;
        _latestCheckpoint = pos;
        if (_firstCheckpoint != Vector3.zero) return;
        _firstCheckpoint = pos;
    }

    public void OnReturnToLastCheckpoint(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        //call screen fade out event
        fadeOutEvent.Raise(this, true, null, null);
        StartCoroutine(DelayCoroutineLast(_latestCheckpoint));
    }

    public void OnReturnToFirstCheckpoint(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        //call screen fade out event
        fadeOutEvent.Raise(this, true, null, null);
        StartCoroutine(DelayCoroutineFirst(_firstCheckpoint));
    }

    IEnumerator DelayCoroutineLast(Vector3 positionToMoveTo)
    {
        yield return new WaitForSeconds(.5f);
        //iterate on all limbs to get their offset with character body
        for (int i = 0; i < _tempLimbsOffsetPosition.Length; i++)
            _tempLimbsOffsetPosition[i] = _limbsTransforms[i].position - transform.position;

        //move the body to last checkpoint position
        transform.position = positionToMoveTo;

        //iterate on all limbs, move limb position to body position + offset
        for (int i = 0; i < _limbsTransforms.Length; i++)
        {
            _limbsTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            _virtualTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
        }

        _overridingInputs = true;
        yield return new WaitForSeconds(.3f);
        _overridingInputs = false;

        onPlayerPressPause.Raise(this, 0, false, null);
        PlayerInputsScript.InGamePauseButton = false;
    }
    IEnumerator DelayCoroutineFirst(Vector3 positionToMoveTo)
    {
        yield return new WaitForSeconds(.5f);
        //iterate on all limbs to get their offset with character body
        for (int i = 0; i < _tempLimbsOffsetPosition.Length; i++)
            _tempLimbsOffsetPosition[i] = _limbsTransforms[i].position - transform.position;
        
        onPlayerPressPause.Raise(this, 0, false, null);
        PlayerInputsScript.InGamePauseButton = false;
        
        //reload the level
        GameObject.Find("GameManager").GetComponent<GameManager>().LoadLevel();

        //reset checkpoint values
        _latestCheckpointIndex = 0;
        _latestCheckpoint = _firstCheckpoint;

        //move the body to last checkpoint position
        transform.position = positionToMoveTo;

        //iterate on all limbs, move limb position to body position + offset
        for (int i = 0; i < _limbsTransforms.Length; i++)
        {
            _limbsTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
            _virtualTransforms[i].position = transform.position + _tempLimbsOffsetPosition[i];
        }

        onClearFruitLists.Raise(this, null, null, null);
        
        _overridingInputs = true;
        yield return new WaitForSeconds(.3f);
        _overridingInputs = false;
    }
}