using UnityEngine;

public class AudioMenuManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.RTPC _sfxVolumeRTPC;
    [SerializeField] [Range(0f, 100f)] private float _sfxVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _musicVolumeRTPC;
    [SerializeField] [Range(0f, 100f)] private float _musicVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _demonVolumeRTPC;
    [SerializeField] [Range(0f, 100f)] private float _demonVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _ambianceVolumeRTPC;
    [SerializeField] [Range(0f, 100f)] private float _ambianceVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _moucheVolumeRTPC;
    [SerializeField] [Range(0f, 100f)] private float _moucheVolume = 100f;


    //  [SerializeField] public AK.Wwise.RTPC _listenMusicRTPC;
    private void Start()
    {
        _demonVolume = 100f;
        _demonVolumeRTPC.SetGlobalValue(_demonVolume);

        _sfxVolume = 100f;
        _sfxVolumeRTPC.SetGlobalValue(_sfxVolume);

        _musicVolume = 100f;
        _musicVolumeRTPC.SetGlobalValue(_musicVolume);

        _ambianceVolume = 100f;
        _ambianceVolumeRTPC.SetGlobalValue(_ambianceVolume);

        _moucheVolume = 100f;
        _moucheVolumeRTPC.SetGlobalValue(_moucheVolume);
    }

    public void OnChangeMusicVolume(Component sender, object data, object unUsed2, object unUsed3) =>
        _musicVolumeRTPC.SetGlobalValue((int) data * 1.1f);

    public void OnChangeSFXVolume(Component sender, object data, object unUsed2, object unUsed3) =>
        _sfxVolumeRTPC.SetGlobalValue((int) data * 1.1f);

    public void OnChangeEntitiesVolume(Component sender, object data, object unUsed2, object unUsed3)
    {
        _demonVolumeRTPC.SetGlobalValue((int) data * 1.1f);
        _moucheVolumeRTPC.SetGlobalValue((int) data * 1.1f);
    }

    #region not deleted in case of, but probably to delete

    // public float _musicVolumeStorage
    // {
    //     //set music volume RTPC to new value acquired by the getter/setter.
    //     get => _musicVolume;
    //     set
    //     {
    //         _musicVolume = value;
    //         _musicVolumeRTPC.SetGlobalValue(value);
    //     }
    // }
    //
    // public float _sfxVolumeStorage
    // {
    //     get => _sfxVolume;
    //     set
    //     {
    //         _sfxVolume = value;
    //         _sfxVolumeRTPC.SetGlobalValue(value);
    //     }
    // }
    //
    // public float _demonVolumeStorage
    // {
    //     //set music volume RTPC to new value acquired by the getter/setter.
    //     get => _demonVolume;
    //     set
    //     {
    //         _demonVolume = value;
    //         _demonVolumeRTPC.SetGlobalValue(value);
    //     }
    // }
    //
    // public float _ambianceVolumeStorage
    // {
    //     //set music volume RTPC to new value acquired by the getter/setter.
    //     get => _ambianceVolume;
    //     set
    //     {
    //         _ambianceVolume = value;
    //         _ambianceVolumeRTPC.SetGlobalValue(value);
    //     }
    // }
    //
    // public float __moucheVolumeStorage
    // {
    //     //set music volume RTPC to new value acquired by the getter/setter.
    //     get => _moucheVolume;
    //     set
    //     {
    //         _moucheVolume = value;
    //         _moucheVolumeRTPC.SetGlobalValue(value);
    //     }
    // }

    #endregion
}