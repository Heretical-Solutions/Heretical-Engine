#version 330 core

out vec4 FragColor;

const float scale = 0.02;

const vec4 color = vec4(1, 0.498, 0.969, 1);

void main()
{
    FragColor = vec4(vec3(max(sign(fract(gl_FragCoord.x * scale) - 0.5) * sign(fract(gl_FragCoord.y * scale) - 0.5), 0.0)), 1.0) * color;
}