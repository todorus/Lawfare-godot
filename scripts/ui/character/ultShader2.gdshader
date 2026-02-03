shader_type canvas_item;

uniform vec4 particle_color : source_color = vec4(0.35, 0.85, 1.0, 1.0);

uniform float intensity : hint_range(0.0, 3.0) = 1.0;
uniform float density : hint_range(0.0, 2.0) = 0.9;      // 0..2, higher = more particles
uniform float speed : hint_range(-3.0, 3.0) = 0.8;         // upward speed
uniform float drift : hint_range(0.0, 2.0) = 0.35;        // sideways wobble
uniform float size : hint_range(0.001, 0.05) = 0.012;     // particle radius in UV units
uniform float softness : hint_range(0.5, 8.0) = 3.5;      // edge falloff
uniform float twinkle : hint_range(0.0, 2.0) = 0.6;       // brightness variation
uniform float cell_count : hint_range(8.0, 80.0) = 34.0;  // grid resolution (perf knob)

// Hash helpers (cheap pseudo-random)
float hash12(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

vec2 hash22(vec2 p) {
    float n = hash12(p);
    return vec2(n, hash12(p + n + 17.13));
}

// Soft circle sprite
float soft_circle(vec2 p, float r, float soft) {
    float d = length(p);
    // 1 at center -> 0 at edge, with controllable softness
    float x = clamp(1.0 - d / r, 0.0, 1.0);
    return pow(x, soft);
}

void fragment() {
    vec2 uv = UV;

    // Grid coordinates
    float N = cell_count;
    vec2 gv = uv * N;
    vec2 id = floor(gv);
    vec2 f  = fract(gv);

    float t = TIME;

    // We'll accumulate a few neighboring cells so particles can overlap cell borders
    float acc = 0.0;

    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 cid = id + vec2(float(x), float(y));

            // Random seed per cell
            vec2 rnd = hash22(cid);

            // Probability gate (controls density)
            // rnd.x in [0,1]. density ~ 0..2 maps to probability ~ 0..~0.9
            float p = clamp(density * 0.55, 0.0, 0.95);
            if (rnd.x > p) {
                continue;
            }

            // Particle base position within cell
            vec2 p0 = rnd;

            // Upward motion with wrap
            float phase = rnd.y; // per-particle phase
            float ymove = fract(p0.y + t * speed + phase);

            // Sideways drift (a little sine wobble)
            float wob = sin((t * (1.5 + rnd.x * 3.0) + phase * 6.283) ) * drift;
            float xmove = fract(p0.x + wob * 0.08);

            // Particle position in grid-local coords (centered around this cell)
            vec2 pp = vec2(xmove, ymove);

            // Convert to local fragment coords within this cell neighborhood:
            // f is fragment in [0,1] inside current cell. Compare to neighbor's particle coords
            vec2 dpos = (f - (pp + vec2(float(x), float(y))));

            // Scale distances so circles stay consistent across N
            dpos /= N;

            // Vary size per particle slightly
            float r = size * mix(0.6, 1.25, rnd.x);

            // Brightness / twinkle variation
            float tw = 1.0;
            if (twinkle > 0.0) {
                tw = 0.6 + 0.4 * sin(t * (2.0 + rnd.y * 6.0) + rnd.x * 12.0);
                tw = mix(1.0, tw, clamp(twinkle, 0.0, 1.0));
            }

            acc += soft_circle(dpos, r, softness) * tw;
        }
    }

    // Optional: bias particles to appear more at the bottom and fade near the top
    float vertical_fade = smoothstep(0.0, 0.25, uv.y) * (1.0 - smoothstep(0.7, 1.0, uv.y));

    // Final alpha
    float a = acc * intensity * vertical_fade;

    // Clamp to avoid blowing out
    a = clamp(a, 0.0, 1.0);

    COLOR = vec4(particle_color.rgb, particle_color.a * a);
}
