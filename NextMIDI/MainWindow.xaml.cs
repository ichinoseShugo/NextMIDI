using NextMidi.DataElement;
using NextMidi.MidiPort.Output;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitMIDI(0);
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

            // Program No.5 に切り替え
            port.Send(new ProgramEvent(4));
        }

        private void MIDIOn_Click(object sender, RoutedEventArgs e)
        {
            // ドレミファソラシド
            foreach (byte n in new byte[8] { 60, 62, 64, 65, 67, 69, 71, 72 })
            {
                // ベロシティ 112 でノートオンを送信
                port.Send(new NoteOnEvent(n, 112));
                Thread.Sleep(n != 72 ? 500 : 1500);
                port.Send(new NoteOffEvent(n));
            }
        }
    }
}
