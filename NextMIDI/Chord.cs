using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextMidi.DataElement;

namespace NextMIDI
{
    
    class Chord
    {
        public NoteEvent[] Notes;
        public NoteEvent Base;
        public int Tick;
        public float Pivot; // コードの高さ(三和音の平均値)を「コードの軸」と呼ぶことにする
        public int PivotRange; // コードの軸のある範囲(Range1～Range5)


        // NoteEventクラスのコンストラクタ：NoteEvent(byte note, byte velocity, int gate)
        public Chord(int tick, byte root)
        {
            Tick = tick;
            Base = new NoteEvent((byte)(root - 24), 80, 240 * 4);
            Base.Tick = tick;

            Notes = new NoteEvent[4];
            Notes[0] = new NoteEvent(root, 80, 240 * 4);
            Notes[1] = new NoteEvent((byte)(root + 4), 80, 240 * 4);
            Notes[2] = new NoteEvent((byte)(root + 7), 80, 240 * 4);
            Notes[3] = new NoteEvent((byte)(root - 24), 80, 240 * 4);
            Notes[0].Tick = tick;
            Notes[1].Tick = tick;
            Notes[2].Tick = tick;
            Notes[3].Tick = tick;

            Pivot = (Notes[0].Note + Notes[1].Note + Notes[2].Note) / 3;

            if (Pivot < 52) PivotRange = 1;
            else if (52 <= Pivot && Pivot < 56) PivotRange = 2;
            else if (56 <= Pivot && Pivot < 60) PivotRange = 3;
            else if (60 <= Pivot && Pivot < 64) PivotRange = 4;
            else if (64 <= Pivot) PivotRange = 5;
        }

        public Chord(int tick, byte root, String mode)
        {
            Tick = tick;
            Base = new NoteEvent((byte)(root - 24), 80, 240 * 4);
            Base.Tick = tick;
            
            Notes = new NoteEvent[4];

           
            Notes[0] = new NoteEvent(root, 80, 240 * 4);
            Notes[1] = new NoteEvent((byte)(root + 4), 80, 240 * 4);
            Notes[2] = new NoteEvent((byte)(root + 7), 80, 240 * 4);
            Notes[3] = new NoteEvent((byte)(root - 24), 80, 240 * 4);
            Notes[0].Tick = tick;
            Notes[1].Tick = tick;
            Notes[2].Tick = tick;
            Notes[3].Tick = tick;
            
            if (mode.Equals("m"))
            { 
                Notes[1] = new NoteEvent((byte)(root + 3), 80, 240 * 4);
                Notes[1].Tick = tick;
            }  

            if (mode.Equals("7"))
            {
                Notes[3] = new NoteEvent((byte)(root + 10), 80, 240 * 4);
                Notes[3].Tick = tick;
            }

            Pivot = (Notes[0].Note + Notes[1].Note + Notes[2].Note) / 3;

            if (Pivot < 44) PivotRange = -1;
            else if (44 <= Pivot && Pivot < 48) PivotRange = 0;
            else if (48 <= Pivot && Pivot < 52) PivotRange = 1;
            else if (52 <= Pivot && Pivot < 56) PivotRange = 2;
            else if (56 <= Pivot && Pivot < 60) PivotRange = 3;
            else if (60 <= Pivot && Pivot < 64) PivotRange = 4;
            else if (64 <= Pivot && Pivot < 68) PivotRange = 5;
            else if (68 <= Pivot && Pivot < 72) PivotRange = 6;
            else if (72 <= Pivot) PivotRange = 7;

        }

        
    }
}
