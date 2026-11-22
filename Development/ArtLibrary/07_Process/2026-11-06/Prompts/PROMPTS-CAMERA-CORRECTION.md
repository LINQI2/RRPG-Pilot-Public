# 本轮修正提示词

工具：内置image_gen。原01只作材质/色彩参考，其镜头明确否决。结果是否符合视角/数量另行审查，不由提示词自行判通过。

## home-spatial-r2（需再修，立面仍高、住宅4而非5）
```text
Use case: stylized-concept.
Asset type: corrected game-space art direction calibration for RRPG village management, wide landscape 16:9, a NEW original gameplay-scale environment mockup.
Input reference: local VISUAL TARGET 01 board supplies ONLY its warm clean illustrated color/material family. Its LOW CAMERA, tall facades, big near foreground buildings, and people posing in perspective are REJECTED. Do NOT copy its composition.
PRIMARY CORRECTION IS GEOMETRY: a high bird's-eye oblique ORTHOGRAPHIC management camera, optical axis 60 DEGREES DOWN FROM THE HORIZONTAL GROUND (30 degrees off vertical), azimuth about45degrees. This is much more top-down than the reference. NO horizon, NO sky, NO distant hills, NO eye-level vanishing point. Ground is a flat continuous map plane all across frame. Buildings equal world size have equal projected size everywhere in frame; no giant foreground houses. Look down on the roofs and heads, not toward tall facades. ROOFS and horizontal land surfaces dominate, vertical walls are shallow low bands. Broad roof top faces plainly visible; back of each roof is visible. Human figures show tops of heads, shoulders and short foreshortened bodies, not full upright side-view portraits. No front-facing illustration camera.
Scene: exactly 5 modest completed houses in a loose readable residential cluster, one community kitchen distinguished by squat chimney and attached compact work shelter, one small tavern with hanging pictorial sign, one rectangular farm with exactly3 broad crop strips. A handful of adult village inhabitants near actual ground-level door/work areas, correctly scaled to building doors; six residents total. Calm open ground between the clusters and access gaps around every building. No roads required as gameplay; subtle bare-ground paths only visual suggestion. Do not add new gameplay buildings, interior simulation, caravans, quests, weather systems, or industry. No roof cutaway as an assumed feature.
Composition: all buildings fully in view, the nearest building is NOT cropped by frame; all stand on same ground scale. Ground-projected footprint makes front/back depth and placement unambiguous. Enough open ground to see people between buildings. A modest common open space at center. The village should fit a management viewport, not a postcard vista. The top half is NOT an atmospheric landscape; it is more usable management ground.
Style: approved warm adventure 2D illustration, ivory plaster, terracotta roof masses, deep brown beams, restrained structural detail, clean edges, soft neutral upper-left daylight, pale sage low-frequency ground. Summarize roof tiles and vegetation to prevent micro-noise. Adult proportions and lived-in utilitarian design; avoid chibi. Strong readable broad masses at game thumbnail size.
No UI, no labels, no text, no logos, no frame, no decorative inset portraits or battle panels. This image validates the HOME spatial camera only. Do not solve a top-down request by merely tilting a foreground cottage roof while keeping an eye-level world.
```

## home-spatial-r3（极端顶视对照，不作为最终推荐）
```text
Use case: stylized-concept. Asset type: ONE overhead 2D village management spatial calibration image, landscape16:9, no UI no words.
Input image is ONLY the chosen warm ivory/terracotta clean illustration palette and material reference. Its perspective is explicitly WRONG and must be discarded.
MANDATORY camera: near-TOP-DOWN BIRD'S-EYE MAP CAMERA looking down at70 degrees BELOW HORIZONTAL ground (20 degrees off straight-down vertical), orthographic projection. This is a map seen from overhead, NOT a conventional isometric diorama, NOT 35-degree view. Roofs read almost as a plan view with only shallow strips of wall beneath, far less facade than roof. Chimney top openings are wide visible squares. Human adults show primarily head tops and shoulders, foreshortened legs, no upright full-body illustrations. Ground squares look almost square, with only subtle depth compression. All ground axes parallel throughout; no horizon, no sky.
Layout MUST be exactly: TOP BAND five identical-size square cottages in a single spaced row (FIVE houses, count5, equal size and identical roof orientation). Bottom left one 3-by-2-proportioned rectangular farm plot with exactly3 broad crop strips, bottom middle one square kitchen with squat chimney and a small attached work shelter, right middle one simple rectangular tavern with small pictorial hanging sign. These are the existing objects only: houses5,kitchen1,farm1,tavern1. SIX small adult inhabitants distributed on visible open ground, never above buildings. No extra buildings. No roads or fences that block ground movement. Quiet grass surface with minimal broad color variation, no scattered debris or dense repetitive grass marks. Clusters of trees only at outer boundary, no scenic hills.
Composition: every building uncropped, same orthographic world scale in foreground and background. Majority of image usable flat ground plane and roofs. Front doors mostly hidden by overhead eaves, do not distort camera merely to show off doors. The camera must prioritize ground-space occupation rather than selling individual building facades. One floor only, low wall heights, broad low pitched roofs. Don't bring houses forward to make them look tall.
Style: warm clean adventure illustration from provided reference. Ivory plaster, terracotta red roofs, deep brown beams, thin controlled dark edges, large grouped tile shapes, soft neutral upper-left daylight. Functional game-map readability. No UI panels, labels, portraits, smoke, fire, or work-state glows.
```

## house_r2_high（高俯视单体实验，未实机验证）
```text
undefined
```

