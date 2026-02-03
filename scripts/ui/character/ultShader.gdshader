shader_type canvas_item;

uniform float intensity : hint_range(0.0, 3.0) = 1.0;
uniform float speed : hint_range(0.0, 5.0) = 1.2;
uniform float ring_thickness : hint_range(0.001, 0.2) = 0.045;
uniform float ring_radius : hint_range(0.0, 1.0) = 0.62;
uniform float noise_scale : hint_range(1.0, 20.0) = 10.0;
uniform float sparkle_density : hint_range(0.0, 2.0) = 0.8;

// Godot 4: use `source_color` for correct sRGB handling in UI.
uniform vec4 glow_color : source_color = vec4(0.35, 0.85, 1.0, 1.0);

float hash12(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

float noise(vec2 p) {
	vec2 i = floor(p);
	vec2 f = fract(p);
	float a = hash12(i);
	float b = hash12(i + vec2(1.0, 0.0));
	float c = hash12(i + vec2(0.0, 1.0));
	float d = hash12(i + vec2(1.0, 1.0));
	vec2 u = f * f * (3.0 - 2.0 * f);
	return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void fragment() {
	vec2 uv = UV;
	vec2 p = uv * 2.0 - 1.0;

	// Aspect-correct so ring stays circular in non-square cards.
	float aspect = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	p.x *= aspect;

	float t = TIME * speed;

	// Radial distance
	float r = length(p);

	// Soft ring mask
	float ring = 1.0 - smoothstep(ring_radius, ring_radius + ring_thickness, r);
	ring *= smoothstep(ring_radius - ring_thickness, ring_radius, r);

	// Wavy modulation on ring
	float n = noise(p * noise_scale + vec2(t * 0.6, -t * 0.4));
	ring *= mix(0.55, 1.25, n);

	// Sparkles: grid noise thresholded, animated drift
	vec2 sp = (uv * noise_scale) + vec2(t * 0.9, -t * 0.7);
	float sn = noise(sp);
	float spark = smoothstep(0.98 - sparkle_density * 0.08, 1.0, sn);

	// Keep sparkles mostly near ring (feels “charged”)
	float ring_band = smoothstep(ring_radius - ring_thickness * 2.5, ring_radius, r)
	                - smoothstep(ring_radius, ring_radius + ring_thickness * 2.5, r);
	spark *= ring_band;

	// Pulse
	float pulse = 0.75 + 0.25 * sin(t * 2.2);

	float alpha = (ring * 0.85 + spark * 0.9) * intensity * pulse;

	// Optional: fade toward center so content remains readable
	float center_fade = smoothstep(0.0, 0.55, r);
	alpha *= center_fade;

	COLOR = vec4(glow_color.rgb, glow_color.a * alpha);
}
