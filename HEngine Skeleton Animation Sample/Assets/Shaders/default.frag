#version 330 core
in vec3 fNorm;
in vec2 fUv;

uniform sampler2D uTexture0;
uniform sampler2D uTexture1;
uniform sampler2D uTexture2;

out vec4 FragColor;

float near = 0.1; 
float far  = 8.0; 
  
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

void main()
{
    float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide by far for demonstration
    FragColor = vec4(vec3(1.0 - depth), 1.0); //Color = depth buffer, linearized
    //FragColor = vec4(vec3(gl_FragCoord.z), 1.0); //Color = depth buffer
    //FragColor = vec4(fUv, 0.0, 1.0); //Color = UV coords
    //FragColor = vec4(fNorm, 1.0); //Color = vertex normal
    //FragColor = vec4(fNorm.xy, 0.0, 1.0); //Color = vertex normal without Z part
    //FragColor = texture(uTexture0, fUv); //Color = sampled pixel color on the texture
    //FragColor = vec4(1.0, 1.0, 1.0, 1.0); //Color = white
}