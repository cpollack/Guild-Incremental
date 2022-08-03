using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier {
	public float borderRadius = 0;

	#region implemented abstract members of ProceduralImageModifier
	public override Vector4 CalculateRadius (Rect imageRect){
		float min = Mathf.Min(imageRect.width, imageRect.height);
		//float r = Mathf.Min (imageRect.width,imageRect.height)*0.5f;
		float perc = 1f - ((min - borderRadius) / min);
		float r = Mathf.Min(imageRect.width, imageRect.height) * perc;
		//return new Vector4 (r,r,r,r);

		return new Vector4(borderRadius, borderRadius, borderRadius, borderRadius);
	}
	#endregion
}
