using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialsorva.FCEEasyMods
{
    public static class CoordNeighborhood
    {

        public static IEnumerator<CubeCoord> CubeWithRadius(int r)
        {
            for (int iy = 0; iy <= r; ++iy)
            {
                for(int iz = 0; iz <= r; ++iz)
                {
                    for(int ix = 0; ix <= r; ++ix)
                    {
                        if (ix != 0 || iy != 0 || iz != 0)
                        {
                            yield return new CubeCoord(ix, iy, iz);
                        }
                    }
                }
            }
        }
        
        public static CubeCoord[] ADJACENT = new CubeCoord[] {
            new CubeCoord(1,0,0),
            new CubeCoord(-1,0,0),
            new CubeCoord(0,1,0),
            new CubeCoord(0,-1,0),
            new CubeCoord(0,0,1),
            new CubeCoord(0,0,-1),
        };
    }
}
