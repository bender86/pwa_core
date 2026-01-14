
#!/bin/bash

# Path to your source image (512x512 or larger)
SOURCE_IMAGE="/home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth/wwwroot/icon-worldbet.png"

# Output directory
OUTPUT_DIR="/home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth/wwwroot"

# Check if source image exists
if [ ! -f "$SOURCE_IMAGE" ]; then
    echo "? Error: Source image not found at $SOURCE_IMAGE"
    exit 1
fi

echo "Generating icons from $SOURCE_IMAGE..."

# Small favicon sizes
convert "$SOURCE_IMAGE" -resize 16x16 "$OUTPUT_DIR/favicon-16x16.png"
convert "$SOURCE_IMAGE" -resize 32x32 "$OUTPUT_DIR/favicon-32x32.png"

# Multi-resolution favicon.ico (16x16 and 32x32)
convert "$SOURCE_IMAGE" -resize 16x16 -background transparent -flatten \
        "$SOURCE_IMAGE" -resize 32x32 -background transparent -flatten \
        "$OUTPUT_DIR/favicon.ico"

# PWA Android icons
convert "$SOURCE_IMAGE" -resize 192x192 "$OUTPUT_DIR/icon-192.png"
convert "$SOURCE_IMAGE" -resize 512x512 "$OUTPUT_DIR/icon-512.png"

# iOS icon
convert "$SOURCE_IMAGE" -resize 180x180 "$OUTPUT_DIR/apple-touch-icon.png"

echo "? All icons generated in $OUTPUT_DIR"
ls -lh "$OUTPUT_DIR"/*.png "$OUTPUT_DIR"/*.ico
