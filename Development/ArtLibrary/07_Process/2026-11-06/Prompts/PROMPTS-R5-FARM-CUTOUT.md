# R5农田抠图修正提示词

工具：内置image_gen；目标farm_r5_background_rejected.png；结果仍须按真实Alpha检查，不由透明参数推断。

```text
Use case: background-extraction.
Asset type: transparent game sprite cutout.
Input image is the EDIT TARGET. Preserve the exact field shape, camera, three crop strip arrangement, crop artwork, soil footprint and3:2 rectangular ground proportions. Do NOT redesign, rotate, stretch or repaint the field.
Remove ALL background, outer brown haze, blurred vignette, large soft shadows, glow, black/grey background and every pixel not part of the cultivated soil bed or actual crops/stakes. Replace everything outside the precise soil/crop silhouette with genuinely transparent alpha=0. Keep crop and soil interiors opaque alpha=255 except antialiased outer contours. No contact shadow needed at all.
The result must be a clean cutout with fully transparent padding, no atmospheric veil, no colored square, no matte and no soft surrounding shadow. Do not add anything.
```

