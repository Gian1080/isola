// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/URP_Grass"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_GrassTexture("Grass Texture", 2D) = "white" {}
		_AlphaClipTreshold("Alpha Clip Treshold", Float) = 0.5
		_WindFoliageAmplitude("Wind Foliage Amplitude", Range( 0 , 1)) = 0
		_WindFoliageSpeed("Wind Foliage Speed", Range( 0 , 1)) = 0
		_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 1)) = 0
		_WindTrunkSpeed("Wind Trunk Speed", Range( 0 , 1)) = 0
		_GrassColor("Grass Color", Color) = (0.5264154,0.7264151,0.2158686,0)
		_HeightColor("Height Color", Color) = (0.4464056,0.6981132,0.1350124,0)
		_HeightStartGradient("Height Start Gradient", Range( 0 , 1)) = 0.1
		_HeightGradient("Height Gradient", Range( 0 , 1)) = 0.3
		_FlowerMainColor01("Flower Main Color 01", Color) = (1,0.9637499,0.759434,0)
		_FlowerInsideColor01("Flower Inside Color 01", Color) = (1,0.7789562,0.1273585,0)
		_FlowerMainColor02("Flower Main Color 02", Color) = (1,0.703345,0.1556604,0)
		_FlowerInsideColor02("Flower Inside Color 02", Color) = (1,0.9507267,0.6084906,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70108

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _GrassTexture;
			CBUFFER_START( UnityPerMaterial )
			float _WindTrunkSpeed;
			float _WindTrunkAmplitude;
			float _WindFoliageSpeed;
			float _WindFoliageAmplitude;
			float4 _GrassColor;
			float4 _HeightColor;
			float _HeightStartGradient;
			float _HeightGradient;
			float4 _GrassTexture_ST;
			float4 _FlowerMainColor02;
			float4 _FlowerMainColor01;
			float4 _FlowerInsideColor02;
			float4 _FlowerInsideColor01;
			float _AlphaClipTreshold;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float temp_output_17_0 = ( _TimeParameters.x * ( 2.0 * _WindTrunkSpeed ) );
				float4 appendResult58 = (float4(( ( sin( temp_output_17_0 ) * _WindTrunkAmplitude ) * v.ase_color.b ) , 0.0 , ( v.ase_color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_17_0 ) ) ) , 0.0));
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 appendResult27 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
				float2 panner42 = ( ( _TimeParameters.x * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult27.xy);
				float simplePerlin2D49 = snoise( panner42 );
				
				o.ase_texcoord6 = v.vertex;
				o.ase_texcoord7.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( appendResult58 + float4( ( simplePerlin2D49 * _WindFoliageAmplitude * v.ase_normal * v.ase_color.r ) , 0.0 ) ).rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float3 WorldNormal = normalize( IN.tSpace0.xyz );
				float3 WorldTangent = IN.tSpace1.xyz;
				float3 WorldBiTangent = IN.tSpace2.xyz;
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				#if SHADER_HINT_NICE_QUALITY
					WorldViewDirection = SafeNormalize( WorldViewDirection );
				#endif

				float clampResult14 = clamp( _HeightStartGradient , 0.0 , 0.5 );
				float4 lerpResult44 = lerp( _GrassColor , _HeightColor , saturate( ( ( IN.ase_texcoord6.xyz.y - clampResult14 ) / _HeightGradient ) ));
				float2 uv_GrassTexture = IN.ase_texcoord7.xy * _GrassTexture_ST.xy + _GrassTexture_ST.zw;
				float4 tex2DNode45 = tex2D( _GrassTexture, uv_GrassTexture );
				float2 uv016 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float smoothstepResult34 = smoothstep( 0.73 , 0.75 , ( uv016.y + 0.3 ));
				float4 lerpResult47 = lerp( _FlowerMainColor02 , _FlowerMainColor01 , smoothstepResult34);
				float4 lerpResult55 = lerp( float4( 0,0,0,0 ) , lerpResult47 , tex2DNode45.b);
				float4 lerpResult46 = lerp( _FlowerInsideColor02 , _FlowerInsideColor01 , smoothstepResult34);
				float4 lerpResult54 = lerp( float4( 0,0,0,0 ) , lerpResult46 , tex2DNode45.r);
				
				float3 Albedo = ( ( lerpResult44 * tex2DNode45.g ) + lerpResult55 + lerpResult54 ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = 0.0;
				float Occlusion = 1;
				float Alpha = tex2DNode45.a;
				float AlphaClipThreshold = _AlphaClipTreshold;
				float3 BakedGI = 0;

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal )));
				#else
					#if !SHADER_HINT_NICE_QUALITY
						inputData.normalWS = WorldNormal;
					#else
						inputData.normalWS = normalize( WorldNormal );
					#endif
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif
				
				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70108

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _GrassTexture;
			CBUFFER_START( UnityPerMaterial )
			float _WindTrunkSpeed;
			float _WindTrunkAmplitude;
			float _WindFoliageSpeed;
			float _WindFoliageAmplitude;
			float4 _GrassColor;
			float4 _HeightColor;
			float _HeightStartGradient;
			float _HeightGradient;
			float4 _GrassTexture_ST;
			float4 _FlowerMainColor02;
			float4 _FlowerMainColor01;
			float4 _FlowerInsideColor02;
			float4 _FlowerInsideColor01;
			float _AlphaClipTreshold;
			CBUFFER_END


			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			float3 _LightDirection;

			VertexOutput ShadowPassVertex( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float temp_output_17_0 = ( _TimeParameters.x * ( 2.0 * _WindTrunkSpeed ) );
				float4 appendResult58 = (float4(( ( sin( temp_output_17_0 ) * _WindTrunkAmplitude ) * v.ase_color.b ) , 0.0 , ( v.ase_color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_17_0 ) ) ) , 0.0));
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 appendResult27 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
				float2 panner42 = ( ( _TimeParameters.x * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult27.xy);
				float simplePerlin2D49 = snoise( panner42 );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( appendResult58 + float4( ( simplePerlin2D49 * _WindFoliageAmplitude * v.ase_normal * v.ase_color.r ) , 0.0 ) ).rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_GrassTexture = IN.ase_texcoord2.xy * _GrassTexture_ST.xy + _GrassTexture_ST.zw;
				float4 tex2DNode45 = tex2D( _GrassTexture, uv_GrassTexture );
				
				float Alpha = tex2DNode45.a;
				float AlphaClipThreshold = _AlphaClipTreshold;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70108

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _GrassTexture;
			CBUFFER_START( UnityPerMaterial )
			float _WindTrunkSpeed;
			float _WindTrunkAmplitude;
			float _WindFoliageSpeed;
			float _WindFoliageAmplitude;
			float4 _GrassColor;
			float4 _HeightColor;
			float _HeightStartGradient;
			float _HeightGradient;
			float4 _GrassTexture_ST;
			float4 _FlowerMainColor02;
			float4 _FlowerMainColor01;
			float4 _FlowerInsideColor02;
			float4 _FlowerInsideColor01;
			float _AlphaClipTreshold;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float temp_output_17_0 = ( _TimeParameters.x * ( 2.0 * _WindTrunkSpeed ) );
				float4 appendResult58 = (float4(( ( sin( temp_output_17_0 ) * _WindTrunkAmplitude ) * v.ase_color.b ) , 0.0 , ( v.ase_color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_17_0 ) ) ) , 0.0));
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 appendResult27 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
				float2 panner42 = ( ( _TimeParameters.x * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult27.xy);
				float simplePerlin2D49 = snoise( panner42 );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( appendResult58 + float4( ( simplePerlin2D49 * _WindFoliageAmplitude * v.ase_normal * v.ase_color.r ) , 0.0 ) ).rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_GrassTexture = IN.ase_texcoord2.xy * _GrassTexture_ST.xy + _GrassTexture_ST.zw;
				float4 tex2DNode45 = tex2D( _GrassTexture, uv_GrassTexture );
				
				float Alpha = tex2DNode45.a;
				float AlphaClipThreshold = _AlphaClipTreshold;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70108

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _GrassTexture;
			CBUFFER_START( UnityPerMaterial )
			float _WindTrunkSpeed;
			float _WindTrunkAmplitude;
			float _WindFoliageSpeed;
			float _WindFoliageAmplitude;
			float4 _GrassColor;
			float4 _HeightColor;
			float _HeightStartGradient;
			float _HeightGradient;
			float4 _GrassTexture_ST;
			float4 _FlowerMainColor02;
			float4 _FlowerMainColor01;
			float4 _FlowerInsideColor02;
			float4 _FlowerInsideColor01;
			float _AlphaClipTreshold;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float temp_output_17_0 = ( _TimeParameters.x * ( 2.0 * _WindTrunkSpeed ) );
				float4 appendResult58 = (float4(( ( sin( temp_output_17_0 ) * _WindTrunkAmplitude ) * v.ase_color.b ) , 0.0 , ( v.ase_color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_17_0 ) ) ) , 0.0));
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 appendResult27 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
				float2 panner42 = ( ( _TimeParameters.x * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult27.xy);
				float simplePerlin2D49 = snoise( panner42 );
				
				o.ase_texcoord2 = v.vertex;
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( appendResult58 + float4( ( simplePerlin2D49 * _WindFoliageAmplitude * v.ase_normal * v.ase_color.r ) , 0.0 ) ).rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float clampResult14 = clamp( _HeightStartGradient , 0.0 , 0.5 );
				float4 lerpResult44 = lerp( _GrassColor , _HeightColor , saturate( ( ( IN.ase_texcoord2.xyz.y - clampResult14 ) / _HeightGradient ) ));
				float2 uv_GrassTexture = IN.ase_texcoord3.xy * _GrassTexture_ST.xy + _GrassTexture_ST.zw;
				float4 tex2DNode45 = tex2D( _GrassTexture, uv_GrassTexture );
				float2 uv016 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float smoothstepResult34 = smoothstep( 0.73 , 0.75 , ( uv016.y + 0.3 ));
				float4 lerpResult47 = lerp( _FlowerMainColor02 , _FlowerMainColor01 , smoothstepResult34);
				float4 lerpResult55 = lerp( float4( 0,0,0,0 ) , lerpResult47 , tex2DNode45.b);
				float4 lerpResult46 = lerp( _FlowerInsideColor02 , _FlowerInsideColor01 , smoothstepResult34);
				float4 lerpResult54 = lerp( float4( 0,0,0,0 ) , lerpResult46 , tex2DNode45.r);
				
				
				float3 Albedo = ( ( lerpResult44 * tex2DNode45.g ) + lerpResult55 + lerpResult54 ).rgb;
				float3 Emission = 0;
				float Alpha = tex2DNode45.a;
				float AlphaClipThreshold = _AlphaClipTreshold;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 70108

			#pragma enable_d3d11_debug_symbols
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _GrassTexture;
			CBUFFER_START( UnityPerMaterial )
			float _WindTrunkSpeed;
			float _WindTrunkAmplitude;
			float _WindFoliageSpeed;
			float _WindFoliageAmplitude;
			float4 _GrassColor;
			float4 _HeightColor;
			float _HeightStartGradient;
			float _HeightGradient;
			float4 _GrassTexture_ST;
			float4 _FlowerMainColor02;
			float4 _FlowerMainColor01;
			float4 _FlowerInsideColor02;
			float4 _FlowerInsideColor01;
			float _AlphaClipTreshold;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float temp_output_17_0 = ( _TimeParameters.x * ( 2.0 * _WindTrunkSpeed ) );
				float4 appendResult58 = (float4(( ( sin( temp_output_17_0 ) * _WindTrunkAmplitude ) * v.ase_color.b ) , 0.0 , ( v.ase_color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_17_0 ) ) ) , 0.0));
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 appendResult27 = (float3(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z));
				float2 panner42 = ( ( _TimeParameters.x * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult27.xy);
				float simplePerlin2D49 = snoise( panner42 );
				
				o.ase_texcoord2 = v.vertex;
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( appendResult58 + float4( ( simplePerlin2D49 * _WindFoliageAmplitude * v.ase_normal * v.ase_color.r ) , 0.0 ) ).rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float clampResult14 = clamp( _HeightStartGradient , 0.0 , 0.5 );
				float4 lerpResult44 = lerp( _GrassColor , _HeightColor , saturate( ( ( IN.ase_texcoord2.xyz.y - clampResult14 ) / _HeightGradient ) ));
				float2 uv_GrassTexture = IN.ase_texcoord3.xy * _GrassTexture_ST.xy + _GrassTexture_ST.zw;
				float4 tex2DNode45 = tex2D( _GrassTexture, uv_GrassTexture );
				float2 uv016 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float smoothstepResult34 = smoothstep( 0.73 , 0.75 , ( uv016.y + 0.3 ));
				float4 lerpResult47 = lerp( _FlowerMainColor02 , _FlowerMainColor01 , smoothstepResult34);
				float4 lerpResult55 = lerp( float4( 0,0,0,0 ) , lerpResult47 , tex2DNode45.b);
				float4 lerpResult46 = lerp( _FlowerInsideColor02 , _FlowerInsideColor01 , smoothstepResult34);
				float4 lerpResult54 = lerp( float4( 0,0,0,0 ) , lerpResult46 , tex2DNode45.r);
				
				
				float3 Albedo = ( ( lerpResult44 * tex2DNode45.g ) + lerpResult55 + lerpResult54 ).rgb;
				float Alpha = tex2DNode45.a;
				float AlphaClipThreshold = _AlphaClipTreshold;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=17800
-1752;152;1704;723;2521.472;1146.965;1.455485;True;True
Node;AmplifyShaderEditor.RangedFloatNode;8;-2360.872,471.3632;Float;False;Property;_WindTrunkSpeed;Wind Trunk Speed;5;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;6;-2349.538,186.0736;Inherit;False;1821.23;666.407;Vertex offset using Blue Vertex Color channel;13;58;52;48;39;37;35;28;25;24;23;17;11;10;Wind Trunk;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2187.48,-1463.959;Float;False;Property;_HeightStartGradient;Height Start Gradient;8;0;Create;True;0;0;False;0;0.1;0.23;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;7;-2152.955,-1763.221;Inherit;False;1252.568;615.0753;;6;38;29;21;20;15;14;Height Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-1936.505,-1701.095;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;14;-1904.042,-1460.531;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;13;-2192.343,-1061.206;Inherit;False;1750.334;1123.101;;16;55;54;53;47;46;45;44;41;40;36;34;33;32;31;30;16;Grass & Flower Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-2259.286,259.1342;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;12;-2345.246,940.5809;Inherit;False;1653.539;798.5751;Vertex offset using Red Vertex Color channel base on panning noise;11;56;51;50;49;43;42;27;26;22;19;18;Wind Foliage;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-2073.674,451.6931;Inherit;False;2;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2189.753,1630.016;Float;False;Property;_WindFoliageSpeed;Wind Foliage Speed;3;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2012.515,-1311.068;Float;False;Property;_HeightGradient;Height Gradient;9;0;Create;True;0;0;False;0;0.3;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1795.931,450.5752;Float;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;4;0;Create;True;0;0;False;0;0;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;19;-2221.964,1460.594;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1918.641,327.341;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-2318.784,1077.569;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;-1711.365,-1478.194;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-2026.756,-232.3899;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-1751.359,-214.948;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;25;-1746.19,675.407;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;24;-1629.443,321.3241;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1883.128,1489.228;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1529.629,495.7152;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1990.577,1097.207;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;29;-1562.189,-1325.193;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;-2139.645,-1015.615;Float;False;Property;_GrassColor;Grass Color;6;0;Create;True;0;0;False;0;0.5264154,0.7264151,0.2158686,0;0.4392355,0.735849,0.2117299,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-2138.545,-853.415;Float;False;Property;_HeightColor;Height Color;7;0;Create;True;0;0;False;0;0.4464056,0.6981132,0.1350124,0;0.7004818,0.8867924,0.4141152,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;-1483.909,-456.4109;Float;False;Property;_FlowerMainColor01;Flower Main Color 01;10;0;Create;True;0;0;False;0;1,0.9637499,0.759434,0;1,0.9637498,0.759434,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;38;-1408.989,-1250.196;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-1193.347,-309.0695;Float;False;Property;_FlowerInsideColor02;Flower Inside Color 02;13;0;Create;True;0;0;False;0;1,0.9507267,0.6084906,0;1,0.9507267,0.6084906,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;34;-1507.377,-221.0811;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.73;False;2;FLOAT;0.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-1188.792,-137.6951;Float;False;Property;_FlowerInsideColor01;Flower Inside Color 01;11;0;Create;True;0;0;False;0;1,0.7789562,0.1273585,0;1,0.7789562,0.1273583,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;42;-1698.198,1045.394;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,2;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1448.942,292.8232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;-1485.041,-632.6556;Float;False;Property;_FlowerMainColor02;Flower Main Color 02;12;0;Create;True;0;0;False;0;1,0.703345,0.1556604,0;1,0.7033451,0.1556602,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1380.664,638.0371;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;37;-1251.056,416.4043;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;-1338.328,1293.361;Float;False;Property;_WindFoliageAmplitude;Wind Foliage Amplitude;2;0;Create;True;0;0;False;0;0;0.073;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;47;-1184.533,-473.695;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;43;-1342.713,1412.864;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;44;-904.9515,-999.3575;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;45;-1577.82,-864.9741;Inherit;True;Property;_GrassTexture;Grass Texture;0;0;Create;True;0;0;False;0;-1;None;9bf606c43682e9d488ea2aa0c944418f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1054.567,652.162;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;49;-1312.286,1037.082;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1007.484,307.0681;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;46;-927.0455,-114.6251;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;51;-1330.041,1549.532;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;54;-731.6466,-578.9875;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;-725.4856,-726.135;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-706.3346,-844.2744;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;58;-707.9425,561.9899;Inherit;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-857.8075,1270.003;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;57;-376.3539,-1391.145;Inherit;False;663;247;Do you want the normals to always be up ? PLug in Local Vertex Normal port;2;63;62;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-262.9182,114.8667;Float;False;Constant;_Null;Null;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;63;-53.87073,-1335.255;Inherit;False;World;Object;True;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;62;-240.8704,-1334.255;Float;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-300.7517,-574.1896;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-314.9473,321.1913;Inherit;False;Property;_AlphaClipTreshold;Alpha Clip Treshold;1;0;Create;True;0;0;False;0;0.5;1.98;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-146.0922,711.111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;SNB_Nature/URP_Grass;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;12;False;False;False;True;2;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;0;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;13;Workflow;1;Surface;0;  Blend;0;Two Sided;0;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;Vertex Position,InvertActionOnDeselection;1;0;6;False;True;True;True;True;True;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;False;True;1;False;-1;True;0;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;14;0;9;0
WireConnection;11;1;8;0
WireConnection;17;0;10;0
WireConnection;17;1;11;0
WireConnection;20;0;15;2
WireConnection;20;1;14;0
WireConnection;30;0;16;2
WireConnection;25;0;17;0
WireConnection;24;0;17;0
WireConnection;26;0;19;0
WireConnection;26;1;18;0
WireConnection;28;0;23;0
WireConnection;27;0;22;1
WireConnection;27;1;22;2
WireConnection;27;2;22;3
WireConnection;29;0;20;0
WireConnection;29;1;21;0
WireConnection;38;0;29;0
WireConnection;34;0;30;0
WireConnection;42;0;27;0
WireConnection;42;1;26;0
WireConnection;39;0;24;0
WireConnection;39;1;23;0
WireConnection;35;0;28;0
WireConnection;35;1;25;0
WireConnection;47;0;41;0
WireConnection;47;1;40;0
WireConnection;47;2;34;0
WireConnection;44;0;31;0
WireConnection;44;1;32;0
WireConnection;44;2;38;0
WireConnection;52;0;37;3
WireConnection;52;1;35;0
WireConnection;49;0;42;0
WireConnection;48;0;39;0
WireConnection;48;1;37;3
WireConnection;46;0;36;0
WireConnection;46;1;33;0
WireConnection;46;2;34;0
WireConnection;54;1;46;0
WireConnection;54;2;45;1
WireConnection;55;1;47;0
WireConnection;55;2;45;3
WireConnection;53;0;44;0
WireConnection;53;1;45;2
WireConnection;58;0;48;0
WireConnection;58;2;52;0
WireConnection;56;0;49;0
WireConnection;56;1;50;0
WireConnection;56;2;43;0
WireConnection;56;3;51;1
WireConnection;63;0;62;0
WireConnection;61;0;53;0
WireConnection;61;1;55;0
WireConnection;61;2;54;0
WireConnection;59;0;58;0
WireConnection;59;1;56;0
WireConnection;1;0;61;0
WireConnection;1;4;60;0
WireConnection;1;6;45;4
WireConnection;1;7;64;0
WireConnection;1;8;59;0
ASEEND*/
//CHKSM=289B563095DCFBFEE223C920C84CF462ED6E18D8