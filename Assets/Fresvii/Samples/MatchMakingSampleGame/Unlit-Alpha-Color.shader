Shader "Unlit/Transparent Color" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	
	Pass
		{
			Cull Back
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			//Offset -1, -1
			//ColorMask RGB
			//AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			//ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				constantColor[_Color]
				Combine Texture * constant, texture * constant
			}
		}
	}
}
