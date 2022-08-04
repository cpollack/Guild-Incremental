using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Rounded Edge")]
public class RoundedEdgeModifier : ProceduralImageModifier
{
	[SerializeField] private float borderRadius = 0;
	public float BorderRadius
	{
		get
		{
			return borderRadius;
		}
		set
		{
			borderRadius = value;
			_ProceduralImage.SetVerticesDirty();
		}
	}

	[SerializeField] private bool topLeft = true;
	[SerializeField] private bool topRight = true;
	[SerializeField] private bool bottomLeft = true;
	[SerializeField] private bool bottomRight = true;

	#region implemented abstract members of ProceduralImageModifier
	public override Vector4 CalculateRadius(Rect imageRect)
	{		
		return new Vector4(topLeft ? borderRadius : 0, topRight ? borderRadius : 0, bottomLeft ? borderRadius : 0, bottomRight ? borderRadius : 0);
	}
	#endregion
}
