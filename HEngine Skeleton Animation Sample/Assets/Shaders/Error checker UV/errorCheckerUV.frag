#version 330 core

in vec2 fUv;

out vec4 FragColor;

const float scale = 4;

const vec4 color = vec4(1, 0.498, 0.969, 1);

void main()
{
	//TODO: check UV checker on the model in Blender. I've made that the engine is flipping .y UV coordinate but I'm not ENTIRELY sure that's correct
    FragColor = vec4(vec3(max(sign(fract(fUv.x * scale) - 0.5) * sign(fract((1.0 - fUv.y) * scale) - 0.5), 0.0)), 1.0) * color;
}