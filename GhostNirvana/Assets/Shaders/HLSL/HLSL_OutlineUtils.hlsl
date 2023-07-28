#ifndef GetCrossSampleUVs_INCLUDED
#define GetCrossSampleUVs_INCLUDED
void GetCrossSampleUVs_float(float4 uv, float2 texelSize, float offsetMultiplier, 
	out float2 uvOriginal, out float2 uvTopLeft, out float2 uvTopRight, out float2 uvBottomLeft, out float2 uvBottomRight) {
	
	uvOriginal = uv;
	uvTopLeft = uv.xy + float2(-texelSize.x, texelSize.y) * offsetMultiplier;
	uvTopRight = uv.xy + float2(texelSize.x, texelSize.y) * offsetMultiplier;
	uvBottomLeft = uv.xy + float2(-texelSize.x, -texelSize.y) * offsetMultiplier;
	uvBottomRight = uv.xy + float2(texelSize.x, -texelSize.y) * offsetMultiplier;
}
#endif


#ifndef RobertsCross_INCLUDED
#define RobertsCross_INCLUDED
void RobertsCross_float(float TopLeft, float TopRight, float BottomLeft, float BottomRight, out float result) {
    float values[4];
    values[0] = TopLeft;
    values[1] = TopRight;
    values[2] = BottomLeft;
    values[3] = BottomRight;
    float G1 = values[0] - values[3];
    float G2 = values[1] - values[2];
    result = sqrt(G1*G1 + G2*G2);
}
void RobertsCross_half(float TopLeft, float TopRight, float BottomLeft, float BottomRight, out float result) {
    float values[4];
    values[0] = TopLeft;
    values[1] = TopRight;
    values[2] = BottomLeft;
    values[3] = BottomRight;
    float G1 = values[0] - values[3];
    float G2 = values[1] - values[2];
    result = sqrt(G1*G1 + G2*G2);
}
#endif

#ifndef RobertsCross2_INCLUDED
#define RobertsCross2_INCLUDED
void RobertsCross2_float(float2 TopLeft, float2 TopRight, float2 BottomLeft, float2 BottomRight, out float result) {
    float2 values[4];
    values[0] = TopLeft;
    values[1] = TopRight;
    values[2] = BottomLeft;
    values[3] = BottomRight;
    float2 G1 = values[0] - values[3];
    float2 G2 = values[1] - values[2];
    result = sqrt(dot(G1,G1) + dot(G2, G2));
}
void RobertsCross2_half(float2 TopLeft, float2 TopRight, float2 BottomLeft, float2 BottomRight, out float result) {
    float2 values[4];
    values[0] = TopLeft;
    values[1] = TopRight;
    values[2] = BottomLeft;
    values[3] = BottomRight;
    float2 G1 = values[0] - values[3];
    float2 G2 = values[1] - values[2];
    result = sqrt(dot(G1,G1) + dot(G2, G2));
}
#endif


