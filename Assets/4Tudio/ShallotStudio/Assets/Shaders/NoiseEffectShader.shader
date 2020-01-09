// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-8428-OUT;n:type:ShaderForge.SFN_TexCoord,id:9325,x:31841,y:32702,varname:node_9325,prsc:2,uv:0;n:type:ShaderForge.SFN_Noise,id:4535,x:32292,y:32842,varname:node_4535,prsc:2|XY-1858-OUT;n:type:ShaderForge.SFN_Time,id:9121,x:31855,y:32933,varname:node_9121,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6492,x:32202,y:32592,ptovrint:False,ptlb:node_6492,ptin:_node_6492,varname:node_6492,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-9325-UVOUT;n:type:ShaderForge.SFN_Add,id:8428,x:32506,y:32644,varname:node_8428,prsc:2|A-6492-RGB,B-4535-OUT;n:type:ShaderForge.SFN_Add,id:1858,x:32055,y:32842,varname:node_1858,prsc:2|A-9325-UVOUT,B-9121-TSL;proporder:6492;pass:END;sub:END;*/

Shader "ShallotStudio/NoiseEffectShader" {
    Properties {
        _node_6492 ("node_6492", 2D) = "black" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_6492; uniform float4 _node_6492_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _node_6492_var = tex2D(_node_6492,TRANSFORM_TEX(i.uv0, _node_6492));
                float4 node_9121 = _Time + _TimeEditor;
                float2 node_1858 = (i.uv0+node_9121.r);
                float2 node_4535_skew = node_1858 + 0.2127+node_1858.x*0.3713*node_1858.y;
                float2 node_4535_rnd = 4.789*sin(489.123*(node_4535_skew));
                float node_4535 = frac(node_4535_rnd.x*node_4535_rnd.y*(1+node_4535_skew.x));
                float3 emissive = (_node_6492_var.rgb+node_4535);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
