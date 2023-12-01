#version 330 core
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec2 VertexUV0;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * uView * uModel * vec4(VertexPosition, 1.0); //Set the vertex position

    fUv = VertexUV0; //To translate the vertex UV coordinates to frag shader
}