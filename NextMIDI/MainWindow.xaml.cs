using NextMidi.Data.Domain;
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
        MidiFileDomain domain;

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

            var midiData = MidiReader.ReadFrom(@"C:\Users\inuga_000\Desktop\c4.mid", Encoding.GetEncoding("shift-jis"));
            // テンポマップを作成
            domain = new MidiFileDomain(midiData);
            player = new MidiPlayer(port);
            player.Play(domain);
            
        }

        private void MIDIOn_Click(object sender, RoutedEventArgs e)
        {
            player.Play(domain);
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
    }
}
