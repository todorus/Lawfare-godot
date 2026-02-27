shader_type canvas_item;

uniform float skew_px = 40.0; // + shifts top right, - shifts top left

void vertex() {
    // UV.y is 0 at top, 1 at bottom (in most UI quads it is consistent).
    // We want symmetrical shear around center: -0.5..+0.5
    float y = UV.y - 0.5;

    // Shift X by y * skew amount (pixels)
    VERTEX.x += y * skew_px;
}

void fragment() {
    // No fragment changes needed; keep original rendering
    COLOR = texture(TEXTURE, UV);
}