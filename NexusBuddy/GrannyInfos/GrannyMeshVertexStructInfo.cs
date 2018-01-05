using System;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyInfos
{
    public class GrannyMeshVertexStructInfo
    {
		public int type;
		public int count;
		public string name;
        public int length;
        public int offset;

        public GrannyMeshVertexStructInfo(int itype, string iname, int icount)
        {
            type = itype;
            name = iname;
            count = icount;
            length = getLength();
        }

        private int getLength() {
	        switch(type) {
		        case 10: return 4;  // 32bit float
                case 11: return 1;  // granny_int8
                case 12: return 1;  // granny_uint8
		        case 14: return 1;  // granny_uint8
		        case 21: return 2;  // 16bit float
		        default: return 0;
	        }
        }

        public unsafe Object convert(int pointer)
        {
            switch (type)
            {
                case 10: return *(float*)pointer;  
                //case 11: return 1;  // granny_int8
                case 12: return *(byte*)pointer;  // granny_uint8
                case 14: return *(byte*)pointer;  // granny_uint8
                case 21: return NumberUtils.halfToFloat(*(UInt16*)pointer);  // 16bit float
                default: return null;
            }  
        }

        public unsafe void* convertBack(Object number)
        {
            switch (type)
            {
                case 10: 
                    float f = (float)number;
                    return &f;
                //case 11: return 1;  // granny_int8
                case 12:
                    byte b = (byte)(int)number;
                    return &b;  // granny_uint8
                case 14:
                    byte b2 = (byte)(int)number;
                    return &b2;  // granny_uint8
                case 21:
                   // UInt16 half = NumberUtils.floatToHalf((float)number);
                   // return &half; // 16bit float
                    return null;
                default: return null;
            }
        }
    }
}
