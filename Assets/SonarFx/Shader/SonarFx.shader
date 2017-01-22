//
// Sonar FX
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
Shader "Hidden/SonarFX"
{
    Properties
    {
        _SonarBaseColor  ("Base Color",  Color)  = (0.1, 0.1, 0.1, 0)
        _SonarWaveColor  ("Wave Color",  Color)  = (1.0, 0.1, 0.1, 0)
        _SonarWaveVector ("Wave Vector", Vector) = (0, 0, 1, 0)
        _SonarAddColor   ("Add Color",   Color)  = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM

        #pragma surface surf Lambert
        #pragma multi_compile SONAR_DIRECTIONAL SONAR_SPHERICAL

        struct Input
        {
            float3 worldPos;
        };

        float3 _SonarBaseColor;
        float3 _SonarWaveColor;
        float3 _SonarWaveVector;
        float3 _SonarAddColor;
		float3 _OldSonarWaveVector;
		float _CustomTime;
		int _Length = 0;
		float _LinesArray[32];
		float3 _AttenuateColor;
		float4 _StartPositions[32];
		float _Distances[32];
		int _CurrentWave;

        void surf(Input IN, inout SurfaceOutput o)
        {
			o.Emission = _SonarBaseColor;

			float size = 0.025;
			float w;
			float inDistance;
			float same;

			for (int i = 0; i < _Length; i++)
			{
				for (int j = 31; j >= 0; j--)
				{
					// ACTUAL
					w = length(IN.worldPos - _StartPositions[j]);
					w = lerp(0, w, max(0, sign(_Distances[j] - w)));
					inDistance = (w - _LinesArray[i]);
					same = (inDistance < size && inDistance > 0) ? 0 : 1;
					// lerp(0, 1, max(max(0, sign(size - inDistance)), sign(inDistance));
					o.Emission = lerp(lerp(_SonarWaveColor, _AttenuateColor, sign(j)), o.Emission, same);
				}
				
				
				//// OLD
				//w = length(IN.worldPos - _StartPositions[1]);
				//w = lerp(0, w, max(0, sign(_Distances[1] - w)));
				//inDistance = (w - _LinesArray[i]);
				//same = (inDistance < size && inDistance > 0) ? 0 : 1;
				//o.Emission = lerp(_AttenuateColor, o.Emission, same);
				//
				//// MORE OLD
				//w = length(IN.worldPos - _StartPositions[2]);
				//w = lerp(0, w, max(0, sign(_Distances[1] - w)));
				//inDistance = (w - _LinesArray[i]);
				//same = (inDistance < size && inDistance > 0) ? 0 : 1;
				//// lerp(0, 1, max(max(0, sign(size - inDistance)), sign(inDistance));
				//o.Emission = lerp(_AttenuateColor, o.Emission, same);

				//// MORE MORE OLD
				//w = length(IN.worldPos - _StartPositions[3]);
				//w = lerp(0, w, max(0, sign(_Distances[3] - w)));
				//inDistance = (w - _LinesArray[i]);
				//same = (inDistance < size && inDistance > 0) ? 0 : 1;
				//// lerp(0, 1, max(max(0, sign(size - inDistance)), sign(inDistance));
				//o.Emission = lerp(_AttenuateColor, o.Emission, same);
			}

			
        }

        ENDCG
    } 
    Fallback "Diffuse"
}
