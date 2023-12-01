#version 330 core

in vec2 fUv;

uniform sampler2D TextureDiffuse;

out vec4 FragColor;

void main()
{
    FragColor = texture(TextureDiffuse, fUv); //Color = sampled pixel color on the texture
}