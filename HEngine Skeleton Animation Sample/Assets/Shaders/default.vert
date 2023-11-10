#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vNorm;
layout (location = 2) in vec2 vUv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fNorm;
out vec2 fUv;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0); //Set the vertex position

    vec3 pNorm = (uView * vec4(vNorm, 1.0)).xyz; //To translate the value of normal multiplied by view matrix to see its projection against the camera
    fNorm = pNorm * 0.5 + 0.5; //To ensure negative values are mapped correctly (i.e. [-1; 1] -> [0; 1])
    //fNorm = vNorm; //To translate vertex normal value the way it is in the model
    //fNorm = mat3(transpose(inverse(uModel))) * vNorm; //Copied from some tutorial. Not sure it works correctly
    
    fUv = vUv; //To translate the vertex UV coordinates to frag shader
}