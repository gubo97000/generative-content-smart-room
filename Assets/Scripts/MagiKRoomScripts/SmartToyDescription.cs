using System.Collections.Generic;

public class Actuators
{
    public List<LightState> lights;
    public List<MotorDescription> motors;
    public Sounds sounds;
}

public class ButtonDescription
{
    public string Id;
    public bool Press;
}

public class CinematicComponent
{
    public float X;
    public float Y;
    public float Z;
}

public class CinematicState
{
    public CinematicComponent accelerometer;
    public CinematicComponent gyroscope;
    public CinematicComponent position;
    public string Id;
}

public class DeviceState
{
    public int battery;
    public int freeHeap;
    public WifiConfiguration network;
    public int streamFreq;
    public int uptime;
}

public class EffectState
{
    public string EffectName;
    public int Id;
    public List<string> Target;
}

public class GenericInformation
{
    public DeviceState deviceState;
    public Information information;
}

public class Information
{
    public string board;
    public string deviceModel;
    public string id;
    public string mdnsAddress;
    public string mdnsService;
    public string productId;
    public SoftwareInformation softwareVersion;
}

public class LightState
{
    public int Brightness;
    public string Color;
    public string Id;
}

public class MotorDescription
{
    public string Direction;
    public string Id;
    public string Position;
    public float Speed;
}

public class RfidDescription
{
    public string Code;
    public string Id;
}

public class Sensors
{
    public List<ButtonDescription> buttons;
    public List<ButtonDescription> capacitives;
    public List<CinematicState> kinematics;
    public List<RfidDescription> rfids;
}

public class SmartToyDescription
{
    public Actuators actuators;
    public List<EffectState> effects;
    public Sensors sensors;
    public string Id;
    public GenericInformation Information;
    public string Ip;
    public string Name;
}

public class SoftwareInformation
{
    public string arduinoCore;
    public string compiledAt;
    public string espSdk;
    public string smartifier;
}

public class SoundEmittersDescription
{
    public List<SoundEmittersState> ids;
    public List<string> sounds;
}

public class SoundEmittersState
{
    public string Id;
    public bool Repeat;
    public string Trackname;
    public int Volume;
}

public class Sounds
{
    public SoundEmittersDescription buzzers;
    public SoundEmittersDescription speakers;
}

public class WifiConfiguration
{
    public string[] availableCredentials;
    public string ip;
    public string ssid;
}