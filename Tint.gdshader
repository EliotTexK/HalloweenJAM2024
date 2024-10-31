shader_type canvas_item;
render_mode unshaded;

uniform vec3 tint_color : source_color = vec3(1.0, 0.0, 0.0);
uniform float intensity : hint_range(0, 1) = 1.0;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment(){
    vec4 tex = texture(screen_texture, SCREEN_UV);
    COLOR = mix(tex, vec4(tint_color, tex.a), intensity);
}