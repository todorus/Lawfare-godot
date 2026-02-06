shader_type canvas_item;

uniform float grayscale : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 col = texture(TEXTURE, UV);

    // Perceptual luminance (standard weights)
    float lum = dot(col.rgb, vec3(0.299, 0.587, 0.114));

    vec3 gray = vec3(lum);

    col.rgb = mix(col.rgb, gray, grayscale);
    COLOR = col;
}
