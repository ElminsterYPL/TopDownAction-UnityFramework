using System;
using System.Collections.Generic;
using System.Numerics;

namespace DesignerTables
{
    public class Buff
    {
        public static Dictionary<string, BuffModel> Data = new Dictionary<string, BuffModel>()
        {
            //无法受到伤害（无敌）
            { "CantBeHurt", new BuffModel( "CantBeHurt", "无法受到伤害", new string[]{"Passive"}, 100, 1, 0,
                "", Array.Empty<object>(),  //occur
                "", Array.Empty<object>(),  //remove
                "", Array.Empty<object>(),  //tick
                "", Array.Empty<object>(),  //cast
                "", Array.Empty<object>(),  //hit
                "CantBeHurt", Array.Empty<object>(),  //hurt
                "", Array.Empty<object>(),  //kill
                "", Array.Empty<object>(),  //dead
                null
            )},
            
            //每帧向面向方向移动（移动距离参数暂设为0.3f）
            { "ConstantMoveToFaceDir", new BuffModel( "ConstantMoveToFaceDir", "持续向面向方向移动", new string[]{"Passive"}, 0, 1, 0.02f,
                "", Array.Empty<object>(),  //occur
                "", Array.Empty<object>(),  //remove
                "MoveToFaceDir", new object[1]{0.30f},  //tick
                "", Array.Empty<object>(),  //cast
                "", Array.Empty<object>(),  //hit
                "", Array.Empty<object>(),  //hurt
                "", Array.Empty<object>(),  //kill
                "", Array.Empty<object>(),  //dead
                null
            )},

            //每帧向某方向移动（移动距离参数暂设为0.3f）。具体方向需要附加buff时调整tickParam的第二个参数。
            { "ConstantMove", new BuffModel( "ConstantMove", "持续移动", new string[]{"Passive"}, 0, 1, 0.02f,
                "", Array.Empty<object>(),  //occur
                "", Array.Empty<object>(),  //remove
                "Move", new object[2]{0.30f, Vector3.Zero},  //tick
                "", Array.Empty<object>(),  //cast
                "", Array.Empty<object>(),  //hit
                "", Array.Empty<object>(),  //hurt
                "", Array.Empty<object>(),  //kill
                "", Array.Empty<object>(),  //dead
                null
            )},
            
            // //每帧向某方向移动（移动距离参数暂设为0.3f）。具体方向需要附加buff时调整tickParam的第二个参数。
            // { "ConstantMove", new BuffModel( "ConstantMove", "持续移动", new string[]{"Passive"}, 0, 1, 0.02f,
            //     "", Array.Empty<object>(),  //occur
            //     "", Array.Empty<object>(),  //remove
            //     "Move", new object[2]{0.30f, Vector3.Zero},  //tick
            //     "", Array.Empty<object>(),  //cast
            //     "", Array.Empty<object>(),  //hit
            //     "", Array.Empty<object>(),  //hurt
            //     "", Array.Empty<object>(),  //kill
            //     "", Array.Empty<object>(),  //dead
            //     null
            // )},
        };
    }
}