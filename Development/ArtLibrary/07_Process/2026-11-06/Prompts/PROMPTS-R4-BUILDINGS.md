# R4同视角建筑提示词

内置image_gen，透明背景；house_r3以home-spatial-r4为参考，kitchen_r3以house_r3为参考。均为未登记源图候选，不是实机通过证据。

## house_r3
```text
Use case: stylized-concept. Asset type: ONE transparent game sprite of a house, original RRPG source-art candidate.
Input reference is HOME SPATIAL R4, the current management-camera and warm illustration reference. Extract/recreate the design of ONE of the five small cottages in the upper row as an isolated asset. Match its high oblique overhead projection, broad roof, short visible ivory walls, brown beams and terracotta gabled roof. DO NOT revert to an eye-level tall cottage or overhead roof-only icon.
Camera: same high management view as reference, orthographic, approximately55degrees looking down FROM HORIZONTAL, azimuth roughly30-45degrees; visible front and right sides, roofs take about two thirds of visible silhouette, walls about one third. Enough top face visible to read square2x2 footprint. Same scale should work when repeated across a village. Vertical walls are vertical, no perspective convergence.
Subject: one small single-story SQUARE2x2 cottage, clear low broad terracotta GABLED pitched roof, small square masonry chimney with visible top opening, restrained dark timber, warm ivory plaster, low door on front and two small broad windows. Same believable clean warm adventure illustration as reference, a few coherent roof tile bands, large planes and restrained outlines, no high-frequency grunge.
Composition: full building with tiny neutral contact shadow inside CENTRAL70PERCENT of a square canvas,15percent TRANSPARENT padding each side,10percent minimum top/bottom. Transparent alpha background, no grass patch, no flowerbox, no trees, no loose barrels, no fence or ground tile. One house only, no people, no fire, no smoke, no text or UI. Avoid detached props. Do not bake whole R4 scene into transparency. This is a production candidate, not a final binding.
IMPORTANT: prioritize matching reference's small houses and camera, not adding more impressive architectural detail or a new roof family.
```

## kitchen_r3
```text
Use case: stylized-concept. Asset type: ONE transparent game sprite for RRPG community KITCHEN, same projection as supplied HOUSE.
Reference is house_r3, camera/style master. Keep EXACT camera and roof/wall proportions, outline, neutral upper-left light, color and scale of this house, but redesign as a community kitchen. Do NOT inherit an earlier taller front-facing kitchen. Low square2x2 single-story body.
Geometry: high oblique birdseye orthographic around55degrees down FROM HORIZONTAL, broad roof planes about two-thirds of visible silhouette, short ivory wall strips one-third. Front and right visible, all parallel edges, roof ridge same orientation as reference, same gabled roof family. Distinguish kitchen with broad squat square masonry chimney and one small recessed open working bay under front-right eave. One simple wooden counter and unlit masonry stove can be seen from above under bay; no large theatrical cutaway or tall front display. Pots large/simple and INSIDE bay. Roof and counter overhead surfaces visible, do not lower camera to expose equipment. Whole building and work bay must fit a single square2x2 footprint, not a detached porch outside.
Warm ivory plaster, red terracotta roof with a few broad seams, deep brown beams, clean warm adventure illustration. Big functional forms with restrained structural detail, no dirt overlays, tiny foliage or dense texture.
Entire building centered within central70percent square canvas, genuinely transparent15percent padding left/right and at least10percent top/bottom. Tiny neutral contact shadow directly underneath only. No grass tile, no baseplinth, no flowerbox, no people/animals, no loose outdoor barrels or tables, no text orUI, no flame/embers/firelight/glow, no smoke plume, no sky/background. Static state-neutral cooking building source candidate, same scale as reference house, not a giant landmark.
```

