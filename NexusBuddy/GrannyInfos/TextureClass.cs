using System.Collections.Generic;

namespace NexusBuddy.GrannyInfos
{
    public class CookParam
    {
        public string name;
        public string defaultVal;

        public CookParam(string defaultVal, string name)
        {
            this.name = name;
            this.defaultVal = defaultVal;
        }
    }

    public class TextureClass
    {
        public string PixelFormat;
        public string MipFilter;
        public float MipSupportScale;
        public int MaxWidth;
        public int MaxHeight;
        public int MaxDepth;
        public int MinWidth;
        public int MinHeight;
        public int MinDepth;
        public float ExportGammaIn;
        public float ExportGammaOut;
        public float ExportClampMin;
        public float ExportClampMax;
        public bool AllowArtistMips;
        public bool RequireSquare;
        public bool RequirePow2;
        public string Type;
        public List<CookParam> CookParams;

        public TextureClass(string pixelFormat, string mipFilter, float mipSupportScale, int maxWidth, int maxHeight, int maxDepth, int minWidth, int minHeight, int minDepth, float exportGammaIn, float exportGammaOut, float exportClampMin, float exportClampMax, bool allowArtistMips, bool requireSquare, bool requirePow2, string type, List<CookParam> cookParams)
        {
            PixelFormat = pixelFormat;
            MipFilter = mipFilter;
            MipSupportScale = mipSupportScale;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            MaxDepth = maxDepth;
            MinWidth = minWidth;
            MinHeight = minHeight;
            MinDepth = minDepth;
            ExportGammaIn = exportGammaIn;
            ExportGammaOut = exportGammaOut;
            ExportClampMin = exportClampMin;
            ExportClampMax = exportClampMax;
            AllowArtistMips = allowArtistMips;
            RequireSquare = requireSquare;
            RequirePow2 = requirePow2;
            Type = type;
            CookParams = cookParams;
        }

        public static Dictionary<string, TextureClass> GetAllTextureClasses()
        {
            return new Dictionary<string, TextureClass>{
{"ColorKey", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,4096,4,4,4,1.000000f,1.000000f,0.000000f,1.000000f,false,false,false,"3D_COLORKEY",new List<CookParam>(new CookParam[]{}))},
{"Decal_BaseColor", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,32,32,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Decal_FOWColor", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,32,32,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Decal_Heightmap", new TextureClass("R8G8B8A8_UNORM","B_SPLINE",1.000000f,4096,4096,1,32,32,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("0.000000","MinHeight"),new CookParam("255.000000","MaxHeight")}))},
{"Decal_Spec", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,32,32,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"FOW", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"FOWGreyscale", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"FOWSprite", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_AO", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_BaseColor", new TextureClass("R8G8B8A8_UNORM","KAISER",0.500000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_BurnMap", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_Emissive", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_Gloss", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_LightMap", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_Metalness", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_Normal", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_OPAC", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Generic_TintMask", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Leader_Anisotropy", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("4","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_AO", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_BaseColor", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,1.000000f,0.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("false","Skip Compression"),new CookParam("3","Quality Loss - Luma"),new CookParam("4","Quality Loss - Chroma"),new CookParam("0","Resolution Loss - Luma"),new CookParam("3","Resolution Loss - Chroma")}))},
{"Leader_BlurWidth", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Fallback", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,false,"2D",new List<CookParam>(new CookParam[]{}))},
{"Leader_FilmGrain", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,false,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Leader_Fuzz", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Gloss", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Metalness", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Normal", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_OPAC", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Tangent", new TextureClass("R8G8B8A8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Tint", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Leader_Translucency", new TextureClass("R8_UNORM","BOX",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("3","Quality Loss"),new CookParam("false","Skip Compression")}))},
{"Overlay", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"SkyboxTexture2D", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,false,"2D",new List<CookParam>(new CookParam[]{}))},
{"StrategicView_CultureBorder", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,256,256,1,256,256,1,1.000000f,1.000000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"StrategicView_Riverbank", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,256,256,1,256,256,1,1.000000f,1.000000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"StrategicView_Sprite", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,false,"2D",new List<CookParam>(new CookParam[]{}))},
{"StrategicView_TerrainBlend", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,256,256,1,256,256,1,1.000000f,1.000000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"StrategicView_TerrainType", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,false,"2D",new List<CookParam>(new CookParam[]{}))},
{"Terrain_BaseColor", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,32,32,1,2.200000f,2.200000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Terrain_EditHeightmap", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,1024,1024,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,false,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Terrain_FOWColor", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,32,32,1,2.200000f,2.200000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Terrain_Fuzz", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,32,32,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"Terrain_Heightmap", new TextureClass("R8_UNORM","B_SPLINE",1.000000f,4096,4096,1,32,32,1,1.000000f,1.000000f,0.000000f,1.000000f,true,true,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("0.000000","MinHeight"),new CookParam("255.000000","MaxHeight")}))},
{"Terrain_Spec", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,32,32,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"TerrainElementBlendmap", new TextureClass("A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"TerrainElementHeightmap", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,false,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("10.000000","HeightScale"),new CookParam("16.000000","BaseHeight")}))},
{"TerrainElementIDMap", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,false,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"TerrainElementNoise2D", new TextureClass("R8G8_UNORM","TENT",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{new CookParam("1.000000","Scale")}))},
{"UserInterface", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,2,2,1,2.200000f,2.200000f,0.000000f,1.000000f,false,false,false,"2D",new List<CookParam>(new CookParam[]{new CookParam("false","IsScalable"),new CookParam("2","TexturePadding"),new CookParam("false","IsStandalone"),new CookParam("false","IsTiled")}))},
{"VFXParticle_BaseColor", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"VFXParticle_Mask", new TextureClass("R8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"VFXParticle_Ramp", new TextureClass("R8G8B8A8_UNORM","GAUSSIAN",1.000000f,4096,4096,1,4,4,1,2.200000f,2.200000f,0.000000f,1.000000f,false,false,true,"2D",new List<CookParam>(new CookParam[]{}))},
{"WaterDensityMap", new TextureClass("R8G8B8A8_UNORM","LANCZOS6",1.000000f,4096,4096,1,4,4,1,1.000000f,1.000000f,0.000000f,1.000000f,true,false,true,"2D",new List<CookParam>(new CookParam[]{}))}
        };
        }
        
    }
}
