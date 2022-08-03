using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Rounded Edge")]
public class RoundedEdge : ProceduralImageModifier
{
	public float borderRadius = 0;
	public bool topLeft = true;
	public bool topRight = true;
	public bool bottomLeft = true;
	public bool bottomRight = true;

	#region implemented abstract members of ProceduralImageModifier
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return new Vector4(topLeft ? borderRadius : 0, topRight ? borderRadius : 0, bottomLeft ? borderRadius : 0, bottomRight ? borderRadius : 0);
	}
	#endregion
}
