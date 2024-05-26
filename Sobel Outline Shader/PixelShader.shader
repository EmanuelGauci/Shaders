Shader "Hidden/PixelShader"
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {} // Texture property for the main texture
        _DeltaX ("Delta X", Float) = 0.001  // X-axis delta value for Sobel edge detection
        _DeltaY ("Delta Y", Float) = 0.001  // Y-axis delta value for Sobel edge detection
        
    }
    
    SubShader {
        Tags { "RenderType"="Overlay" } // Tags for rendering the shader as an overlay
        LOD 500 // Level of detail for the shader
        
        CGINCLUDE
        
        #include "UnityCG.cginc" // Include Unity's common CG code
        
        sampler2D _MainTex; // Main texture sampler
        float _DeltaX; // X-axis delta value uniform
        float _DeltaY; // Y-axis delta value uniform
        float4 _EdgeColor;

        // Function for Sobel edge detection
        float sobel (sampler2D tex, float2 uv) {
            float2 delta = float2(_DeltaX, _DeltaY); // Delta vector
            
            float4 hr = float4(0, 0, 0, 0); // Horizontal gradient
            float4 vt = float4(0, 0, 0, 0); // Vertical gradient
            
            // Sobel filter calculation for horizontal gradient
            hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) * -1.0;
            hr += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) * -2.0;
            hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
            hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) * -2.0;
            hr += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
            hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) *  2.0;
            hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
            hr += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) *  2.0;
            hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) *  1.0;
            
            // Sobel filter calculation for vertical gradient
            vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) * -1.0;
            vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) * -2.0;
            vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
            vt += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  0.0;
            vt += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
            vt += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) *  0.0;
            vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
            vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) *  2.0;
            vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) *  1.0;
            
            // Combine horizontal and vertical gradients
            return sqrt(hr * hr + vt * vt); // Magnitude of the gradient
        }
        
        // Fragment shader function
        float4 frag (v2f_img IN) : COLOR {
            float4 texColor = tex2D(_MainTex, IN.uv); // Sample color from main texture
            
            // Check if the color is in the red range
            if (texColor.r > 0.5 && texColor.g < 0.5 && texColor.b < 0.5) {
                // Return original color if it's in the red range
                return texColor;
            } else {
                // Apply Sobel edge detection otherwise
                float s = sobel(_MainTex, IN.uv);
                return float4(s, s, s, 0); // Return Sobel edge-detected color
            }
        }
        
        ENDCG
        
        // Pass block for vertex and fragment shaders
        Pass {
            CGPROGRAM
            #pragma vertex vert_img // Vertex shader directive
            #pragma fragment frag // Fragment shader directive
            ENDCG
        }
        
    } 
    FallBack "Diffuse" // Fallback shader if this one is not supported
}
