// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-5696-OUT,emission-9581-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32005,y:32555,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4776,x:32005,y:32798,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4776,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5360-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6662,x:32248,y:33239,ptovrint:False,ptlb:ColorIntensity,ptin:_ColorIntensity,varname:node_6662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5696,x:32223,y:32711,varname:node_5696,prsc:2|A-1304-RGB,B-4776-RGB;n:type:ShaderForge.SFN_Multiply,id:9581,x:32499,y:33127,varname:node_9581,prsc:2|A-6485-OUT,B-6662-OUT;n:type:ShaderForge.SFN_Slider,id:702,x:31992,y:33032,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_702,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:6485,x:32338,y:32908,varname:node_6485,prsc:2|A-5696-OUT,B-702-OUT;n:type:ShaderForge.SFN_TexCoord,id:9650,x:31423,y:32534,varname:node_9650,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:1083,x:31268,y:32770,varname:node_1083,prsc:2;n:type:ShaderForge.SFN_Vector4Property,id:1356,x:31241,y:32977,ptovrint:False,ptlb:XYSpeed,ptin:_XYSpeed,varname:node_1356,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Multiply,id:3936,x:31450,y:32757,varname:node_3936,prsc:2|A-1083-T,B-1356-X;n:type:ShaderForge.SFN_Multiply,id:9932,x:31450,y:32963,varname:node_9932,prsc:2|A-1083-T,B-1356-Y;n:type:ShaderForge.SFN_Add,id:6975,x:31629,y:32600,varname:node_6975,prsc:2|A-9650-U,B-3936-OUT;n:type:ShaderForge.SFN_Add,id:4802,x:31629,y:32788,varname:node_4802,prsc:2|A-9650-V,B-9932-OUT;n:type:ShaderForge.SFN_Append,id:5360,x:31825,y:32731,varname:node_5360,prsc:2|A-6975-OUT,B-4802-OUT;proporder:1304-4776-702-6662-1356;pass:END;sub:END;*/

Shader "ShallotStudio/ColorAndBrightnessUVAnim" {
    Properties {
        _MainColor ("MainColor", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Brightness ("Brightness", Range(-1, 1)) = 0
        _ColorIntensity ("ColorIntensity", Float ) = 0
        _XYSpeed ("XYSpeed", Vector) = (0,0,0,0)
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
            uniform float4 _XYSpeed;
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
                float4 node_1083 = _Time + _TimeEditor;
                float2 node_5360 = float2((i.uv0.r+(node_1083.g*_XYSpeed.r)),(i.uv0.g+(node_1083.g*_XYSpeed.g)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5360, _MainTex));
                float3 node_5696 = (_MainColor.rgb*_MainTex_var.rgb);
                float3 diffuseColor = node_5696;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = ((node_5696+_Brightness)*_ColorIntensity);
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
            uniform float4 _XYSpeed;
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
                float4 node_1083 = _Time + _TimeEditor;
                float2 node_5360 = float2((i.uv0.r+(node_1083.g*_XYSpeed.r)),(i.uv0.g+(node_1083.g*_XYSpeed.g)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5360, _MainTex));
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
