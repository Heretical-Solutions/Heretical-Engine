#version 330 core

in vec2 fUv;

out vec4 FragColor;

const float scale = 20; //it's 2x in Blender when using checker texture node

const vec4 color = vec4(1, 0.498, 0.969, 1);

void main()
{
	FragColor = vec4(vec3(max(sign(fract(fUv.x * scale) - 0.5) * sign(fract(fUv.y * scale) - 0.5), 0.0)), 1.0) * color;
}