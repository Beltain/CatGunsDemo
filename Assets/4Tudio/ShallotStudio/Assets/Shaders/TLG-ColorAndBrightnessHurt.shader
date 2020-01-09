// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-5696-OUT,emission-2259-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31913,y:32554,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4776,x:31913,y:32723,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4776,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:acac682a6904d1e42b73adf56f04e996,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:6662,x:31966,y:33050,ptovrint:False,ptlb:ColorIntensity,ptin:_ColorIntensity,varname:node_6662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5696,x:32140,y:32660,varname:node_5696,prsc:2|A-1304-RGB,B-4776-RGB;n:type:ShaderForge.SFN_Multiply,id:9581,x:32398,y:32961,varname:node_9581,prsc:2|A-6485-OUT,B-6662-OUT;n:type:ShaderForge.SFN_Slider,id:702,x:31725,y:32928,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_702,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:6485,x:32163,y:32854,varname:node_6485,prsc:2|A-5696-OUT,B-702-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:5163,x:32274,y:33303,ptovrint:False,ptlb:WillHurt,ptin:_WillHurt,varname:node_5163,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Lerp,id:2259,x:32485,y:33195,varname:node_2259,prsc:2|A-9581-OUT,B-7221-OUT,T-5163-OUT;n:type:ShaderForge.SFN_Add,id:7221,x:32114,y:33180,varname:node_7221,prsc:2|A-9581-OUT,B-9593-OUT;n:type:ShaderForge.SFN_Color,id:6626,x:31732,y:33156,ptovrint:False,ptlb:HurtColor,ptin:_HurtColor,varname:node_6626,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6323529,c2:0.6323529,c3:0.6323529,c4:1;n:type:ShaderForge.SFN_Sin,id:8113,x:31704,y:33385,varname:node_8113,prsc:2|IN-8927-OUT;n:type:ShaderForge.SFN_Time,id:6427,x:31294,y:33318,varname:node_6427,prsc:2;n:type:ShaderForge.SFN_Abs,id:1033,x:31879,y:33385,varname:node_1033,prsc:2|IN-8113-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6076,x:31294,y:33471,ptovrint:False,ptlb:HurtFlashTime,ptin:_HurtFlashTime,varname:node_6076,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:20;n:type:ShaderForge.SFN_Multiply,id:8927,x:31535,y:33385,varname:node_8927,prsc:2|A-6427-T,B-6076-OUT;n:type:ShaderForge.SFN_Multiply,id:9593,x:31955,y:33256,varname:node_9593,prsc:2|A-6626-RGB,B-1033-OUT;proporder:1304-6626-4776-702-6662-6076-5163;pass:END;sub:END;*/

Shader "ShallotStudio/TLG-ColorAndBrightnessHurt" {
    Properties {
        _MainColor ("MainColor", Color) = (1,1,1,1)
        _HurtColor ("HurtColor", Color) = (0.6323529,0.6323529,0.6323529,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Brightness ("Brightness", Range(-1, 1)) = 0
        _ColorIntensity ("ColorIntensity", Float ) = 0
        _HurtFlashTime ("HurtFlashTime", Float ) = 20
        [MaterialToggle] _WillHurt ("WillHurt", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _MainColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _ColorIntensity;
            uniform float _Brightness;
            uniform fixed _WillHurt;
            uniform float4 _HurtColor;
            uniform float _HurtFlashTime;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5696 = (_MainColor.rgb*_MainTex_var.rgb);
                float3 diffuseColor = node_5696;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 node_9581 = ((node_5696+_Brightness)*_ColorIntensity);
                float4 node_6427 = _Time + _TimeEditor;
                float3 emissive = lerp(node_9581,(node_9581+(_HurtColor.rgb*abs(sin((node_6427.g*_HurtFlashTime))))),_WillHurt);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _MainColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _ColorIntensity;
            uniform float _Brightness;
            uniform fixed _WillHurt;
            uniform float4 _HurtColor;
            uniform float _HurtFlashTime;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5696 = (_MainColor.rgb*_MainTex_var.rgb);
                float3 diffuseColor = node_5696;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
