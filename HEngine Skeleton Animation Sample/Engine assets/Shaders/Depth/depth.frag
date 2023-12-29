#version 330 core

out vec4 FragColor;

uniform float near = 0.1; 
uniform float far  = 8.0; 
  
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

void main()
{
    float depth = LinearizeDepth(gl_FragCoord.z) / far; // divide by far for demonstration
    
	FragColor = vec4(vec3(1.0 - depth), 1.0); //Color = depth buffer, linearized
}