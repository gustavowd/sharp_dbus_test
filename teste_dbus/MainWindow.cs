using System;
using Gtk;

using DBus;
using org.freedesktop.DBus;

using System.Threading;


public partial class MainWindow : Gtk.Window
{
    ITestInterface sample;
    ITestInterface2 sample2;
    ITestInterface sample3;

    // dispara uma nova thread para executar
    Thread t;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        t = new Thread(NovaThread);
        t.Start();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnButton1Clicked(object sender, EventArgs e)
    {

        ObjectPath opath = new ObjectPath("/org/example/TestObject");
        string name = "org.example.TestServer";

        Bus bus = Bus.Session;
        sample = bus.GetObject<ITestInterface>(name, opath);

        int teste = sample.Adder(5, 12);
        label1.Text = Convert.ToString(teste);
        label2.Text = sample.Ping();
        string teste3 = sample.Echo("Hello dbus");
    }

    private void Store_OnEmitSignal()
    {
        label4.Text = "Signal received!";
    }


    public delegate void OnEmitSignalHandler();


    [Interface("org.example.TestInterface")]
    public interface ITestInterface : Introspectable
    {
        event OnEmitSignalHandler OnEmitSignal;
        int Adder(int a, int b);
        string Ping();
        string Echo(string a);
        void EmitSignal();
    }

    [Interface("org.freedesktop.DBus.Properties")]
    public interface ITestInterface2
    {
        string Get(string a, string b);
        //string GetAll(string a);
    }

    protected void OnButton2Clicked(object sender, EventArgs e)
    {
        ObjectPath opath = new ObjectPath("/org/example/TestObject");
        string name = "org.example.TestServer";

        Bus bus = Bus.Session;
        sample2 = bus.GetObject<ITestInterface2>(name, opath);
        label3.Text = sample2.Get("org.example.TestInterface", "Version");

        //sample = bus.GetObject<ITestInterface>(name, opath);
        //sample.OnEmitSignal += Store_OnEmitSignal;
        //sample.EmitSignal();


    }

    void NovaThread()
    {
        ObjectPath opath = new ObjectPath("/org/example/TestObject");
        string name = "org.example.TestServer";
        Bus bus = Bus.Session;
        sample3 = bus.GetObject<ITestInterface>(name, opath);

        sample3.OnEmitSignal += Store_OnEmitSignal;

        while (true)
        {
            bus.Iterate();
        }
    }
}

