using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenuManager : MonoBehaviour
{
    private static AudioMenuManager instance;
    public static AudioMenuManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(Instance.gameObject);
        instance = this;

        DontDestroyOnLoad(this.gameObject);

    }

    //RTPCs

    [SerializeField] private AK.Wwise.RTPC _sfxVolumeRTPC;
    [SerializeField]
    [Range(0f, 100f)]
    private float _sfxVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _musicVolumeRTPC;
    [SerializeField]
    [Range(0f, 100f)]
    private float _musicVolume = 100f;

    [SerializeField] AK.Wwise.RTPC _demonVolumeRTPC;
    [SerializeField]
    [Range(0f, 100f)]
    private float _demonVolume = 100f;

  //  [SerializeField] public AK.Wwise.RTPC _listenMusicRTPC;


    private void Start()
    {
        _demonVolume = 100f;
        _demonVolumeRTPC.SetGlobalValue(_demonVolume);

        _sfxVolume = 100f;
        _sfxVolumeRTPC.SetGlobalValue(_demonVolume);

        _musicVolume = 100f;
        _musicVolumeRTPC.SetGlobalValue(_demonVolume);


        _musicVolumeRTPC.GetValue(gameObject);


      

        // StartCoroutine(ExampleCoroutine());
        //pour test changement de volume

        //AudioManager.Instance.MenuMusic(true);   
    }

    public float _musicVolumeStorage
    {
        //set music volume RTPC to new value acquired by the getter/setter.
        get => _musicVolume;
        set
        {
            _musicVolume = value;
            _musicVolumeRTPC.SetGlobalValue(value);
        }
    }
    public float _fsxVolumeStorage
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = value;
            _sfxVolumeRTPC.SetGlobalValue(value);
        }
    }
    public float _demonVolumeStorage
    {
        //set music volume RTPC to new value acquired by the getter/setter.
        get => _demonVolume;
        set
        {
            _demonVolume = value;
            _demonVolumeRTPC.SetGlobalValue(value);
        }
    }


    /*  TEST DE CHANGEMENT DE VOLUME DU BUS DEMON AU BOUT DE 3 SECONDES   
      IEnumerator ExampleCoroutine()
     {
         _demonVolume = 50f;
         _demonVolumeRTPC.SetGlobalValue(_demonVolume);
         Debug.Log(_demonVolume);

         yield return new WaitForSeconds(3);

         _demonVolume = 100f;
         _demonVolumeRTPC.SetGlobalValue(_demonVolume);
         Debug.Log(_demonVolume);
     }
    */

}
