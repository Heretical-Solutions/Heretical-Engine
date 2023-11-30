#version 330 core
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VertexUV0;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fNorm;
out vec2 fUv;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * uView * uModel * vec4(VertexPosition, 1.0); //Set the vertex position
    //gl_Position = vec4(VertexPosition, 1.0);

    vec3 pNorm = (uView * vec4(VertexNormal, 1.0)).xyz; //To translate the value of normal multiplied by view matrix to see its projection against the camera
    fNorm = pNorm * 0.5 + 0.5; //To ensure negative values are mapped correctly (i.e. [-1; 1] -> [0; 1])
    //fNorm = VertexNormal; //To translate vertex normal value the way it is in the model
    //fNorm = mat3(transpose(inverse(uModel))) * VertexNormal; //Copied from some tutorial. Not sure it works correctly
    
    fUv = VertexUV0; //To translate the vertex UV coordinates to frag shader
}