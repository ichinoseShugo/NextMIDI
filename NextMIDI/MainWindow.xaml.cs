using NextMidi.Data;
using NextMidi.Data.Domain;
using NextMidi.Data.Track;
using NextMidi.DataElement;
using NextMidi.Filing.Midi;
using NextMidi.MidiPort.Output;
using NextMidi.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NextMIDI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MidiOutPort port;
        MidiPlayer player;
        MidiFileDomain domain1;
        int tickUnit = 240 * 4;
        bool WholeFlag = true;
        bool ArpFlag = false;
        bool Free = false;

        List<Chord> chordlist = new List<Chord>(); // chordlistというChordクラスのリストを用意


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitMIDI(0);
            CompositionTarget.Rendering += Render;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// MIDIの初期化
        /// </summary>
        /// <param name="portnum"></param>
        void InitMIDI(int portnum)
        {

            // Midiデータの作成
            String[] chordStrs = { "C", "Am", "F", "G", "Em", "F", "G", "C" };

            

            MidiData mid = new MidiData();
            MidiTrack track = new MidiTrack();
            mid.Tracks.Add(track);
            // System.Console.WriteLine("tracks.count: " + mid.Tracks.Count);
           

            int tick = 0;
            foreach (String chordStr in chordStrs)
            {
                byte root = 60;
                if (chordStr.StartsWith("C#") || chordStr.StartsWith("Db"))
                {
                    root = 61;
                }
                else if (chordStr.StartsWith("D"))
                {
                    root = 62;
                }
                else if(chordStr.StartsWith("D#")||chordStr.StartsWith("Eb"))
                {
                    root = 63;
                }
                else if (chordStr.StartsWith("E"))
                {
                    root = 64;
                }
                else if (chordStr.StartsWith("F"))
                {
                    root = 65;
                }
                else if (chordStr.StartsWith("F#") || chordStr.StartsWith("Gb"))
                {
                    root = 66;
                }
                else if (chordStr.StartsWith("G"))
                {
                    root = 55;
                }
                else if (chordStr.StartsWith("G#") || chordStr.StartsWith("Ab"))
                {
                    root = 56;
                }
                else if (chordStr.StartsWith("A"))
                {
                    root = 57;
                }
                else if (chordStr.StartsWith("A#") || chordStr.StartsWith("Bb"))
                {
                    root = 58;
                }
                else if (chordStr.StartsWith("B"))
                {
                    root = 59;
                }
                String mode = "M";
                if (chordStr.Length > 1) {
                    mode = chordStr.Substring(1, 1);
                    // ToDoMemo:ハーフディミニッシュ、ディミニッシュなども取得可能にすること。
                }
              
                Chord ch = new Chord(tick, root, mode); // newでインスタンスを作成し、変数chに格納
                track.Insert(ch.Base);
                track.Insert(ch.Notes[0]);
                track.Insert(ch.Notes[1]);
                track.Insert(ch.Notes[2]);
                track.Insert(ch.Notes[3]);
            
                
                chordlist.Add(ch); // chをリストchordlistに追加
                tick += tickUnit;

            }

            
            // 各chordlistの各構成音を表示
            for(int i = 0; i < chordlist.Count; i++)
            {
                System.Console.WriteLine("chordlist[" + i +"]");
                System.Console.WriteLine("Note[0]=" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Note[1]=" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Note[2]=" + chordlist[i].Notes[2].Note);
                System.Console.WriteLine("Pivot=" + chordlist[i].Pivot);
                System.Console.WriteLine("PivotRange=" + chordlist[i].PivotRange + "\n");
            }

            
            // 全コードのPivotRangeを一度4に統一する
            /*
            for(int i = 0; i < chordlist.Count; i++)
            {
                System.Console.WriteLine(i + "\nbefore PivotRange:" + chordlist[i].PivotRange);
                System.Console.WriteLine("Notes[0]:" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Notes[1]:" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Notes[2]:" + chordlist[i].Notes[2].Note);
                switch (4 - chordlist[i].PivotRange)
                {
                    case 4:
                        Turn(4);
                        break;
                    case 3:
                        Turn(3);
                        break;
                    case 2:
                        Turn(2);
                        break;
                    case 1:
                        Turn(1);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1);
                        break;
                    case -2:
                        Turn(-2);
                        break;
                    case -3:
                        Turn(-3);
                        break;
                    case -4:
                        Turn(-4);
                        break;
                }
                System.Console.WriteLine("after PivotRange:" + chordlist[i].PivotRange);
                System.Console.WriteLine("Notes[0]:" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Notes[1]:" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Notes[2]:" + chordlist[i].Notes[2].Note + "\n");
            }
            */



            port = new MidiOutPort(portnum);
            try
            {
                port.Open();
            }
            catch
            {
                Console.WriteLine("no such port exists");
                return;
            }

            //楽器を変更
            //port.Send(new ProgramEvent(2));
            //note = new NoteEvent(69,112,1,100);

            //midiファイルを読み込み
            //midiファイルの場所  @"C:\user" or "C:\\user" or "C:/user"
            // var mid = MidiReader.ReadFrom("C:\\Users\\ri200\\Music\\EW_WholeNote.mid", Encoding.GetEncoding("shift-jis"));

            // テンポマップを作成
            domain1 = new MidiFileDomain(mid);
            player = new MidiPlayer(port);





            /* 
             * メモ
             * 
             * ・四分音符
             * ・テンションスイッチ
             * ・リズム音の追加
             * 
             * 
             * ・コードの高さについて
             * 
             */


            // 全ての MIDI ノートを 4 半音上げる
            /*
            foreach (var track1 in mid.Tracks)
            {
                foreach (var note in track1.GetData<NoteEvent>())
                {
                    note.Note += 4;  // 音の高さを4半音上げる
                    // System.Console.WriteLine("Note = " + note.Note); //音の高さ
                    // System.Console.WriteLine("Tick = " + note.Tick); //音の開始時刻
                    // System.Console.WriteLine("Gate = " + note.Gate); //音の長さ
                            
                }
            } 
            */

        }
    


        // *****音源再生ボタン*****
        private void MIDIOn_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            player.Play(domain1);

            
        }

        // *****音源終了ボタン*****
        private void MIDIOff_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }


        void Render(object sender, EventArgs e)
        {
            //if (player.Playing == false) player.Play(domain);
            //Check.Text = player.MusicTime.Tick+","+player.Time+",";
            if (player.MusicTime.Tick >= 100)
            {
                //player.Stop();
                //player.Play(domain);
            }
        }



        // *****全音符(WholeNote)*****
        private void Rbtn1_Checked(object sender, RoutedEventArgs e)
        {
            

            WholeFlag = true;
            
            if (ArpFlag == true) // アルペジオ伴奏の場合
            {
                for(int i = 0; i < chordlist.Count; i++)
                {
                    chordlist[i].Notes[0].Gate = 240 * 4;
                    chordlist[i].Notes[1].Gate = 240 * 4;
                    chordlist[i].Notes[2].Gate = 240 * 4;
                    chordlist[i].Notes[1].Tick -= 240;
                    chordlist[i].Notes[2].Tick -= 480;
                    chordlist[i].Notes[3].Note = chordlist[i].Base.Note;
                    chordlist[i].Notes[3].Gate = 240 * 4;
                    chordlist[i].Notes[3].Tick -= 720;
                }
                ArpFlag = false;
            }
        }
        
        // *****分散和音(Arpeggioボタン)*****
        private void Rbtn2_Checked(object sender, RoutedEventArgs e)
        {
            // Noteの小さい順に鳴らすようにする？
            // [3]には一番低い音の+12となる数値を代入する？

            // MEMO...Gate:音の長さ  Tick:発音時刻  Velocity:音の大きさ
            ArpFlag = true;
            if(WholeFlag == true) // 全音符伴奏の場合
            {
                for(int i = 0; i < chordlist.Count; i++)
                {
                    chordlist[i].Notes[0].Gate = 240;
                    chordlist[i].Notes[1].Gate = 240;
                    chordlist[i].Notes[2].Gate = 240;
                    chordlist[i].Notes[1].Tick += 240;
                    chordlist[i].Notes[2].Tick += 480;
                    chordlist[i].Notes[3].Note = chordlist[i].Notes[0].Note;
                    chordlist[i].Notes[3].Gate = 240;
                    chordlist[i].Notes[3].Tick += 720;
                    
                }
                WholeFlag = false;
            }
            

           // player.Stop();
           // player.Play(domain2);
        }


