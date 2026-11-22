using UnityEngine;

/// <summary>Candidate B art canvas margins. Presentation only; not a building footprint authority.</summary>
internal static class HomeArtCalibration
{
    internal static float CanvasWidth(string definition,float width,float depth)
        => (width+depth)*.85f/(definition=="building_farm" ? .83f : .66f);
    internal static float AnchorY(string definition) => definition=="building_farm" ? .54f : .70f;
}
