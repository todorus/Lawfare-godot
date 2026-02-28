shader_type canvas_item;

// Amount in pixels that the TOP edge shifts.
// Positive value = visually leans right when left-anchored.
uniform float skew_px = 0.0;

// false  -> bottom-left anchored
// true   -> bottom-right anchored
uniform bool anchor_right = false;

void vertex() {
    // 0 at bottom, 1 at top (typical for UI UVs)
    float k = 1.0 - UV.y;

    float shift = k * skew_px;

    if (anchor_right) {
        // Bottom-right anchored:
        // shift everything left instead
        VERTEX.x -= shift;
    } else {
        // Bottom-left anchored:
        VERTEX.x += shift;
    }
}