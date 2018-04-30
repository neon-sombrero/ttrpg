Shader "SeeThrough/Bumped Diffuse"
{
	Properties
	{
		_Color ("Main Colour", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[Normal] 
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_FadeDistance ("Fade Distance", Range(0,10)) = 7.0
		_FadePower ("Fade Power", Range(0,1)) = 0.4
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.8
	}
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 300
		
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		float4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _RandomNoise;
		float _FadeDistance;
		float _FadePower;
		uniform float4 _FadeData[10];
		uniform float _RandomVal;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
          	float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			
			o.Alpha = c.a * _Color.a;
			
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          	screenUV *= float2(10,10);
			half4 Noise = tex2D(_RandomNoise, screenUV);
          	screenUV *= float2(10,10);
			half4 Noise2 = tex2D(_RandomNoise, screenUV);
			
			float FinalMultiplier = 1;
			
			for(int i = 0; i < 10; i++)
			{
				float dist = distance(float3(_FadeData[i].x,_FadeData[i].y,_FadeData[i].z), IN.worldPos);
				
				if(dist <= _FadeData[i].w - _FadeDistance)
				{
					FinalMultiplier = 0;
				}
				else if(dist <= _FadeData[i].w)
				{
					float NormalisedMin = _FadeData[i].w - _FadeDistance;
					float NormalisedCurrent = dist - NormalisedMin;
					
					float NormalAmount = NormalisedCurrent/_FadeDistance;
					
					float FinalAmount = pow(NormalAmount,_FadePower)*Noise.g/Noise2.g;
					
					if(FinalAmount < FinalMultiplier)
						FinalMultiplier = FinalAmount;
				}
			}		
			o.Alpha = FinalMultiplier * _Color.a;
		}
		ENDCG
		
		Pass
        { 
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
 
			Fog {Mode Off}
			ZWrite On ZTest Less Cull Off
			Offset 1, 1
 
			CGPROGRAM
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			
			struct v2f 
			{ 
				V2F_SHADOW_CASTER; 
				float2 uv : TEXCOORD1;
			};
 
 
			v2f vert( appdata_full v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
 
			  return o;
			}
 
			float4 frag( v2f i ) : COLOR
			{
				//fixed4 texcol = tex2D( _MainTex, i.uv );
				clip( 1 );
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
        } 	
	} 
	FallBack "Transparent/Cutout/Bumped Diffuse"
}