　　　　// *****任意タイミングでの発音(Freeボタン)*****
        private void Rbtn3_Checked(object sender, RoutedEventArgs e)
        {
            Free = true;
            for (int i = 0; i < chordlist.Count; i++)
            {
                Rbtn1_Checked(sender, e); // Alpggioの場合一度WholeNoteに戻す

                chordlist[i].Notes[0].Velocity = 0;
                chordlist[i].Notes[1].Velocity = 0;
                chordlist[i].Notes[2].Velocity = 0;
                chordlist[i].Notes[3].Velocity = 0;
            }            
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            // タップで発音
            if(Free == true)
            {
                //この時の和音だけを鳴らす。
                MusicTime current = player.MusicTime; // 現在(Tap時)の演奏カーソルを取得
                System.Console.WriteLine("curent 小節: "+current.Measure+", Tick: "+ current.Tick );
                System.Console.WriteLine("Notes.Tick:" + chordlist[current.Measure].Notes[0].Tick);

                chordlist[current.Measure].Notes[0].Tick = tickUnit * current.Measure + current.Tick + 1;
                chordlist[current.Measure].Notes[1].Tick = tickUnit * current.Measure + current.Tick + 1;
                chordlist[current.Measure].Notes[2].Tick = tickUnit * current.Measure + current.Tick + 1;
                chordlist[current.Measure].Notes[3].Tick = tickUnit * current.Measure + current.Tick + 1;
                chordlist[current.Measure].Notes[0].Velocity = 80;
                chordlist[current.Measure].Notes[1].Velocity = 80;
                chordlist[current.Measure].Notes[2].Velocity = 80;
                chordlist[current.Measure].Notes[3].Velocity = 80;

                chordlist[current.Measure].Notes[0].Gate = 240;
                chordlist[current.Measure].Notes[1].Gate = 240;
                chordlist[current.Measure].Notes[2].Gate = 240;
                chordlist[current.Measure].Notes[3].Gate = 240;

                chordlist[current.Measure].Notes[0].Speed = 120;
                chordlist[current.Measure].Notes[1].Speed = 120;
                chordlist[current.Measure].Notes[2].Speed = 120;
                chordlist[current.Measure].Notes[3].Speed = 120;

            }
        }




        // *****次の小節の高さ(PivotRange)の決定*****
        int Range;

        //  Range1 : pivot < 52
        private void TurnRbtn1_Checked(object sender, RoutedEventArgs e)
        {
            Range = 1;
            MusicTime current = player.MusicTime;

            System.Console.WriteLine("Range = "+Range+"\n chordlist[次の小節].PivotRange" + chordlist[current.Measure + 1].PivotRange);
            for (int i = current.Measure + 1; i < chordlist.Count; i++)
            {
                switch (Range - chordlist[i].PivotRange)
                {
                    case 2:
                        Turn(2, i);
                        break;
                    case 1:
                        Turn(1, i);
                        break;
                    case 0:
                        // 次の小節のコードのPivotRangeと指定Range(=1)が同じ
                        break;
                    case -1:
                        // 次の小節のコードのPivotRangeが指定Range(=1)より1高い
                        // Turnメソッドで-1転回する
                        Turn(-1, i);
                        break;
                    case -2:
                        Turn(-2, i);
                        break;
                    case -3:
                        Turn(-3, i);
                        break;
                    case -4:
                        Turn(-4, i);
                        break;
                    case -5:
                        Turn(-5, i);
                        break;
                    case -6:
                        Turn(-6, i);
                        break;

                }
                System.Console.WriteLine(i + "小節目のコードのPivotRangeは" + chordlist[i].PivotRange);
            }

        }

        // Range2 : 52 <= pivot < 56
        private void TurnRbtn2_Checked(object sender, RoutedEventArgs e)
        {
            Range = 2;
            MusicTime current = player.MusicTime;

            for (int i = current.Measure + 1; i < chordlist.Count; i++)
            {
                switch (Range - chordlist[i].PivotRange)
                {
                    case 3:
                        Turn(3, i);
                        break;
                    case 2:
                        Turn(2, i);
                        break;
                    case 1:
                        Turn(1, i);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1, i);
                        break;
                    case -2:
                        Turn(-2, i);
                        break;
                    case -3:
                        Turn(-3, i);
                        break;
                    case -4:
                        Turn(-4, i);
                        break;
                    case -5:
                        Turn(-5, i);
                        break;
                }
                System.Console.WriteLine(i + "小節目のコードのPivotRangeは" + chordlist[i].PivotRange);
            }
        }

        // Range3 : 56 <= pivot < 60
        private void TurnRbtn3_Checked(object sender, RoutedEventArgs e)
        {
            Range = 3;
            MusicTime current = player.MusicTime;

            for (int i = current.Measure + 1; i < chordlist.Count; i++)
            {
                switch (Range - chordlist[i].PivotRange)
                {
                    case 4:
                        Turn(4, i);
                        break;
                    case 3:
                        Turn(3, i);
                        break;
                    case 2:
                        Turn(2, i);
                        break;
                    case 1:
                        Turn(1, i);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1, i);
                        break;
                    case -2:
                        Turn(-2, i);
                        break;
                    case -3:
                        Turn(-3, i);
                        break;
                    case -4:
                        Turn(-4, i);
                        break;
                }
                System.Console.WriteLine(i + "小節目のコードのPivotRangeは" + chordlist[i].PivotRange);
            }
        }

        // Range4 : 60 <= pivot < 64
        private void TurnRbtn4_Checked(object sender, RoutedEventArgs e)
        {
            Range = 4;
            MusicTime current = player.MusicTime;

            for (int i = current.Measure + 1; i < chordlist.Count; i++)
            {
                switch (Range - chordlist[i].PivotRange)
                {
                    case 5:
                        Turn(5, i);
                        break;
                    case 4:
                        Turn(4, i);
                        break;
                    case 3:
                        Turn(3, i);
                        break;
                    case 2:
                        Turn(2, i);
                        break;
                    case 1:
                        Turn(1, i);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1, i);
                        break;
                    case -2:
                        Turn(-2, i);
                        break;
                    case -3:
                        Turn(-3, i);
                        break;
                }
                System.Console.WriteLine(i + "小節目のコードのPivotRangeは" + chordlist[i].PivotRange);
                System.Console.WriteLine("Note[0]:" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Note[1]:" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Note[2]:" + chordlist[i].Notes[2].Note);

            }
        }

        // Range5 : 64 <= pivot
        private void TurnRbtn5_Checked(object sender, RoutedEventArgs e)
        {
            Range = 5;
            MusicTime current = player.MusicTime;

            for (int i = current.Measure + 1; i < chordlist.Count; i++)
            {
                switch (Range - chordlist[i].PivotRange)
                {
                    case 6:
                        Turn(6, i);
                        break;
                    case 5:
                        Turn(5, i);
                        break;
                    case 4:
                        Turn(4, i);
                        break;
                    case 3:
                        Turn(3, i);
                        break;
                    case 2:
                        Turn(2, i);
                        break;
                    case 1:
                        Turn(1, i);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1, i);
                        break;
                    case -2:
                        Turn(-2, i);
                        break;
                }
                System.Console.WriteLine(i + "小節目のコードのPivotRangeは" + chordlist[i].PivotRange);
            }
        }


        // *****転回メソッド : i番目のコードをk回だけ転回する*****
        
        private void Turn(int k, int i)
        {
            if (k > 0) // +k転回
            {
                for (int j = 0; j < k; j++)
                {
                    // Notes[0]からNotes[3]のNoteうち、要素の値が一番小さいNotes[].Noteを一オクターブ上げる
                    int min = chordlist[i].Notes[0].Note;
                    int minIndex = 0;
                    if (min > chordlist[i].Notes[1].Note)
                    {
                        min = chordlist[i].Notes[1].Note;
                        minIndex = 1;
                    }
                    if (min > chordlist[i].Notes[2].Note)
                    {
                        min = chordlist[i].Notes[2].Note;
                        minIndex = 2;
                    }
                    // int MinIndex = Array.IndexOf(chordlist[i].Notes, max);
                    chordlist[i].Notes[minIndex].Note += 12;
                }

                // Pivot, PivotRangeの更新
                chordlist[i].Pivot = (chordlist[i].Notes[0].Note + chordlist[i].Notes[1].Note + chordlist[i].Notes[2].Note) / 3;

                if (chordlist[i].Pivot < 44) chordlist[i].PivotRange = -1;
                else if (44 <= chordlist[i].Pivot && chordlist[i].Pivot < 48) chordlist[i].PivotRange = 0;
                else if (48 <= chordlist[i].Pivot && chordlist[i].Pivot < 52) chordlist[i].PivotRange = 1;
                else if (52 <= chordlist[i].Pivot && chordlist[i].Pivot < 56) chordlist[i].PivotRange = 2;
                else if (56 <= chordlist[i].Pivot && chordlist[i].Pivot < 60) chordlist[i].PivotRange = 3;
                else if (60 <= chordlist[i].Pivot && chordlist[i].Pivot < 64) chordlist[i].PivotRange = 4;
                else if (64 <= chordlist[i].Pivot && chordlist[i].Pivot < 68) chordlist[i].PivotRange = 5;
                else if (68 <= chordlist[i].Pivot && chordlist[i].Pivot < 72) chordlist[i].PivotRange = 6;
                else if (72 <= chordlist[i].Pivot) chordlist[i].PivotRange = 7;
            }

            if (k < 0) // -k転回
            {
                k = k * (-1);
                for (int j = 0; j < k; j++)
                {
                    // Notes[0]からNotes[3]のNoteうち、要素の値が一番小さいNotes[].Noteを一オクターブ上げる
                    int max = chordlist[i].Notes[0].Note;
                    int maxIndex = 0;
                    if (max < chordlist[i].Notes[1].Note)
                    {
                        max = chordlist[i].Notes[1].Note;
                        maxIndex = 1;
                    }
                    if (max < chordlist[i].Notes[2].Note)
                    {
                        max = chordlist[i].Notes[2].Note;
                        maxIndex = 2;
                    }
                    // int MinIndex = Array.IndexOf(chordlist[i].Notes, max);
                    chordlist[i].Notes[maxIndex].Note -= 12;
                }

                // Pivot, PivotRangeの更新
                chordlist[i].Pivot = (chordlist[i].Notes[0].Note + chordlist[i].Notes[1].Note + chordlist[i].Notes[2].Note) / 3;

                if (chordlist[i].Pivot < 44) chordlist[i].PivotRange = -1;
                else if (44 <= chordlist[i].Pivot && chordlist[i].Pivot < 48) chordlist[i].PivotRange = 0;
                else if (48 <= chordlist[i].Pivot && chordlist[i].Pivot < 52) chordlist[i].PivotRange = 1;
                else if (52 <= chordlist[i].Pivot && chordlist[i].Pivot < 56) chordlist[i].PivotRange = 2;
                else if (56 <= chordlist[i].Pivot && chordlist[i].Pivot < 60) chordlist[i].PivotRange = 3;
                else if (60 <= chordlist[i].Pivot && chordlist[i].Pivot < 64) chordlist[i].PivotRange = 4;
                else if (64 <= chordlist[i].Pivot && chordlist[i].Pivot < 68) chordlist[i].PivotRange = 5;
                else if (68 <= chordlist[i].Pivot && chordlist[i].Pivot < 72) chordlist[i].PivotRange = 6;
                else if (72 <= chordlist[i].Pivot) chordlist[i].PivotRange = 7;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 全コードのPivotRangeを4に統一したい
            // 　
            /*
            for (int i = 0; i < chordlist.Count; i++)
            {
                System.Console.WriteLine(i + "\nbefore PivotRange:" + chordlist[i].PivotRange);
                System.Console.WriteLine("Notes[0]:" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Notes[1]:" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Notes[2]:" + chordlist[i].Notes[2].Note);
                switch (4 - chordlist[i].PivotRange)
                {
                    case 4:
                        Turn(4);
                        break;
                    case 3:
                        Turn(3);
                        break;
                    case 2:
                        Turn(2);
                        break;
                    case 1:
                        Turn(1);
                        break;
                    case 0:
                        break;
                    case -1:
                        Turn(-1);
                        break;
                    case -2:
                        Turn(-2);
                        break;
                    case -3:
                        Turn(-3);
                        break;
                    case -4:
                        Turn(-4);
                        break;
                }
                System.Console.WriteLine("after PivotRange:" + chordlist[i].PivotRange);
                System.Console.WriteLine("Notes[0]:" + chordlist[i].Notes[0].Note);
                System.Console.WriteLine("Notes[1]:" + chordlist[i].Notes[1].Note);
                System.Console.WriteLine("Notes[2]:" + chordlist[i].Notes[2].Note + "\n");
            }
            */
        }
        
    }
}



/*やること
 * 
 * 転回系の書き換え
 * →turn1からのturn0とか
 * ※数字で指定できるように。
 * SDKのことを質問する
 * 
 * 
 * 配列の書き換え
 */
