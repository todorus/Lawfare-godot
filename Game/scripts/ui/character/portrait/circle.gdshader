shader_type canvas_item;

// ---- Circle shape ----
uniform float radius : hint_range(0.0, 1.0) = 0.45;
uniform float smoothing : hint_range(0.0, 0.05) = 0.004;

// ---- Background (INSIDE the circle) ----
uniform vec4 background_color : source_color = vec4(0.0, 0.0, 0.0, 0.35);

// ---- Outline (around the circle) ----
uniform float outline_thickness : hint_range(0.0, 0.1) = 0.025;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    vec2 center = vec2(0.5);
    float dist = distance(UV, center);

    // Inside-circle mask (anti-aliased)
    float circle = 1.0 - smoothstep(
    radius - smoothing,
    radius + smoothing,
    dist
    );

    // Outer circle for outline
    float outer = 1.0 - smoothstep(
    (radius + outline_thickness) - smoothing,
    (radius + outline_thickness) + smoothing,
    dist
    );

    // Ring = outer minus inner (only the border band)
    float outline = clamp(outer - circle, 0.0, 1.0);

    vec4 tex = texture(TEXTURE, UV);

    // Compose INSIDE circle:
    // background first, then portrait on top (portrait alpha respected)
    vec4 inside = background_color;
    inside.rgb = mix(inside.rgb, tex.rgb, tex.a);
    inside.a = max(inside.a, tex.a); // keeps background visible if portrait is transparent

    // Final: outline + inside, clipped by their masks
    vec4 result = vec4(0.0);

    // Outside both circle and outline = transparent
    result += outline_color * outline;
    result += inside * circle;

    COLOR = result;
}
