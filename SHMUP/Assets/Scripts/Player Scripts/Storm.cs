using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum StormState
{
    Setting,
    Damaging
}

public class Storm : MonoBehaviour
{
    #region FIELDS
    public StormState stormState;
    public bool active;
    public GameObject stormPrefab;
    public GameObject stormCrosshair; 
    public float stormDamage = 40f;
    Camera cam;

    float settingTIme = 3f;
    Vector3 stormPosition = new Vector3(0, 0, 0);
    [SerializeField] Vector3 lockedPosition = new Vector3(0, 0, 0);
    Vector3 mousePos;
    [SerializeField] bool locationSet = false;

    [SerializeField] float stormTime = 0.3f;
    public bool stormFinished = false;
    bool instantiated = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        mousePos = cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            switch (stormState)
            {
                case StormState.Setting:
                    stormFinished = false;

                    if (!instantiated)
                    {
                        stormCrosshair = Instantiate(stormPrefab);
                        instantiated = true;
                    }

                    stormCrosshair.GetComponent<SpriteRenderer>().color = Color.blue;

                    if (!locationSet || settingTIme > 3f)
                    {
                        settingTIme -= Time.deltaTime;
                        mousePos = cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
                        stormPosition = mousePos;
                        stormCrosshair.transform.position = new Vector3(stormPosition.x, stormPosition.y, 0);
                    }

                    if(locationSet || settingTIme <= 0)
                    {
                        lockedPosition = new Vector3(stormPosition.x, stormPosition.y, 0);
                        stormCrosshair.transform.position = lockedPosition;
                        stormState = StormState.Damaging;
                    }
                    break;

                case StormState.Damaging:
                    if(stormTime > 0)
                    {
                        stormCrosshair.transform.position = lockedPosition;
                        stormCrosshair.GetComponent<SpriteRenderer>().color = Color.black;
                        stormTime -= Time.deltaTime;
                    } else
                    {
                        stormFinished = true;
                    }
                    break;
            }
        } else
        {
            // Reset variables
            settingTIme = 3f;
            locationSet = false;
            stormPosition = Vector3.zero;
            lockedPosition = Vector3.zero;
            stormFinished = false;
            instantiated = false;
            stormTime = 0.3f;
            stormState = StormState.Setting;
            Destroy(stormCrosshair);
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (active)
        {
            if (stormState == StormState.Setting)
            {
                if (settingTIme > 3f || !locationSet)
                {
                    stormPosition = mousePos;
                    stormCrosshair.transform.position = stormPosition;
                    locationSet = true;
                }
            }
        }
    }
}
