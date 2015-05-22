// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/TransparentAlphaMap" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_AlphaTex("Alpha Map(RGBA)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

	LOD 100
	
	ZWrite Off
	
	ZTest Always

	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Lighting Off		
		SetTexture [_MainTex]
		 { 
				constantColor[_Color]
				Combine Texture * constant, texture * constant
		 } 
		SetTexture [_AlphaTex] { combine previous * texture }
	}
}
}
