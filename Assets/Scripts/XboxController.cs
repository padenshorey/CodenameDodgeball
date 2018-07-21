public class XboxController
{
    public int controllerId;

    public string a = "A_";
    public string b = "B_";
    public string x = "X_";
    public string y = "Y_";

    public string lt = "TriggersL_";
    public string rt = "TriggersR_";
    public string lb = "LB_";
    public string rb = "RB_";

    public string start = "Start_";
    public string back = "Back_";

    public string joyLeftHori = "L_XAxis_";
    public string joyLeftVert = "L_YAxis_";
    public string joyLeftClick = "LS_";
    public string joyRightHori = "R_XAxis_";
    public string joyRightVert = "R_YAxis_";
    public string joyRightClick = "RS_";

    public string dpadVert = "Dpad_YAxis_";
    public string dpadHori = "Dpad_XAxis_";

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    public string dpadUp = "Dpad_Up_";
    public string dpadDown = "Dpad_Down_";
    public string dpadLeft = "Dpad_Left_";
    public string dpadRight = "Dpad_Right_";

    public string xboxButton = "XboxButton_";
#endif

    public XboxController(int cId)
    {
        controllerId = cId;
        string id = controllerId.ToString();
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        id = "MAC_" + id;
#endif
        a += id;
        b += id;
        x += id;
        y += id;

        lt += id;
        rt += id;
        lb += id;
        rb += id;

        start += id;
        back += id;

        joyLeftHori += id;
        joyLeftVert += id;
        joyLeftClick += id;
        joyRightHori += id;
        joyRightVert += id;
        joyRightClick += id;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        dpadUp += id;
        dpadDown += id;
        dpadLeft += id;
        dpadRight += id;
        xboxButton += id;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        dpadVert += id;
        dpadHori += id;
#endif
    }
}
