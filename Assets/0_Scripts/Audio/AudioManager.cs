using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("Bank")] [SerializeField] private AK.Wwise.Bank mainSb;

    [Space] [Header("Select event section")] [SerializeField]
    private AK.Wwise.Event crieAndGroml;

    [SerializeField] private AK.Wwise.Event inspiration;
    [SerializeField] private AK.Wwise.Event expiration;
    [SerializeField] private AK.Wwise.Event kompa1;
    [SerializeField] private AK.Wwise.Event kompa2;
    [SerializeField] private AK.Wwise.Event kompa3;
    [SerializeField] private AK.Wwise.Event kompa4;
    [SerializeField] private AK.Wwise.Event menu_music;
    [SerializeField] private AK.Wwise.Event playAmbJungle;
    [SerializeField] private AK.Wwise.Event playAmbCascade;
    [SerializeField] private AK.Wwise.Event playAmbVillage;
    [SerializeField] private AK.Wwise.Event initMenuEvent;
    [SerializeField] private AK.Wwise.Event grab_arm_01;
    [SerializeField] private AK.Wwise.Event grab_arm_02;
    [SerializeField] private AK.Wwise.Event grab_arm_03;
    [SerializeField] private AK.Wwise.Event grab_arm_04;
    [SerializeField] private AK.Wwise.Event throwTruelle;
    [SerializeField] private AK.Wwise.Event truelleHitUI;
    [SerializeField] private AK.Wwise.Event rots;
    [SerializeField] private AK.Wwise.Event charlineBourree;


    [SerializeField] private AK.Wwise.Event truelleHitJoystick;
    /*[Header("Cinematic")]
    [SerializeField] private AK.Wwise.Event book_fall_whoosh;
    [SerializeField] private AK.Wwise.Event book_falling_01;
    [SerializeField] private AK.Wwise.Event book_falling_02;
    [SerializeField] private AK.Wwise.Event book_falling_03;
    [SerializeField] private AK.Wwise.Event demon_pop;
    [SerializeField] private AK.Wwise.Event door_open_bar;
    [SerializeField] private AK.Wwise.Event door_open_sorcier;
    */


    [Space] [Header("Select RTPC")] public AK.Wwise.RTPC listenMusicRtpc;
    [SerializeField] private AK.Wwise.RTPC listenScreamRtpc;

    // [SerializeField] private AK.Wwise.RTPC listenRespiRtpc;
    // [SerializeField] private AK.Wwise.RTPC listenInspiRtpc;
    // [SerializeField] private AK.Wwise.RTPC listenExpiRtpc;
    [SerializeField] private AK.Wwise.RTPC distanceDemon;
    [SerializeField] private AK.Wwise.RTPC musicIntensity;

    [Space] [Header("SELECT SWITCHS")] [Header("Grab Surface Switch Group")] [SerializeField]
    AK.Wwise.Switch grab_wood;

    [SerializeField] AK.Wwise.Switch grab_rock;
    [SerializeField] AK.Wwise.Switch grab_fruit;

    [Space] [Header("Reference to obj to initialize at start")] [SerializeField]
    private RTPCMeterCriRespi rtpcMeterCriRespi;

    [SerializeField] private BlendShapesAnim blendShapesAnim;
    [SerializeField] private RTPClistenerTest rtpClistenerTest;
    [SerializeField] private RTPCMeterInspiExpi rtpcMeterInspiExpi;

    private GameObject _player; // decommenter les lignes qui font refs
    private GameObject _demon;

    float randomNumberMusic;

    [Header("Reference scene")]
    [SerializeField] private GameObject ParpaingSieste;
    [SerializeField] private GameObject ParpaingBourre;

    //private uint playingID;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //mainSb.Load(true);
    }

    private void Start() => Init();
    public void MainSBLoad() => mainSb.Load(true);
    public void Init()
    {
        //rtpcMeterInspiExpi.RawAmplitudeScream = listenScreamRtpc;
        //rtpcMeterCriRespi.listenScreamRtpcAmplitudeValue = listenScreamRtpc;
        //rtpcMeterCriRespi.listenRespiRtpcAmplitudeValue = listenExpiRtpc;
        CriAndGroml(blendShapesAnim.gameObject);
        //rtpClistenerTest.listenMusicRtpcAmplitudeValue = listenMusicRtpc;
        //rtpcMeterInspiExpi.listenInspiRtpcAmplitudeValue = listenInspiRtpc;
        //rtpcMeterInspiExpi.listenExpiRtpcAmplitudeValue = listenExpiRtpc;

        // _player = GameObject.FindGameObjectWithTag("Player");
        // _demon = GameObject.FindGameObjectWithTag("Demon");
    }

    

    private void Update()
    {
        //demon sound modifier :
        //mettre la logique de bruit de demon avec un vector3.Distance(_player,_demon)

        //distanceDemon.SetGlobalValue(Vector3.Distance(_player.transform.position, _demon.transform.position));

        //background music modifier :
        //avancement dans le niveau peut etre fait comme au dessu entre le demon (a la fin) et le player
    }


    public void TestIDEvent()
    {
        AkSoundEngine.PostEvent("Play_Kompa1", gameObject);
        //Debug.Log(IsEventPlayingOnGameObject("Play_Kompa1", gameObject));
        //Debug.Log(IsEventPlayingOnGameObject("Play_Kompa2", gameObject));
        Debug.Log("gggg");
    }


    /* public static bool IsEventPlayingOnGameObject(string eventName, GameObject gom)
      {
 
             eventName = "Play_Kompa1";
             uint testEventId = AkSoundEngine.GetIDFromString(eventName);
 
             uint count = (uint)playingIds.Length;
             AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(gom, ref count, playingIds);
 
             for (int i = 0; i < count; i++)
             {
                 uint playingId = playingIds[i];
                 uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);
 
                 if (eventId == testEventId)
                     return true;
             }
 
             return false;
 
      }*/


    //grab sound modifier :
    public void OnGrabSoundEvent(Component sender, object whatIsGrabbed, object limbID, object unUsed)
    {
        //whatIsGrabbed
        //0 -> environment
        //1 -> fruit
        //2 -> plate (if there is one)

        //faire ton code ici, ne pas oublier de mettre (int) devant limbID et whatIsGrabbed
        if ((int) whatIsGrabbed == 0) grab_wood.SetValue(gameObject);
        if ((int) whatIsGrabbed == 1) grab_fruit.SetValue(gameObject);
        if ((int) whatIsGrabbed == 2) grab_rock.SetValue(gameObject);
        //Id du membre
        if ((int) limbID == 0) grab_arm_01.Post(gameObject);
        if ((int) limbID == 1) grab_arm_02.Post(gameObject);
        if ((int) limbID == 2) grab_arm_03.Post(gameObject);
        if ((int) limbID == 3) grab_arm_04.Post(gameObject);

        if (whatIsGrabbed is not int) return;
        if (limbID is not int) return;
    }

    //character sound modifier :
    //a implementer

    //wiggle sound modifier :
    public void OnWiggleLimb(Component sender, object wiggleValue, object unUsed1, object unUsed2)
    {
        if (wiggleValue is not float) return;

        //faire ton code ici, ne pas oublier de mettre (float) devant wiggleValue
        //wiggleValue float entre 0 et 1
    }

    //UI sound modifier
    public void OnThrowTruelle(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        throwTruelle.Post(gameObject);
    }

    public void OnTruelleHitUI(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        truelleHitUI.Post(gameObject);
    }

    public void OnTruelleHitJoysitck(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        truelleHitJoystick.Post(gameObject);
    }

    //start game
    public void InitializeMenuSounds()
    {
        //stop all sounds then play the intro music
        initMenuEvent.Post(gameObject);
        //menu musique
        menu_music.Post(gameObject);

    }


    public void SetupLevelMucsic()
    {
        menu_music.Stop(gameObject);

        randomNumberMusic = Random.Range(0f, 1f);
        switch (randomNumberMusic)
        {
            case < 0.25f:
                kompa1.Post(gameObject);
                break;
            case < 0.50f:
                kompa2.Post(gameObject);
                break;
            case < 0.75f:
                kompa3.Post(gameObject);
                break;
            default:
                kompa4.Post(gameObject);
                break;
        }
    }

    public void LaunchAmbianSounds()
    {
        playAmbJungle.Post(gameObject);
        charlineBourree.Post(ParpaingSieste);
        rots.Post(ParpaingBourre);

    }

    public void AmbVillage()
    {
        playAmbVillage.Post(gameObject);
    }


    public float GetScreamRTPCValue(GameObject go)
    {
        return listenScreamRtpc.GetValue(go);
    }

    /* public void MenuMusic(bool play)
     {
         if (play) kompa1.Post(gameObject);
         else kompa1.Stop(gameObject);
     }
    */

    public void InspirationSound(GameObject go) => inspiration.Post(go);
    public void ExpirationSound(GameObject go) => expiration.Post(go);
    public void CriAndGroml(GameObject go) => crieAndGroml.Post(go);

    //cinematic Sounds
}