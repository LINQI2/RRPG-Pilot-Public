# R5农田提示词

工具：内置image_gen；透明背景。首轮参考R5场景，第二轮增加自制精确3×2几何导引。没有第三方原图作为输入。

## 首轮：farm_r5_square_rejected
```text
Use case: stylized-concept. Asset type: ONE isolated transparent game sprite, RRPG FARM3x2, aligned to approved village diamond-grid art reference.
Input: home-spatial-r5-grid.png is STYLE, CAMERA and GROUND AXIS reference. Recreate just its single farm as a production source candidate; do not copy ground, UI, residents or houses.
Subject: one compact rectangular cultivated field with exactly3 coherent broad crop strips: golden wheat, broad green cabbages, restrained tomato vines. Together all crops sit WITHIN one exact3-by-2 proportioned flat ground footprint. No fences outside footprint, no workers, no scarecrows, no baskets, no extra fields. A very low neat earth rim is allowed, not a tall raised planter box.
Ground axes MUST match the reference village grid: screen u=(1,+0.527), v=(-1,+0.527), y downwards. The two long edges of farm parallel u and short edges parallel v, corners form the same diamond-grid parallelogram family as house foundations. Planting strips follow these same axes; no third angle, no horizontal rectangle, no arbitrary perspective. Orthographic, no vanishing point or near/far scale change. High oblique management camera consistent with R5, ground surface readable from above. Crop height restrained to keep base plane/outline visible.
Style: original warm adventure illustration, clean grouped color masses, readable middle-scale crop shapes, low noise, broad neutral upper-left light, thin controlled outline. Don't trace franchise assets. Crop details should survive small game size, not hundreds of tiny individual leaves or floating seeds.
Composition: single fully visible asset centered, canvas square, object fits within central70percent width, minimum15percent REAL TRANSPARENT padding on left/right and10percent top/bottom; whole footprint not cropped. Genuine alpha background with no surrounding green ground or square base tile. Soil belongs to the cultivated rectangle only; no scenery or broad ambient shadow. No people, tools, buildings, text, labels, numbers, UI, smoke/fire, sky or diagrams; don't bake the calibration grid into the source sprite.
```

## 修正：farm_r5
```text
undefined
```

