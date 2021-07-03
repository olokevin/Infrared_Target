using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

public enum CustomMoveType { UpDown, LeftRight, Rebound, Flash, Random }
public enum CustomRotateType { Clockwise, Anticlockwise, Random }
public enum CustomScaleType { Fixed, Random }
public enum CustomOptin { Revise, Create }

public class ShowCustomGoalItem
{
    public string Name { get; set; }
    public ImageSource ShowImage { get; set; }
}

public class CustomGoalInfo
{
    public string name;
    public ImageInfo image;
    public MoveInfo move;
    public RotateInfo rotate;
    public ScaleInfo scale;
    public AliveInfo alive;
    public OtherInfo other;
}

public struct ImageInfo
{
    public string path;
    public ImageSource source;
}

public struct MoveInfo
{
    public bool enabel;
    public CustomMoveType type;
    public int speed;
}

public struct RotateInfo
{
    public bool enabel;
    public CustomRotateType type;
    public int angle;
}

public struct ScaleInfo
{
    public CustomScaleType type;
    public float fixValue;
    public float minValue;
    public float maxValue;
}

public struct AliveInfo
{
    public int createSpan;
    public int createCount;
    public int aliveTime;
    public int toplimit;
    public bool destoryWhenHit;
}

public struct OtherInfo
{

}
