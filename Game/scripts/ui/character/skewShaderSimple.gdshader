shader_type canvas_item;

// Pixel shear: positive = top shifts right, negative = top shifts left
uniform float skew_px : hint_range(-100.0, 100.0) = 0.0;

// Where the skew pivots in local UV Y (0 = top, 1 = bottom, 0.5 = center)
uniform float pivot_y : hint_range(0.0, 1.0) = 0.5;

void vertex() {
    // Use UV space so it scales consistently with Control size.
    float y = UV.y - pivot_y;
    VERTEX.x += y * skew_px;
}