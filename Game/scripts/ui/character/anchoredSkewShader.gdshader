shader_type canvas_item;

// Amount in pixels that the TOP edge shifts.
// Positive value = visually leans right when left-anchored.
uniform float skew_px = 0.0;

// false  -> bottom-left anchored
// true   -> bottom-right anchored
uniform bool anchor_right = false;

// Flip the TEXTURE sampling horizontally (UV flip), after skewing.
uniform bool flip_h = false;

void vertex() {
    // 0 at bottom, 1 at top (typical for UI UVs)
    float k = 1.0 - UV.y;
    float shift = k * skew_px;
    bool anchor = anchor_right;
    if(flip_h) {
        anchor = !anchor;
    }

    if (anchor) {
        VERTEX.x -= shift;
    } else {
        VERTEX.x += shift;
    }
}

void fragment() {
    vec2 uv = UV;

    if (flip_h) {
        uv.x = 1.0 - uv.x;
    }

    COLOR = texture(TEXTURE, uv);
}